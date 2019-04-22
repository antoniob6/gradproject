using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MapManager : NetworkBehaviour {
    public MapGenerator[] mapGenerator;
    public BoxPlatform boxPlatform;


    private int mapIndex = 0;
    private int oldMapIndex = 0;
    private List<BoxPlatform> platforms;

	void Awake () {
        platforms = new List<BoxPlatform>();
        generatedMap = generateMap(transform.position);

    }
    MapGenerator generatedMap;
    void Update() {

        // if (Input.GetKeyDown(KeyCode.G)) {
        //   if (!generatedMap)
        //       generatedMap = generateMap(transform.position);
        //   else if (oldMapIndex != mapIndex) {    
        //        generatedMap = generateMap(generatedMap.transform.position+generatedMap.GetComponent<MapGenerator>().RightEdge);
        //    }
         //} 
        if (Input.GetKeyDown(KeyCode.P)) {
            addPlatform();
        }
        if (Input.GetKeyDown(KeyCode.O)) {
            addPlatformLevel2();
        }

    }
    int lastUsedIndex = 0;
    Vector3 lastPlatformEnd=Vector3.zero;
    private BoxPlatform addPlatform() {//returns null if it can't add
        float jumpHeight = generatedMap.jumpHeight+1;
        Vector3[] surfaceVerts = generatedMap.getSurfaceVerts();//in local space 


        Vector3 spawnPosition = generatedMap.transform.position + Vector3.up * (jumpHeight+generatedMap.thickness);
        int randomIndexOffset = Random.Range(4, 20);

        do {
            if (randomIndexOffset + lastUsedIndex >= surfaceVerts.Length)
                return null;

            spawnPosition = generatedMap.transform.position + Vector3.up*(jumpHeight + generatedMap.thickness);
            spawnPosition += surfaceVerts[randomIndexOffset + lastUsedIndex];
            lastUsedIndex = randomIndexOffset + lastUsedIndex;
        } while (lastPlatformEnd.x > spawnPosition.x);//we didn't pass over the last platform


        // GameObject GO = Instantiate(boxPlatform.gameObject, spawnPosition, Quaternion.identity);
        //GO.GetComponent<NetCorrector>().scale = new Vector3(Random.Range(1, 10), 1, 1);
        //  platforms.Add(GO.GetComponent<BoxPlatform>());


        GameObject GO = Instantiate(mapGenerator[0].gameObject, spawnPosition, Quaternion.identity);
        MapGenerator MP = GO.GetComponent<MapGenerator>();
        MP.seed = Random.Range(1, 50);
        MP.length = 50;
        MP.totalSurfaceVerts = 50;
        MP.updateMapPlatform(generatedMap.getSurfaceVertsInGlobalSpace());
        lastPlatformEnd = MP.RightEdge+GO.transform.position;
        


        NetworkServer.Spawn(GO);
        return GO.GetComponent<BoxPlatform>();
       

    }
    private void addPlatformLevel2() {
        float jumpHeight = generatedMap.jumpHeight+1;
        print(jumpHeight);
   
        foreach(BoxPlatform bp in platforms) {
            float randomOffset = Random.Range(-10, 20);
            Vector3 spawnPosition = Vector3.up * jumpHeight+ Vector3.right * randomOffset+ bp.transform.position;


            GameObject GO = Instantiate(boxPlatform.gameObject, spawnPosition, Quaternion.identity);
            GO.GetComponent<NetCorrector>().scale = new Vector3(Random.Range(1, 10), 1, 1);


            NetworkServer.Spawn(GO);
        }





    }

    MapGenerator generateMap(Vector3 position, Rules rules=null) {
        oldMapIndex = mapIndex;
        lastUsedIndex = 0;
       GameObject GO= Instantiate(mapGenerator[Random.Range(0,mapGenerator.Length)].gameObject, position,Quaternion.identity);
        GO.GetComponent<MapGenerator>().updateMap(mapFinishedUpdating);
        NetworkServer.Spawn(GO);
        return GO.GetComponent<MapGenerator>();
    }





    public void mapFinishedUpdating() {
        mapIndex++;
    }

    private void OnDrawGizmos() {
        //Debug.Log("drawing gizmos");
       // Gizmos.DrawSphere(Vector3.zero, 5f);
       // foreach (Vector3 v in generatedMap.GetComponent<MapGenerator>().getSurfaceVerts()) {
           // Gizmos.DrawSphere(v, 0.5f);

       // }
    }

}
