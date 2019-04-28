using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCube : MonoBehaviour {

    //Return the position of the point at its center top
	public Vector3 GetTopPosition()
    {
        Vector3 position = transform.position + new Vector3(0f, (transform.localScale.y / 2f), 0f);
        return position;
    }

}
