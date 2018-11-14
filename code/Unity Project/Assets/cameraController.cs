using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour {

    public GameObject player;

    private Vector3 offset;
	
    private Rigidbody2D rb;
	private static ILogger logger = Debug.unityLogger;

    void Start()
    {
		rb = GetComponent<Rigidbody2D>();
        
    }

    void LateUpdate()
    {
        offset = transform.position - player.transform.position;
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);

        //rb.MovePosition(new Vector2(player.transform.position.x+offset.x, player.transform.position.y + offset.y));
        //rb.AddRelativeForce(new Vector2(-1*offset.x, -1*offset.y));
    }
}
