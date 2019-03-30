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

	void Start () {
        platforms = new List<BoxPlatform>();
        
	}
    MapGenerator generatedMap;
    void Update () {

        if (Input.GetKeyDown(KeyCode.G)) {
            if (!generatedMap)
                generatedMap = generateMap(transform.position);
            else if (oldMapIndex != mapIndex) {    
                generatedMap = generateMap(generatedMap.transform.position+generatedMap.GetComponent<MapGenerator>().endVertixPos);
            }
        } else if (Input.GetKeyDown(KeyCode.P)) {
            addPlatform();
        } else if (Input.GetKeyDown(KeyCode.O)) {
            addPlatformLevel2();
        }

    }
    int lastUsedIndex = 0;
    private BoxPlatform addPlatform() {
        float jumpHeight = generatedMap.jumpHeight+1;
        Vector3[] surfaceVerts = generatedMap.getSurfaceVerts();

        Vector3 spawnPosition= generatedMap.transform.position+ Vector3.up*jumpHeight;

        int randomIndexOffset = Random.Range(4, 20);
        spawnPosition += surfaceVerts[randomIndexOffset+lastUsedIndex];
        lastUsedIndex = randomIndexOffset + lastUsedIndex;

        GameObject GO = Instantiate(boxPlatform.gameObject, spawnPosition, Quaternion.identity);
        GO.GetComponent<NetCorrector>().scale = new Vector3(Random.Range(1, 10), 1, 1);
        platforms.Add(GO.GetComponent<BoxPlatform>());
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
        GameObject GO= Instantiate(mapGenerator[Random.Range(0,mapGenerator.Length)].gameObject, position,Quaternion.identity);
        GO.GetComponent<MapGenerator>().updateMap(mapFinishedUpdating);
        NetworkServer.Spawn(GO);
        return GO.GetComponent<MapGenerator>();
    }





    public void mapFinishedUpdating() {
        mapIndex++;
    }

    public void OnDrawGizmos() {
        foreach (Vector3 v in generatedMap.GetComponent<MapGenerator>().getSurfaceVerts()) {
           // Gizmos.DrawSphere(v, 0.5f);
        }
    }
}
