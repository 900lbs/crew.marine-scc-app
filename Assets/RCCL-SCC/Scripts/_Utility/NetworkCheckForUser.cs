using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;

public class NetworkCheckForUser : MonoBehaviour
{
    SignalBus _signalBus;

    [Inject]
    public void Construct (SignalBus signal)
    {
        _signalBus = signal;
    }

    [SerializeField] CanvasGroup cg;
    [SerializeField] ColorProfile profile;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    async void Awake ()
    {
        _signalBus.Subscribe<Signal_NetworkClient_OnActiveUsersUpdated> (ActiveUsersUpdated);

        await profile.StateChange (ActiveState.Selected);

        if (cg == null)
        {
            try
            {
                cg = GetComponent<CanvasGroup> ();
            }
            catch (System.Exception)
            {
                Debug.LogError ("Canvas group was not found.", this);
                throw;
            }
        }
        cg.alpha = 0;
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy ()
    {
        _signalBus.TryUnsubscribe<Signal_NetworkClient_OnActiveUsersUpdated> (ActiveUsersUpdated);
    }

    async void ActiveUsersUpdated (Signal_NetworkClient_OnActiveUsersUpdated signal)
    {
        //Debug.Log("<color=#e5e5e5>" + signal.Users.ToString() + "</color>");
        if ((signal.Users.HasFlag (UserNameID.ShoresideOperations) | signal.Users.HasFlag (UserNameID.IncidentCommandCenter)) && !DOTween.IsTweening (cg))
        {

            await AssignColorBasedOnUser((signal.Users.HasFlag(UserNameID.ShoresideOperations) ? UserNameID.ShoresideOperations : UserNameID.IncidentCommandCenter));
            cg.DOFade (1, 0.25f).OnComplete (() => cg.DOFade (0, 0.25f).SetDelay (5));
        }
        else
        {
            if (DOTween.IsTweening (cg))
                DOTween.Kill (cg);

            cg.alpha = 0;
        }
    }

    async Task AssignColorBasedOnUser (UserNameID user)
    {
        for (int i = 0; i < profile.Components.Length; i++)
        {
            await UIHelpers.ToggleColor (profile.Components[i], AnnotationManager.GetLineColorBasedOnUser (user));
        }

    }

}