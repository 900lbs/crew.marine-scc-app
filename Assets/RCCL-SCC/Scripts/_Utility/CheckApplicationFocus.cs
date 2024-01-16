using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

public class CheckApplicationFocus : MonoBehaviour
{
    [Inject]
    XMLWriterDynamic.Factory xmlFactory;
    XMLWriterDynamic XMLWriter;
    async void OnApplicationFocus(bool focusStatus)
    {
        if (focusStatus)
        {
            if(XMLWriter == null)
                XMLWriter = xmlFactory.Create(gameObject, XMLType.MainPage);
                
            await XMLWriter.Save();
        }
    }

    void Start()
    {
        XMLWriter = xmlFactory.Create(gameObject, XMLType.MainPage);
    }
}
