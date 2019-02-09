/*
 * this script is responsible for the game flow, meaning it gives the quests to the players and 
 * monitors them, giving out new quest when the old quests are completed.
 * also it displays the message to each client, give out round messages, rewards the players,
 * ends the game, and display player scores after the end.
 * */ 





using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {
    // public MapGenerator mapGenerator;
    public float questTimeLimit;
    public GameObject enemieSpawner;
    public Rules gameRules;
    public int wantedRounds = 2;
    public GameObject map;
    public GameObject center;

    private int currentRound = 0;
    private bool QuestActive = false;
    private int totalQuests = 4;


    [SerializeField]
    private GameObject messageToAllPrefab;


    private GameObject winnedPlayer = null;
    List<GameObject> players;

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
    private bool spawningCollectibles = false;

    int spawnIndex = 0;

    private bool haveQuest = false;

    private int questCount = 0;
    public bool waited = false;
    private List<Quest> activeQuests = new List<Quest>();

    private bool shouldBeQuest = true;
    // Use this for initialization
    void Start() {
        quests = new List<Quest>();
        players = new List<GameObject>();

        //StartCoroutine(GameCoroutine());

    }





    // Update is called once per frame
    void Update() {
        GameObject[] pl = GameObject.FindGameObjectsWithTag("Player");
        players.Clear();
        foreach (GameObject p in pl) {
            players.Add(p);
        }

        TickQuests();
        checkRounds();

        if (Input.GetKey(KeyCode.G)) {
            displayMessageToAll("testing message: hello this is a test message");
        }

    }

    IEnumerator GameCoroutine() {
        displayMessageToAll("round is starting");
        yield return new WaitForSeconds(2);

        print(Time.time);
        StartCoroutine(GameCoroutine());
    }

    public void displayMessageToAll(string m,float time=3) {

        StartCoroutine(displayMessage(m, time));

    }
    IEnumerator displayMessage(string m,float time) {

        GameObject message= Instantiate(messageToAllPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(message);
        message.GetComponent<GameMessage>().RpcUpdateText(m);
        yield return new WaitForSeconds(time);
        NetworkServer.Destroy(message);
    } 




    public void questCompleted(Quest q) {
        StartCoroutine(questCompletedCo(q));
    }
    IEnumerator questCompletedCo(Quest q) {
        q.RewardPlayers();
        if (q.winners.Count != 0) {
            foreach (GameObject p in q.winners) {
                if (isServer) {
                    if(p)
                        p.GetComponent<PlayerData>().RpcUpdateText("you have won this round");

                }
            }
        }

        q.DestroyQuest();
        displayMessageToAll("round ending");
        yield return new WaitForSeconds(3);
        activeQuests.Clear();
    }



    bool hasGameover = false;
    public void checkRounds() {

        if (currentRound < wantedRounds) {
            //            checkPlayersScore();

            if (activeQuests.Count == 0) {
                Quest l = generateQuest();
                
                if (l != null) {
                    
                    activeQuests.Add(l);
                    displayMessageToAll("roundStarting now");
                    CancelInvoke("completeQuests");
                    Invoke("completeQuests", questTimeLimit);
                    currentRound++;
                } 
            }
        } else {
            if (!hasGameover) {
                GameObject gameOver = Instantiate(gameOverCanvas, Vector3.zero, Quaternion.identity) as GameObject;
                NetworkServer.Spawn(gameOver);
                hasGameover = true;

                string scores="";
                int index = 0;
                foreach (GameObject p in players) {
                    index++;
                    PlayerData pdata = p.GetComponent<PlayerData>();
                    scores += "player " + index + " score: " + pdata.scoref+"\n";

                }
                displayMessageToAll(scores, 20);
            }
        }

    }
    public void completeQuests() {
        displayMessageToAll("round ending now");

        foreach (Quest q in activeQuests) {
            q.EndQuestNow();
        }

    }

    private Quest generateQuest() {

        if (players.Count > 1) {

            if (currentRound % 3 == 0) {

                return new KillQuest(players, 100, "kill another player", this);
             
            } else if (currentRound % 3 == 1) {
                return new LocationQuest(players, center, 100, "find the things", this, 2, 20);
            }else {
                return new SurviveQuest(players, center, 100, "survive the enemies", this);

            }

        } else {
            Debug.Log("need at least two players");
            return null;
        }

    }


    private bool timeOut=false;
    private bool lockResults = false;
    [Server]
    public void checkPlayersScore()
    {
        //Debug.Log("playersCount= " + players.Length);
        if (players.Count == 0)
            return;

        foreach (GameObject p in players)
        {
            PlayerData pdata = p.GetComponent<PlayerData>();
            if (pdata.scoref >= 100 && questCount != 3)
            {
                winnedPlayer = p;

            }
            else if (questCount == 3)
            {

                if (!lockResults)
                {
                    if (pdata.hasDied)
                    {
                        pdata.RpcUpdateText("you lost");
                    }
                    else if (timeOut)
                    {
                        pdata.RpcUpdateText("you Survived");
                        lockResults = true;

                    }
                    else if (!timeOut)
                    {
                        pdata.RpcUpdateText("survive");
                    }
                }
            }
            
            
        }
    }




    void addQuest(Quest q)
    {
        quests.Add(q);
    }

    

    [Server]
    public void ResetPlayersScore()
    {
        foreach (GameObject p in players)
        {
            PlayerData pdata = p.GetComponent<PlayerData>();
            pdata.RpcUpdateScore(0);
            pdata.RpchasDied(false);
        }
    }

    [Server]
    public GameObject networkSpawn(string message,Vector3 position)
    {
        if (message == "locationPrefab")
        {
            GameObject enemy = Instantiate(foundablePrefab, position, Quaternion.identity) as GameObject;
            NetworkServer.Spawn(enemy);
            return enemy;
        }
        return null;
    }
    [Server]
    public void networkDestroy(GameObject foundable) {
        NetworkServer.Destroy(foundable);
    }
    public void TickQuests() {
        Debug.Log("number of active quests: " + activeQuests.Count);
        foreach (Quest q in activeQuests) {
           
            q.tick();
        }
    }

    public void CheckCurQuests() {
        foreach(Quest q in activeQuests) {
            if (q.isComplete) {
                foreach(GameObject g in q.winners) {
                    PlayerData pd = g.GetComponent<PlayerData>();
                    pd.RpcUpdateScore(pd.scoref + q.reward);
                }
            }

        }
    }


    [Server]
    public void startSpawingEnemies() {
        enemieSpawner.SetActive(true);

    }
    [Server]
    public void stopSpawingEnemies() {
        enemieSpawner.SetActive(false);
        

    }
    public void UpdatePlayerObjective(GameObject player,string s) {
        player.GetComponent<PlayerData>().RpcUpdateText("good job, waiting for other players");

    }

}
