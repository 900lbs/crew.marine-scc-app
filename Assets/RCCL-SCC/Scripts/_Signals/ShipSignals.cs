using Zenject;

public class Signal_ShipManager_OnFeatureToolCreated
{
    public Signal_ShipManager_OnFeatureToolCreated(FeatureTool tool, ShipFeatureData data)
    {
        Tool = tool;
        Data = data;
    }

    public FeatureTool Tool { get; private set; }
    public ShipFeatureData Data { get; private set; }
}

