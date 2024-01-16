using System.Diagnostics;
using System.Threading;
// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Josh Bowers
// Created          : 07-01-2019
//
// Last Modified By : Josh Bowers
// Last Modified On : 07-01-2019
// ***********************************************************************
// <copyright file="XMLWriterDynamic.cs" company="900lbs of Creative">
//     Copyright (c) 900lbs of Creative. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

using Zenject;
using System;
using Debug = UnityEngine.Debug;
/// <summary>
/// A version of <see cref="XMLWriter"/> that allows for easier creation via script, use this for instantiating 
/// new objects with this component. 
/// Implements the <see cref="UI_Button" />
/// </summary>
/// <seealso cref="UI_Button" />
public class XMLWriterDynamic : MonoBehaviour
{
    XMLBaseFactory xmlBaseFactory;
   
    CancellationTokenSource cts;

    SignalBus _signalBus;

    [Inject]
    public void Construct(XMLBaseFactory baseFactory, CancellationTokenSource c, SignalBus signal)
    {
        xmlBaseFactory = baseFactory;
        cts = c;
        _signalBus = signal;
    }

    public XMLType xmlType;
    /// <summary>
    /// The request
    /// </summary>
    [SerializeField]
    public xmlBase Request;

    /// <summary>
    /// The button
    /// </summary>
    [HideInInspector]
    public Button Button;

    /// <summary>
    /// The file path
    /// </summary>
    public string FilePath;

    public bool CanSendHTTP = true;


    /*----------------------------------------------------------------------------------------------------------------------------*/

    void Start()
    {
        _signalBus.Subscribe<Signal_HTTP_OnPostCapabilitiesChanged>(PostCapabilitiesChanged);
    }

    private void OnDestroy()
    {
        _signalBus.Unsubscribe<Signal_HTTP_OnPostCapabilitiesChanged>(PostCapabilitiesChanged);
    }

    void PostCapabilitiesChanged(Signal_HTTP_OnPostCapabilitiesChanged signal)
    {
        CanSendHTTP = signal.IsActivated;
    }
    /// <summary>
    /// Attempts to convert the Request to an xmlBase and await CustomSave() if it's overriden on the extended class.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <returns>Task.</returns>
    public async Task AttemptCustomSave(params object[] parameters)
    {
        //Debug.Log("Attempting Custom Save.", this);
        if (!cts.IsCancellationRequested)
        {
            xmlBase xml = (xmlBase)Request;
            await xml.CustomSave(parameters);
        }
    }

    /// <summary>
    /// Saves this instance.
    /// </summary>
    /// <returns>Task.</returns>
    public async Task Save()
    {
        try
        {
#if !UNITY_EDITOR
            if (!cts.IsCancellationRequested && CanSendHTTP)
            {
                FilePath = Application.streamingAssetsPath + @"/event.xml";

                var serializer = new XmlSerializer(Request.GetType());
                XmlSerializerNamespaces myNameSpaces = new XmlSerializerNamespaces();
                myNameSpaces.Add("", "");
                var encoding = Encoding.GetEncoding("UTF-8");

                using (StreamWriter stream = new StreamWriter(FilePath, false, encoding))
                {
                    serializer.Serialize(stream, Request, myNameSpaces);
                }

                XmlDocument doc = new XmlDocument();
                doc.Load(FilePath);

                Debug.Log(await postXMLData("https://smetrics.royalcaribbean.com/b/ss//6", doc.OuterXml));
                Debug.Log(doc.OuterXml);
                //Debug.Log(Request.GetType().ToString() + " was sent via xml.", this);
            }

#elif UNITY_EDITOR
            if (!cts.IsCancellationRequested && CanSendHTTP)
            {
                FilePath = Application.streamingAssetsPath + @"/event.xml";

                var serializer = new XmlSerializer(Request.GetType());
                XmlSerializerNamespaces myNameSpaces = new XmlSerializerNamespaces();
                myNameSpaces.Add("", "");
                var encoding = Encoding.GetEncoding("UTF-8");

                using (StreamWriter stream = new StreamWriter(FilePath, false, encoding))
                {
                    serializer.Serialize(stream, Request, myNameSpaces);
                }

                XmlDocument doc = new XmlDocument();
                doc.Load(FilePath);
                Debug.Log(await postXMLData("https://smetrics.royalcaribbean.com/b/ss//6", doc.OuterXml));

                Debug.Log(doc.OuterXml);
            }
#endif
        }
        catch (System.Exception)
        {
            if (CanSendHTTP)
                //_signalBus.Fire<Signal_HTTP_OnPostCapabilitiesChanged>(new Signal_HTTP_OnPostCapabilitiesChanged(false));
            Debug.Log("XML post failed, cancelling now.", this);
            throw;
        }
        await new WaitForEndOfFrame();

    }


    /// <summary>
    /// Posts the XML data.
    /// </summary>
    /// <param name="destURL">The dest URL.</param>
    /// <param name="requestXML">The request XML.</param>
    /// <returns>Task&lt;System.String&gt;.</returns>
    public async Task<string> postXMLData(string destURL, string requestXML)
    {
        if (!cts.IsCancellationRequested && CanSendHTTP)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = TrustCertificate;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destURL);
                //Debug.Log("XML Request full address: " + request.Address, this);
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
                    //Debug.Log("Response string: " + responseSTR, this);
                    await new WaitForEndOfFrame();
                    return responseSTR;
                }
            }
            catch (System.Exception)
            {
                Debug.Log("Posting XML data failed, suspending indefinitely.", this);
                if(CanSendHTTP)
                    //_signalBus.Fire<Signal_HTTP_OnPostCapabilitiesChanged>(new Signal_HTTP_OnPostCapabilitiesChanged(false));
                return null;
            }
        }
        return null;
    }

    private static bool TrustCertificate(object sender, X509Certificate x509Certificate, X509Chain x509Chain, SslPolicyErrors sslPolicyErrors)
    {
        // all Certificates are accepted
        return true;
    }

    /// <summary>
    /// Creates a dynamic xml writer, be sure to feed the gameobject that requires the component through.
    /// Implements the <see cref="Zenject.PlaceholderFactory{UnityEngine.Object, XMLType, XMLWriterDynamic}" />
    /// </summary>
    /// <seealso cref="Zenject.PlaceholderFactory{UnityEngine.Object, XMLType, XMLWriterDynamic}" />
    public class Factory : PlaceholderFactory<UnityEngine.Object, XMLType, XMLWriterDynamic> { }
}
