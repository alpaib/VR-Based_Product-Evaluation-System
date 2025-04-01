using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SceneViewSnapshot))]
public class SceneViewSnapshotEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SceneViewSnapshot sceneViewSnapshot = (SceneViewSnapshot)target;

        if (GUILayout.Button("CAPTURE"))
        {
            sceneViewSnapshot.CaptureScreenshot();
            Debug.Log("Capture done from Editor.");
        }
    }
}

