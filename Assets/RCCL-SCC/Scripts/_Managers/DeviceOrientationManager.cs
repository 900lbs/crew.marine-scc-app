using UnityEngine;

public class DeviceOrientationManager : MonoBehaviour
{
    private const float ORIENTATION_CHECK_INTERVAL = 0.5f;

    private float nextOrientationCheckTime;

    private static ScreenOrientation m_currentOrientation;
    public static ScreenOrientation CurrentOrientation
    {
        get
        {
            return m_currentOrientation;
        }
        private set
        {
            if (m_currentOrientation != value)
            {
                m_currentOrientation = value;
                Screen.orientation = value;

                OnScreenOrientationChanged?.Invoke(value);
            }
        }
    }

    public static bool AutoRotateScreen = true;
    public static event System.Action<ScreenOrientation> OnScreenOrientationChanged = null;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Init()
    {
        DontDestroyOnLoad(new GameObject("DeviceOrientationManager", typeof(DeviceOrientationManager)));
    }

    void Awake()
    {
        m_currentOrientation = Screen.orientation;
        nextOrientationCheckTime = Time.realtimeSinceStartup + 1f;
    }

    void Update()
    {
        if (!AutoRotateScreen)
            return;

        if (Time.realtimeSinceStartup >= nextOrientationCheckTime)
        {
            DeviceOrientation orientation = Input.deviceOrientation;
            if (orientation == DeviceOrientation.Portrait || orientation == DeviceOrientation.PortraitUpsideDown ||
                orientation == DeviceOrientation.LandscapeLeft || orientation == DeviceOrientation.LandscapeRight)
            {
                if (orientation != DeviceOrientation.LandscapeLeft)
                {
                    CurrentOrientation = ScreenOrientation.LandscapeLeft;
                }
            }

            nextOrientationCheckTime = Time.realtimeSinceStartup + ORIENTATION_CHECK_INTERVAL;
        }
    }
}