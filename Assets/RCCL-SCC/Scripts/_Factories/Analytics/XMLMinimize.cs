using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;
[System.Xml.Serialization.XmlType("request")]
[System.Serializable]
public class XMLMinimize : xmlBase
{
    public XMLMinimize() { }

    public string linkName = "SCC Close";
    public string linkType = "o";
    public string eVar175 = "scc:main:sccmin";
    public new class Factory : PlaceholderFactory<XMLMinimize> { }
}