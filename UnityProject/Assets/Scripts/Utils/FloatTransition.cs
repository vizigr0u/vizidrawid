using System;
using System.Collections;
using UnityEngine;

public static class FloatTransitions
{
    private static IEnumerator TransitionFloatOverTime(float from, float to, float overTime, Action<float> onUpdate, Action onComplete = null)
    {
        float time = overTime;
        float alpha = 0.0f;

        while (time > 0.0f)
        {
            yield return new WaitForEndOfFrame();
            time -= Time.deltaTime;
            alpha = time / overTime;
            onUpdate(Mathf.SmoothStep(from, to, 1.0f - alpha));
        }

        onComplete?.Invoke();
        onUpdate(to);
    }

    public static void Transition(this MonoBehaviour monoBehaviour, float from, float to, float overTime, Action<float> onUpdate, Action onComplete = null)
    {
        monoBehaviour.StartCoroutine(TransitionFloatOverTime(from, to, overTime, onUpdate, onComplete));
    }
}
