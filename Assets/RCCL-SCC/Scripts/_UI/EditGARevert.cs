using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Zenject;
[RequireComponent(typeof(Button))]
public class EditGARevert : UI_Button
{
    BuildSaveGAEdits gaEdits;

    [Inject]
    public void Construct(BuildSaveGAEdits edits)
    {
        gaEdits = edits;
    }

    protected override void BTN_OnClick()
    {
        gaEdits.ResetPrefabs();
    }

    protected override void OnStateChange()
    {
        
    }
}
