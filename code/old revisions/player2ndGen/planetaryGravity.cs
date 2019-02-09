using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planetaryGravity : MonoBehaviour {
    public GameObject center;
    
    public float gravity = 10f;


    private Rigidbody2D m_Rigidbody2D;
    // Use this for initialization
    void Start () {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

        center=(GameObject.FindGameObjectsWithTag("GravityCenter"))[0];
    }
	
	// Update is called once per frame
	void Update () {
        if (center == null)
        {
            center = (GameObject.FindGameObjectsWithTag("GravityCenter"))[0];
        }
		
	}
    void FixedUpdate()
    {
        addforce();
    }

    void addforce()
    {
        m_Rigidbody2D.AddForce((center.transform.position - transform.position).normalized * 30f);
    }
}
