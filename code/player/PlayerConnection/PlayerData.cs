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

    public Canvas canvas;
    public Text scoreField;
    public Text objectiveField;
    public SimpleHealthBar healthBar;


    [SyncVar] public string playerName = "default";
    [SyncVar] public Color playerColor;


    [SyncVar]public float score = 0f;
    [SyncVar]public string objectiveText = "objectiveText";
    [SyncVar]public bool hasDied = false;
    [SyncVar] public bool playerIsReady = false;

    [SyncVar] public int killedEntityCount = 0;
    [SyncVar] public int killedPlayerCount = 0;
    [SyncVar] public int deathCount = 0;

    [SyncVar] public int roundKilledEntityCount = 0;
    [SyncVar] public int roundKilledPlayerCount = 0;

    private PlayerConnectionObject PCO;
    private void Start() {
        PCO = GetComponent<PlayerConnectionObject>();
    }

    public void resetRoundStats() {
        roundKilledEntityCount = 0;
        roundKilledPlayerCount = 0;

    }




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
        if(s>0)
            TextManager.instance.createTextOnLocalInstance(PCO.playerBoundingCollider.gameObject.transform.position, "+" + (int)(s));
        else if(s<0)
            TextManager.instance.createRedTextOnLocalInstance(PCO.playerBoundingCollider.gameObject.transform.position,  ""+(int)(s));
    }
    [ClientRpc]public void RpcUpdateText(string text) {
        objectiveText = text;
        objectiveField.text = objectiveText;
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
        //if (isServer) 
          //  Debug.Log("server taken damage");
        
        RpcTakenDamage(currentHealth, maxHealth);

        PCO.PC.RpcPlayerTakenDamage();//notify that was hurt
    }
    [ClientRpc] private void RpcTakenDamage(float currentHealth, float maxHealth) {
        healthBar.UpdateBar(currentHealth, maxHealth);
    }




    public PlayerReadyBtn PRB;
    public void playerReadyBtn() {
        if (!isLocalPlayer)
            return;

        CmdPlayerReadyBtn();
        PRB.gameObject.SetActive(false);
      //  Debug.Log("player ready and Btn disabled");
        
    }

    [Command] public void CmdPlayerReadyBtn() {
        playerIsReady = true;
    }

    [ClientRpc] public void RpcResetReadyBtn() {
        playerIsReady = false;
        if (isLocalPlayer) {//activate btn on local player
            PRB.gameObject.SetActive(true);
        }

    }
    public void playerDied() {
        hasDied = true;
        deathCount++; 
        RpcAddScore(-100);//penalty for dying
        PCO.PC.RpcPlayerDied();//make the player die
    }
    [ClientRpc] public void RpcPlayerDied() {

    }

    public void playerKilledPlayer() {
        RpcAddScore(100);
        killedPlayerCount++;
        roundKilledPlayerCount++;
    }
    public void playerKilledEntity() {
        RpcAddScore(50);
        killedEntityCount++;
        roundKilledEntityCount++;
    }
}
