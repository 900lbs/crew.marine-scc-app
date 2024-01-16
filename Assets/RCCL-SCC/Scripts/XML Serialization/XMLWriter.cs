using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Net;
using System;
using UnityEngine;
using System.Reflection;
using Unity;

using Zenject;

public class XMLWriter : MonoBehaviour
{
    public enum XMLType
    {
        MainPage,
        Timers,
        Overlays,
        Widgets,
        Isolate,
        Minimize,
        Close,
        CloseConfirmation
    }

    public XMLType xmlType;
    public mainPageRequest mainPage = new mainPageRequest();
    public timerRequest timer = new timerRequest();
    public overlayRequest overlay = new overlayRequest();
    public widget_closeRequest widget_close = new widget_closeRequest();
    public isolateDecksRequest isolateDecks = new isolateDecksRequest();
    public min_confRequest min_conf = new min_confRequest();

    public object request;

    public string filePath;

    public bool IsInstantiatedAtRuntime;
    XMLShipData data;

    private void Start()
    {
        if (!IsInstantiatedAtRuntime)
        {
            data = FindObjectOfType<XMLShipData>();

            SetXmlType(request);

            PopulateClass(request);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Save(request);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            PopulateClass(request);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            SetXmlType(request);
        }
    }

    public void PopulateClass(object req)
    {
        int num = 0;
        foreach (var field in req.GetType().GetFields())
        {
            if (field.ToString() != "System.String sc_xml_ver" && num == 0)
            {
                continue;
            }

            else
            {
                if (num == 1)
                {
                    field.SetValue(req, GetIPAddress());
                }

                else
                {
                    field.SetValue(req, data.shipData[num]);
                }

                num++;
            }
        }
        // req.GetType().GetField("ipAddress").SetValue(req, GetIPAddress());

        // req.GetType().GetField("ipAddress");

        // string startTime = RoundDown(DateTime.Now, TimeSpan.FromSeconds(30));

        // req.GetType().GetField("timestamp").SetValue(req, startTime);

        // req.GetType().GetField("timestamp");
    }

    public void Save(object req)
    {
        System.Type type = req.GetType();

        filePath = Application.streamingAssetsPath + @"/event.xml";

        var serializer = new XmlSerializer(type);
        XmlSerializerNamespaces myNameSpaces = new XmlSerializerNamespaces();
        myNameSpaces.Add("", "");
        var encoding = Encoding.GetEncoding("UTF-8");

        using (StreamWriter stream = new StreamWriter(filePath, false, encoding))
        {
            serializer.Serialize(stream, req, myNameSpaces);
        }

        XmlDocument doc = new XmlDocument();
        doc.Load(filePath);

        Debug.Log(postXMLData("https://smetrics.royalcaribbean.com/b/ss//6", doc.OuterXml));
    }

    public void SetXmlType(object req)
    {
        switch (xmlType)
        {
            case XMLType.MainPage:
                request = mainPage as mainPageRequest;
                break;

            case XMLType.Timers:
                request = timer as timerRequest;
                break;

            case XMLType.Overlays:
                request = overlay as overlayRequest;
                break;

            case XMLType.Widgets:
                request = widget_close as widget_closeRequest;
                break;

            case XMLType.Isolate:
                request = isolateDecks as isolateDecksRequest;
                break;

            case XMLType.Minimize:
                request = min_conf as min_confRequest;
                break;

            case XMLType.Close:
                request = widget_close as widget_closeRequest;
                break;

            case XMLType.CloseConfirmation:
                request = min_conf as min_confRequest;
                break;
        }


    }

    public static string postXMLData(string destURL, string requestXML)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destURL);
        byte[] bytes;
        bytes = System.Text.Encoding.ASCII.GetBytes(requestXML);
        request.ContentType = "text/xml; encoding=utf-8";
        request.ContentLength = bytes.Length;
        request.Method = "POST";
        Stream requestStream = request.GetRequestStream();
        requestStream.Write(bytes, 0, bytes.Length);
        requestStream.Close();
        HttpWebResponse response;
        response = (HttpWebResponse)request.GetResponse();
        if (response.StatusCode == HttpStatusCode.OK)
        {
            Stream responseStream = response.GetResponseStream();
            string responseSTR = new StreamReader(responseStream).ReadToEnd();
            return responseSTR;
        }
        return null;
    }

    public static string GetIPAddress()
    {
        IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());

        IPAddress[] addr = ipEntry.AddressList;

        return addr[1].ToString();
    }

    string RoundDown(DateTime DT, TimeSpan TS)
    {
        var delta = DT.Ticks % TS.Ticks;
        return new DateTime(DT.Ticks - delta, DT.Kind).ToString("yyyy-MM-ddTHH:mm:ss") + "+0000";
    }

    public class Factory : PlaceholderFactory<XMLType, XmlWriter> { }
}
