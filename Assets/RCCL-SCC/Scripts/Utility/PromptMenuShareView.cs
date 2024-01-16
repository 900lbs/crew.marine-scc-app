using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptMenuShareView : PromptMenu
{
    public bool IsKidTrackingShare;
    protected override void PromptMenuChanged(Signal_MainMenu_OnPromptMenuChanged signal)
    {
        if (signal.PromptType == PromptType && !IsKidTrackingShare)
        {
            if(signal.OptionalParams.Length > 0)
                AssignMessage((string) signal.OptionalParams[0]);
            FadeCanvas((signal.PromptType == PromptType && cg.alpha == 0));
        }

/*         else if (signal.OptionalParams.Length == 0 && IsKidTrackingShare)
            FadeCanvas((signal.PromptType == PromptType && cg.alpha == 0)); */
    }

}