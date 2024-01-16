using UnityEngine;
using Zenject;

#if SCC_2_5
public static class DebugUtils
{
    [Inject] static readonly NetworkClientManager networkManager;
    public static void Log(DebugType type, Color color, string message, Object context = null)
    {
        string col = ColorUtility.ToHtmlStringRGB(color);

        if(type == networkManager.DebugDisplayType)
            Debug.Log("<color=" + col + ">" + message + "</color>", context);
    }
}
#endif