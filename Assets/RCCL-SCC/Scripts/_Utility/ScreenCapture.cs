using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class creates a screenshot of the current deck view, overlays, annotations, and all and saves it to a
/// .jpg for external viewing/sharing. It lives on a game object that holds our screenshot camera.
/// </summary>
public class ScreenCapture : MonoBehaviour
{
#region Public Variables
    public string folderPath;
    public Camera screenshotCamera;
    public Canvas deckHoldingCanvas;
#endregion

#region Private Variables
    private Rect rect;
    private RenderTexture rendTexture;
    private Texture2D text2d;
    private Button screenshotButton;
#endregion

    /// <summary>
    /// We will use start to make sure the folder we want to save to exists, if it doesn't we will create it here. We will also
    /// grab the button component from the gameobject the script lives on and assign a listener for our capture screen method
    /// </summary>    
    void Start()
    {
        screenshotButton = this.GetComponent<Button>();
        CreateFolder();
        screenshotButton.onClick.AddListener(CaptureScreen);
    }

/// <summary>
/// This function will be used to create our screenshot and save it to our predesignated folder. Here, we enable our 
/// screenshot camera, change the render camera of the ui canvas holding the decks to our screenshot camera, set our
/// screenshot name by utilizing datetime.now, create a render texture, apply it to our screenshot camera, read those
/// pixels to a byte array, encode that data to .jpg, spin up a new thread to write it so it doesn't slow us down,
/// and finally switch our canvas back to the main camera and disable our screenshot camera.
/// </summary>
    public void CaptureScreen()
    {
        //Enabling screenshot camera, setting the canvas to that camera, and getting the screenshot file name
        screenshotCamera.enabled = true;
        deckHoldingCanvas.worldCamera = screenshotCamera;
        string filePath = GetScreenshotFileName(DateTime.Now);

        //If our rendertexture is null, create one
        if(rendTexture == null)
        {
            rect = new Rect(0, 0, Screen.width, Screen.height);
            rendTexture = new RenderTexture(Screen.width, Screen.height, 24);
            text2d = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        }

        //Set our render texture for the screenshot camera and render out the current screen, save render to a texture
        //and remove it from our screenshot camera
        screenshotCamera.targetTexture = rendTexture;
        screenshotCamera.Render();
        RenderTexture.active = rendTexture;
        text2d.ReadPixels(rect, 0, 0);
        screenshotCamera.targetTexture = null;
        RenderTexture.active = null;

        //Create our byte array and fill it with the necessary data to save our screenshot
        byte[] fileData = null;
        fileData = text2d.EncodeToJPG();

        //Spin up a thread to write the information to the folder and then close it
        new System.Threading.Thread(() =>
        {
            var f = System.IO.File.Create(filePath);

            f.Write(fileData, 0, fileData.Length);
            f.Close();
        }).Start();

        //Set our canvas back to main camera and disable our screenshot camera
        deckHoldingCanvas.worldCamera = Camera.main;
        screenshotCamera.enabled = false;
    }

    /// <summary>
    /// We use this function to get our screenshot file name based on the current date time.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    private string GetScreenshotFileName(DateTime dateTime)
    {
        string filename = "";

        filename = folderPath + @"\" + dateTime.ToString("yyyy-MM-ddTHH:mm:ss").Replace(":", "-") + "_ScreenCapture.jpg";

        return filename;
    }

    /// <summary>
    /// This method makes sure our folder exists and creates it if it doesn't.
    /// </summary>
    public void CreateFolder()
    {
        folderPath = Application.dataPath;

        if(Application.isEditor)
        {
            Debug.Log("Is editor!");
            var stringPath = folderPath + "/..";
            folderPath = Path.GetFullPath(stringPath);
        }

        folderPath +=  @"\ScreenShots\";

        Directory.CreateDirectory(folderPath);
    }
}
