using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class LegendFeature : UI_Button
{

    public ColorProfile[] ColorProfiles;

    LegendFeatureData legendData;
    public LegendFeatureData LegendData
    {
        get
        {
            return legendData;
        }

        set
        {
            legendData = value;
            UpdateLegend();
        }
    }

    public TextMeshProUGUI tabText;

    //[HideInInspector]
    public LegendsHandler legendsHandler;

    private void OnValidate()
    {
        int profileLength = ColorProfiles.Length;
        for (int i = 0; i < profileLength; ++i)
        {
            ColorProfiles[i].OnValidate();
        }
    }

    void Start()
    {
        tabText = GetComponentInChildren<TextMeshProUGUI>();
    }

    protected override void BTN_OnClick()
    {
        base.BTN_OnClick();

        legendsHandler.TabSelected(this);
    }

    protected override void OnStateChange()
    {
        int profileLength = ColorProfiles.Length;
        for (int i = 0; i < profileLength; ++i)
        {
            ColorProfiles[i].StateChange(State);
        }
    }

    internal void UpdateLegend()
    {
        tabText.text = LegendData.LegendName;
        name = LegendData.LegendName;
        CalculateButtonSize();
    }

    internal void CalculateButtonSize()
    {
        int nameLength = LegendData.LegendName.Length;
        RectTransform rect = GetComponent<RectTransform>();
        float xValue = nameLength * tabText.fontSize;
        rect.sizeDelta = new Vector2((xValue < 150) ? 150 : xValue, rect.sizeDelta.y);
    }
}
