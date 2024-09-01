using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class ARPlacementAndPlaneDetectionController : MonoBehaviour
{
    ARPlaneManager m_ARPlaneManager;
    ARPlacement m_ARPlacementManager;

    public GameObject placeButton;
    public GameObject adjustButton;
    public GameObject searchForGameButton;
    public TextMeshProUGUI informUIPanel_Text;
    public GameObject scaleSlider;

    private void Awake()
    {
        m_ARPlacementManager = GetComponent<ARPlacement>();
        m_ARPlaneManager = GetComponent<ARPlaneManager>();
    }


    // Start is called before the first frame update
    void Start()
    {
        placeButton.SetActive(true);
        adjustButton.SetActive(false);
        searchForGameButton.SetActive(false);
        scaleSlider.SetActive(true);

        informUIPanel_Text.text = "Move phone to detect planes and place the Battle Arena!";

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisableARPlacementAndPlaneDetection()
    {
        m_ARPlaneManager.enabled = false;
        m_ARPlacementManager.enabled = false;

        SetAllPlanesActiveOrDeactive(false);

        placeButton.SetActive(false);
        adjustButton.SetActive(true);
        searchForGameButton.SetActive(true);
        scaleSlider.SetActive(false);

        informUIPanel_Text.text = "Arena placed! Search for game to BATTLE!";


    }

    public void EnableARPlacementAndPlaneDetection()
    {
        m_ARPlaneManager.enabled = true;
        m_ARPlacementManager.enabled = true;

        SetAllPlanesActiveOrDeactive(true);

        placeButton.SetActive(true);
        adjustButton.SetActive(false);
        searchForGameButton.SetActive(false);
        scaleSlider.SetActive(true);

        informUIPanel_Text.text = "Move phone to detect planes and place the Battle Arena!";
    }

    private void SetAllPlanesActiveOrDeactive(bool value)
    {
        foreach (var plane in m_ARPlaneManager.trackables)
        {
            plane.gameObject.SetActive(value);
        }
    }

}
