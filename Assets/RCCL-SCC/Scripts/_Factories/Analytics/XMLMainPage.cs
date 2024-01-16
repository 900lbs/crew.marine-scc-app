using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;
[System.Xml.Serialization.XmlType("request")]
[System.Serializable]
public class XMLMainPage : xmlBase
{
    public string pageName = "scc:main";

    public XMLMainPage() { }

    public new class Factory : PlaceholderFactory<XMLMainPage> { }
}
