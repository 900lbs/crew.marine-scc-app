using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;
public class XMLBaseFactory : IFactory<XMLType, xmlBase>
{

    readonly Settings xmlSettings;
    readonly XMLClose.Factory closeFactory;
    readonly XMLCloseConfirmation.Factory closeConfirmFactory;
    readonly XMLIsolate.Factory isolateFactory;
    readonly XMLMainPage.Factory mainPageFactory;
    readonly XMLMinimize.Factory minimizeFactory;
    readonly XMLOverlays.Factory overlaysFactory;
    readonly XMLTimers.Factory timersFactory;
    readonly XMLWidgets.Factory widgetsFactory;

    [Inject]
    public XMLBaseFactory(Settings xmlBaseCla,
    XMLClose.Factory closeFact,
    XMLCloseConfirmation.Factory closeConfirmFact,
    XMLIsolate.Factory isolateFact,
    XMLMainPage.Factory mainPageFact,
    XMLMinimize.Factory minimizeFact,
    XMLOverlays.Factory overlaysFact,
    XMLTimers.Factory timersFact,
    XMLWidgets.Factory widgetsFact)
    {
        xmlSettings = xmlBaseCla;
        closeFactory = closeFact;
        closeConfirmFactory = closeConfirmFact;
        isolateFactory = isolateFact;
        mainPageFactory = mainPageFact;
        minimizeFactory = minimizeFact;
        overlaysFactory = overlaysFact;
        timersFactory = timersFact;
        widgetsFactory = widgetsFact;
    }
    public xmlBase Create(XMLType type)
    {
        switch (type)
        {
            case XMLType.Close:
                XMLClose close = closeFactory.Create();
                xmlSettings.XmlBase.CopyBaseTo(close);
                return close;

            case XMLType.CloseConfirmation:
                XMLCloseConfirmation closeConfirm = closeConfirmFactory.Create();
                xmlSettings.XmlBase.CopyBaseTo(closeConfirm);
                return closeConfirm;
            case XMLType.Isolate:
                XMLIsolate isolate = isolateFactory.Create();
                xmlSettings.XmlBase.CopyBaseTo(isolate);
                return isolate;
            case XMLType.MainPage:
                XMLMainPage main = mainPageFactory.Create();
                xmlSettings.XmlBase.CopyBaseTo(main);
                return main;

            case XMLType.Minimize:
                XMLMinimize minimize = minimizeFactory.Create();
                xmlSettings.XmlBase.CopyBaseTo(minimize);
                return minimize;

            case XMLType.Overlays:
                XMLOverlays overlays = overlaysFactory.Create();
                xmlSettings.XmlBase.CopyBaseTo(overlays);
                return overlays;

            case XMLType.Timers:
                XMLTimers timers = timersFactory.Create();
                xmlSettings.XmlBase.CopyBaseTo(timers);
                return timers;

            case XMLType.Widgets:
                XMLWidgets widgets = widgetsFactory.Create();
                xmlSettings.XmlBase.CopyBaseTo(widgets);
                return widgets;

            default:
                return null;
        }
    }
}
