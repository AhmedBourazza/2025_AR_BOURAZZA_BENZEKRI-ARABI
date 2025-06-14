﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceObjectOnPlane : MonoBehaviour
{
    public GameObject objectToPlace;
    public GameObject placementIndicator;
    private Pose placementPose;
    private Transform placementTransform;
    private bool placementPoseIsValid = false;
    private bool isObjectPlaced = false;
    private TrackableId placedPlaneId = TrackableId.invalidId;

    ARRaycastManager m_RaycastManager;
    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    public static event Action onPlacedObject;
    void Awake()
    {
        if (TryGetComponent(out ARRaycastManager aRRaycast))
            m_RaycastManager = aRRaycast;

    }

    void Update()
    {
        Debug.Log("test");
        if (!isObjectPlaced)
        {

            Debug.Log("test2");

            if (placementPoseIsValid && Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                Debug.Log("Touch détecté avec le nouveau système !");
                PlaceObject();
            }
            Debug.Log("test3");

            UpdatePlacementPosistion();
            Debug.Log("test4");

            UpdatePlacementIndicator();
        }
    }

    private void UpdatePlacementPosistion()
    {
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        if (m_RaycastManager.Raycast(screenCenter, s_Hits, TrackableType.PlaneWithinPolygon))
        {
            Debug.Log("Raycast a touché un plan !");

            placementPoseIsValid = s_Hits.Count > 0;
            if (placementPoseIsValid)
            {
                placementPose = s_Hits[0].pose;
                placedPlaneId = s_Hits[0].trackableId;

                var planeManager = GetComponent<ARPlaneManager>();
                ARPlane arPlane = planeManager.GetPlane(placedPlaneId);
                placementTransform = arPlane.transform;
            }
        }
    } //end of UpdatePlacementIndicator

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementTransform.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void PlaceObject()
    {

        Instantiate(objectToPlace, placementPose.position, placementTransform.rotation);
        onPlacedObject?.Invoke();
        isObjectPlaced = true;
        placementIndicator.SetActive(false);
    }
}


