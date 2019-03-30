/*
 * this file generates a map randomly
 * it uses the construsts a mesh from scratch accroding to the design in the project 
 * it construsts vertices, triangle and UVs, by using mathmatics
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using UnityEngine.Networking;

public class MapGenerator: NetworkBehaviour {
    public delegate void VoidFunctionDelegate();
    public GameManager GM;
    public Vector3 endVertixPos;


    [SyncVar]public float length = 2f;
    [SyncVar]public float width = 2f;
    [SyncVar]public int resX = 10; // 2 minimum
    [SyncVar]public int seed=0;
    // public float stepHeight = 2f;
    [SyncVar]public float jumpHeight = 1.5f;
    [SyncVar]public int stepWidth=4;
    [SyncVar]public float Radius = 4;
    [SyncVar]public Boolean isCircle= true;

    private int resZ = 2;


    private Mesh mainMesh;

    private int oldSeed;
    // Use this for initialization
    private EdgeCollider2D edgeCollider;
    void Start () {
        MeshFilter filter = gameObject.GetComponent<MeshFilter>();
        mainMesh = filter.mesh;

        edgeCollider = gameObject.GetComponent<EdgeCollider2D>();

        oldSeed = seed;
    }

   public  void setRules(Rules rules)
    {
    length = rules.length;
  width =rules.width;
    resX = rules.resX; // 2 minimum
     seed = rules.seed;
     jumpHeight = rules.jumpHeight;
     stepWidth = rules.stepWidth;
    Radius = rules.Radius;
    isCircle = rules.isCircle;
}




// Update is called once per frame
void Update () {
        //if (oldSeed != seed)

    }
    public void updateMap(VoidFunctionDelegate callback=null) {
        if (!mainMesh)
            mainMesh = gameObject.GetComponent<MeshFilter>().mesh;

        createMesh(mainMesh);

        if (isCircle)
            turnIntoCircle(mainMesh, mainMesh.vertices[0], Radius);
        makeCollision(mainMesh.vertices);
        mainMesh.RecalculateBounds();

        if (callback != null) {
            callback();
        }

    }

 


    public void createMesh(Mesh mesh)
    {
        mesh.Clear();
        Vector3[] vertices = new Vector3[resX * resZ];

        generateSurfaceVerts(vertices,jumpHeight,stepWidth,seed, jumpHeight);

        vertices=createBottomVertices(vertices);

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int v = 0; v < resZ; v++){
            for (int u = 0; u < resX; u++) { 
                uvs[u + v * resX] = new Vector2((float)u / (resX - 1), (float)v / (resZ - 1));
            }
        }


        int nbFaces = (resX - 1) * (resZ - 1);
        int[] triangles = new int[nbFaces * 6];
        int t = 0;
        for (int faceZ =0; faceZ < resZ-1; faceZ++){
            for (int faceX = 0; faceX < resX- 1; faceX++)
            {
                int i = faceX + faceZ * resX;
                
                triangles[t++] = i + resX;
                triangles[t++] = i;
                triangles[t++] = i + 1;


                triangles[t++] = i + resX;
                triangles[t++] = i + 1;
                triangles[t++] = i + resX + 1;
 
            }
        }

        mesh.vertices = vertices;

        mesh.uv = uvs;

        mesh.triangles = triangles;

        

    }
    private Vector3[] createBottomVertices(Vector3[] vertices) {
        float angle = 180;
        for (int x = 2; x < resX; x++) {//generate bottom surface verticies


            Vector2 to = new Vector2(vertices[x].x - vertices[x - 1].x, vertices[x].y - vertices[x - 1].y);
            Vector2 from = new Vector2(vertices[x - 2].x - vertices[x - 1].x, vertices[x - 2].y - vertices[x - 1].y);
            angle = Vector2.SignedAngle(to, from);
            float desiredAngle;
            if (angle >= 0)
                desiredAngle = angle / 2;
            else
                desiredAngle = angle / 2 + 180;


            Vector2 bottomVertLoc = new Vector2(to.x, to.y);
            bottomVertLoc.Normalize();
            bottomVertLoc = bottomVertLoc * length;
            bottomVertLoc = Quaternion.Euler(0, 0, desiredAngle) * bottomVertLoc;//rotate vector

            bottomVertLoc = (Vector2)vertices[x - 1] - bottomVertLoc;
            vertices[(x - 1) + resX] = bottomVertLoc;

        }
        vertices[resX * resZ - 1] = new Vector3(vertices[resX - 1].x, vertices[resX - 1].y - length);

        return vertices;
    }
 


    public void makeCollision(Vector3[] verts)
    {

       Vector2[] colliderPath = new Vector2[resX*2+resZ*2-1];
       
        int index = 0;
        for (int i = 0; i < resX; i++){//top surface edges
             colliderPath[index] = new Vector2(verts[i].x, verts[i].y);
             index++;
        }
        for (int i = 0; i < resZ-1; i++)//right surface edges
        {
            colliderPath[index] = new Vector2(verts[i*resX+resX-1].x, verts[i * resX + resX - 1].y);
            index++;
        }
        for (int i = 0; i < resX; i++)
        {//bottom surface edges
            colliderPath[index] = new Vector2(verts[resX*resZ-1-i].x, verts[resX * resZ - 1-i].y);
            index++;
        }
        for (int i = resZ - 1; i >=0; i--)//left surface edges
        {
            colliderPath[index] = new Vector2(verts[i * resX ].x, verts[i * resX ].y);
            index++;
        }
        if (!edgeCollider) {
            edgeCollider = gameObject.GetComponent<EdgeCollider2D>();
            print("looking for collider componenet");
        }


         edgeCollider.points = colliderPath;
    }

    private void generateSurfaceVerts(Vector3[] vertices, float stepHight, int stepWidth, int seed,float jumpHeight)
    {
        Random.InitState(seed);
        int x = 0;
        float unitWidth = ((float)1 / (resX - 1)) * width;
        int prvDirection = 0;
        Vector3 prvVert = new Vector3(0, 0, 0);
        for (; x < resX; )
        {
            
           
            int paltformDirection= Random.Range(0, 5);

            if (paltformDirection == 0 || paltformDirection == prvDirection)
            {
               prvVert= platformStraight(vertices, prvVert, stepWidth, resX, Vector3.right * unitWidth, ref x);
            }
            else if (paltformDirection == 1)//up
            {

                    prvVert = platformStraight(vertices, prvVert, stepWidth, resX,
                        Vector3.up * (jumpHeight / (float)stepWidth) + Vector3.right * unitWidth,
                        ref x);
                
            }
            else if (paltformDirection == 2)//dn
            {

                prvVert = platformStraight(vertices, prvVert, stepWidth, resX,
Vector3.down * (jumpHeight / (float)stepWidth) + Vector3.right * unitWidth,
ref x);
              
                
            }
            else if (paltformDirection == 3&&prvDirection!=4)//wall up
            {
                prvVert = platformStraight(vertices, prvVert, stepWidth/3, resX, Vector3.right * unitWidth, ref x);
                prvVert = platformStraight(vertices, prvVert, stepWidth / 3, resX, Vector3.up * jumpHeight, ref x);
                prvVert = platformStraight(vertices, prvVert, stepWidth / 3, resX, Vector3.right * unitWidth, ref x);

                
            }
            else if (paltformDirection == 4 && prvDirection != 3)//wall dn
            {

                prvVert = platformStraight(vertices, prvVert, stepWidth / 3, resX, Vector3.right * unitWidth, ref x);
                prvVert = platformStraight(vertices, prvVert, stepWidth / 3, resX, Vector3.down * jumpHeight, ref x);
                prvVert = platformStraight(vertices, prvVert, stepWidth / 3, resX, Vector3.right * unitWidth, ref x);

            }
            //Debug.Log(prvVert);
            // float groundLevel = Mathf.PerlinNoise(x * 0.1f, x * x * 0.3f);
            prvDirection = paltformDirection;
        
        }
        vertices[resX] = new Vector3(vertices[0].x, vertices[0].y - length);

        endVertixPos = vertices[resX-1];

    }
    private Vector3 platformStraight(Vector3[] vertices,Vector3 prvVert,int numOfVerts,int maxVertIndex,
                                        Vector3 step, ref int x)
    {
        for (int i = 0; i < numOfVerts && x < maxVertIndex; i++)
        {

            vertices[x] = prvVert + step;
            prvVert = vertices[x];
            x++;
        }
        return prvVert;
    }
    private void turnIntoCircle(Mesh mesh, Vector3 origin,float radius,float hightScale=1.0f){
        Vector3[] verts = mesh.vertices;
        for(int i=0;i< verts.Length; i++) {
            Vector3 to=  Vector3.up*((verts[i].y - origin.y)* hightScale + radius);
            to = Quaternion.Euler(0, 0,- verts[i].x/(verts[verts.Length - 1].x-verts[0].x) * 360f) * to ;
            verts[i] = to;
        }
        mesh.vertices = verts;

    }

    public Vector3[] getSurfaceVerts() {
        Vector3[] surfVerts = new Vector3[resX];
        for (int i = 0; i < resX; i++)
            surfVerts[i] = mainMesh.vertices[i];
        return surfVerts;
    }







}
