using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EraseAllPromptHandler : PromptMenu
{
    [Zenject.Inject]
    AnnotationManager annotationManager;
    public GameObject AdminLayout;
    protected override void PromptMenuChanged(Signal_MainMenu_OnPromptMenuChanged signal)
    {
        base.PromptMenuChanged(signal);

        if (Photon.Pun.PhotonNetwork.IsMasterClient && annotationManager.GetCurrentPropertyState().HasFlag(ShipRoomProperties.Annotations))
        {
            AdminLayout.SetActive(true);
            AssignMessage("SELECT LAYERS TO DELETE.");
        }
        else if(Photon.Pun.PhotonNetwork.IsMasterClient && annotationManager.GetCurrentPropertyState().HasFlag(ShipRoomProperties.GAOverlay))
        {
            AdminLayout.SetActive(false);
            AssignMessage("ARE YOU SURE YOU WANT TO DELETE ARRANGEMENT EDITS?");
        }
        else
        {
            AdminLayout.SetActive(false);
            AssignMessage("ARE YOU SURE YOU WANT TO DELETE ANNOTATIONS?");
        }
    }


}
