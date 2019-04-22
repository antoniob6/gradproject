using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    public GameObject bulletHitEffect;
    [HideInInspector]
    public PlayerReceiveDamage owner;
    [HideInInspector]
    public PlayerData ownerPD;
    private bool hit = false;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnTriggerEnter2D(Collider2D collider) {
        if (!hit &&collider.gameObject!=owner.gameObject) {
            hit = true;
            //Debug.Log("bullet has hit the target");
           //  Instantiate(bulletHitEffect, gameObject.transform.position, transform.rotation);
           
           // Destroy(collider.gameObject);
        }


    }



    }
