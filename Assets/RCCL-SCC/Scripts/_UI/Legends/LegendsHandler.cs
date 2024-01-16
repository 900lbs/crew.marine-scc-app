using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using Zenject;

public class LegendsHandler : PromptMenu
{
    [ReadOnly]
    [Inject(Id = "CurrentShip")]
    public ShipVariable CurrentShip;

    public Image LegendImage;
    public Button FirstTab;
    public Button OtherTab;

    private List<LegendFeature> legendList;

    public void Start()
    {
        legendList = new List<LegendFeature>();
    }

    public async Task SpawnLegends()
    {
        int legendsCount = CurrentShip.Ship.Legends.Length;
        for (int i = 0; i < legendsCount; ++i)
        {
            LegendFeature newLegend = Instantiate((i == 0) ? FirstTab : OtherTab, ButtonsParent).GetComponent<LegendFeature>(); ;
            newLegend.State = (i == 0) ? ActiveState.Selected : ActiveState.Enabled;
            newLegend.LegendData = CurrentShip.Ship.Legends[i];
            newLegend.legendsHandler = this;
            if (legendList == null)
                legendList = new List<LegendFeature>();

            legendList.Add(newLegend);
            //Debug.Log(legendList.Count + " legends found", this);
            if (i == 0)
            {
                TabSelected(newLegend);
            }
        }

        await new WaitForEndOfFrame();
    }

    private void OnDestroy()
    {
        RemoveAllLegends();
    }

    public void RemoveAllLegends()
    {
        LegendFeature[] buttonChildren = ButtonsParent.GetComponentsInChildren<LegendFeature>();

        int buttonChildCount = buttonChildren.Length;
        for (int i = 0; i < buttonChildCount; ++i)
        {
            legendList.Remove(buttonChildren[i]);
            Destroy(buttonChildren[i].gameObject);
        }
    }

    public void TabSelected(LegendFeature legend)
    {
        int legendListCount = legendList.Count;
        for (int i = 0; i < legendListCount; ++i)
        {
            if (legendList[i] == legend)
            {
                legendList[i].State = ActiveState.Selected;

                LegendImage.sprite = legend.LegendData.LegendImage;
                if (LegendImage.preserveAspect == false)
                    LegendImage.preserveAspect = true;
            }
            else
            {
                legendList[i].State = ActiveState.Enabled;
            }
        }
    }

    internal void SetupLegendButton(LegendFeature legend)
    {
    }
}