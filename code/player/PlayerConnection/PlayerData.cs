/*
 *this script hold player data(as the name implies),
 * from on side it stores the data about the specific player,
 * and from the other side, it connects the user interface to the game mechanics, 
 * and displays the required info, such as the player health, score and current objective. 
 */
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class PlayerData : NetworkBehaviour
{
    public PlayerConnectionObject PCO;
    public Canvas canvas;
    public Text scoreField;
    public Text objectiveField;
    public SimpleHealthBar healthBar;

    [SyncVar]public Color spriteColor = Color.white;
    [SyncVar]public string Cname = "default";
    [SyncVar]public float score = 0f;
    [SyncVar]public string objectiveText = "objectiveText";
    [SyncVar]public bool hasDied = false;

    [SyncVar] public int KilledEntityCount = 0;
    [SyncVar] public int KilledPlayerCount = 0;



    public SpriteRenderer spriteRenderer;
    public Text playerNameText;


    [ClientRpc]
    public void RpcUpdateScore(float s) {
        score = s;
        scoreField.text = "score: " + score.ToString();
         TextManager.instance.createTextOnLocalInstance(PCO.playerBoundingCollider.gameObject.transform.position,"+" + (int)(s - score));
    }
    [ClientRpc]
    public void RpcAddScore(float s) {
        score += s;
        scoreField.text = "score: " + score.ToString();
        //Debug.Log("score updated");

         TextManager.instance.createTextOnLocalInstance(PCO.playerBoundingCollider.gameObject.transform.position, "+" + (int)(s));
    }
    [ClientRpc]public void RpcUpdateText(string text) {
        objectiveText = text;
        objectiveField.text = objectiveText;
    }
    [ClientRpc]public void RpcKilledEntityCount(int c) {
        KilledEntityCount = c;
    }
    [ClientRpc]public void RpcKilledPlayerCount(int c) {
        KilledPlayerCount = c;
    }
    [ClientRpc]
    public void RpcUpdateColor(Color c) {
        spriteColor = c;

    }
    [Command]
    public void CmdUpdateColor(Color c) {
        spriteColor = c;
    }


    [ClientRpc]
    public void RpchasDied(bool text) {
        hasDied = text;

    }

    public float[] getStats() {
        float[] stats =new float[10];
        stats[0] = score;
        stats[1] = hasDied?0f:1f;
        stats[2] = gameObject.GetComponent<PlayerReceiveDamage>().currentHealth;
        stats[3] = gameObject.GetComponent<Rigidbody2D>().velocity.magnitude;
        return stats;
    }

    public void takenDamage(float currentHealth,float maxHealth) {
        //Debug.Log("updated health bar");
        healthBar.UpdateBar(currentHealth, maxHealth);
    }
}
