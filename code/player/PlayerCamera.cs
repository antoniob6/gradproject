using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class PlayerCamera : MonoBehaviour {
    public GameObject TargetObject;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (TargetObject) { 
            transform.position = TargetObject.transform.position;
            transform.position += Vector3.back;
        }
        
	}
}
