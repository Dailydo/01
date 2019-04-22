using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {

    //General variables
    private Transform _transformCamera;         //Camera's transform
    private Transform _transformParent;         //Parent's (empty object) transform
    private Vector3 _localRotation;             //Vector used to determine the position to reach for the camera through a pivot-based rotation        
    private Vector3 _pivotPosition;
    private Vector3 _panningDirection;          //Vector describing the direction in which to move the camera and its pivot when panning
    private float _cameraDistance = 5f;         //Distance between the camera and its pivot point
    private float _zoomAmout;                   //How much the camera should zoom with the pivot point

    public bool _cameraDisabled = false;        //Inputs don't affect the camera when true
    public float _orbitDampening = 10f;         //How long it takes for the camera to reach its destination
    public float _scrollDampening = 6f;
    public float _pivotTranslationDuration = 1f;

    //Mouse input variables
    private float _t0;                          //Timer used to determine wether the a mouse left click is short or held
    private bool _shortClick = false;

    public float _mouseSensitivity_W = 4f;      //Modifier applied to the drag mouse inputs in Windows environment
    public float _scrollSensitivity_W = 2f;     //Modifier applied to the zoom value in Windows environment
    public float _panSensitivity_W = 0.25f;     //Modifier applied to the panning sensitivity in Windows environment

    //Touch input variables
    private Vector2 _touch0PreviousPos;          //Position of the touch[0] during previous frame
    private Vector2 _touch0Direction;
    private bool _touch0DirectionChoosen = false;

    public float _moveSensitivity_A = 1f;


    // Use this for initialization
    void Start () {
        this._transformCamera = this.transform;
        this._transformParent = this.transform.parent;
	}
	
    void Update()
    {
        //Raycast initialization
        Ray ray = new Ray();
        RaycastHit hit;

        //Touch inputs declaration
        Touch touch0 = new Touch();
        _touch0PreviousPos = Vector2.zero;
        _touch0Direction = Vector2.zero;

        //Camera status check (Windows only)
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _cameraDisabled = !_cameraDisabled;
        }

        //Store at each frame the gameobject pointed by the cursor, if any
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.touchCount > 0)
            {
                touch0 = Input.GetTouch(0);
            }
            ray = Camera.main.ScreenPointToRay(touch0.position);
        }

        //Process inputs
        if (!_cameraDisabled) {

            //WINDOWS inputs
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                //Shortclick, from mouse letf click, defines (bool) "_shortclick"
                if (Input.GetMouseButtonDown(0))
                    _t0 = Time.time;
                if (Input.GetMouseButtonUp(0))
                    _shortClick = ((Time.time - _t0) <= 0.2f) ? true : false;       //0.2f as holding threshold duration

                //Drag and drop, from mouse left button held, defines (Vector3) "_locationRotation"
                if (Input.GetMouseButton(0) && (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0))
                {
                    _localRotation.x += Input.GetAxis("Mouse X") * _mouseSensitivity_W;
                    _localRotation.y -= Input.GetAxis("Mouse Y") * _mouseSensitivity_W;
                }

                //Zoom, from mouse scroll wheel, defines (float) "_zoomAmount"
                if (Input.GetAxis("Mouse ScrollWheel") != 0f)
                {
                    _zoomAmout = Input.GetAxis("Mouse ScrollWheel") * _scrollSensitivity_W;
                }

                //Panning, from mouse wheel held, defines (Vector3) "_paningMovement"
                if (Input.GetMouseButton(2) && (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0))
                {
                    _panningDirection = new Vector3(Input.GetAxis("Mouse X") * -1f, Input.GetAxis("Mouse Y") * -1f, 0f);
                    _panningDirection *= _panSensitivity_W;
                }
            }

            //TOUCH inputs
            if (Application.platform == RuntimePlatform.Android) {

                //Register touches 
                if (Input.touchCount > 0)
                    touch0 = Input.GetTouch(0);

                //Shortclick & Drag and drop
                if (Input.touchCount == 1)      //Handle short click and drag and drop
                {
                    switch (touch0.phase)
                    {
                        case (TouchPhase.Began):
                            _touch0PreviousPos = touch0.position;
                            _touch0DirectionChoosen = false;
                            _t0 = Time.time;
                            break;
                        case (TouchPhase.Moved):
                            Debug.Log("Touch[0] delta: " + touch0.deltaPosition.ToString());
                            _localRotation.x += touch0.deltaPosition.x * _moveSensitivity_A;
                            _localRotation.y -= touch0.deltaPosition.y * _moveSensitivity_A;
                            break;
                        case (TouchPhase.Ended):
                            _touch0DirectionChoosen = true;
                            _shortClick = ((Time.time - _t0) <= 0.2f) ? true : false;
                            break;
                        default:
                            Debug.Log("Unknown TouchPhase status.");
                            break;
                    }
                }

                //Zoom in/out
                //Panning
            }


            //Process generic values
            //Clamp the y rotation to horizon and not flipping over at the top
            _localRotation.y = Mathf.Clamp(_localRotation.y, -10f, 90f);

            //Apply inputs 
            //Translate pivot to clicked "focusable" object
            if (_shortClick)
            {
                if (Physics.Raycast(ray, out hit) && hit.transform.tag == "Focusable") {
                    StartCoroutine(MovePivotToPosition(hit.transform.position));
                }
            }

            //Camera zoom
            _zoomAmout *= this._cameraDistance;         //Makes the zoom proportional to the distance (the longer the faster)        
            this._cameraDistance += _zoomAmout * -1f;   //Add to the camera distance so the camera moves during the "Camera transformations" part   
            this._cameraDistance = Mathf.Clamp(this._cameraDistance, 2f, 20f);      //Makes camera go no closer than "min" from target and no further than "max"

            //Camera panning
            transform.parent.transform.Translate(_panningDirection, Space.Self);

            //Camera transformations
            Quaternion QT = Quaternion.Euler(_localRotation.y, _localRotation.x, 0);
            this._transformParent.rotation = Quaternion.Lerp(this._transformParent.rotation, QT, Time.deltaTime * _orbitDampening);

            if (this._transformCamera.localPosition.z != this._cameraDistance * -1f) {
                this._transformCamera.localPosition = new Vector3(0f, 0f, Mathf.Lerp(this._transformCamera.localPosition.z, this._cameraDistance * -1f, Time.deltaTime * _scrollDampening));
            }

            //Debug.Log("Axes input: (" + Input.GetAxis("Mouse X") + ", " + Input.GetAxis("Mouse Y") + ")");
            //Debug.Log("Local rotation: " + _localRotation.ToString());
           
            //Input values reset (so values aren't carried to the next frames)
            _shortClick = false;
            _zoomAmout = 0;     
            _panningDirection = Vector3.zero;
            _touch0PreviousPos = touch0.position;
        }
    }

    IEnumerator MovePivotToPosition(Vector3 pos)
    {
        float journey = 0f;
        while (journey <= _pivotTranslationDuration)
        {
            journey += Time.deltaTime;
            float percent = Mathf.Clamp01(journey / _pivotTranslationDuration);

            _transformParent.position = Vector3.Lerp(_transformParent.position, pos, percent);

            yield return null;
        }
    }
}
