using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisions : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void makeCollisionPath(Vector3[] verts,int resX,int resZ,Vector2[] colliderPath)
    {
        colliderPath = new Vector2[resX * 2 + resZ * 2 - 2];

        int index = 0;
        for (int i = 0; i < resX; i++)
        {//top surface edges
            colliderPath[index] = new Vector2(verts[i].x, verts[i].y);
            index++;
        }
        for (int i = 1; i < resZ - 1; i++)//right surface edges
        {
            colliderPath[index] = new Vector2(verts[i * resX + resX - 1].x, verts[i * resX + resX - 1].y);
            index++;
        }
        for (int i = 0; i < resX; i++)
        {//bottom surface edges
            colliderPath[index] = new Vector2(verts[resX * resZ - 1 - i].x, verts[resX * resZ - 1 - i].y);
            index++;
        }
        for (int i = resZ - 1; i >= 0; i--)//left surface edges
        {
            colliderPath[index] = new Vector2(verts[i * resX].x, verts[i * resX].y);
            index++;
        }

        
    }
}
