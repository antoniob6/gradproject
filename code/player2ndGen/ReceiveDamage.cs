using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ReceiveDamage : NetworkBehaviour {

	[SerializeField]
	private int maxHealth = 10;

	[SyncVar]
	public int currentHealth;

	[SerializeField]
	private string enemyTag;
    [SerializeField]
    private string deathTag="Death";


    [SerializeField]
	private bool destroyOnDeath;

	private Vector2 initialPosition;
    private Rigidbody2D m_Rigidbody2D;
    public int getHealth()
    {
        return currentHealth;
    }

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

    }
    // Use this for initialization
    void Start () {
		this.currentHealth = this.maxHealth;

		this.initialPosition = this.transform.position;
	}

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.tag == this.enemyTag) {
			this.TakeDamage (1);

			Destroy (collider.gameObject);
		}
        if (collider.tag == this.deathTag)
        {
            this.TakeDamage(100);
            m_Rigidbody2D.velocity = new Vector3(0, 0, 0);

        }
    }

	void TakeDamage(int amount) {
		if (this.isServer) {
			this.currentHealth -= amount;
			if (this.currentHealth <= 0) {
				if (this.destroyOnDeath) {
					Destroy (this.gameObject);
				} else {
					this.currentHealth = this.maxHealth;
					RpcRespawn ();
				}
			}
		}
	}

	[ClientRpc]
	void RpcRespawn() {
		this.transform.position = this.initialPosition;
	}
	

}
