using MLAgents;
using UnityEngine;
using SwordGC.AI.Goap;

public class GameMonitor : Monitor
{
    public const string LOG_LOCATION_TOP_LEFT = "LOG_LOCATION_TOP_LEFT";
    public const string LOG_LOCATION_TOP_CENTER = "LOG_LOCATION_TOP_CENTER";
    public const string LOG_LOCATION_TOP_RIGHT = "LOG_LOCATION_TOP_RIGHT";
    public const string LOG_LOCATION_BOTTOM_LEFT = "LOG_LOCATION_BOTTOM_LEFT";
    public const string LOG_LOCATION_BOTTOM_CENTER = "LOG_LOCATION_BOTTOM_CENTER";
    public const string LOG_LOCATION_BOTTOM_RIGHT = "LOG_LOCATION_BOTTOM_RIGHT";

    private static Transform topLeft;
    private static Transform topCenter;
    private static Transform topRight;
    private static Transform bottomLeft;
    private static Transform bottomCenter;
    private static Transform bottomRight;
    private static bool _wasSetup;

    public static void LogToPos(string key, string stringValue, string position = LOG_LOCATION_BOTTOM_CENTER,
                                Camera camera = null) {
        _handleLogging(position, camera, key, stringValue);
    }

    public static void LogToPos(string key, float floatValue, string position = LOG_LOCATION_BOTTOM_CENTER,
                                Camera camera = null) {
        _handleLogging(position, camera, key, null, floatValue);
    }

    public static void LogToPos(string key, float[] floatArray, string position = LOG_LOCATION_BOTTOM_CENTER,
                                Camera camera = null) {
        _handleLogging(position, camera, key, null, float.NaN, floatArray);
    }

    private static void _handleLogging(string position, Camera camera, string key, string stringValue = null,
                                       float floatValue = float.NaN, float[] floatArray = null)
    {
        if (!_wasSetup) _setup();

        Transform screenPosition;
        switch (position)
        {
            case LOG_LOCATION_TOP_LEFT:
                screenPosition = topLeft;
                break;
            case LOG_LOCATION_TOP_CENTER:
                screenPosition = topCenter;
                break;
            case LOG_LOCATION_TOP_RIGHT:
                screenPosition = topRight;
                break;
            case LOG_LOCATION_BOTTOM_LEFT:
                screenPosition = bottomLeft;
                break;
            case LOG_LOCATION_BOTTOM_CENTER:
                screenPosition = bottomCenter;
                break;
            case LOG_LOCATION_BOTTOM_RIGHT:
                screenPosition = bottomRight;
                break;
            default:
                Debug.LogError("Did not find screenposition for monitor position type " + position + ".");
                return;
        }

        if (stringValue != null)
        {
            Log(key, stringValue, 0f, screenPosition, camera);
        } else if (!float.IsNaN(floatValue))
        {
            Log(key, floatValue, 0f, screenPosition, camera);
        } else if (floatArray != null)
        {
            Log(key, floatArray, null, screenPosition, DisplayType.Independent, camera);
        } else
        {
            Debug.LogWarning("Invalid logging values encountered in GameMonitor.");
        }
    }

    private static void _setup()
    {
        _wasSetup = true;

        topLeft = GameObject.Find(LOG_LOCATION_TOP_LEFT).transform;
        topCenter = GameObject.Find(LOG_LOCATION_TOP_CENTER).transform;
        topRight = GameObject.Find(LOG_LOCATION_TOP_RIGHT).transform;
        bottomLeft = GameObject.Find(LOG_LOCATION_BOTTOM_LEFT).transform;
        bottomCenter = GameObject.Find(LOG_LOCATION_BOTTOM_CENTER).transform;
        bottomRight = GameObject.Find(LOG_LOCATION_BOTTOM_RIGHT).transform;

        if (GameAcademy.USE_GOAP) 
        {
            scaler = 0.5f;
        } else
        {
            scaler = 0.7f;
            foreach (string action in NpcActions.ALL_ACTIONS) LogToPos(action, "0", LOG_LOCATION_TOP_LEFT);
        }
    }
}