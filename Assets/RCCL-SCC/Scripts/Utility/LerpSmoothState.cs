using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpSmoothState : MonoBehaviour
{
    public enum Smoothing
    {
        eastOut,
        easeIn,
        exponential,
        smoothStep,
        smootherStep
    }

    public static float SmoothLerp(float t, Smoothing smoothState)
    {
        switch(smoothState)
        {
            case Smoothing.eastOut:
                t = Mathf.Sin(t * Mathf.PI * 0.5f);
                break;

            case Smoothing.easeIn:
                t = 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
                break;

            case Smoothing.exponential:
                t = t * t;
                break;

            case Smoothing.smoothStep:
                t = t * t * (3f - 2f*t);
                break;

            case Smoothing.smootherStep:
                t = t * t * t * (t * (6f * t - 15f) + 10f);
                break;
        }

        return t;
    }
}
