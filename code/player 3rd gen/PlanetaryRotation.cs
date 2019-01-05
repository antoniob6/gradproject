using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetaryRotation : MonoBehaviour {
    private GameObject center;
    public bool isActive = true;

    // Use this for initialization
    void Start () {
        center = (GameObject.FindGameObjectsWithTag("GravityCenter"))[0];
    }
	
	// Update is called once per frame
	void Update () {
        if (isActive)
        {
            if (center == null)
                center = (GameObject.FindGameObjectsWithTag("GravityCenter"))[0];


            Vector3 gravityUp = (transform.position - center.transform.position).normalized;

            Quaternion targetRotatoion = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotatoion, 50 * Time.deltaTime);

        }
    }
}
