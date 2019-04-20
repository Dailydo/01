using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchInput : MonoBehaviour {

    public GameObject _TextInputTouch0;
    public GameObject _TextInputTouch1;
    public GameObject _Distance0to1;

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        Touch touch0 = new Touch();
        Touch touch1 = new Touch();

        if (Input.touchCount > 0)
        {
            touch0 = Input.GetTouch(0);
            _TextInputTouch0.GetComponent<Text>().text = "Input touch [0]: " + touch0.position.ToString();
        }
        else
            _TextInputTouch0.GetComponent<Text>().text = "Input touch [0]: -";

        if (Input.touchCount > 1)
        {
            touch1 = Input.GetTouch(1);
            _TextInputTouch1.GetComponent<Text>().text = "Input touch [1]: " + touch1.position.ToString();
            _Distance0to1.GetComponent<Text>().text = "Distance [0-1]: " + Vector2.Distance(touch0.position, touch1.position);
        }
        else
        {
            _TextInputTouch1.GetComponent<Text>().text = "Input touch [1]: -";
            _Distance0to1.GetComponent<Text>().text = "Distance [0-1]: -";
        }
    }
}
