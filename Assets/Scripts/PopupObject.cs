using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupObject: MonoBehaviour
{
    public PopupObjectResult objectToTrigger;
    public RectTransform topSection;
    public RectTransform sideSection;
    public float maxHeightModifier;
    public float toggleDuration;
    public float delayAfterTrigger;

    private RectTransform rootTransform;
    private float startingDimensions;
    private bool isOut;

    private void Start()
    {
        isOut = false;
        rootTransform = GetComponent<RectTransform>();
        startingDimensions = rootTransform.sizeDelta.x;
        ToggleState(true);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    ToggleState(!isOut);
        //}
    }

    private void ToggleState(bool up)
    {
        if (up)
            StartCoroutine(RaiseObject());
        else
            StartCoroutine(LowerObject());
        isOut = up;
    }

    private IEnumerator RaiseObject()
    {
        yield return new WaitForSeconds(delayAfterTrigger);
        float elapsedTime = 0;
        float waitTime = toggleDuration;
        while (elapsedTime < waitTime)
        {
            float value = Mathf.Lerp(0, maxHeightModifier, elapsedTime / waitTime);
            if (elapsedTime >= waitTime)
                value = maxHeightModifier;
            SetRootHeight(value);
            SetTopOffset(value);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        objectToTrigger.Execute();

        yield return null;
    }

    private IEnumerator LowerObject()
    {
        yield return new WaitForSeconds(delayAfterTrigger);
        float elapsedTime = 0;
        float waitTime = toggleDuration;
        while (elapsedTime < waitTime)
        {
            float value = Mathf.Lerp(maxHeightModifier, 0, elapsedTime / waitTime);
            if (elapsedTime >= waitTime)
                value = maxHeightModifier;
            SetRootHeight(value);
            SetTopOffset(value);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return null;
    }

    private void SetRootHeight(float heightModifier)
    { 
        rootTransform.sizeDelta = new Vector2(startingDimensions, startingDimensions + heightModifier);
    }

    private void SetTopOffset(float offset)
    {
        topSection.offsetMin = new Vector2(0, offset);
    }
}
