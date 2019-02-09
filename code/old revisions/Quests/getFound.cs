using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class getFound : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            ReceiveDamage RD = collider.gameObject.GetComponent<ReceiveDamage>();
            collider.gameObject.GetComponent<playerdata>().RpcUpdateScore(100);


            Debug.Log("got Found by player");
            Destroy(gameObject);

        }
        if (collider.gameObject.tag == "Bullet")
        {
            Bullet bullet = collider.gameObject.GetComponent<Bullet>();

            ReceiveDamage RD = bullet.owner.GetComponent<ReceiveDamage>();


            bullet.owner.GetComponent<playerdata>().RpcUpdateScore(100);


            Debug.Log("got hit by bullet");
            Destroy(gameObject);
        }

    }
}
