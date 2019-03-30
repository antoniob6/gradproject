/*
 *this script hold player data(as the name implies),
 * from on side it stores the data about the specific player,
 * and from the other side, it connects the user interface to the game mechanics, 
 * and displays the required info, such as the player health, score and current objective. 
 */
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class PlayerData : NetworkBehaviour
{
    public Canvas canvas;
    public Text scoreField;
    public Text objectiveField;
    public Text healthField;

    [SyncVar]public Color spriteColor = Color.white;
    [SyncVar]public string Cname = "default";
    [SyncVar]public float score = 0f;
    [SyncVar]public string objectiveText = "objectiveText";
    [SyncVar]public bool hasDied = false;


    public SpriteRenderer spriteRenderer;
    public Text playerNameText;


    void Start() {
        InvokeRepeating("checkHp", 1.0f, 1.0f);
        


    }
    void LateUpdate() {
        
        if (!playerNameText) {
            playerNameText = GetComponent<PlayerConnectionObject>().GetComponentInChildren<Text>();
        }
        else if (playerNameText.text != Cname)
            playerNameText.text = Cname;
  
        if (spriteRenderer.color != spriteColor) {
            //   Debug.Log("color cahnged");
            spriteRenderer.color = spriteColor;

        }
        scoreField.text = "score: " + score.ToString();
        objectiveField.text = objectiveText;
        if (playerNameText.text != Cname)
            playerNameText.text = Cname;
    }
    [ClientRpc]
    public void RpcUpdateScore(float scorefn) {
        score = scorefn;

    }
    [ClientRpc]
    public void RpcAddScore(float scorefn) {
        score += scorefn;

    }
    [ClientRpc]
    public void RpcUpdateText(string text) {
        objectiveText = text;

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

    public void checkHp() {
        ReceiveDamage RD = GetComponent<ReceiveDamage>();
        updateHealth(RD.getHealth());
    }


    public void updateHealth(int hp) {
        healthField.text = hp.ToString();

    }
    public float[] getStats() {
        float[] stats =new float[10];
        stats[0] = score;
        stats[1] = hasDied?0f:1f;
        stats[2] = gameObject.GetComponent<ReceiveDamage>().currentHealth;
        stats[3] = gameObject.GetComponent<Rigidbody2D>().velocity.magnitude;
        return stats;
    }

}
