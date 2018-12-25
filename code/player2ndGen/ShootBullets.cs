using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ShootBullets : NetworkBehaviour {

	[SerializeField]
	private GameObject bulletPrefab;

	[SerializeField]
	private float bulletSpeed;

	void Update () {
		if (this.isLocalPlayer && Input.GetButtonDown("Shoot")) {
			this.CmdShoot ();
		}
	}

	[Command]
	void CmdShoot() {
		GameObject bullet = Instantiate (bulletPrefab, this.transform.position, Quaternion.identity);
        GameObject bullet2 = Instantiate(bulletPrefab, this.transform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D> ().velocity = Vector2.up * bulletSpeed+Vector2.left*bulletSpeed;
        bullet2.GetComponent<Rigidbody2D>().velocity = Vector2.up * bulletSpeed+Vector2.right*bulletSpeed;
        bullet.GetComponent<Bullet>().owner = gameObject.GetComponent<ReceiveDamage>();
        bullet2.GetComponent<Bullet>().owner = gameObject.GetComponent<ReceiveDamage>();
        NetworkServer.Spawn (bullet);
        NetworkServer.Spawn(bullet2);
        Destroy (bullet, 5.0f);
        Destroy(bullet2, 5.0f);
    }
}
