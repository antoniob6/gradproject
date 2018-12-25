using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetaryRotation : MonoBehaviour {
    public Transform center;
    public float gravity = 20f;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 gravityUp = (transform.position - center.position).normalized * gravity;

        Quaternion targetRotatoion = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotatoion, 50 * Time.deltaTime);

    }
}
