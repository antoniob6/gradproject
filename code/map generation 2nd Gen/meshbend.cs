using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meshbend : MonoBehaviour {

    public Chunk chunk;

    public float length = 2f;
    public float width = 2f;
    public int resX = 10; // 2 minimum
    private int resZ = 2;


    // Use this for initialization
    void Start()
    {
        MeshFilter filter = gameObject.AddComponent<MeshFilter>();
        Mesh mesh = filter.mesh;

        //chunk = GetComponent<Chunk>();
        // chunk.init(0, 100,0, 100, 0, true);

        makePlatform(mesh);
        makeCollision(mesh.vertices);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void makePlatform(Mesh mesh)
    {
        mesh.Clear();


        Vector3[] vertices = new Vector3[resX * resZ];

        for (int x = 0; x < resX; x++)//generate top surface verticies
        {
            // [ -width / 2, width / 2 ]
            float xPos = ((float)x / (resX - 1)) * width;
            float groundLevel = Mathf.PerlinNoise(x * 0.1f, x * x * 0.3f);
            if (x == 5)
                groundLevel = 2;
            if (x == 6)
                groundLevel = 2;
            vertices[x] = new Vector3(xPos, groundLevel);

        }

        float angle = 180;
        vertices[resX] = new Vector3(vertices[0].x, vertices[0].y - length);
        for (int x = 2; x < resX; x++)
        {//generate bottom surface verticies


            Vector2 to = new Vector2(vertices[x].x - vertices[x - 1].x, vertices[x].y - vertices[x - 1].y);
            Vector2 from = new Vector2(vertices[x - 2].x - vertices[x - 1].x, vertices[x - 2].y - vertices[x - 1].y);
            angle = Vector2.SignedAngle(to, from);


            float desiredAngle;
            if (angle >= 0)
                desiredAngle = angle / 2;
            else
                desiredAngle = angle / 2 + 180;

            Debug.Log(desiredAngle);
            Vector2 bottomVertLoc = new Vector2(to.x, to.y);
            bottomVertLoc.Normalize();
            bottomVertLoc = bottomVertLoc * length;
            bottomVertLoc = Quaternion.Euler(0, 0, desiredAngle) * bottomVertLoc;//rotate vector
            Debug.Log(bottomVertLoc);
            bottomVertLoc = (Vector2)vertices[x - 1] - bottomVertLoc;



            // vertices[resX+x-1] = new Vector3(vertices[x+resX-1].x, vertices[x+resX-1].y + length);
            vertices[(x - 1) + resX] = bottomVertLoc;

        }
        vertices[resX * resZ - 1] = new Vector3(vertices[resX - 1].x, vertices[resX - 1].y - length);




        #region UVs		
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int v = 0; v < resZ; v++)
        {
            for (int u = 0; u < resX; u++)
            {//
             // Debug.Log((float)u / (resX - 1));
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
                //int i = face % (resX - 1) + (face / (resZ - 1) * resX);
                int i = faceX + faceZ * resX;

                triangles[t++] = i + resX;
                triangles[t++] = i;
                triangles[t++] = i + 1;


                triangles[t++] = i + resX;
                triangles[t++] = i + 1;
                triangles[t++] = i + resX + 1;

            }
        }
        #endregion

        mesh.vertices = vertices;

        mesh.uv = uvs;

        mesh.triangles = triangles;

        mesh.RecalculateBounds();

    }

    private EdgeCollider2D edgeCollider;
    public PhysicsMaterial2D physicMaterial;
    public void makeCollision(Vector3[] verts)
    {

        edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
        edgeCollider.sharedMaterial = physicMaterial;

        Vector2[] colliderPath = new Vector2[resX * 2 + resZ * 2 - 1];

        int index = 0;
        for (int i = 0; i < resX; i++)
        {//top surface edges
            colliderPath[index] = new Vector2(verts[i].x, verts[i].y);
            index++;
        }
        for (int i = 0; i < resZ - 1; i++)//right surface edges
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

        edgeCollider.points = colliderPath;
    }


}
