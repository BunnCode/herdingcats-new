using UnityEngine;
using UnityEngine.UI;

public class Indicator : MonoBehaviour
{
    [SerializeField] private IndicatorType indicatorType;
    private Image indicatorImage;
    private Text distanceText;

    //get object if it is active
    public bool Active
    {
        get
        {
            return transform.gameObject.activeInHierarchy;
        }
    }

    //get the type of indicator (box or arrow)
    public IndicatorType Type
    {
        get
        {
            return indicatorType;
        }
    }

    void Awake()
    {
        indicatorImage = transform.GetComponent<Image>();
        distanceText = transform.GetComponentInChildren<Text>();
    }

    //set color property to indicator
    public void SetImageColor(Color color)
    {
        indicatorImage.color = color;
    }

    //set indicator as active or inactive
    public void Activate(bool value)
    {
        transform.gameObject.SetActive(value);
    }
}

//types of indicators
public enum IndicatorType
{
    BOX,
    ARROW
}
