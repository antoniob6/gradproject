using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mesh1 : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //some parts of the code are from http://wiki.unity3d.com/index.php/ProceduralPrimitives
    public void makePlatform(Mesh mesh,int resX,int resZ,float length,float width)
    {
        mesh.Clear();


        Vector3[] vertices = new Vector3[resX * resZ];
        for (int z = 0; z < resZ; z++)
        {
            // [ -length / 2, length / 2 ]
            float zPos = ((float)z / (resZ - 1) - .5f) * length;
            for (int x = 0; x < resX; x++)
            {
                // [ -width / 2, width / 2 ]
                float xPos = ((float)x / (resX - 1) - .5f) * width;
                vertices[x + z * resX] = new Vector3(xPos, zPos + xPos * xPos);
            }
        }

        #region UVs		
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int v = 0; v < resZ; v++)
        {
            for (int u = 0; u < resX; u++)
            {
                uvs[u + v * resX] = new Vector2((float)u / (resX - 1), (float)v / (resZ - 1));
            }
        }
        #endregion

        #region Triangles
        int nbFaces = (resX - 1) * (resZ - 1);
        int[] triangles = new int[nbFaces * 6];
        int t = 0;
        for (int faceZ = 0; faceZ < resZ - 1; faceZ++)
        {
            for (int faceX = 0; faceX < resX - 1; faceX++)
            {
                // Retrieve top left corner from face ind
                int i = faceX + faceZ * resX;

                triangles[t++] = i + resX;
                triangles[t++] = i + 1;
                triangles[t++] = i;

                triangles[t++] = i + resX;
                triangles[t++] = i + resX + 1;
                triangles[t++] = i + 1;
            }
        }
        #endregion

        mesh.vertices = vertices;

        mesh.uv = uvs;

        mesh.triangles = triangles;

        mesh.RecalculateBounds();

    }
}
