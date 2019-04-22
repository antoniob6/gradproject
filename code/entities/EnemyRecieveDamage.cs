using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemyRecieveDamage : NetworkBehaviour {
    public GameObject onDeathEffect;
    [SerializeField] private int maxHealth = 3;
    [SyncVar] public int currentHealth;

    // Use this for initialization
    void Start () {
        this.currentHealth = this.maxHealth;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    
    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag == "Bullet") {
            TakeDamage(1,collider);
            Destroy(collider.gameObject);
        }
    }
    [Command] public void CmdTakeDamage(int amount) {
        if (!isServer)
            return;
        currentHealth -= amount;
        if (this.currentHealth <= 0) {
            Destroy(gameObject);
        }


    }

    void TakeDamage(int amount, Collider2D collider) {
        if (!isServer) 
            return;
        currentHealth -= amount;
        if (this.currentHealth <= 0) {
           PlayerData PD= collider.GetComponent<Bullet>().ownerPD;
           PD.RpcAddScore(10);
            PD.RpcKilledEntityCount(PD.KilledEntityCount + 1);
            
            Destroy(gameObject);  
        }

        
    }
    private void OnDestroy() {
        if (onDeathEffect&&isActiveAndEnabled) {
            GameObject GO = Instantiate(onDeathEffect, transform.position,transform.rotation);


        }
    }
}
