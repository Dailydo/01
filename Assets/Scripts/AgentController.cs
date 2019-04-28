using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentController : MonoBehaviour {

    public NavMeshAgent _navMeshAgent;

	// Update is called once per frame
	void Update ()
    {
	    if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "BaseCube")
                {
                    //Debug.Log("Targeted position: " + hit.collider.GetComponent<BaseCube>().GetTopPosition().ToString());
                    _navMeshAgent.SetDestination(hit.collider.GetComponent<BaseCube>().GetTopPosition());
                }
            }
        }
	}
}
