using UnityEngine;
using System.Collections;

public class SelectionRing : MonoBehaviour {

    private GameObject cameraObj;

	void Awake()
    {
        cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
    }
    
    void FixedUpdate()
    {
        transform.LookAt(cameraObj.transform);
    }

}
