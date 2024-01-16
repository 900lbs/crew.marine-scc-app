using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class KillCardFactory : IFactory<NetworkKillCardObject, Task<KillCardClass>>
{
    [Inject]
    KillCardPhotoManager killCardPhotoManager;
    public async Task<KillCardClass> Create(NetworkKillCardObject value)
    {
        //await new WaitForBackgroundThread();

        object[] dataArray = (object[]) value.NetData;
        KillCardClass killCard = new KillCardClass();

        killCard.title = (string) dataArray[0];
        killCard.type = (string) dataArray[1];
        killCard.deckNumber = (string) dataArray[2];
        killCard.mfz = (string) dataArray[3];
        killCard.lastEdit = (string) dataArray[4];
        
        killCard.refPhotos = new List<string>();
        string[] refPhotos = (string[]) dataArray[5];

        if (refPhotos.Length > 0)
        {
            foreach (var item in refPhotos)
            {
                killCard.refPhotos.Add(item);
                Debug.Log("KillCardFactory Found image: " + item);
            }
        }
        else
        {
            Debug.Log("No ref images found for new kill card.");
        }

        killCard.killCardSteps = new List<KillCardStepsClass>();

        /*----------------------------------------------------------------------------------------------------------------------------*/
        //Step creation
        Hashtable stepsList = (Hashtable) dataArray[6];
        foreach (var step in stepsList)
        {
            KillCardStepsClass newStep = new KillCardStepsClass();
            newStep.DeserializeFromNetwork((string[]) step.Value);

            killCard.killCardSteps.Add(newStep);
        }

        /*----------------------------------------------------------------------------------------------------------------------------*/
        
        // Get back on the main thread and return
        await new WaitForUpdate();
        return killCard;
    }
}