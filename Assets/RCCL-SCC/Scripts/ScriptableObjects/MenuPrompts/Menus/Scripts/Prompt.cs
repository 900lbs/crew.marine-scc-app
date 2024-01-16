using UnityEngine;
using UnityEngine.UI;
using PrimitiveFactory.ScriptableObjectSuite;


using Zenject;

using Object = UnityEngine.Object;
public class Prompt : RuntimeSet<Prompt>
{
    #region Injection Construction
    [Inject]
    PromptMenu.Factory promptMenuFactory;

    #endregion
    public PromptMenu MenuPrefab;

    public GameObject[] MenuButtons;

    public string Message;

    PromptMenu menu;

    public override void OnBeforeSerialize() { }
    public override void OnAfterDeserialize() { }

    public void PromptAction(PromptMenuAction action)
    {
        switch (action)
        {
            case PromptMenuAction.Prompt:
                CreatePrompt();
                break;

            case PromptMenuAction.Cancel:
            case PromptMenuAction.Confirmation:
                if (menu != null)
                    DestroyPrompt();
                break;

            default:
                break;
        }
    }

    public void CreatePrompt()
    {
        if(promptMenuFactory == null)
        {
            Debug.LogError("Could not find PromptMenu factory.");
            return;
        }
        Debug.Log("Creating menu prompt.", this);
        menu = promptMenuFactory.Create(MenuPrefab.gameObject);
        menu.TargetPrompt = this;
        menu.SpawnButtons(MenuButtons);
        menu.AssignMessage(Message);
        menu.FadeCanvas(true);
        Debug.Log("Finished creating menu prompt.", this);
    }

    public void UpdatePrompt(string message)
    {
        Message = message;

        if (menu != null)
            menu.AssignMessage(Message);
    }

    public void DestroyPrompt()
    {
        if (menu != null)
        {
            Destroy(menu);
            menu = null;
        }
    }
}