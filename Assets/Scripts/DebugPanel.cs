using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugPanel : MonoBehaviour {

    public GameObject _panningModeGameobject;


    public void DisplayPanningMode(string panningMode)
    {
        _panningModeGameobject.GetComponent<Text>().text = panningMode;
    }

}
