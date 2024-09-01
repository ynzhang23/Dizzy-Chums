using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.UI;

public class ScaleController : MonoBehaviour
{
    XROrigin xrOrigin;
    public Slider scaleSlider;

    private void Awake()
    {
        xrOrigin = GetComponent<XROrigin>();
    }

    void Start()
    {
        scaleSlider.onValueChanged.AddListener(OnSliderValueChange);
    }

    void Update()
    {
        // Your update logic here
    }

    public void OnSliderValueChange(float value)
    {
        if (scaleSlider != null && xrOrigin != null)
        {
            xrOrigin.transform.localScale = Vector3.one / value;
        }
    }
}