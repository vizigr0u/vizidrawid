using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class Spline
{
    public static int MaxControlPoints => 512;

    public Vector4[] controlPoints;

    public Spline(Vector2[] controlPoints)
    {
        this.controlPoints = controlPoints.Select(v2 => (Vector4)v2).ToArray();
    }

    static Vector2[] testControlPoints = new Vector2[]
    {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 2),
            new Vector2(0, 2),
            new Vector2(-1, 3),
            new Vector2(-2, 3),
            new Vector2(-2, 3),
            new Vector2(-4, 3),
    };

    public static Spline CreateTest()
    {
        return new Spline(testControlPoints);
    }
}
