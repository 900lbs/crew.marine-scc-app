using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using Zenject;
[System.Xml.Serialization.XmlType("request")]
[System.Serializable]
public class XMLOverlays : xmlBase
{
    public XMLOverlays() { }

    public string linkName = "SCC Overlay Selection";
    public string linkType = "o";

    public string eVar175;

    public override async Task CustomSave(params object[] parameters)
    {
        try
        {
            eVar175 = "scc:" + (string)parameters[0];
            await new WaitForEndOfFrame();
        }
        catch (System.Exception)
        {

            throw;
        }
    }
    public new class Factory : PlaceholderFactory<XMLOverlays> { }
}
