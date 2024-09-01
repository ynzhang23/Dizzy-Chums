using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacement : MonoBehaviour
{
    ARRaycastManager m_ARRaycastManager;
    static List<ARRaycastHit> raycast_Hits = new List<ARRaycastHit>();

    public Camera ARCamera;
    public GameObject battleArenaGameObject;

    private void Awake()
    {
        m_ARRaycastManager = GetComponent<ARRaycastManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the origin of the AR ray to be from the center of the screen
        Vector3 centerOfScreen = new Vector3(Screen.width/2, Screen.height/2);
        Ray ray = ARCamera.ScreenPointToRay(centerOfScreen);

        if (m_ARRaycastManager.Raycast(ray, raycast_Hits, TrackableType.PlaneWithinPolygon))
        {
            // AR Ray intersects with a flat plane
            Pose hitPose = raycast_Hits[0].pose;
            Vector3 positionToBePlaced = hitPose.position;

            // Place arena at the intersection between AR Ray and the Plane
            battleArenaGameObject.transform.position = positionToBePlaced;
        }
    }
}
