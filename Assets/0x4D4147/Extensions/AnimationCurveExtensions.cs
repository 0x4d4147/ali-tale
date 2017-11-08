using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimationCurveExtensions
{
    public static float Duration(this AnimationCurve curve)
    {
        if (curve.length == 0) return 0f;
        var keys = curve.keys;
        var finalKey = keys[curve.length - 1];
        return finalKey.time;
    }
}
