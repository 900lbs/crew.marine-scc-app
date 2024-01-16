using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DisableButton))]
public class UserProfileSelectAllButtons : UI_Button
{
    public UserProfileSelectButtonHandler ProfileButtonHandler;

    /*     public ColorProfile[] ColorProfiles;

        void OnValidate()
        {
            int colorProfilesCount = ColorProfiles.Length;
            for (int i = 0; i < colorProfilesCount; ++i)
            {
                ColorProfiles[i].OnValidate();
            }
        } */
    protected override void Awake()
    {
        base.Awake();
        if (ProfileButtonHandler == null)
            ProfileButtonHandler = GetComponentInParent<UserProfileSelectButtonHandler>();
    }

    protected override void BTN_OnClick()
    {
        base.BTN_OnClick();

        ProfileButtonHandler.AllUsersSelected();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    protected override void OnStateChange()
    {
        throw new System.NotImplementedException();
    }
}
