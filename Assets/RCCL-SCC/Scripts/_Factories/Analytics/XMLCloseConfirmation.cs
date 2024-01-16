using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

using Zenject;
[System.Serializable]
[XmlType("request")]
public class XMLCloseConfirmation : xmlBase
{
    public XMLCloseConfirmation() { }

    public string linkName;
    public string linkType = "o";
    public string eVar175;

    public override async Task CustomSave(params object[] parameters)
    {
        try
        {
            linkName = (string)parameters[0];
            eVar175 = "scc:" + (string)parameters[1];

            await new WaitForEndOfFrame();
        }
        catch (System.Exception)
        {

            throw;
        }
    }
    public new class Factory : PlaceholderFactory<XMLCloseConfirmation> { }
}
