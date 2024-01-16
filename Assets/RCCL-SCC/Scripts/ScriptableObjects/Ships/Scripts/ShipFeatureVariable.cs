using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(menuName = "Feature/New Feature", fileName = "New Feature")]
public class ShipFeatureVariable : ScriptableObject
{
    public ShipFeatureData data = new ShipFeatureData(ShipID.None, null, FeatureTool.None, ColumnID.Left, null);

    private void OnValidate()
    {
        if (data.FeatureName != "")
        {
            name = data.FeatureName.ToLower();
        }
    }
}
