using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class PromptCloseButton : PromptAction
{
    XMLWriterDynamic.Factory xmlWriterFactory;

    [Inject]
    public void Construct(XMLWriterDynamic.Factory xmlFact)
    {
        xmlWriterFactory = xmlFact;
    }
    
    XMLWriterDynamic xmlWriter;

    Task customSave;


    public override void Start()
    {
        base.Start();
        
        if (xmlWriter == null)
            xmlWriter = xmlWriterFactory.Create(gameObject, (AssignedMenuAction == PromptMenuAction.Confirmation) ? XMLType.CloseConfirmation : XMLType.Close);
    }
    protected override void OnStateChange()
    {

    }

    protected async override void BTN_OnClick()
    {
        base.BTN_OnClick();
        
        Debug.Log("Clicked: " + AssignedMenuAction.ToString(), this);
        switch (AssignedMenuAction)
        {
            case PromptMenuAction.Prompt:
                _signalBus.Fire<Signal_MainMenu_OnPromptMenuChanged>(new Signal_MainMenu_OnPromptMenuChanged(PromptMenuType.Quit));
                await xmlWriter.AttemptCustomSave("SCC Close", "main:sccclose");
                await xmlWriter.Save();
                break;

            case PromptMenuAction.Cancel:
                _signalBus.Fire<Signal_MainMenu_OnPromptMenuChanged>(new Signal_MainMenu_OnPromptMenuChanged(PromptMenuType.Quit));
                string linkName = "SCC Close No";
                string eVar175 = "main:sccclose_no";
                await xmlWriter.AttemptCustomSave(linkName, eVar175);
                await xmlWriter.Save();
                break;

            case PromptMenuAction.Confirmation:
                string linkNameConf = "SCC Close Yes";
                string eVar175Conf = "main:sccclose_yes";
                Application.Quit();
                customSave = Task.Run(async () => await xmlWriter.AttemptCustomSave(linkNameConf, eVar175Conf));
                await xmlWriter.Save();

                while (!customSave.IsCompleted)
                {
                    Debug.Log("Waiting on custom save to finish.", this);
                    await new WaitForEndOfFrame();
                }
                break;
        }
    }
}
