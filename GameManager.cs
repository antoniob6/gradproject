using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {
    // public MapGenerator mapGenerator;
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


    bool clea = false;


    // Update is called once per frame
    void Update() {
        GameObject[] pl = GameObject.FindGameObjectsWithTag("Player");
        players.Clear();
        foreach (GameObject p in pl) {
            players.Add(p);
        }

        TickQuests();
        checkRounds();
        checkPlayers();

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

    public void displayMessageToAll(string m) {

        StartCoroutine(displayMessage(m));

    }
    IEnumerator displayMessage(string m) {

        GameObject message= Instantiate(messageToAllPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(message);
        message.GetComponent<GameMessage>().RpcUpdateText(m);
        yield return new WaitForSeconds(5);
        NetworkServer.Destroy(message);
    } 


        public void checkPlayers() {

        foreach (Quest q in activeQuests) {
            if (q.isComplete) {
                shouldBeQuest = false;
                if (q.winners.Count != 0) {
                    foreach (GameObject g in q.winners) {
                        if(isServer)
                          g.GetComponent<playerdata>().RpcUpdateText("you have won this round");
                    }
                }

                q.DestroyQuest();
                if (!clea) {
                    Invoke("clearQuests", 4);
                    clea = true;
                }
            }
        }
    }



    public void clearQuests() {
        shouldBeQuest = true;
        activeQuests.Clear();
        clea = false;
    }

    bool hasGameover = false;
    public void checkRounds() {

        if (currentRound < wantedRounds) {
            //            checkPlayersScore();

            if (activeQuests.Count == 0 && shouldBeQuest) {
                Quest l = generateQuest();
                if (l != null) {
                    
                    activeQuests.Add(l);
                    displayMessageToAll("roundStarting now");
                    Invoke("completeQuests", 10);
                    currentRound++;
                }
            }
        } else {
            if (!hasGameover) {
                GameObject gameOver = Instantiate(gameOverCanvas, Vector3.zero, Quaternion.identity) as GameObject;
                NetworkServer.Spawn(gameOver);
                hasGameover = true;
            }
        }

    }
    public void completeQuests() {
        displayMessageToAll("round ending now");

        foreach (Quest q in activeQuests) {
            q.isComplete = true;
        }

    }

    private Quest generateQuest() {
        if (players.Count > 1) {
            if (currentRound % 2 == 0) {
                Debug.Log("starting location for: " + players.Count);
                return new LocationQuest(players, center, 100, "find the things", this,2,20);
            } else {
                Debug.Log("starting survive for: " + players.Count);
                return new SurviveQuest(players, center, 100, "survive the enemies", this);
            }

        } else
            return null;
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
            playerdata pdata = p.GetComponent<playerdata>();
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
            playerdata pdata = p.GetComponent<playerdata>();
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
        foreach (Quest q in activeQuests) {
            Debug.Log("number of active quests: " + activeQuests.Count);
            q.tick();
        }
    }

    public void CheckCurQuests() {
        foreach(Quest q in activeQuests) {
            if (q.isComplete) {
                foreach(GameObject g in q.winners) {
                    playerdata pd = g.GetComponent<playerdata>();
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

}
