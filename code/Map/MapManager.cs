/* this is a map manager class that manages the map 
 * once it receive an order from GameManger it spawns a map 
 * also it calculates where are the surfaces
 * so that they can be used to spawn objects
 */



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MapManager : NetworkBehaviour {
    public MapGenerator[] mapGeneratorPrefabs;
    public GameManager GM;
    public GameObject blackHolePrefab;
    public GameObject deathPlatformPrefab;

    [HideInInspector] public List<Vector3[]> surfacesInGlobalSpace;
    [HideInInspector] public List<Vector3[]> spreadSurfacesInGlobalSpace;
    private List<MapGenerator> platforms;

    public bool isBusyMakingMap = true;
    public bool finishedCreatingPlatforms = false;


    private int lastUsedIndex = 0;
    private Vector3 lastPlatformEnd = Vector3.zero;

    MapGenerator generatedMapBase;
    public void StartGenerating() {
        surfacesInGlobalSpace = new List<Vector3[]>();
        spreadSurfacesInGlobalSpace = new List<Vector3[]>();
        platforms = new List<MapGenerator>();

        createNewMap();
        //generatedMapBase = generateMap(transform.position,GM.currentRules);

    }


    private bool createdDeathBarrier = false;
    void Update() {
        if (!isServer)
            return;
        if (!isBusyMakingMap && !finishedCreatingPlatforms) {
            if (!createdDeathBarrier)
                createDeathBarier();
            platforms.Add( addPlatform(GM.currentRules));
        }



    }

    
    

    MapGenerator generateMap(Vector3 position, Rules rules=null) {
        isBusyMakingMap = true;
       GameObject GO= Instantiate(mapGeneratorPrefabs[Random.Range(0,mapGeneratorPrefabs.Length)].gameObject, position,Quaternion.identity);
        GO.transform.parent = transform;
        MapGenerator MG = GO.GetComponent<MapGenerator>();
        MG.setRules(rules);
       // MG.length = 500f;
       // MG.totalSurfaceVerts = 500;
        MG.updateMap(mapFinishedUpdating);
        NetworkServer.Spawn(GO);

        RpcUpdateBaseMapOnClients(GO);

        return GO.GetComponent<MapGenerator>();
    }

    [ClientRpc]public void RpcUpdateBaseMapOnClients(GameObject mapGO) {
        MapGenerator MG = mapGO.GetComponent<MapGenerator>();
        if (!MG) {
            Debug.Log("MapGenerator component not found");
            return;
        }
        Debug.Log("updating map base on clients");
        MG.updateMap();
    }

    private MapGenerator addPlatform(Rules rules = null) {//returns null if it can't add
        isBusyMakingMap = true;
        float jumpHeight = generatedMapBase.jumpHeight + 1;
        Vector3[] surfaceVerts = generatedMapBase.getSpreadSurfaceVertsInGlobalSpace();//in local space 


        Vector3 spawnPosition;

        int randomIndexOffset = Random.Range(4, 20);

        do {
            if (randomIndexOffset + lastUsedIndex >= surfaceVerts.Length) {
                finishedCreatingPlatforms = true;
                isBusyMakingMap = false;
                return null;

            }
            //Debug.Log(spawnPosition);
            spawnPosition = Vector3.up * (jumpHeight + generatedMapBase.thickness);
            spawnPosition += surfaceVerts[randomIndexOffset + lastUsedIndex];
            lastUsedIndex = randomIndexOffset + lastUsedIndex;
        } while (lastPlatformEnd.x > spawnPosition.x);//we didn't pass over the last platform


        //       Debug.Log(generatedMapBase.transform.position);

        GameObject GO = Instantiate(mapGeneratorPrefabs[0].gameObject, spawnPosition, Quaternion.identity);
        GO.transform.parent = transform;

        MapGenerator MG = GO.GetComponent<MapGenerator>();
        MG.setRules(rules);
        MG.seed += Random.Range(1, 50);
        MG.length = Random.Range(5, 50);
        MG.totalSurfaceVerts = (int)MG.length;
        MG.Radius += MG.jumpHeight;
        Vector3[] vertsToAvoid = spreadSurfacesInGlobalSpace[0];
        MG.updateMapPlatform(vertsToAvoid, mapFinishedUpdating);
        lastPlatformEnd = MG.getSpreadSurfaceVertsInGlobalSpace()[MG.totalSurfaceVerts - 1];



        NetworkServer.Spawn(GO);
        return MG;


    }

    [ClientRpc]public void RpcUpdateMapPlatformOnClients(GameObject mapGO) {
        MapGenerator MG = mapGO.GetComponent<MapGenerator>();
        if (!MG) {
            Debug.Log("MapGenerator component not found");
            return;
        }
        MG.updateMap();
    }





    public void mapFinishedUpdating(MapGenerator MG) {
        surfacesInGlobalSpace.Add(MG.getSurfaceVertsInGlobalSpace());
        spreadSurfacesInGlobalSpace.Add(MG.getSpreadSurfaceVertsInGlobalSpace());
        isBusyMakingMap = false;

    }



    public void createNewMap() {

        deleteOldMap();
        //create new map

        generatedMapBase = generateMap(transform.position,GM.currentRules);
        finishedCreatingPlatforms = false;
        createdDeathBarrier = false;

    }
    private void deleteOldMap() {

        if (!generatedMapBase)
            return;
        NetworkServer.Destroy(generatedMapBase.gameObject);
        generatedMapBase = null;
        foreach(MapGenerator MG in platforms) {
            if (!MG)
                continue;
            NetworkServer.Destroy(MG.gameObject);
        }
        platforms.Clear();
        surfacesInGlobalSpace.Clear();
        isBusyMakingMap = false;


        lastPlatformEnd = Vector3.zero;
        lastUsedIndex = 0;
    }

    public Vector3 getRandomPosition() {
        if (surfacesInGlobalSpace.Count == 0)
            return Vector3.zero;
        int randomSurfaceIndex = Random.Range(0, surfacesInGlobalSpace.Count);
        Vector3[] surfaceVerts = surfacesInGlobalSpace[randomSurfaceIndex];

        int randomVertIndex = Random.Range(0, surfaceVerts.Length);

        Vector3 randomPosition = surfaceVerts[randomVertIndex];

        return randomPosition;
    }

    public void createDeathBarier() {
        if (GM.currentRules.isCircle) {
            //add black hole
            Debug.Log("spawned black hole");
            GameObject GO = Instantiate(blackHolePrefab, Vector3.zero, Quaternion.identity);
            Vector3 tmpScale = GO.transform.localScale;
            tmpScale = tmpScale * GM.currentRules.Radius;
            GO.transform.localScale = tmpScale;
            NetworkServer.Spawn(GO);
        } else {
            //add death platform
            Debug.Log("spawned death playform");
            Vector3 rightend = generatedMapBase.RightEdge;
            Vector3 middle = new Vector3(rightend.x / 2, -100, 0);

            GameObject GO = Instantiate(deathPlatformPrefab, middle, Quaternion.identity);
            Vector3 tmpScale = GO.transform.localScale;
            tmpScale.x = rightend.x;
            GO.transform.localScale = tmpScale;
            NetworkServer.Spawn(GO);
        }
    }

    public void circlizeAllMaps() {


    }




    private void OnDrawGizmos() {
        //Debug.Log("drawing gizmos");
       // Gizmos.DrawSphere(Vector3.zero, 5f);
       // foreach (Vector3 v in generatedMap.GetComponent<MapGenerator>().getSurfaceVerts()) {
           // Gizmos.DrawSphere(v, 0.5f);

       // }
    }

}
