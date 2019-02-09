/*this makes the player shoot bullets and remembers who is the one that shot them,
 * in the latest revision the bullets are shot according to where the mouse is clicked
 */
using UnityEngine;
using UnityEngine.Networking;

public class ShootBullets : NetworkBehaviour {

    [SerializeField]
    private Camera playerCamera;

    [SerializeField]
	private GameObject bulletPrefab;

	[SerializeField]
	private float bulletSpeed;

	void Update () {
		if (this.isLocalPlayer &&(Input.GetMouseButtonDown(0))) {
			this.CmdShoot ();
		}
	}

	[Command]
	void CmdShoot() {
		GameObject bullet = Instantiate (bulletPrefab, this.transform.position, Quaternion.identity);
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = playerCamera.ScreenToWorldPoint(mousePosition);
        float distance = Vector2.Distance(transform.position, mousePosition);
        //Debug.Log(distance);
        if (distance < 5)
            distance = 5;
        if (distance > 10)
            distance = 10;


        bullet.GetComponent<Rigidbody2D> ().velocity = (mousePosition-transform.position)*bulletSpeed*distance/10;
        bullet.GetComponent<Bullet>().owner = gameObject.GetComponent<ReceiveDamage>();
        NetworkServer.Spawn (bullet);
        Destroy (bullet, 9.0f);
    }
}
