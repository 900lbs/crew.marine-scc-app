using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace ZenFulcrum.EmbeddedBrowser
{

    public partial class BrowserUpdater : Browser
    {
        [InjectOptional (Id = "CurrentShip")]
        [SerializeField] ShipVariable currentShip;


        public void SetURL ()
        {
            Debug.Log ("Assigning EMustering url: " + currentShip?.Ship.EMusteringURL);
            Url = currentShip?.Ship.EMusteringURL;
        }
    }
}