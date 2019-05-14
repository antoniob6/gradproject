using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemyRecieveDamage : NetworkBehaviour {
    public GameObject onDeathEffect;
    public GameObject onDamageEffect;
    [SerializeField] private int maxHealth = 3;
    [SyncVar] public int currentHealth=3;

    public bool destroyWhenDead=true;
    private bool dead = false;

    FollowClosestPlayer FCP;
    // Use this for initialization
    void Start () {
        FCP = GetComponent<FollowClosestPlayer>();
        currentHealth = maxHealth;
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
    void TakeDamage(int amount, Collider2D collider) {
        if (!isServer)
            return;
        currentHealth -= amount;
        onDamage();
        if (this.currentHealth <= 0 && !dead) {//entity dead
            EntityDied();

            Bullet B = collider.GetComponent<Bullet>();
            if (B)
                B.ownerPD.playerKilledEntity();

            if (destroyWhenDead)
                Destroy(gameObject);
        }


    }
    [Command] public void CmdTakeDamage(int amount) {
        if (!isServer)
            return;
        onDamage();
        currentHealth -= amount;
        if (currentHealth <= 0) {
            EntityDied();
            if (destroyWhenDead) {
                Destroy(gameObject);
            }
        }
    }
    private void EntityDied() {
        dead = true;
        if (FCP)//if the entity follows the player
            FCP.dead = true;
    }
    [Command]public void CmdTakeDamageWithGO(int amount,GameObject damagerGO) {
        if (!isServer)
            return;
        currentHealth -= amount;
        onDamage();
        if (currentHealth <= 0) {
            EntityDied(damagerGO);
            if (destroyWhenDead) {
                Destroy(gameObject);
            }
        }
    }

    private void EntityDied(GameObject damagerGO) {
        PlayerData PD = damagerGO.GetComponent<PlayerData>();
        if (PD) {
            PD.playerKilledEntity();
         //   Debug.Log("success in registering kill");
        } else
            Debug.Log("entity killed but couldn't identify killer");
        dead = true;
        if (FCP)//if the entity follows the player
            FCP.dead = true;
    }

    private void onDamage() {
        if (onDamageEffect) {
            GameObject GO = Instantiate(onDamageEffect, transform.position, transform.rotation);
        }
    }


    private void OnDestroy() {
        if (onDeathEffect) {
            GameObject GO = Instantiate(onDeathEffect, transform.position,transform.rotation);
        }
    }
}
