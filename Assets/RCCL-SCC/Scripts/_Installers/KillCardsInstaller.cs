using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using Zenject;

public class KillCardsInstaller : MonoInstaller
{
    public KillCardManager KillCardManagerObject;
    public KillCardPhotoManager KillCardPhotoManagerObject;
    public KillCardDeckFilter KillCardDeckFilterObject;

    public KillCardNetworkingBuilder KillCardNetworkingBuilderObject;

    public UpdateLastEdit updateLastEdit;

    public override void InstallBindings()
    {
        BindManagers();
        BindHelpers();
        BindFactories();
    }

    void BindManagers()
    {
        Container.BindInterfacesAndSelfTo<KillCardMainHolder>()
        .AsSingle()
        .WhenInjectedInto<KillCardManager>()
        .NonLazy();

        Container.Bind<KillCardManager>()
        .FromComponentOn(KillCardManagerObject.gameObject)
        .AsSingle()
        .NonLazy();

        Container.Bind<KillCardPhotoManager>()
        .FromComponentOn(KillCardPhotoManagerObject.gameObject)
        .AsSingle()
        .NonLazy();

    }

    void BindHelpers()
    {
        Container.Bind<KillCardDeckFilter>()
        .FromComponentOn(KillCardDeckFilterObject.gameObject)
        .AsSingle();

        Container.Bind<KillCardSendOverNetwork>()
        .FromNew()
        .AsSingle();

/*         Container.Bind<KillCardNetworkingHandler>()
        .FromNew()
        .AsSingle(); */

        Container.Bind<KillCardNetworkingBuilder>()
        .FromComponentOn(KillCardNetworkingBuilderObject.gameObject)
        .AsSingle();

        Container.Bind<UpdateLastEdit>()
        .FromComponentOn(updateLastEdit.gameObject)
        .AsSingle();
    }

    void BindFactories()
    {
        Container.BindFactory<NetworkKillCardObject, Task<KillCardClass>, KillCardClass.Factory>()
        .FromFactory<KillCardFactory>();
    }
}
