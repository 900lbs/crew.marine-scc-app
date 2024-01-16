using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
public class PromptRevertGAButton : PromptAction
{
    [Zenject.Inject]
    AnnotationManager annotationManager;
    public ColorProfile ColorProfile;

    void OnValidate()
    {
        ColorProfile.OnValidate();
    }

    protected override void BTN_OnClick()
    {
        if(annotationManager.GetCurrentPropertyState().HasFlag(ShipRoomProperties.GAOverlay))
            base.BTN_OnClick();

        ColorProfile.ColorFlash(ActiveState.Enabled, ActiveState.Selected);
    }
}
