using System.Collections;
using UnityEngine;
using Unity.XR.CoreUtils; // Requis pour XROrigin
using System;

public class Dart : MonoBehaviour
{
    private Rigidbody rg;
    private GameObject dirObj;
    public bool isForceOK = false;
    bool isDartRotating = false;
    bool isDartReadyToShoot = true;
    bool isDartHitOnBoard = false;

    XROrigin xrOrigin;       // <-- Remplace ARSessionOrigin
    GameObject ARCam;

    public Collider dartFrontCollider;

    void Start()
    {
        xrOrigin = FindObjectOfType<XROrigin>();
        if (xrOrigin != null)
        {
            ARCam = xrOrigin.Camera.gameObject;
        }
        else
        {
            Debug.LogError("XROrigin not found in the scene!");
        }

        if (TryGetComponent(out Rigidbody rigid))
            rg = rigid;

        dirObj = GameObject.FindGameObjectWithTag("DartThrowPoint");


    }

    private void FixedUpdate()
    {
        if (isForceOK)
        {
            dartFrontCollider.enabled = true;
            StartCoroutine(InitDartDestroyVFX());

            if (TryGetComponent(out Rigidbody rigid))
                rigid.isKinematic = false;

            isForceOK = false;
            isDartRotating = true;
        }

        // Add Force
        Debug.Log("position de cible"+dirObj.transform.forward);
        rg.AddForce(dirObj.transform.forward * (12f + 6f) * Time.deltaTime, ForceMode.VelocityChange);
        Debug.Log("position de fleche" + dirObj.transform.forward * (12f + 6f));

        // Dart ready
        if (isDartReadyToShoot)
        {
            transform.Rotate(Vector3.forward * Time.deltaTime * 20f);
        }

        // Dart rotating
        if (isDartRotating)
        {
            isDartReadyToShoot = false;
            transform.Rotate(Vector3.forward * Time.deltaTime * 400f);
        }
    }

    IEnumerator InitDartDestroyVFX()
    {
        yield return new WaitForSeconds(7f);
         if (!isDartHitOnBoard) { 
        Destroy(gameObject);
         }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("dart_board"))
        {
            // Trigger vibration
            Handheld.Vibrate();

            GetComponent<Rigidbody>().isKinematic = true;
            isDartRotating = false;

            // Dart hit the board
            isDartHitOnBoard = true;
            // Get hit position
            Vector3 hitPoint = transform.position;
            Vector3 boardCenter = other.bounds.center;
            float distance = Vector3.Distance(hitPoint, boardCenter);

            int score = 0;
            if (distance < 0.05f)
                score = 50;
            else if (distance < 0.1f)
                score = 25;
            else if (distance < 0.2f)
                score = 10;
            else
                score = 0;

            ScoreManager.Instance.AddScore(score);
        }


    }
}
