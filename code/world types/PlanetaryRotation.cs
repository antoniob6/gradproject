/*
 * give the apropriate rotation to the player so he keeps standing upwards
 * (not upside down)
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetaryRotation : MonoBehaviour {
    private GameObject center;
    private GravitySystem GS;
    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        if (GS == null)
            GS = FindObjectOfType<GravitySystem>();
        if (GS.gravityDirection==GravitySystem.GravityType.ToCenter)
        {
            if (center == null)
                center = (GameObject.FindGameObjectsWithTag("GravityCenter"))[0];


            Vector3 gravityUp = (transform.position - center.transform.position).normalized;

            Quaternion targetRotatoion = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotatoion, 50 * Time.deltaTime);

        }
        else if(GS.gravityDirection == GravitySystem.GravityType.Down) {
            transform.rotation = Quaternion.identity;
        }
    }
}
