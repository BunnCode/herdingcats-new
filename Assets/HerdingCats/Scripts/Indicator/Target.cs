using UnityEngine;

[DefaultExecutionOrder(0)]
public class Target : MonoBehaviour
{
    [SerializeField] private Color targetColor = Color.red;
    [SerializeField] private bool needBoxIndicator = true;
    [SerializeField] private bool needArrowIndicator = true;

    //value stored here by indicator panel script
    [HideInInspector] public Indicator indicator;

    //get defaulted color
    public Color TargetColor
    {
        get
        {
            return targetColor;
        }
    }

    //gets box indicator
    public bool NeedBoxIndicator
    {
        get
        {
            return needBoxIndicator;
        }
    }

    //gets arrow indicator
    public bool NeedArrowIndicator
    {
        get
        {
            return needArrowIndicator;
        }
    }

    //add object to target list
    private void OnEnable()
    {
        if(IndicatorPanel.TargetStateChanged != null)
        {
            IndicatorPanel.TargetStateChanged.Invoke(this, true);
        }
    }

    //remove object from target list
    private void OnDisable()
    {
        if(IndicatorPanel.TargetStateChanged != null)
        {
            IndicatorPanel.TargetStateChanged.Invoke(this, false);
        }
    }

    //gets distance from camera
    public float GetDistanceFromCamera(Vector3 cameraPosition)
    {
        float distanceFromCamera = Vector3.Distance(cameraPosition, transform.position);
        return distanceFromCamera;
    }
}
