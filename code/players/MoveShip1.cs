using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MoveShip : NetworkBehaviour {

	[SerializeField]
	private float speed;

	void FixedUpdate () {
		if (this.isLocalPlayer) {
			float movementx = Input.GetAxis ("Horizontal");
            float movementy = Input.GetAxis("Vertical");
            if(movementx!=0 || movementy!=0)
              GetComponent<Rigidbody2D> ().velocity = new Vector2 (movementx * speed, movementy*speed);
		}
	}
}
