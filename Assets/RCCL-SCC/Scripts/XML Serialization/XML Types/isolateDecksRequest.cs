using System.Xml.Serialization;
using System.Collections.Generic;
[XmlType("request")]
[System.Serializable]
public class isolateDecksRequest : xmlBase
{
    public string linkName;
    public string eVar1;
    public string list1;
}
