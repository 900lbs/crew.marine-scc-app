using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Object = UnityEngine.Object;
using Zenject;

public class XMLWriterFactory : IFactory<Object, XMLType, XMLWriterDynamic>
{
    DiContainer container;
    XMLBaseFactory xmlFactory;

    //SignalBus _signalBus;
    [Inject]
    public XMLWriterFactory(DiContainer containerRef, XMLBaseFactory xmlBaseFact/* , SignalBus signal */)
    {
        container = containerRef;
        xmlFactory = xmlBaseFact;
        //_signalBus = signal;
    }

    public XMLWriterDynamic Create(Object target, XMLType type)
    {
        XMLWriterDynamic newWriter = container.InstantiateComponent<XMLWriterDynamic>((GameObject)target);
        
        /* if(newWriter._signalBus == null)
            newWriter._signalBus = _signalBus; */
        newWriter.Request = xmlFactory.Create(type);
        return newWriter;
    }
}
