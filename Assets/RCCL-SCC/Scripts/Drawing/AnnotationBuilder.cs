using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI.Extensions;

using Zenject;

using Object = UnityEngine.Object;

/// <summary>
/// Class used to create annotations either via Queue or instantaneous.
/// </summary>
/// 
/// <remarks>
/// This class is setup for a singleton via dependency injections, simply use the Zenject namespace
/// and assign the [Inject] attribute to either a constructor (using this class as one of the parameters), method
/// (same thing), or parameter.
/// </remarks>
#if SCC_2_5
public class AnnotationBuilder : MonoBehaviour, IShipNetworkActions, IDisposable, IInRoomCallbacks
{
    #region Dependency Setup

    DeckManager deckManager;
    LineRendererFactory lineFactory;
    IconBehavior.Factory iconFactory;

    [Inject]
    public void Construct(DeckManager deckMan, LineRendererFactory lineFact, IconBehavior.Factory iconFact)
    {
        deckManager = deckMan;
        lineFactory = lineFact;
        iconFactory = iconFact;
    }

    public async void Start()
    {
        if (AnnotationQueue == null)
        {
            AnnotationQueue = new SortedDictionary<short, object[]>();
        }

        foreach (var item in Storage)
        {
            item.AddListener(this);
        }

        PhotonNetwork.AddCallbackTarget(this);
    }

    public void Dispose()
    {
        if (this == null)
            return;
        foreach (var item in Storage)
        {
            item?.RemoveListener(this);
        }

        PhotonNetwork.RemoveCallbackTarget(this);
    }

    #endregion

    #region Variables
    public List<NetworkStorageAsset> Storage;

    public bool IsProcessingAnnotations;
    public SortedDictionary<short, object[]> AnnotationQueue;

    #endregion

    public async void NetworkAction(params object[] additionalInfo)
    {
        //AnnotationQueue.Add((short)additionalInfo[3], additionalInfo);
        //return;
        // Old method
        bool isDirty = (bool)additionalInfo[0];
        NetworkStorageType type = (NetworkStorageType)additionalInfo[1];
        string playerID = (string)additionalInfo[2];
        short key = (short)additionalInfo[3];
        string[] data = GetAnnotationArray((string)additionalInfo[4]);

        if (isDirty)
        {
            BuildAnnotationDirty(type, playerID, data, key);
        }
        else
        {
            await BuildAnnotation(type, playerID, data, key);
        }
    }

    /*----------------------------------------------------------------------------------------------------------------------------*/

    private async void FixedUpdate()
    {
        //if(AnnotationQueue.Count > 0 && ProcessAnnotationRequests().Status != TaskStatus.Running)
        //{
        //    await ProcessAnnotationRequests();
        //}
    }

    /// <summary>
    /// This coroutine is constantly checking for a queue increase.
    /// </summary>
    /// <remarks>
    /// If you want to create a new annotation cleanly (queued up), simply add the required object to the list.
    /// </remarks>
    /// <returns>Returns null if the queue is 0 or yields start the creation coroutine BuildAnnotation.</returns>
    public async Task ProcessAnnotationRequests()
    {
        if (Application.isPlaying)
        {
            while (AnnotationQueue.Count > 0)
            {
                Debug.Log("Processing Annotations", this);
                KeyValuePair<short, object[]> currentObject = AnnotationQueue.First();
                await BuildAnnotation((NetworkStorageType)currentObject.Value[1], (string)currentObject.Value[2], (string[])GetAnnotationArray((string)currentObject.Value[4]), currentObject.Key);
                AnnotationQueue.Remove(currentObject.Key);
            }

             await new WaitForUpdate();
        }
    } 

    /*----------------------------------------------------------------------------------------------------------------------------*/

    public async Task BuildAnnotation(NetworkStorageType type, string playerID, string[] info, short key = 0)
    {
/*         Debug.Log("AnnotationBuilder received order to create " + type.ToString(), this);

        foreach (var item in Storage)
        {
            Debug.Log(((type.HasFlag(item.StorageData.AssetType)) ? "Matched listener: " : "Did not match listener: ") + item.StorageData.AssetType.ToString(), this);
        } */

        if (type.HasFlag(NetworkStorageType.LineRenderer))
        {
            NewLineRendererSave receivedSave = new NewLineRendererSave();
            await receivedSave.SetByArray(info, playerID);
            lineFactory.Create(receivedSave);
            //Debug.Log("Creating LineRenderer: " + info, this);
        }

        if (type.HasFlag(NetworkStorageType.Icon))
        {
            NewIconSave receivedIconSave = new NewIconSave();
            await receivedIconSave.SetByArray(info, playerID);
            //Debug.Log("Creating Icon: " + info, this);
            iconFactory.Create(GetPrefab(info[1]), receivedIconSave);

        }

        await new WaitForEndOfFrame();
    }

    public async void BuildAnnotationDirty(NetworkStorageType type, string playerID, string[] info, short key = 0)
    {
        //Debug.Log("AnnotationBuilder received order to create " + type.ToString() + ": " + info[1], this);

        switch (type)
        {
            case NetworkStorageType.LineRenderer:
                NewLineRendererSave receivedSave = new NewLineRendererSave();
                await receivedSave.SetByArray(info, playerID);
                lineFactory.Create(receivedSave);
                break;

            case NetworkStorageType.Icon:

                NewIconSave receivedIconSave = new NewIconSave();
                await receivedIconSave.SetByArray(info, playerID);
                iconFactory.Create(GetPrefab(info[1]), receivedIconSave);
                break;
        }
    }

    //Accessing the correct prefab, this method is called above when the string array is being parsed.
    public static Object GetPrefab(string name)
    {
        Object newPrefab;

        newPrefab = Resources.Load("Prefabs/SafetyIconPrefabs/" + name);

        return newPrefab;
    }

    public static string[] GetAnnotationArray(string value)
    {
        string convertedData = value;
        string[] splitValue = convertedData.Split(","[0]);

        return splitValue;
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
    }

    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (NetworkClientManager.GetRoomProperty(ShipRoomProperties.Annotations)?.Count >= 1)
        {
            int storageCount = Storage.Count;
            for (int i = 0; i < storageCount; ++i)
            {
                //Storage[i].ForceAnnotationPropertyUpdate();
            }
        }
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
    }
}

#elif !SCC_2_5

public class AnnotationBuilder : MonoBehaviour, IShipNetworkAnnotation
{

    public static string[] delimiter = new string[] {","};
        /// <summary>
    /// Our network annotation listener, take in serialized values and react accordingly.
    /// </summary>
    /// <param name="value">The parameter passed from the network.</param>
    public void OnNewAnnotationReceived(string value)
    {
        Debug.Log("Received a new annotation", this);

        string[] splitValue = value.Split(","[0]);

        foreach (string s in splitValue)
        {
            Debug.Log("<color=red>" + s.ToString() + "</color>", this);
        }
        
        BuildAnnotation(splitValue, false);
        
    }

    /* Description
     * Method we can use to build the annotation over the network. The first variable is the string array that holds the info on the annotation. The second
     * variable is a reference to the DrawOnDeck script we are using in the current scene. The third variable is a boolean to tell the builder whether or 
     * not the annotation needs to be on the GA or in the temporary LineHolder game object. I assume that every time this method is used to send from the 
     * tablet to the main table then the gaHolder variable will be set to false.
     */


    public static void BuildAnnotation(string[] info, bool gaHolder)
    {
        //Figuring out the annotations parent object.
        Transform parentObject = gaHolder ? DrawOnDeck.Instance.gaHolders[int.Parse(info[1])].transform : DrawOnDeck.Instance.lineHolders[int.Parse(info[1])].transform;

        /*
         * If the annotation is a line renderer then we build it here. Each string in the array that is passed will be parsed here to populate the 
         * gameobject.
         */
        if (info[0] == "UI Line Renderer")
        {
            GameObject prefabToLoad = GetPrefab(info[0]);

            prefabToLoad.transform.SetParent(parentObject, false);

            UILineRenderer newLineRenderer = prefabToLoad.GetComponent<UILineRenderer>();

            Color lineColor;

            ColorUtility.TryParseHtmlString(info[2], out lineColor);

            newLineRenderer.color = lineColor;

            newLineRenderer.lineThickness = float.Parse(info[3]);

            if (info[4] == "true")
            {
                prefabToLoad.AddComponent<UIMultiplyEffect>();
            }

            Vector2[] linePoints = new Vector2[info.Length - 5];

            for (int i = 0; i < linePoints.Length; i++)
            {
                string[] vectorPoint = info[i + 5].Split("="[0]);

                linePoints[i].x = float.Parse(vectorPoint[0]);
                linePoints[i].y = float.Parse(vectorPoint[1]);
            }

            newLineRenderer.Points = linePoints;
        }

        //If the annotation is an icon, we build it here using a variation of the technique above.
        else
        {
            GameObject prefabToLoad = GetPrefab(info[0]);

            Vector3 prefabPosition = new Vector3(float.Parse(info[2]), float.Parse(info[3]), float.Parse(info[4]));

            prefabToLoad.transform.SetParent(parentObject, false);

            prefabToLoad.GetComponent<RectTransform>().anchoredPosition = prefabPosition;

            prefabToLoad.GetComponent<IconBehavior>().master = DrawOnDeck.Instance;

            if(info[5] != "normal")
            {
                prefabToLoad.GetComponent<IconBehavior>().iconText = info[6];
                prefabToLoad.GetComponent<IconBehavior>().nameOrNumber.text = info[6];
            }

            if (info[7] == "true")
            {
                prefabToLoad.GetComponent<IconBehavior>().isGA = true;
            }
        }
    }

    //Accessing the correct prefab, this method is called above when the string array is being parsed.
    private static GameObject GetPrefab(string name)
    {
        GameObject newPrefab;

        if (name == "UI Line Renderer")
        {
            newPrefab = Instantiate(Resources.Load("Prefabs/" + name)) as GameObject;
        }

        else
        {
            newPrefab = Instantiate(Resources.Load("Prefabs/SafetyIconPrefabs/" + name)) as GameObject;
        }

        return newPrefab;
    }
}


#endif