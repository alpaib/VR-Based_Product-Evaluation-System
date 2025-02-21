using UnityEngine;
using System.IO;
using UnityEditor;

public class SceneViewSnapshot : MonoBehaviour
{
    [Header("Configuraci�n de Captura")]
    public Camera externalCamera;  // C�mara desde la que se tomar� la captura
    public int imageWidth = 1920;  // Ancho de la imagen
    public int imageHeight = 1080; // Alto de la imagen
    public string fileName = "SceneSnapshot.png"; // Nombre del archivo

    private bool snapshotCreated = false;

    public void CaptureScreenshot()
    {
        if (externalCamera == null)
        {
            Debug.LogError("No hay una c�mara asignada para la captura.");
            return;
        }

        // Guardar el estado actual de la c�mara externa
        bool wasActive = externalCamera.gameObject.activeSelf;
        externalCamera.gameObject.SetActive(true); // Activar la c�mara temporalmente

        // Crear RenderTexture con la resoluci�n especificada
        RenderTexture renderTexture = new RenderTexture(imageWidth, imageHeight, 24);
        externalCamera.targetTexture = renderTexture;
        Texture2D screenshot = new Texture2D(imageWidth, imageHeight, TextureFormat.RGB24, false);

        // Renderizar la c�mara a la textura
        externalCamera.Render();
        RenderTexture.active = renderTexture;
        screenshot.ReadPixels(new Rect(0, 0, imageWidth, imageHeight), 0, 0);
        screenshot.Apply();

        // Restaurar el estado de la c�mara
        externalCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        // Convertir la imagen a PNG
        byte[] bytes = screenshot.EncodeToPNG();
        Destroy(screenshot);

        // Asegurarse de que la carpeta StreamingAssets exista
        string folderPath = Path.Combine(Application.streamingAssetsPath);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath); // Crear la carpeta si no existe
        }

        // Guardar en StreamingAssets
        string filePath = Path.Combine(folderPath, fileName);
        File.WriteAllBytes(filePath, bytes);

        Debug.Log($"Captura guardada en: {filePath}");

        // Restaurar el estado original de la c�mara
        externalCamera.gameObject.SetActive(wasActive);
    }
}

// Script Editor para a�adir el bot�n en el Editor
[CustomEditor(typeof(SceneViewSnapshot))]
public class SceneViewSnapshotEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SceneViewSnapshot sceneViewSnapshot = (SceneViewSnapshot)target;

        // Agregar un bot�n de captura al Editor
        if (GUILayout.Button("Capturar Snapshot"))
        {
            sceneViewSnapshot.CaptureScreenshot(); // Llamar a la funci�n de captura
            Debug.Log("Captura realizada desde el Editor.");
        }
    }
}





