using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EyeTrackingRecorder : MonoBehaviour
{    
    [Header("Blink")]
    public OVRFaceExpressions faceExpression;
    public OVRFaceExpressions.FaceExpression leftEyeBlink;
    public OVRFaceExpressions.FaceExpression rightEyeBlink;
    private float weightL;
    private float weightR;

    [Header("Volumes of interest")]
    public GameObject[] volumesOfInterest;    
   
    [Header("Export data to HMD?")]
    public bool toHMD = false;

    [Header("Other")]
    public bool dataExported = false;

    private List<string> voi = new();
    private string filename;

    private int frequence = 120;
    private float realFrequency;

    void Start()
    {
        if (!toHMD)
        {
            filename = Application.streamingAssetsPath + "\\" + gameObject.GetComponent<Manager>().userCode + "_Eye-tracking" + ".csv";
        }

        else
        {
            filename = Application.persistentDataPath + "\\" + gameObject.GetComponent<Manager>().userCode + "_Eye-tracking" + ".csv";
        }

        UpdateInvoke();
    }
    void UpdateInvoke()
    {
        if (frequence <= 0)
        {
            Debug.LogError("Frecuencia debe ser mayor que 0");
            return;
        }

        realFrequency = 1f / frequence;
        CancelInvoke("ReadVolume");
        InvokeRepeating("ReadVolume", 0, realFrequency);
    }
    void Update()
    {
        volumesOfInterest = GameObject.FindGameObjectsWithTag("VoI");

        // Update face expression
        weightL = faceExpression[leftEyeBlink];
        weightR = faceExpression[rightEyeBlink];

        if (dataExported)
        {
            Invoke("ExportData", 0);
        }
    }

    public void ReadVolume()
    {
        bool stimuliAddedToList = false;
        bool isBlinking = false;

        if (!dataExported)
        {
            if (weightL >= 0.6f && weightR >= 0.6f)
            {
                isBlinking = true;
            }

            for (int i = 0; i < volumesOfInterest.Length; i++)
            {
                if (volumesOfInterest[i].GetComponent<EyeInteractable>().isHovered == true && isBlinking == false)
                {
                    voi.Add(volumesOfInterest[i].name);
                    stimuliAddedToList = true;
                }
            }

            //Blink detection
            if (weightL >= 0.6f && weightR >= 0.6f)
            {
                voi.Add("blink");

                //isHovered should be false for each volume of interest
                for (int a = 0; a < volumesOfInterest.Length; a++)
                {
                    volumesOfInterest[a].GetComponent<EyeInteractable>().isHovered = false;
                }
            }

            else if (stimuliAddedToList == false && isBlinking == false)
            {
                voi.Add("Environment");
            }
        }    
    }

    public void ExportData()
    {
        TextWriter tw = new StreamWriter(filename, false);

        tw.WriteLine("Volume");

        for (int i = 0; i < voi.Count; ++i)
        {
            tw.WriteLine(voi[i]);
        }

        tw.Close();
        
    }
}
