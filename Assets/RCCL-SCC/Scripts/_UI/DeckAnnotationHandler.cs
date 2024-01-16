using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using Zenject;

public class DeckAnnotationHandler : MonoBehaviour
{
    [Inject]
    SignalBus _signalBus;

    public Dictionary<UserNameID, DeckAnnotationHolder> UserAnnotationHolders;

    public GameObject UserAnnotationHolderPrefab;
    void Awake()
    {
        UserAnnotationHolders = new Dictionary<UserNameID, DeckAnnotationHolder>();
        _signalBus.Subscribe<Signal_AnnoMan_OnActiveUserAnnotationsUpdated>(ActiveUserAnnotationsUpdated);
    }

    void OnDestroy()
    {
        _signalBus.Unsubscribe<Signal_AnnoMan_OnActiveUserAnnotationsUpdated>(ActiveUserAnnotationsUpdated);

    }

    public void ActiveUserAnnotationsUpdated(Signal_AnnoMan_OnActiveUserAnnotationsUpdated signal)
    {
        string[] users = Enum.GetNames(typeof(UserNameID));

        foreach (string user in users)
        {
            UserNameID userName;
            Enum.TryParse(user, out userName);
            Debug.Log("Activate User: <color=green> " + userName + "</color>");
            if(!UserAnnotationHolders.ContainsKey(userName))
                {
                    Debug.Log("User " + userName + " not found in dictionary.");
                    return;
                }
            UserAnnotationHolders[userName].gameObject.SetActive(signal.Users.HasFlag(userName));
        }
    }

    public async Task SpawnHolders()
    {
        string[] totalUserNames = Enum.GetNames(typeof(UserNameID));
        GameObject copyHandler = gameObject;

        foreach (string item in totalUserNames)
        {
            UserNameID userName;
            Enum.TryParse(item, out userName);

            GameObject newHolderGO = Instantiate(UserAnnotationHolderPrefab, transform);
            newHolderGO.transform.localPosition = Vector3.zero;
            newHolderGO.name = userName.ToString();

            DeckAnnotationHolder newHolder = newHolderGO.AddComponent(typeof(DeckAnnotationHolder)) as DeckAnnotationHolder;
            newHolder.User = userName;

            UserAnnotationHolders.Add(userName, newHolder);
        }

        await new WaitForEndOfFrame();
    }
}
