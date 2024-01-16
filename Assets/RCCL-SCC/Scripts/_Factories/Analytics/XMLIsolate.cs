using System.Threading.Tasks;
using UnityEngine;

using Zenject;

[System.Xml.Serialization.XmlType("request")]
[System.Serializable]
public class XMLIsolate : xmlBase
{
    [Inject]
    public XMLIsolate() { }

    public string linkName = "SCC Isolate Decks";
    public string linkType = "o";
    public string eVar64;
    public string list1;

    public override async Task CustomSave(params object[] parameters)
    {
        try
        {
            eVar64 =  (string)parameters[0];
            list1 =  (string)parameters[0];

            await new WaitForEndOfFrame();
        }
        catch
        {
            Debug.LogError("Could not cast type: " + parameters[0].GetType().ToString() + " from: " + parameters[0]);
            throw;
        }
    }
    public new class Factory : PlaceholderFactory<XMLIsolate> { }
}
