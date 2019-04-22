using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableCharacter : MonoBehaviour {
    public PlayerReceiveDamage RD;
    public BoxCollider2D PlayerBoundingCollider;
   // public float timeBetweenDmg;
    public BoxCollider2D getPlayerBoundingCollider() {
        if (!PlayerBoundingCollider) {
            Debug.Log("player bounding collider not assigned");
            return null;
        }
        return PlayerBoundingCollider;
    }


    void OnTriggerEnter2D(Collider2D collider) {
        if (!RD) {
            Debug.Log("RD not assigned");
            return;
        }
        RD.characterTriggered(collider);
    }


    }
