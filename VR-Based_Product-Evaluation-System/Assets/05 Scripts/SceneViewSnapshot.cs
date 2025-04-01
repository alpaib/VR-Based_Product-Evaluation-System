using UnityEngine;
using System.IO;

public class SceneViewSnapshot : MonoBehaviour
{
    [Header("Capture configuration")]
    public Camera externalCamera;
    public int imageWidth = 1920;
    public int imageHeight = 1080;
    public string fileName = "SceneSnapshot.png";

    private bool snapshotCreated = false;

    public void CaptureScreenshot()
    {
        if (externalCamera == null)
        {
            Debug.LogError("There is not a camera assigned.");
            return;
        }

        bool wasActive = externalCamera.gameObject.activeSelf;
        externalCamera.gameObject.SetActive(true);

        RenderTexture renderTexture = new RenderTexture(imageWidth, imageHeight, 24);
        externalCamera.targetTexture = renderTexture;
        Texture2D screenshot = new Texture2D(imageWidth, imageHeight, TextureFormat.RGB24, false);

        externalCamera.Render();
        RenderTexture.active = renderTexture;
        screenshot.ReadPixels(new Rect(0, 0, imageWidth, imageHeight), 0, 0);
        screenshot.Apply();

        externalCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        byte[] bytes = screenshot.EncodeToPNG();
        Destroy(screenshot);

        string folderPath = Path.Combine(Application.streamingAssetsPath);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string filePath = Path.Combine(folderPath, fileName);
        File.WriteAllBytes(filePath, bytes);

        Debug.Log($"Capture saved in: {filePath}");

        externalCamera.gameObject.SetActive(wasActive);
    }
}

