using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public delegate T LerpCallback<T>(T initial, T last, float timeValue);
public delegate void SetValueCallback<T>(T value);

public interface ILerpInstruction
{
    void Process(float perc);
}

public struct LerpInstruction<T> : ILerpInstruction
{
    public T firstValue;
    public T lastValue;
    public LerpCallback<T> lerp;
    public SetValueCallback<T> set;

    public LerpInstruction(T first, T last, LerpCallback<T> lerp, SetValueCallback<T> set)
    {
        this.firstValue = first;
        this.lastValue = last;
        this.lerp = lerp;
        this.set = set;
    }

    public void Process(float perc)
    {
        set(lerp(firstValue, lastValue, perc));
    }
}

public static class LerpInstructionExtensions
{
    public static IEnumerator Lerp<T>(this object me, float time, T first, T last, LerpCallback<T> lerp, SetValueCallback<T> set)
    {
        yield return me.Lerp(time, new LerpInstruction<T>(first, last, lerp, set));
    }
    public static IEnumerator Lerp(this object me, float time, params ILerpInstruction[] instructions)
    {
        float t = 0f;
        while (t <= time)
        {
            t += Time.deltaTime;
            foreach (ILerpInstruction instruction in instructions)
                instruction.Process(t / time);
            yield return null;
        }
        foreach (ILerpInstruction instruction in instructions)
            instruction.Process(1f);
    }
}