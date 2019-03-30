/* 
 * gives the object a way to interact with other objects
 * it checks the objects that collides with this object and gives the apropriate response 
 * such as gitting hit by bullets, reducing the health, and giving the effect of the player dying
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ReceiveDamage : NetworkBehaviour {
    public GameObject deathEffect;

	[SerializeField]private int maxHealth = 10;

	[SyncVar]public int currentHealth;

	[SerializeField]
	private string enemyTag;
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

	void OnTriggerEnter2D(Collider2D collider) {
        if (gameObject.tag == "Player" && collider.tag == "Bullet" && collider.gameObject.GetComponent<Bullet>().owner != this.gameObject.GetComponent<ReceiveDamage>()) {
            this.TakeDamage(1);
            m_Rigidbody2D.velocity = new Vector3(0, 0, 0);
            lastHitby = collider.gameObject.GetComponent<Bullet>().owner.gameObject;
            Destroy(collider.gameObject);

        }
        else if (collider.tag == this.enemyTag) {
            if (collider.tag == "Bullet") {
                lastHitby = collider.gameObject.GetComponent<Bullet>().owner.gameObject;
            //    Debug.Log("got hit by bullet");
            }
            this.TakeDamage (1);
			Destroy (collider.gameObject);
		}
        else if (collider.tag == this.deathTag)
        {
            this.TakeDamage(100);
            m_Rigidbody2D.velocity = new Vector3(0, 0, 0);

        }

    }
    bool didWeCheckDeath = false;
	void TakeDamage(int amount) {

		if (this.isServer) {

			this.currentHealth -= amount;
			if (this.currentHealth <= 0) {
				if (this.destroyOnDeath) {
                    if (lastHitby) {//alien killed by player
                        PlayerData hitByPD = lastHitby.gameObject.GetComponent<PlayerData>();
                        if (hitByPD != null) {
                            hitByPD.RpcAddScore(30);
                        }
                    }
                        Destroy (this.gameObject);
				} else if(!didWeCheckDeath){//player got killed by player
                    didWeCheckDeath = true;
					
					RpcRespawn ();
                    if(PD!=null)
                      PD.hasDied = true;
                    if (lastHitby) {
                        PlayerData hitByPD = lastHitby.gameObject.GetComponent<PlayerData>();
                        if (hitByPD != null) {
                            hitByPD.RpcAddScore(100);
                            
                        }


                    } else {
                        Debug.Log("player died but couldn't find bullet owner");
                    }

                    
				}
			}

            if (gameObject.tag == "Player")
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
        StartCoroutine(deathCo(3));

	}
    bool spriteColorSet = false;
    IEnumerator deathCo(float time) {
        if (deathEffect) {
         GameObject DE=   Instantiate(deathEffect, gameObject.transform.position, transform.rotation);
            DE.transform.parent = transform;
        }

        // GameObject message = Instantiate(messageToAllPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        // NetworkServer.Spawn(message);
        //  message.GetComponent<GameMessage>().RpcUpdateText(m);

        m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;

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
