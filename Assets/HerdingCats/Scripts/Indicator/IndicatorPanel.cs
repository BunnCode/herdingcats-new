//using IndicatorMain;
using System;
using System.Collections.Generic;
//using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class IndicatorPanel : MonoBehaviour
{
    [Range(0.5f, 0.9f)]
    [SerializeField] private float screenBoundOffset = 0.9f;

    private List<Target> targets = new List<Target>();

    public static Action<Target, bool> TargetStateChanged;

    private Camera mainCamera;
    private Vector3 screenCentre;
    private Vector3 screenBounds;

    void Awake()
    {
        mainCamera = Camera.main;
        screenCentre = new Vector3(Screen.width, Screen.height, 0) / 2;
        screenBounds = screenCentre * screenBoundOffset;
        TargetStateChanged += HandleTargetStateChanged;
    }

    void LateUpdate()
    {
        DrawIndicators();
    }

    //create indicators for all targets on target list
    void DrawIndicators()
    {
        foreach(Target target in targets)
        {
            Vector3 screenPosition = IndicatorMain.IndicatorMain.GetScreenPosition(mainCamera, target.transform.position);
            bool isTargetVisible = IndicatorMain.IndicatorMain.IsTargetVisible(screenPosition);
            Indicator indicator = null;

            //this will set the box indicator for onscreen
            if(target.NeedBoxIndicator && isTargetVisible)
            {
                screenPosition.z = 0;
                indicator = GetIndicator(ref target.indicator, IndicatorType.BOX);
            }
            //this will set the arrow indicator for offscreen
            else if(target.NeedArrowIndicator && !isTargetVisible)
            {
                float angle = float.MinValue;
                IndicatorMain.IndicatorMain.GetArrowIndicatorPositionAndAngle(ref screenPosition, ref angle, screenCentre, screenBounds);
                indicator = GetIndicator(ref target.indicator, IndicatorType.ARROW);
                indicator.transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
            }
            //set properties of indicator
            if(indicator)
            {
                indicator.SetImageColor(target.TargetColor);// Sets the image color of the indicator.
                indicator.transform.position = screenPosition; //Sets the position of the indicator on the screen.
            }
        }
    }

    //add and remove targets from target list
    private void HandleTargetStateChanged(Target target, bool active)
    {
        if(active)
        {
            targets.Add(target);
        }
        else
        {
            target.indicator?.Activate(false);
            target.indicator = null;
            targets.Remove(target);
        }
    }

    //get the correct indicator based on the pool it's from and if it is null or not
    private Indicator GetIndicator(ref Indicator indicator, IndicatorType type)
    {
        if(indicator != null)
        {
            //switch indicator types from box to arrow or viceversa
            if(indicator.Type != type)
            {
                indicator.Activate(false);
                indicator = type == IndicatorType.BOX ? BoxObjectPool.current.GetPooledObject() : ArrowObjectPool.current.GetPooledObject();
                indicator.Activate(true);
            }
        }
        else
        {
            indicator = type == IndicatorType.BOX ? BoxObjectPool.current.GetPooledObject() : ArrowObjectPool.current.GetPooledObject();
            indicator.Activate(true);
        }
        return indicator;
    }

    //destroy indicator
    private void OnDestroy()
    {
        TargetStateChanged -= HandleTargetStateChanged;
    }
}
