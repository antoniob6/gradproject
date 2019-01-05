using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {
   // public MapGenerator mapGenerator;
    public Rules gameRules;
    public int wantedRounds = 2;

    private int currentRound = 0;
    private bool QuestActive=false;
    private int totalQuests=3;

    private GameObject winnedPlayer = null;
    GameObject[] players;

    [SerializeField]
    private float spawnrange = 60f;

    [SerializeField]
    private GameObject collectiblePrefab;
    [SerializeField]
    private GameObject foundablePrefab;
    [SerializeField]
    private float spawnInterval = 1.0f;

    [SerializeField]
    private GameObject gameOverCanvas;


    public Vector3[] spawnPoints;


    public List<Quest> quests;

    public bool mapGenerated = false;
    private bool spawningCollectibles=false;

    int spawnIndex = 0;

    private bool haveQuest = false;

    private int questCount = 0;
    public bool waited = false;
    // Use this for initialization
    void Start () {
        quests = new List<Quest>();
       

		
	}



    // Update is called once per frame
    void Update () {
        bool hasEnded= checkRoundCount();
        if (!hasEnded)
        {
            questTick();
             checkPlayersScore();
        }
        else
        {
            GameObject gameOver = Instantiate(gameOverCanvas, Vector3.zero, Quaternion.identity) as GameObject;

            NetworkServer.Spawn(gameOver);
        }

       

    }
    [Server]
    public bool checkRoundCount()
    {
        if (currentRound >= wantedRounds)
            return true;
        return false;
    }
    [Server]
    public void checkPlayersScore()
    {
        players = GameObject.FindGameObjectsWithTag("Player");

        //Debug.Log("playersCount= " + players.Length);

        if (players.Length == 0)
            return;

        bool hasWinningPlayer = false;
        foreach (GameObject p in players)
        {
            playerdata pdata = p.GetComponent<playerdata>();
            if (pdata.scoref >= 100)
            {
                winnedPlayer = p;
                hasWinningPlayer = true;
            }
            if (winnedPlayer == null)
            {
                if(questCount==0)
                     pdata.RpcUpdateText("collect the balls");
                else if (questCount==1)
                    pdata.RpcUpdateText("find the hidden ball");
                else if (questCount == 2)
                    pdata.RpcUpdateText("kill any player");
                else
                    pdata.RpcUpdateText("game completed");
            }
            else
            {
                
                if (winnedPlayer == p)
                {
                    
                    pdata.RpcUpdateText("you won");
                }
                else
                    pdata.RpcUpdateText("you lost");
            }

            if (!hasWinningPlayer)
                winnedPlayer = null;
            // pdata.RpcUpdateScore(p.transform.position.x);
        }
    }


    public void questTick()
    {
        if (spawnIndex == 20)
        {
            spawnIndex = 0;
            CancelInvoke();
            
        }
        if (winnedPlayer&&waited)
        {
            QuestActive = false;
            waited = false;

        }


    }

    void addQuest(Quest q)
    {
        quests.Add(q);
    }

    void SpawnCollectible()
    {
        Random.seed = System.DateTime.Now.Millisecond;
        Vector2 spawnPosition = new Vector2(Random.Range(-spawnrange, spawnrange), this.transform.position.y); ;
        //Debug.Log(spawnIndex +" spawning at:"+ spawnPosition.ToString());
        
        GameObject enemy = Instantiate(collectiblePrefab, spawnPosition, Quaternion.identity) as GameObject;

        NetworkServer.Spawn(enemy);

        spawnIndex++;
    }
    void SpawnFind()
    {
        Random.seed = System.DateTime.Now.Millisecond;
        Vector2 spawnPosition = new Vector2(Random.Range(-spawnrange, spawnrange), this.transform.position.y); ;
        Debug.Log(" spawning find");

        GameObject find = Instantiate(foundablePrefab, spawnPosition, Quaternion.identity) as GameObject;

        NetworkServer.Spawn(find);

        spawnIndex++;
    }
    void startKillAny()
    {

    }

    [Server]
    public void ResetPlayersScore()
    {
        foreach (GameObject p in players)
        {
            playerdata pdata = p.GetComponent<playerdata>();
            pdata.RpcUpdateScore(0);
        }
    }

    [Server]
   public void StartLocationQuest()
    {
        Invoke("hasWaited", 2);
        if (QuestActive)
            return;

        ResetPlayersScore();
        questCount++;
        currentRound++;
        if (questCount >= totalQuests)
            questCount = 0;
        if (questCount == 0)
        {

            Debug.Log("startingLocationQuest");
            InvokeRepeating("SpawnCollectible", this.spawnInterval, this.spawnInterval);
            spawningCollectibles = true;
        }
        else if (questCount == 1)
        {

            Debug.Log("startingFindGoalQuest");
            Invoke("SpawnFind",0);

        }
        else if (questCount == 2)
        {
            Debug.Log("startingkillQuest");
            Invoke("startKillAny", 0);
            
        }


        QuestActive = true;
        


    }
    public void hasWaited()
    {
        waited = true;
    }

}
