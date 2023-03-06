using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Extensions
{
    public static bool ApproximateEquals(this Vector3 me, Vector3 other)
    {
        return Mathf.Abs(me.x - other.x) < 0.001f 
            && Mathf.Abs(me.y - other.y) < 0.001f
            && Mathf.Abs(me.z - other.z) < 0.001f;
    }
}

