/*
 * © 900lbs of Creative
 * Creation Date: DATE HERE
 * Date last Modified: MOST RECENT MODIFICATION DATE HERE
 * Name: AUTHOR NAME HERE
 * 
 * Description: DESCRIPTION HERE
 * 
 * Scripts referenced: LIST REFERENCED SCRIPTS HERE
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;

using Zenject;
#if SCC_2_5
public class MinimizeApp : MonoBehaviour 
{
    #region Injection Construction

    XMLWriterDynamic.Factory xmlWriterFactory;

    [Inject]
    public void Construct(XMLWriterDynamic.Factory  xmlWriterFact)
    {
        xmlWriterFactory = xmlWriterFact;
    }

    #endregion
    public Button minButton;


    XMLWriterDynamic xmlWriter;

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    private void Start()
    {
        minButton.onClick.AddListener(onMinimizeButtonClick);
    }

    public async void onMinimizeButtonClick()
    {
        xmlWriter = xmlWriterFactory.Create(gameObject, XMLType.Minimize);
        await xmlWriter.Save();
        ShowWindow(GetActiveWindow(), 2);
    }
}

#elif !SCC_2_5
public class MinimizeApp : MonoBehaviour 
{
    public Button minButton;

    public Button closeButton;

    public GameObject AreYouSure;

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    private void Start()
    {
        minButton.onClick.AddListener(onMinimizeButtonClick);

        closeButton.onClick.AddListener(onCloseButtonClick);
    }

    public void onMinimizeButtonClick()
    {
        ShowWindow(GetActiveWindow(), 2);
    }

    public void onCloseButtonClick()
    {
        AreYouSure.SetActive(true);
    }

    public void QuitProgram()
    {
        Application.Quit();
    }
}
#endif