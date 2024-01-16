using System.Threading.Tasks;
using Zenject;
[System.Serializable]
public class xmlBase
{
    public string sc_xml_ver;
    public string ipAddress { get { return XMLWriter.GetIPAddress(); } }
    public string prop67;
    public string eVar139;
    public string eVar140;
    public string eVar143;
    public string timestamp;
    public string reportSuiteID;
    public string eVar138;

    public void CopyBaseTo(xmlBase target)
    {
        target.sc_xml_ver = sc_xml_ver;
        target.prop67 = prop67;
        target.eVar139 = eVar139;
        target.eVar140 = eVar140;
        target.eVar143 = eVar143;
        target.timestamp = timestamp;
        target.reportSuiteID = reportSuiteID;
        target.eVar138 = eVar138;
    }

    public virtual async Task CustomSave(params object[] parameters) { await new WaitForUpdate(); }

    public class Factory : PlaceholderFactory<XMLType, xmlBase> { }
}
