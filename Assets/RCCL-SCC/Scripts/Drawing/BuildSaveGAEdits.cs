using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using TMPro;
using Zenject;
using System;

#if SCC_2_5

public class BuildSaveGAEdits : MonoBehaviour
{
    private AnnotationManager annotationManager;
    private DeckManager deckManager;
    private Settings settings;
    private NetworkClient networkClient;
    private SignalBus _signalBus;

    private LineRendererFactory lineRendFactory;
    private IconBehavior.Factory iconFactory;

    [Inject]
    public void Construct(
    AnnotationManager annoMan,
    DeckManager deckMan,
    Settings setting,
    NetworkClient netClient,
    SignalBus signal,
    LineRendererFactory lineRendFact,
    IconBehavior.Factory iconFact)
    {
        annotationManager = annoMan;
        deckManager = deckMan;
        settings = setting;
        networkClient = netClient;
        _signalBus = signal;
        lineRendFactory = lineRendFact;
        iconFactory = iconFact;
    }

    public string fileName;

    public string file;

    public List<string> lines;

    public GameObject confirmationScreen;

    public Button resetButton;
    private Image resetFrame;
    private TextMeshProUGUI resetTitle;

    private void Awake()
    {
        NetworkClient.OnNewGAOverlayEventReceived += OnGAUpdated;
    }

    // Use this for initialization
    private void Start()
    {
        _signalBus.Subscribe<Signal_MainMenu_OnGameStateChanged>(GameStateChanged);
        resetButton?.onClick.AddListener(ConfirmReset);
        resetFrame = resetButton?.transform.GetChild(0).GetComponent<Image>();
        resetTitle = resetFrame?.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        //LoadPrefabs();
    }

    private void OnDestroy()
    {
        NetworkClient.OnNewGAOverlayEventReceived -= OnGAUpdated;
        _signalBus.TryUnsubscribe<Signal_MainMenu_OnGameStateChanged>(GameStateChanged);
    }

    private void GameStateChanged(Signal_MainMenu_OnGameStateChanged signal)
    {
        CheckNewGameState(signal.State);
    }

    private async void CheckNewGameState(GameState state)
    {
        if (state == GameState.Room)
        {
            Debug.Log("<color=green>Spawning GA edits.</color>");
            await LoadPrefabs();
        }
    }

    public async void SavePrefabs()
    {
        Debug.Log("Saving GA prefabs.", this);
        lines.Clear();

        List<GameObject> annotations = new List<GameObject>();
        annotations = deckManager.GetAnnotations(ShipRoomProperties.GAOverlay);

        for (int i = 0; i < annotations.Count; i++)
        {
            if (annotations[i].GetComponent<LineBehavior>())
            {
                Debug.Log("Creating new annotation for GA.", this);
                await NewAnnotation(annotations[i]);
            }
            else
            {
                Debug.Log("Creating new Icon for GA.", this);
                await NewIcon(annotations[i]);
            }
        }

        await WriteOverlaysToCSV();
    }

    private async Task NewAnnotation(GameObject currentAnnotation)
    {
        NewLineRendererSave newAnnotation = currentAnnotation.GetComponent<LineBehavior>().lineRendSave;

        lines.Add(newAnnotation.GetSingleLine());

        await new WaitForEndOfFrame();
    }

    private async Task NewIcon(GameObject currentIcon)
    {
        NewIconSave newIcon = currentIcon.GetComponent<IconBehavior>().IconSave;

        lines.Add(newIcon.GetSingleLine());

        await new WaitForEndOfFrame();
    }

    public void ConfirmReset()
    {
        confirmationScreen.SetActive(true);
    }

    public async void ResetPrefabs()
    {
        List<GameObject> annotations = new List<GameObject>();

        annotations = deckManager.GetAnnotations(ShipRoomProperties.GAOverlay);

        for (int i = 0; i < annotations.Count; i++)
        {
            Destroy(annotations[i].gameObject);
        }

        if (lines != null)
        {
            await LoadPrefabs();
        }

        confirmationScreen.SetActive(false);
    }

    public void ToggleReset(Color newColor)
    {
        resetButton.interactable = !resetButton.interactable;
        resetFrame.color = newColor;
        resetTitle.color = newColor;
    }

    public async Task InitializePrefabs(NetworkClient netClient)
    {
        networkClient = netClient;
        await LoadPrefabs();
    }

    public async Task LoadPrefabs()
    {
        file = System.IO.File.ReadAllText(GetPath());
        if (Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.INFO ||
                    Photon.Pun.PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging == ExitGames.Client.Photon.DebugLevel.ALL)
            Debug.Log("Building GA Edits, size: " + file.Length, this);
        //lines = file.Split('\n').ToList();
        List<string> rowLines = file.Split('\n').ToList();

        for (int i = 0; i < rowLines.Count; i++)
        {
            if (rowLines[i] != "")
            {
                string convertedData = rowLines[i];
                List<string> splitValue = convertedData.Split(","[0]).ToList();

                NetworkStorageType storage;

                Enum.TryParse(splitValue[0], out storage);

                if (storage == NetworkStorageType.LineRenderer)
                {
                    NewLineRendererSave newSave = new NewLineRendererSave();
                    await newSave.SetByArray(splitValue.ToArray(), NetworkClient.GetMasterClientID());
                    lineRendFactory.Create(newSave);
                    continue;
                }
                if (storage == NetworkStorageType.Icon)
                {
                    Debug.Log("ICON found, attempting to create now.");
                    NewIconSave newIconSave = new NewIconSave();
                    await newIconSave.SetByArray(splitValue.ToArray(), NetworkClient.GetMasterClientID());
                    iconFactory.Create(AnnotationBuilder.GetPrefab(newIconSave.PrefabName), newIconSave);
                    continue;
                }
            }
        }
        await new WaitForEndOfFrame();
    }

    public void OnGAUpdated(Dictionary<string, string> value)
    {
        LoadPrefabsFromNetwork(value);
    }

    public async void LoadPrefabsFromNetwork(Dictionary<string, string> value)
    {
        foreach (var item in deckManager.AllDecks)
        {
            if (item.Value.GAHolder.childCount > 0)
            {
                for (int i = 0; i < item.Value.GAHolder.childCount; i++)
                {
                    Destroy(item.Value.GAHolder.GetChild(i).gameObject);
                }
            }
        }
        if (value == null)
            Debug.LogError("Prefab from network was null.", this);

        if (value != null || value?.Count > 0)
        {
            foreach (var item in value)
            {
                if (item.Value != "")
                {
                    file = item.Value;
                    string[] fileArray = file.Split(","[0]);

                    NetworkStorageType storage;
                    Enum.TryParse(fileArray?[0], out storage);

                    Debug.Log("Attempting to create GA annotation: " + file, this);

                    NewLineRendererSave newLineRendererSave = new NewLineRendererSave();
                    if (storage == NetworkStorageType.LineRenderer && await newLineRendererSave.SetBySingleLine(file, NetworkClient.GetMasterClientID()))
                    {
                        Debug.Log("<color=green>LINE created.</color>", this);
                        lineRendFactory.Create(newLineRendererSave);
                        continue;
                    }

                    NewIconSave newIconSave = new NewIconSave();
                    if (storage == NetworkStorageType.Icon && await newIconSave.SetBySingleLine(file, NetworkClient.GetMasterClientID()))
                    {
                        Debug.Log("<color=green>ICON created.</color>", this);
                        iconFactory.Create(AnnotationBuilder.GetPrefab(newIconSave.PrefabName), newIconSave);
                        continue;
                    }
                }
            }
        }
    }

    private async Task WriteOverlaysToCSV()
    {
        try
        {
            string filePath = GetPath();

            StreamWriter writer = new StreamWriter(filePath);

            for (int i = 0; i < lines.Count; i++)
            {
                writer.WriteLine(lines[i]);
            }

            writer.Flush();

            writer.Close();
            await new WaitForEndOfFrame();

            if (networkClient.IsMasterClient)
                networkClient?.UpdateRoomProperty((byte)ShipRoomProperties.GAOverlay);
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    private string GetPath()
    {
        return Application.streamingAssetsPath + "/GAOverlay.csv";
    }
}

#elif !SCC_2_5
public class BuildSaveGAEdits : MonoBehaviour
{
    public string fileName;

    public string file;

    public List<string> lines;
    public string[] parts;

    public GameObject confirmationScreen;

    public Button resetButton;
    private Image resetFrame;
    private TextMeshProUGUI resetTitle;

    // Use this for initialization
    void Start()
    {
        resetButton.onClick.AddListener(ConfirmReset);
        resetFrame = resetButton.transform.GetChild(0).GetComponent<Image>();
        resetTitle = resetFrame.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        DrawOnDeck.Instance.buildSaveGAEdits = this;
        LoadPrefabs();
    }

    public void SavePrefabs()
    {
        lines.Clear();

        for (int i = 0; i < DrawOnDeck.Instance.gaHolders.Count; i++)
        {
            if (DrawOnDeck.Instance.gaHolders[i].transform.childCount > 0)
            {
                for (int j = 0; j < DrawOnDeck.Instance.gaHolders[i].transform.childCount; j++)
                {
                    if (DrawOnDeck.Instance.gaHolders[i].transform.GetChild(j).gameObject.GetComponent<LineBehavior>())
                    {
                        NewAnnotation(i, j);
                    }
                    else
                    {
                        NewIcon(i, j);
                    }
                }
            }
        }

        WriteOverlaysToCSV();
    }

    public void NewAnnotation(int i, int j)
    {
        NewAnnotationSave newAnnotation = new NewAnnotationSave();

        UILineRenderer uILineRenderer = DrawOnDeck.Instance.gaHolders[i].transform.GetChild(j).GetComponent<UILineRenderer>();

        newAnnotation.prefabName = DrawOnDeck.Instance.gaHolders[i].transform.GetChild(j).name.Replace("(Clone)", "");
        newAnnotation.GANumber = i.ToString();
        newAnnotation.color = "#" + ColorUtility.ToHtmlStringRGB(uILineRenderer.color);
        newAnnotation.thickness = uILineRenderer.lineThickness.ToString();

        newAnnotation.isHighlighter = uILineRenderer.gameObject.GetComponent<UIMultiplyEffect>() != null ? "true" : "false";

        newAnnotation.points = new string[uILineRenderer.Points.Length];

        for (int k = 0; k < newAnnotation.points.Length; k++)
        {
            newAnnotation.points[k] = uILineRenderer.Points[k].x.ToString() + "=" + uILineRenderer.Points[k].y.ToString();
        }

        string lineToAdd = newAnnotation.prefabName + "," + newAnnotation.GANumber + "," + newAnnotation.color + "," +
            newAnnotation.thickness + "," + newAnnotation.isHighlighter;

        foreach (string newPoint in newAnnotation.points)
        {
            lineToAdd = lineToAdd + "," + newPoint;
        }

        lines.Add(lineToAdd);
    }

    public void NewIcon(int i, int j)
    {
        NewIconSave newIcon = new NewIconSave();

        newIcon.prefabName = DrawOnDeck.Instance.gaHolders[i].transform.GetChild(j).name.Replace("(Clone)", "");
        newIcon.GANumber = i.ToString();
        newIcon.X = DrawOnDeck.Instance.gaHolders[i].transform.GetChild(j).GetComponent<RectTransform>().anchoredPosition.x.ToString();
        newIcon.Y = DrawOnDeck.Instance.gaHolders[i].transform.GetChild(j).GetComponent<RectTransform>().anchoredPosition.y.ToString();
        newIcon.Z = "0";
        newIcon.type = DrawOnDeck.Instance.gaHolders[i].transform.GetChild(j).GetComponent<IconBehavior>().type.ToString();
        newIcon.text = DrawOnDeck.Instance.gaHolders[i].transform.GetChild(j).GetComponent<IconBehavior>().iconText;
        newIcon.isGA = DrawOnDeck.Instance.gaHolders[i].transform.GetChild(j).GetComponent<IconBehavior>().isGA ? "true" : "false";

        lines.Add(newIcon.prefabName + "," + newIcon.GANumber + "," + newIcon.X + "," + newIcon.Y + "," + newIcon.Z + "," + newIcon.type + ","
            + newIcon.text + "," + newIcon.isGA);
    }

    public void ConfirmReset()
    {
        confirmationScreen.SetActive(true);
    }

    public void ResetPrefabs()
    {
        for (int i = 0; i < DrawOnDeck.Instance.gaHolders.Count; i++)
        {
            if (DrawOnDeck.Instance.gaHolders[i].transform.childCount > 0)
            {
                for (int j = 0; j < DrawOnDeck.Instance.gaHolders[i].transform.childCount; j++)
                {
                    Destroy(DrawOnDeck.Instance.gaHolders[i].transform.GetChild(j).gameObject);
                }
            }
        }

        if (lines != null)
        {
            LoadPrefabs();
        }

        confirmationScreen.SetActive(false);
    }

    public void ResetPrefabs(string value)
    {
        for (int i = 0; i < DrawOnDeck.Instance.gaHolders.Count; i++)
        {
            if (DrawOnDeck.Instance.gaHolders[i].transform.childCount > 0)
            {
                for (int j = 0; j < DrawOnDeck.Instance.gaHolders[i].transform.childCount; j++)
                {
                    Destroy(DrawOnDeck.Instance.gaHolders[i].transform.GetChild(j).gameObject);
                }
            }
        }

        if (lines != null)
        {
            LoadPrefabs(value);
        }
    }

    public void ToggleReset(Color newColor)
    {
        resetButton.interactable = !resetButton.interactable;
        resetFrame.color = newColor;
        resetTitle.color = newColor;
    }

    private void LoadPrefabs()
    {
        file = System.IO.File.ReadAllText(GetPath());

        lines = file.Split('\n').ToList();

        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i] == "")
            {
                lines.Remove(lines[i]);
            }
            else
            {
                string convertedData = (string)lines[i];
                string[] splitValue = convertedData.Split(","[0]);
                AnnotationBuilder.BuildAnnotation(splitValue, true);
            }
        }
    }

    public void LoadPrefabs(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            file = value;

            lines = file.Split('\n').ToList();

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i] == "")
                {
                    lines.Remove(lines[i]);
                }
                else
                {
                    string convertedData = (string)lines[i];
                    string[] splitValue = convertedData.Split(","[0]);
                    AnnotationBuilder.BuildAnnotation(splitValue, true);
                }
            }
        }
    }

    void WriteOverlaysToCSV()
    {
        string filePath = GetPath();

        StreamWriter writer = new StreamWriter(filePath);

        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i] != "")
            {
                writer.WriteLine(lines[i]);
            }
        }

        writer.Flush();

        writer.Close();
    }

    private string GetPath()
    {
        return Application.streamingAssetsPath + "/GAOverlay.csv";
    }
}

[System.Serializable]
public class NewIconSave
{
    public string prefabName;
    public string GANumber;
    public string X;
    public string Y;
    public string Z;
    public string type;
    public string text;
    public string isGA;
}

[System.Serializable]
public class NewAnnotationSave
{
    public string prefabName;
    public string GANumber;
    public string color;
    public string thickness;
    public string isHighlighter;
    public string[] points;
}

#endif