using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    private Transform _transformCamera;         //Camera's transform
    private Transform _transformParent;         //Parent's (empty object) transform

    private Vector3 _localRotation;             //Vector used to determine the position to reach for the camera through a pivot-based rotation        
    private float _cameraDistance = 5f;
    private Vector3 _pivotPosition;

    public float _mouseSensitivity = 4f;
    public float _scrollSensitivity = 2f;
    public float _orbitDampening = 10f;         //How long it takes for the camera to reach its destination
    public float _scrollDampening = 6f;
    public float _panSpeed = 0.5f;
    public float _pivotTranslationDuration = 1f;

    private float _t0;
    private bool _shortClick = false;

    public bool _cameraDisabled = false;



	// Use this for initialization
	void Start () {
        this._transformCamera = this.transform;
        this._transformParent = this.transform.parent;
	}
	
    void Update()
    {
        //Store at each frame the gameobject pointed by the cursor, if any
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.transform.tag == "Focusable")
        {
            Debug.Log(hit.transform.name + " is focusable.");
        }

        //In case of mouse click, determine if single click or held button
        if (Input.GetMouseButtonDown(0))
            _t0 = Time.time;
        if (Input.GetMouseButtonUp(0))
            _shortClick = ((Time.time - _t0) <= 0.2f) ? true : false;

        //if (Input.GetMouseButtonDown(0))
        if (_shortClick)
        {
            if (Physics.Raycast(ray, out hit) && hit.transform.tag == "Focusable")
            {
                //this.transform.parent.position = hit.transform.position;
                StartCoroutine(MovePivotToPosition(hit.transform.position));
            }
        }

    }

	// Update is called once per frame, after Update() on every game object in the scene
	void LateUpdate () {
        if (Input.GetKeyDown(KeyCode.LeftShift))    //Input to disable the camera control
            _cameraDisabled = !_cameraDisabled;

        if (!_cameraDisabled)
        {
            //Rotation of the camera based on mouse coordinates, when the left click is held
            if (Input.GetMouseButton(0) && (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0))
            {
                _localRotation.x += Input.GetAxis("Mouse X") * _mouseSensitivity;
                _localRotation.y -= Input.GetAxis("Mouse Y") * _mouseSensitivity;

                //Clamp the y rotation to horizon and not flipping over at the top
                _localRotation.y = Mathf.Clamp(_localRotation.y, -10f, 90f);
            }

            //Zooming Input from our mouse scroll wheel
            if (Input.GetAxis("Mouse ScrollWheel") != 0f)
            {
                float scrollAmount = Input.GetAxis("Mouse ScrollWheel") * _scrollSensitivity;

                //Makes camera zoom faster the further away it is from its target
                scrollAmount *= (this._cameraDistance * 0.3f);

                this._cameraDistance += scrollAmount * -1f;

                //This makes camera go no closer than 1.5m from target and no further than 100m
                this._cameraDistance = Mathf.Clamp(this._cameraDistance, 2f, 20f);
            }
        }

        //Actual camera rig transformations
        Quaternion QT = Quaternion.Euler(_localRotation.y, _localRotation.x, 0);
        this._transformParent.rotation = Quaternion.Lerp(this._transformParent.rotation, QT, Time.deltaTime * _orbitDampening);
        
        if (this._transformCamera.localPosition.z != this._cameraDistance * -1f)
        {
            this._transformCamera.localPosition = new Vector3(0f, 0f, Mathf.Lerp(this._transformCamera.localPosition.z, this._cameraDistance * -1f, Time.deltaTime * _scrollDampening));
        }

        //Paning movement based on mouse coordinates, when the middle click is held
        if (Input.GetMouseButton(2) && (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0))
        {
            Vector3 move = new Vector3(Input.GetAxis("Mouse X") * -1f, Input.GetAxis("Mouse Y") * -1f, 0f);
            transform.parent.transform.Translate(move * _panSpeed, Space.Self);
        }

        _shortClick = false;
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
