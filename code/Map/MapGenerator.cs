/*
 * this file generates a map randomly
 * it uses the construsts a mesh from scratch accroding to the design in the project 
 * it construsts vertices, triangle and UVs, by using mathmatics
 * 
 */
using System;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

public class MapGenerator: NetworkBehaviour {
    public delegate void VoidFunctionDelegate();
    public GameManager GM;
    public Vector3 RightEdge;


    [SyncVar]public float thickness = 2f;
    [SyncVar]public float length = 50f;
    [SyncVar]public int totalSurfaceVerts = 50; // 2 minimum
    [SyncVar]public int seed=0;
    // public float stepHeight = 2f;
    [SyncVar]public float jumpHeight = 1.5f;
    [SyncVar]public int vertsPerPlatform=4;
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
    thickness = rules.length;
  length =rules.width;
    totalSurfaceVerts = rules.resX; 
     seed = rules.seed;
     jumpHeight = rules.jumpHeight;
     vertsPerPlatform = rules.stepWidth;
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


    Vector3[] currentSurface;

    public void updateMapPlatform(Vector3[] surfaceToAvoid, VoidFunctionDelegate callback = null) {
        if (!mainMesh)
            mainMesh = gameObject.GetComponent<MeshFilter>().mesh;

        surfaceToAvoid = tranlateVertsToLocalSpace(surfaceToAvoid);
        Vector3[] surfaceVerts= createSurfacePlaneThatAvoids(surfaceToAvoid,totalSurfaceVerts);
        currentSurface = surfaceVerts;
        
        Mesh mesh=createMeshFromSurfacePlane(surfaceVerts);

        if (isCircle)
            turnIntoCircle(mesh, mesh.vertices[0], Radius);
        makeCollision(mesh.vertices);
        mainMesh.RecalculateBounds();

        if (callback != null) {
            callback();
        }
        mainMesh.vertices = mesh.vertices;

        mainMesh.uv = mesh.uv;

        mainMesh.triangles = mesh.triangles;


    }


    private Mesh createMeshFromSurfacePlane(Vector3[] surface) {
        Mesh mesh=new Mesh();
        Vector3[] vertices = createMeshVerticesFromSurface(surface);
       // currentSurface = vertices;
        #region uvsAndTriangles
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int v = 0; v < resZ; v++) {
            for (int u = 0; u < totalSurfaceVerts; u++) {
                // uvs[u + v * totalSurfaceVerts] = new Vector2((float)u / (totalSurfaceVerts - 1), (float)v / (resZ - 1));
                uvs[u + v * totalSurfaceVerts] = new Vector2((float)u * 0.1f, (float)v * 0.1f);

            }
        }


        int nbFaces = (totalSurfaceVerts - 1) * (resZ - 1);
        int[] triangles = new int[nbFaces * 6];
        int t = 0;
        for (int faceZ = 0; faceZ < resZ - 1; faceZ++) {
            for (int faceX = 0; faceX < totalSurfaceVerts - 1; faceX++) {
                int i = faceX + faceZ * totalSurfaceVerts;

                triangles[t++] = i + totalSurfaceVerts;
                triangles[t++] = i;
                triangles[t++] = i + 1;


                triangles[t++] = i + totalSurfaceVerts;
                triangles[t++] = i + 1;
                triangles[t++] = i + totalSurfaceVerts + 1;

            }
        }
        #endregion
        mesh.vertices = vertices;

        mesh.uv = uvs;

        mesh.triangles = triangles;
        return mesh;


    }
    private Vector3[] createMeshVerticesFromSurface(Vector3[] surface) {
        float angle = 180;
        Vector3[] vertices = new Vector3[surface.Length * 2];
        for(int i = 0; i < surface.Length; i++) {
            vertices[i] = surface[i];
        }
        vertices[surface.Length] = new Vector3(surface[0].x, surface[0].y - thickness);
        for (int x = 2; x < surface.Length; x++) {//generate bottom surface verticies


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
            bottomVertLoc = bottomVertLoc * thickness;
            bottomVertLoc = Quaternion.Euler(0, 0, desiredAngle) * bottomVertLoc;//rotate vector

            bottomVertLoc = (Vector2)vertices[x - 1] - bottomVertLoc;
            vertices[(x - 1) + surface.Length] = bottomVertLoc;

        }
        vertices[surface.Length * 2 - 1] = new Vector3(vertices[surface.Length - 1].x, vertices[surface.Length - 1].y - thickness);
     //   Debug.Log(vertices[surface.Length * 2 - 1]);
        return vertices;
    }

    public void createMesh(Mesh mesh)
    {
        mesh.Clear();
        Vector3[] vertices = new Vector3[totalSurfaceVerts * resZ];

        generateSurfaceVerts(vertices,jumpHeight,vertsPerPlatform,seed, jumpHeight);

        vertices=createBottomVertices(vertices);

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int v = 0; v < resZ; v++){
            for (int u = 0; u < totalSurfaceVerts; u++) {
                //uvs[u + v * totalSurfaceVerts] = new Vector2((float)u / (totalSurfaceVerts - 1), (float)v / (resZ - 1));
                // uvs[u + v * totalSurfaceVerts] = new Vector2(u * 0.1f, v * 0.1f);
                uvs[u + v * totalSurfaceVerts] = new Vector2(vertices[u + v * totalSurfaceVerts].x * 0.1f, vertices[u + v * totalSurfaceVerts].y * 0.1f);
            }
        }


        int nbFaces = (totalSurfaceVerts - 1) * (resZ - 1);
        int[] triangles = new int[nbFaces * 6];
        int t = 0;
        for (int faceZ =0; faceZ < resZ-1; faceZ++){
            for (int faceX = 0; faceX < totalSurfaceVerts- 1; faceX++)
            {
                int i = faceX + faceZ * totalSurfaceVerts;
                
                triangles[t++] = i + totalSurfaceVerts;
                triangles[t++] = i;
                triangles[t++] = i + 1;


                triangles[t++] = i + totalSurfaceVerts;
                triangles[t++] = i + 1;
                triangles[t++] = i + totalSurfaceVerts + 1;
 
            }
        }

        mesh.vertices = vertices;

        mesh.uv = uvs;

        mesh.triangles = triangles;

        

    }

    private Vector3[] createBottomVertices(Vector3[] vertices) {
        float angle = 180;
        for (int x = 2; x < totalSurfaceVerts; x++) {//generate bottom surface verticies


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
            bottomVertLoc = bottomVertLoc * thickness;
            bottomVertLoc = Quaternion.Euler(0, 0, desiredAngle) * bottomVertLoc;//rotate vector

            bottomVertLoc = (Vector2)vertices[x - 1] - bottomVertLoc;
            vertices[(x - 1) + totalSurfaceVerts] = bottomVertLoc;

        }
        vertices[totalSurfaceVerts * resZ - 1] = new Vector3(vertices[totalSurfaceVerts - 1].x, vertices[totalSurfaceVerts - 1].y - thickness);

        return vertices;
    }
 


    public void makeCollision(Vector3[] verts)
    {

       Vector2[] colliderPath = new Vector2[totalSurfaceVerts*2+resZ*2-1];
       
        int index = 0;
        for (int i = 0; i < totalSurfaceVerts; i++){//top surface edges
             colliderPath[index] = new Vector2(verts[i].x, verts[i].y);
             index++;
        }
        for (int i = 0; i < resZ-1; i++)//right surface edges
        {
            colliderPath[index] = new Vector2(verts[i*totalSurfaceVerts+totalSurfaceVerts-1].x, verts[i * totalSurfaceVerts + totalSurfaceVerts - 1].y);
            index++;
        }
        for (int i = 0; i < totalSurfaceVerts; i++)
        {//bottom surface edges
            colliderPath[index] = new Vector2(verts[totalSurfaceVerts*resZ-1-i].x, verts[totalSurfaceVerts * resZ - 1-i].y);
            index++;
        }
        for (int i = resZ - 1; i >=0; i--)//left surface edges
        {
            colliderPath[index] = new Vector2(verts[i * totalSurfaceVerts ].x, verts[i * totalSurfaceVerts ].y);
            index++;
        }
        if (!edgeCollider) {
            edgeCollider = gameObject.GetComponent<EdgeCollider2D>();
            //print("looking for collider componenet");
        }


         edgeCollider.points = colliderPath;
    }

    private void generateSurfaceVerts(Vector3[] vertices, float stepHight, int stepWidth, int seed,float jumpHeight)
    {
        Random.InitState(seed);
        int x = 0;
        float unitWidth = ((float)1 / (totalSurfaceVerts - 1)) * length;
        int prvDirection = 0;
        Vector3 prvVert = new Vector3(0, 0, 0);
        for (; x < totalSurfaceVerts; )
        {
            
           
            int paltformDirection= Random.Range(0, 5);

            if (paltformDirection == 0 || paltformDirection == prvDirection)
            {
               prvVert= platformStraight(vertices, prvVert, stepWidth, totalSurfaceVerts, Vector3.right * unitWidth, ref x);
            }
            else if (paltformDirection == 1)//up
            {

                    prvVert = platformStraight(vertices, prvVert, stepWidth, totalSurfaceVerts,
                        Vector3.up * (jumpHeight / (float)stepWidth) + Vector3.right * unitWidth,
                        ref x);
                
            }
            else if (paltformDirection == 2)//dn
            {

                prvVert = platformStraight(vertices, prvVert, stepWidth, totalSurfaceVerts,
Vector3.down * (jumpHeight / (float)stepWidth) + Vector3.right * unitWidth,
ref x);
              
                
            }
            else if (paltformDirection == 3&&prvDirection!=4)//wall up
            {
                prvVert = platformStraight(vertices, prvVert, stepWidth/3, totalSurfaceVerts, Vector3.right * unitWidth, ref x);
                prvVert = platformStraight(vertices, prvVert, stepWidth / 3, totalSurfaceVerts, Vector3.up * jumpHeight, ref x);
                prvVert = platformStraight(vertices, prvVert, stepWidth / 3, totalSurfaceVerts, Vector3.right * unitWidth, ref x);

                
            }
            else if (paltformDirection == 4 && prvDirection != 3)//wall dn
            {

                prvVert = platformStraight(vertices, prvVert, stepWidth / 3, totalSurfaceVerts, Vector3.right * unitWidth, ref x);
                prvVert = platformStraight(vertices, prvVert, stepWidth / 3, totalSurfaceVerts, Vector3.down * jumpHeight, ref x);
                prvVert = platformStraight(vertices, prvVert, stepWidth / 3, totalSurfaceVerts, Vector3.right * unitWidth, ref x);

            }
            //Debug.Log(prvVert);
            // float groundLevel = Mathf.PerlinNoise(x * 0.1f, x * x * 0.3f);
            prvDirection = paltformDirection;
        
        }
        vertices[totalSurfaceVerts] = new Vector3(vertices[0].x, vertices[0].y - thickness);

        RightEdge = vertices[totalSurfaceVerts-1];

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
        Vector3[] surfVerts = new Vector3[totalSurfaceVerts];
        for (int i = 0; i < totalSurfaceVerts; i++)
            surfVerts[i] = mainMesh.vertices[i];
        return surfVerts;
    }
    public Vector3[] getSurfaceVertsInGlobalSpace() {
        Vector3[] surfVerts = new Vector3[totalSurfaceVerts];
        for (int i = 0; i < totalSurfaceVerts; i++)
            surfVerts[i] = mainMesh.vertices[i]+transform.position;
        return surfVerts;
    }
    public Vector3[] tranlateVertsToLocalSpace(Vector3[] verts) {
        Vector3[] surfVerts = new Vector3[verts.Length];
        for (int i = 0; i < verts.Length; i++)
            surfVerts[i] = verts[i] - transform.position;
        return surfVerts;
    }
    public Vector3[] tranlateVertsToGlobalSpace(Vector3[] verts) {
        Vector3[] surfVerts = new Vector3[verts.Length];
        for (int i = 0; i < verts.Length; i++)
            surfVerts[i] = verts[i] + transform.position;
        return surfVerts;
    }



    private Vector3[] preventedCollision;
    private int pcindex = 0;

    public Vector3[] createSurfacePlaneThatAvoids(Vector3[] surfaceToAvoid,int numOfVerts) {
        preventedCollision = new Vector3[100];



        Vector3[] vertices = new Vector3[numOfVerts];
        Random.InitState(seed);
        int x = 0;
        float unitWidth = ((float)1 / (totalSurfaceVerts - 1)) * length;
        int prvDirection = 0;
        Vector3 prvVert = new Vector3(0, 0, 0);
        for (; x < vertices.Length;) {

           

            int platformDirection = Random.Range(0, 5);


            Vector3[] temp =  createPlatformSurface(prvVert,
                        Vector3.down * (jumpHeight/vertsPerPlatform) + Vector3.right * unitWidth,vertsPerPlatform);
            Vector3 predictedVert = temp[temp.Length-1]+Vector3.down*thickness;

            if (minDistance(predictedVert, surfaceToAvoid) < jumpHeight*1.3f) {
                preventedCollision[pcindex] = predictedVert;
                pcindex++;
               Debug.Log("prevented collision");
                platformDirection = 1;
            }

            if (platformDirection == 0 || platformDirection == prvDirection && platformDirection!= 1) {
                prvVert = platformStraight(vertices, prvVert, vertsPerPlatform, numOfVerts, Vector3.right * unitWidth, ref x);
            } else if (platformDirection == 1)//up
              {

                prvVert = platformStraight(vertices, prvVert, vertsPerPlatform, numOfVerts,
                    Vector3.up * (jumpHeight / (float)vertsPerPlatform) + Vector3.right * unitWidth,
                    ref x);

            } else if (platformDirection == 2)//dn
              {

                prvVert = platformStraight(vertices, prvVert, vertsPerPlatform, numOfVerts,
Vector3.down * (jumpHeight / (float)vertsPerPlatform) + Vector3.right * unitWidth,
ref x);


            } else if (platformDirection == 3 && prvDirection != 4)//wall up
              {
                prvVert = platformStraight(vertices, prvVert, vertsPerPlatform / 3, numOfVerts, Vector3.right * unitWidth, ref x);
                prvVert = platformStraight(vertices, prvVert, vertsPerPlatform / 3, numOfVerts, Vector3.up * jumpHeight, ref x);
                prvVert = platformStraight(vertices, prvVert, vertsPerPlatform / 3, numOfVerts, Vector3.right * unitWidth, ref x);


            } else if (platformDirection == 4 && prvDirection != 3)//wall dn
              {

                prvVert = platformStraight(vertices, prvVert, vertsPerPlatform / 3, numOfVerts, Vector3.right * unitWidth, ref x);
                prvVert = platformStraight(vertices, prvVert, vertsPerPlatform / 3, numOfVerts, Vector3.down * jumpHeight, ref x);
                prvVert = platformStraight(vertices, prvVert, vertsPerPlatform / 3, numOfVerts, Vector3.right * unitWidth, ref x);

            }
            //Debug.Log(prvVert);
            // float groundLevel = Mathf.PerlinNoise(x * 0.1f, x * x * 0.3f);
            prvDirection = platformDirection;

        }
       // vertices[vertices.Length-1] = new Vector3(vertices[0].x, vertices[0].y - length);

        RightEdge = vertices[totalSurfaceVerts - 1];
        return vertices;
    }


    private Vector3[] createPlatformSurface(Vector3 origin,Vector3 direction, int vertCount, float stepSize=1) {
        Vector3[] temp = new Vector3[vertCount];
        for (int i = 0; i < temp.Length; i++) {
            temp[i] = origin+ direction * stepSize * i;
        }
        return temp;
    }
    private int getClosestPoint(Vector3 position, Vector3[] points) {

        int closest=-1;
        float minDist = Mathf.Infinity;
        int i = 0;
        foreach (Vector3 t in points) {
            float dist = Vector3.SqrMagnitude(position-t);
            if (dist < minDist) {
                closest = i;
                minDist = dist;
            }
            i++;
        }
        return closest;
    }
    private float minDistance(Vector3 position, Vector3[] points) {
        return Vector3.Magnitude(position - points[getClosestPoint(position, points)]);
    }

   
    private void OnDrawGizmosSelected() {
       // Debug.Log("drawing gizmox");
        //Gizmos.DrawSphere(Vector3.zero, 5f);
       
        Vector3[] GS1CurrentSurface = tranlateVertsToGlobalSpace(currentSurface);
        Vector3[] GSCurrentSurface = tranlateVertsToGlobalSpace(preventedCollision);
        foreach (Vector3 v in GSCurrentSurface) {
            Gizmos.DrawSphere(v, 0.2f);
        }
        foreach (Vector3 v in GS1CurrentSurface) {
            Gizmos.DrawCube(v, new Vector3(0.1f, 0.1f, 0.1f));
        }
    }
    
   

}
