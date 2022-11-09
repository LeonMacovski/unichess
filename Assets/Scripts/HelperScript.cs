using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperScript : MonoBehaviour
{
    public static HelperScript instance;

    private void Awake()
    {
        instance = this;
    }

    public void DelayedExecution(float delay, Action onExecute)
    {
        if (delay < 0) 
            return;

        StartCoroutine(DelayRoutine(delay, onExecute));
    }

    private IEnumerator DelayRoutine(float delay, Action onExecute)
    {
        yield return new WaitForSeconds(delay);
        onExecute?.Invoke();
    }
}
