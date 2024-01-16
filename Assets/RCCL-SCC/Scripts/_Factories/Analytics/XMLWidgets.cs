using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using Zenject;
[System.Xml.Serialization.XmlType("request")]
[System.Serializable]
public class XMLWidgets : xmlBase
{
    public string linkName = "SCC Widget Toggle";
    public string linkType = "o";

    public string eVar175;
    public XMLWidgets() { }

    public async override Task CustomSave(params object[] parameters)
    {
        string command = (string)parameters[0];
        if (command != null)
        {
            eVar175 = "scc:" + command;
        }
        await new WaitForEndOfFrame();
    }
    public new class Factory : PlaceholderFactory<XMLWidgets> { }
}
