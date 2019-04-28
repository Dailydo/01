using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DebugPanel : MonoBehaviour {

    public GameObject _panningModeGameobject;


    public void DisplayPanningMode(string panningMode)
    {
        _panningModeGameobject.GetComponent<Text>().text = panningMode;
    }

    public void SwitchScene()
    {
        Scene scene = SceneManager.GetActiveScene();

        switch (scene.name)
        {
            case "01":
                SceneManager.LoadScene("02", LoadSceneMode.Single);
                break;
            case "02":
                SceneManager.LoadScene("01", LoadSceneMode.Single);
                break;
            default:
                Debug.Log("Current scene is weird.");
                break;
        }
    }
}
