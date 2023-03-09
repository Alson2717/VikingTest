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
    public static bool ApproximateEqualsIgnoreY(this Vector3 me, Vector3 other)
    {
        return Mathf.Abs(me.x - other.x) < 0.001f
            && Mathf.Abs(me.z - other.z) < 0.001f;
    }

    public static T ExtractRandom<T>(this IList<T> me)
    {
        return me.ExtractAt(me.RandomIndex());
    }
    public static T ExtractAt<T>(this IList<T> me, int index)
    {
        T value = me[index];
        me.RemoveAt(index);
        return value;
    }
    public static int RandomIndex<T>(this IList<T> me)
    {
        return UnityEngine.Random.Range(0, me.Count);
    }
    public static T Random<T>(this IList<T> me)
    {
        return me[me.RandomIndex()];
    }
}

