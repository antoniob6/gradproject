/* 
 * gives the object a way to interact with other objects
 * it checks the objects that collides with this object and gives the apropriate response 
 * such as gitting hit by bullets, reducing the health, and giving the effect of the player dying
 * 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerReceiveDamage : NetworkBehaviour {
    public GameObject recieveDamageEffect;
    public GameObject deathEffect;

	[SerializeField]private int maxHealth = 10;

	[SyncVar]public int currentHealth;

	[SerializeField]
	private string enemyTag="Enemy";
    [SerializeField]
    private string deathTag="Death";



    [SerializeField]
	private bool destroyOnDeath;

	private Vector2 initialPosition;
    private Rigidbody2D m_Rigidbody2D;
    private bool isdead = false;
    public Animator animator;

    public GameObject lastHitby = null;

    private PlayerData PD;
    public int getHealth()
    {
        return currentHealth;
    }

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        PD = GetComponent<PlayerData>();

    }
    // Use this for initialization
    void Start () {
		this.currentHealth = this.maxHealth;
		this.initialPosition = this.transform.position;
	}

    public void characterTriggered(Collider2D collider) {

        //Debug.Log("character triggered: "+ currentHealth);
        //hit by bullet
        if (collider.tag == "Bullet" && collider.gameObject.GetComponent<Bullet>().owner != this.gameObject.GetComponent<PlayerReceiveDamage>()) {
            Destroy(collider.gameObject);
            this.TakeDamage(1);
            m_Rigidbody2D.velocity = new Vector3(0, 0, 0);
            lastHitby = collider.gameObject.GetComponent<Bullet>().owner.gameObject;
           

        }
        //hit by enemy
        else if (collider.tag == this.enemyTag) {
            Destroy(collider.gameObject);
            this.TakeDamage (1);
        //    Debug.Log("destroying enemy");
			
		}
        //touched the death layer
        else if (collider.tag == this.deathTag)
        {
            this.TakeDamage(100);
            m_Rigidbody2D.velocity = new Vector3(0, 0, 0);

        }

    }
    

    bool didWeCheckDeath = false;


	void TakeDamage(int amount) {
		if (isServer) {
			currentHealth -= amount;
            if (recieveDamageEffect) {
                GameObject GO = Instantiate(recieveDamageEffect, transform.position, Quaternion.identity);
                NetworkServer.Spawn(GO);
            } else {
                Debug.Log("effect recieve damage  not assigned");
            }
            if (PD)
                PD.takenDamage(currentHealth, maxHealth);
            else
                Debug.Log("please assign PD");

                if (this.currentHealth <= 0) {
                if(!didWeCheckDeath){
                    didWeCheckDeath = true;
					
					RpcRespawn ();
                    if(PD)
                      PD.hasDied = true;
                    if (lastHitby) {//last hit was by another player
                        PlayerData hitByPD = lastHitby.gameObject.GetComponent<PlayerData>();
                        if (hitByPD != null) {
                            hitByPD.RpcAddScore(100);
                            hitByPD.RpcKilledPlayerCount(hitByPD.KilledPlayerCount + 1);
                        }
                    } else {
                       // Debug.Log("player died but couldn't find bullet owner");
                    }

                    
				}
			}

            if (animator &&gameObject.tag == "Player")
            {
                animator.SetBool("IsHurt", true);
                Invoke("finishedTakingDamage", 1);
            }
        }
	}
    void finishedTakingDamage()
    {
        animator.SetBool("IsHurt", false);
    }

	[ClientRpc]
	void RpcRespawn() {
       // StartCoroutine(deathCo(3));

	}
    bool spriteColorSet = false;


    IEnumerator deathCo(float time) {
        if (deathEffect) {
         GameObject DE=   Instantiate(deathEffect, gameObject.transform.position, transform.rotation);
            DE.transform.parent = transform;
        } else {
            Debug.Log("death effect not assigned");
        }

        // GameObject message = Instantiate(messageToAllPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        // NetworkServer.Spawn(message);
        //  message.GetComponent<GameMessage>().RpcUpdateText(m);

      //  m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;

        Color oldSpriteColor =Color.white;
        PlayerData pd = gameObject.GetComponent<PlayerData>();
        if (pd != null) {
            if (!spriteColorSet) {
                oldSpriteColor = pd.spriteColor;
                spriteColorSet = true;
            }
            pd.CmdUpdateColor(Color.black);
        
        }


        yield return new WaitForSeconds(time);
        if (pd != null) {
            pd.CmdUpdateColor(oldSpriteColor);

        }
        m_Rigidbody2D.constraints = RigidbodyConstraints2D.None;
        this.transform.position = this.initialPosition;
        this.currentHealth = this.maxHealth;
        didWeCheckDeath = false;

    }


}
