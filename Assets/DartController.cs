using System.Collections;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem;
public class DartController : MonoBehaviour
{
    public GameObject DartPrefab;
    public Transform DartThrowPoint;
    XROrigin xrOrigin;
    GameObject ARCam;
    Transform DartboardObj;
    private GameObject DartTemp;
    private Rigidbody rb;
    private bool isDartBoardSearched = false;
    private float m_distanceFromDartBoard = 0f;
    public TMP_Text text_distance;

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
    }

    void OnEnable()
    {
        PlaceObjectOnPlane.onPlacedObject += DartsInit;
    }

    void OnDisable()
    {
        PlaceObjectOnPlane.onPlacedObject -= DartsInit;
    }

    void Update()
    {
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.isPressed &&
            Touchscreen.current.primaryTouch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
        {
            Vector2 touchPos = Touchscreen.current.primaryTouch.position.ReadValue();
            Ray raycast = Camera.main.ScreenPointToRay(touchPos);
            RaycastHit raycastHit;
            if (Physics.Raycast(raycast, out raycastHit))
            {
                if (raycastHit.collider.CompareTag("dart"))
                {
                    //Disable back touch Collider from dart 
                    raycastHit.collider.enabled = false;
                    DartTemp.transform.parent = xrOrigin.transform;

                    Dart currentDartScript = DartTemp.GetComponent<Dart>();
                    currentDartScript.isForceOK = true;

                    //Load next dart
                    DartsInit();
                }
            }
        }
        if (isDartBoardSearched)
        {
            m_distanceFromDartBoard = Vector3.Distance(DartboardObj.position, ARCam.transform.position);
            text_distance.text = m_distanceFromDartBoard.ToString().Substring(0, 3);
        }

    }
    void DartsInit()
    {
        DartboardObj = GameObject.FindWithTag("dart_board").transform;
        if (DartboardObj)
        {
            isDartBoardSearched = true;
        }
        StartCoroutine(WaitAndSpawnDart());
    }

    public IEnumerator WaitAndSpawnDart()
    {
        yield return new WaitForSeconds(1f);
        DartTemp = Instantiate(DartPrefab, DartThrowPoint.position, ARCam.transform.localRotation);
        DartTemp.transform.parent = ARCam.transform;
        rb=DartTemp.GetComponent<Rigidbody>();
        if(DartTemp.TryGetComponent(out Rigidbody rigid))
        rb = rigid;
      
        rb.isKinematic = true;

        SoundManager.Instance.play_dartReloadSound();
    }
}