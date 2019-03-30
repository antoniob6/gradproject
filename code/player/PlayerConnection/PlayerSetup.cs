/*because the game is not single-player
 * every one has a diffrent copy of each game object
 * this script removes the copies that are not on the required game object
 * such as the cameras that are on other players.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour  {
    [SerializeField]
    Behaviour[] componentsToDisable;

    // Use this for initialization
    void Start () {

        if (!isLocalPlayer)
        {
            for (int i = 0; i < componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
