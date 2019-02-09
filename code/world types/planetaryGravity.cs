/*
 * adds the gravity
 * not only planet like gravity but also downward and upward directed gravity
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planetaryGravity : MonoBehaviour {
    private GameObject center;
    
    public float gravity = 10f;
    public bool isActive = true;
    private GravitySystem GS;


    private Rigidbody2D m_Rigidbody2D;
    // Use this for initialization
    void Start () {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();


    }
	
	// Update is called once per frame
	void Update () {
       
        if (isActive)
            if (center == null)
                 center = (GameObject.FindGameObjectsWithTag("GravityCenter"))[0];
        
		
	}
    void FixedUpdate()
    {
        if (GS == null) {
            GS = FindObjectOfType<GravitySystem>();
            return;
        }

        if (GS.gravityDirection == GravitySystem.GravityType.ToCenter)
            m_Rigidbody2D.AddForce((center.transform.position - transform.position).normalized * GS.gravityForce);
        else if (GS.gravityDirection == GravitySystem.GravityType.Down) {
            m_Rigidbody2D.AddForce(Vector3.down* GS.gravityForce);
        }
    }

}
