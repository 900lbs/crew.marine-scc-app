using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

public class SettingsRestrictions : MonoBehaviour
{
    [Inject]
    ProjectSettings settings;

    [SerializeField] bool isMultiship;
    [SerializeField] bool isDev;
    [SerializeField] bool isDebug;

    void Start()
    {
        if(isMultiship != settings.MultishipEnabled || isDev != settings.DevEnabled || isDebug != settings.DebugEnabled)
            gameObject.SetActive(false);
    }
}
