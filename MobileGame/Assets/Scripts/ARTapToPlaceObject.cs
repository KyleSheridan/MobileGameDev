using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using System;

public class ARTapToPlaceObject : MonoBehaviour
{
    public GameObject objectToPlace;
    public GameObject placementIndicator;
    public GameObject placementIndicatorTexture;

    public Material aimMat;
    public Material placeMat;

    public TextMeshProUGUI planeText;

    private ARRaycastManager raycastManager;

    private Pose placementPose;
    private bool placementPoseIsValid = false;

    private bool levelPlaced = false;

    private MeshRenderer placmentTexture;

    // Start is called before the first frame update
    void Start()
    {
        raycastManager = FindObjectOfType<ARRaycastManager>();

        placmentTexture = placementIndicatorTexture.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(levelPlaced) { return; }
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if(placementPoseIsValid && Input.touchCount > 0) 
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    placmentTexture.material = placeMat;
                    break;
                case TouchPhase.Ended:
                    PlaceObject();
                    break;
            }
        }
        else
        {
            placmentTexture.material = aimMat;
        }
    }

    private void PlaceObject()
    {
        Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
        levelPlaced = true;
        Destroy(placementIndicator);
        planeText.gameObject.SetActive(false);
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);

            planeText.text = "Tap to place level!";
        }
        else
        {
            placementIndicator.SetActive(false);
            planeText.text = "Aim camera at flat surface";
        }
    }

    private void UpdatePlacementPose()
    {
        Vector2 screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        raycastManager.Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;

            Vector3 cameraForward = Camera.current.transform.forward;
            Vector3 cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
}
