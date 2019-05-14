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




    }
