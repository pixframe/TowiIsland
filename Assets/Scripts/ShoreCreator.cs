/********************************
** PARADISE PAINTBALL
**
** Module: 		Dev/AssetUtilies
**
** Author(s): 	Thomas Franken
** Copyright:	(c) CMUNE Ltd.
********************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Create dynamically a shoreline around an arbitrary Terrain on runtime.
/// Attach this script to a plane and position it such that it creates an intersection line with an terrain.
/// 
/// TODO: 
/// - respect the rotation of the plane
/// - rewrite UV coordinate computation
/// - support partial shoreline computation
/// - optimize form of inner and outer borderline
/// - detach sampling resultion from resulting mesh resolution
/// </summary>
public class ShoreCreator : MonoBehaviour
{
    //public Terrain terrain;  
  
    /// <summary>
    /// Reference to the Gameobject of the Terrain (or other object) you want to create a shoreline around
    /// </summary>
    public GameObject mainTerrain;

    /// <summary>
    /// Threshold when to drop a shore triangle if too small
    /// Default: 0.03F
    /// </summary>
    public float threshold = 0.03F;

    /// <summary>
    /// Increase the neighbourhood to smooth out the outer borderline (seaside)
    /// Default: 2
    /// </summary>
    public int neighbourhood = 2;

    /// <summary>
    /// Resolution for the sampling of the terrain and the final mesh
    /// Default: 100
    /// </summary>
    public int meshResolution = 150;

    /// <summary>
    /// Define how deep should the shoreline should reach into the sea
    /// Default: 1
    /// </summary>
    public int scaling = 1;

    private MeshFilter shoreLine;
    private float height = 0;

    private Vector3[] vertices;
    private int[] triangles;

    private Triangle[] myTriangles;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        shoreLine = (MeshFilter)GetComponent(typeof(MeshFilter));
        height = transform.position.y;

        //if(terrain != null) 
        //{
        //    mainTerrain = terrain.gameObject;
        //}

        if (shoreLine != null)
        {
            //resample the terrain inside of the "selection" area of the plane
            adaptPlaneToTerrain();

            //find the triangles defining a shoreline
            extractShoreTriangles();
        }
    }

    /// <summary>
    /// We sample the area defined by our plane, using the resulution resx and resy.
    /// To do so we cast a ray for every sample point and create a new vertex for every hitpoint.
    /// So basically we just adapt the plane on the terrain giving a specific resolution.
    /// After that the vertices are triangulated and saved as the new mesh of the plane.
    /// </summary>
    private void adaptPlaneToTerrain()
    {
        Vector3 origin = GetComponent<Renderer>().bounds.min;
        Vector3 size = GetComponent<Renderer>().bounds.size;

        int resx = meshResolution;
        int resy = meshResolution;

        float aspect = size.x / size.z;
        if (aspect < 1) resx = Mathf.RoundToInt(resx * aspect);
        else if (aspect > 1) resy = Mathf.RoundToInt(resy / aspect);
        size.Scale(new Vector3(1 / (float)(resx - 1), 0, 1 / (float)(resy - 1)));

        vertices = new Vector3[resx * resy];
        triangles = new int[6 * (resx - 1) * (resy - 1)];
        myTriangles = new Triangle[triangles.Length / 3];

        int layer = 1 << mainTerrain.layer;
        for (int i = 0; i < resx; i++)
        {
            for (int j = 0; j < resy; j++)
            {
                // CREATE VERTICES
                RaycastHit hit;
                if (Physics.Raycast(new Ray(new Vector3(origin.x + i * size.x, 10000, origin.z + j * size.z), Vector3.down), out hit, 10000, layer))
                {
                    vertices[i * resy + j] = hit.point;
                }
                else
                {
                    vertices[i * resy + j] = new Vector3(origin.x + i * size.x, 0, origin.z + j * size.z);
                }

                // DO TESSELATION
                if (i < resx - 1 && j < resy - 1)
                {
                    int tidx = 6 * (i * (resy - 1) + j);
                    triangles[tidx + 0] = (i * resy + j + 1);// % (res * res);
                    triangles[tidx + 1] = ((i + 1) * resy + j);// % (res * res);
                    triangles[tidx + 2] = (i * resy + j);// % (res * res);

                    triangles[tidx + 3] = (i * resy + j + 1);// % (res * res);
                    triangles[tidx + 4] = ((i + 1) * resy + j + 1);// % (res * res);
                    triangles[tidx + 5] = ((i + 1) * resy + j);// % (res * res);

                    int ntidx = 2 * (i * (resy - 1) + j);

                    //here we save all triangles in a helper data structure, used to search for the shoreline later on
                    myTriangles[ntidx + 0] = new Triangle(ntidx, i, j, resx, resy, true);
                    myTriangles[ntidx + 0].SetVertices(triangles[tidx + 0], triangles[tidx + 1], triangles[tidx + 2]);

                    myTriangles[ntidx + 1] = new Triangle(ntidx + 1, i, j, resx, resy, false);
                    myTriangles[ntidx + 1].SetVertices(triangles[tidx + 3], triangles[tidx + 4], triangles[tidx + 5]);
                }
            }
        }

        shoreLine.transform.position = Vector3.zero;
        shoreLine.transform.localScale = Vector3.one;
        shoreLine.transform.rotation = Quaternion.identity;

        shoreLine.mesh.Clear();
        shoreLine.mesh.vertices = vertices;
        shoreLine.mesh.triangles = triangles;
        shoreLine.mesh.RecalculateNormals();
    }

    ///// <summary>
    ///// not in use
    ///// </summary>
    //private void convertTerrainToMesh()
    //{
    //    Vector3 origin = terrain.transform.position;
    //    int res = meshResolution;  //should not exceed 255, because maximum number of vertices is 64000
    //    Vector3 size = terrain.terrainData.size / (float)(res - 1);

    //    vertices = new Vector3[res * res];
    //    triangles = new int[6 * (res - 1) * (res - 1)];
    //    myTriangles = new Triangle[triangles.Length / 3];

    //    for (int i = 0; i < res; i++)
    //    {
    //        for (int j = 0; j < res; j++)
    //        {
    //            vertices[i * res + j].x = origin.x + i * size.x;
    //            vertices[i * res + j].y = terrain.terrainData.GetInterpolatedHeight((i + 0.4F) / (float)res, (j + 0.7F) / (float)res);
    //            vertices[i * res + j].z = origin.z + j * size.z;

    //            //TRIANGLES
    //            if (i < res - 1 && j < res - 1)
    //            {
    //                int tidx = 6 * (i * (res - 1) + j);
    //                triangles[tidx + 0] = (i * res + j + 1);// % (res * res);
    //                triangles[tidx + 1] = ((i + 1) * res + j);// % (res * res);
    //                triangles[tidx + 2] = (i * res + j);// % (res * res);

    //                triangles[tidx + 3] = (i * res + j + 1);// % (res * res);
    //                triangles[tidx + 4] = ((i + 1) * res + j + 1);// % (res * res);
    //                triangles[tidx + 5] = ((i + 1) * res + j);// % (res * res);

    //                int ntidx = 2 * (i * (res - 1) + j);
    //                myTriangles[ntidx + 0] = new Triangle(ntidx, i, j, res, res, true);
    //                myTriangles[ntidx + 0].SetVertices(triangles[tidx + 0], triangles[tidx + 1], triangles[tidx + 2]);

    //                myTriangles[ntidx + 1] = new Triangle(ntidx + 1, i, j, res, res, false);
    //                myTriangles[ntidx + 1].SetVertices(triangles[tidx + 3], triangles[tidx + 4], triangles[tidx + 5]);
    //            }
    //        }
    //    }

    //    shoreLine.mesh.Clear();
    //    shoreLine.mesh.vertices = vertices;
    //    shoreLine.mesh.triangles = triangles;
    //    shoreLine.mesh.RecalculateNormals();
    //}

    /// <summary>
    /// 
    /// </summary>
    void extractShoreTriangles()
    {
        //create an array of bool values for every vertex
        bool[] vertexUnderShoreLine = new bool[vertices.Length];

        //and determine if the vertex is above or under the shoreline
        for (int i = 0; i < vertices.Length; i++)
            vertexUnderShoreLine[i] = vertices[i].y < height;

        //we define a list that will contain all triangles that intersect with the shoreline
        List<Triangle> shoreTriangles = new List<Triangle>();

        //a triangle intersects with the shoreline, if 1 vertex is not in the same linear division as the other 2 (above or under the shoreline)
        foreach (Triangle t in myTriangles)
        {
            t.shore = (vertexUnderShoreLine[t.v1] ^ vertexUnderShoreLine[t.v2]) || (vertexUnderShoreLine[t.v1] ^ vertexUnderShoreLine[t.v3]);
            if (t.shore) shoreTriangles.Add(t);
        }

        //now we have to re-position the vertices of our shore-triangle, 
        //such that they are following the correct intersection line between plane and terrain (inner borderline)
        foreach (Triangle t in shoreTriangles)
            calculateShoreVertices(t);

        //based on the number of neighbours we reposition the the vertices to create a smooth transition (outer borderline)
        foreach (Triangle t in shoreTriangles)
            averageDirectionToSeaOverNeighbourhood(t, neighbourhood);

        //we are ready to create a real mesh from our helper structure
        List<Vector2> newUVcoordinates = new List<Vector2>(shoreTriangles.Count * 2);
        List<Vector3> newVertices = new List<Vector3>(shoreTriangles.Count * 2);
        List<int> newTriangles = new List<int>();

        // CREATE VERTICES
        for (int i = 0; i < shoreTriangles.Count; i++)
        {
            Triangle t = shoreTriangles[i];
            newVertices.Add(t.shoreVertex1);
            newVertices.Add(t.shoreVertex2);

            newUVcoordinates.Add(new Vector2(0, 0));
            newUVcoordinates.Add(new Vector2(0, 1));

            t.shoreVertexIdx1 = i * 2;
            t.shoreVertexIdx2 = (i * 2) + 1;
        }

        int tricount = 0;

        // DO TESSELATION
        for (int i = 0; i < shoreTriangles.Count; i++)
        {
            Triangle t = shoreTriangles[i];

            int idx;
            if (t.GetLeftShoreNeighbour(myTriangles, out idx))
            {
                Triangle n = myTriangles[idx];

                float area = areaTriangle(n.shoreVertex2, t.shoreVertex1, t.shoreVertex2);
                if (area > threshold)
                {
                    newTriangles.Add(n.shoreVertexIdx2);
                    newTriangles.Add(t.shoreVertexIdx1);
                    newTriangles.Add(t.shoreVertexIdx2);
                    t.lowerTri = tricount; tricount += 3;

                    //we don't consider triangles that are too small
                    area = areaTriangle(n.shoreVertex2, n.shoreVertex1, t.shoreVertex1);
                    if (area > threshold)
                    {
                        newTriangles.Add(n.shoreVertexIdx2);
                        newTriangles.Add(n.shoreVertexIdx1);
                        newTriangles.Add(t.shoreVertexIdx1);
                        t.upperTri = tricount; tricount += 3;
                    }
                    //if the area is not big enough we drop a vertex and merge with a neighbour triangle
                    else
                    {
                        //SHIFT VERTEX (lower case)
                        newVertices[n.shoreVertexIdx1] = newVertices[t.shoreVertexIdx1];
                    }
                }
                else
                {
                    //SHIFT VERTEX (upper case)
                    newVertices[n.shoreVertexIdx2] = newVertices[t.shoreVertexIdx2];

                    area = areaTriangle(n.shoreVertex2, n.shoreVertex1, t.shoreVertex1);
                    if (area > threshold)
                    {
                        newTriangles.Add(n.shoreVertexIdx2);
                        newTriangles.Add(n.shoreVertexIdx1);
                        newTriangles.Add(t.shoreVertexIdx1);
                        t.upperTri = tricount; tricount += 3;
                    }
                }
            }
        }

        //refineBorder(ref newUVcoordinates, ref newVertices, ref newTriangles, ref shoreTriangles);

        //CREATE UV COORDINATES FOR EVERY ISLAND
        produceUVMapping(ref newUVcoordinates, ref newVertices, ref newTriangles, ref shoreTriangles);

        shoreLine.mesh.Clear();
        shoreLine.mesh.vertices = newVertices.ToArray();
        shoreLine.mesh.triangles = newTriangles.ToArray();
        shoreLine.mesh.uv = newUVcoordinates.ToArray();
        shoreLine.mesh.RecalculateNormals();
    }

    /// <summary>
    /// Do the UV mapping for each shore loop/line separately
    /// TODO:  preprocessore step to correctly separate loops and lines
    /// </summary>
    /// <param name="uv"></param>
    /// <param name="vert"></param>
    /// <param name="tri"></param>
    /// <param name="shoreTriangles"></param>
    void produceUVMapping(ref List<Vector2> uv, ref List<Vector3> vert, ref List<int> tri, ref List<Triangle> shoreTriangles)
    {
        const int MAXISLANDS = 100;
        int numSubIslands = 0;

        List<Triangle> starts = new List<Triangle>();
        List<Triangle> ends = new List<Triangle>();
        List<float> closure = new List<float>();
        List<int> elements = new List<int>();

        while (numSubIslands < MAXISLANDS && shoreTriangles.Count > 0)
        {
            numSubIslands++;
            int count = 1;

            Triangle t = shoreTriangles[0];
            shoreTriangles.Remove(t);

            Triangle start = t;
            Triangle end = t;

            int idx;

            Vector3 center = Vector3.zero;
            while (count < 1000 && t.GetLeftShoreNeighbour(myTriangles, out idx))
            {
                end = myTriangles[idx];
                shoreTriangles.Remove(end);

                center += t.shoreVertex2;
                if (end == start) break;
                else t = end;
                count++;
            }
            if (count == 1000)
            {
                Debug.LogError("Error in shore genereation 1");
                break;
            }

            //LOOP
            if (end == start && t != start)
            {
                //shift the first 2 vertices of the end to the position of the begining of the loop
                //to prevent texture squeeze on the x-axis from n to 0;
                vert[tri[t.lowerTri + 1]] = vert[tri[t.upperTri + 1]];
                vert[tri[t.lowerTri + 2]] = vert[tri[t.upperTri + 0]];

                center /= count;
                closure.Add(Vector3.Dot((center - end.shoreVertex1).normalized, end.directionToSea.normalized));
                starts.Add(start);
                ends.Add(t);
                elements.Add(-1);

            }
            //LINE --> search cw
            else
            {
                start = t;
                //t = start;
                while (count < 1000 && t.GetRightShoreNeighbour(myTriangles, out idx))
                {
                    end = myTriangles[idx];
                    shoreTriangles.Remove(end);
                    t = end;
                    count++;
                }
                if (count == 1000) Debug.LogError("Error in shore genereation 2");

                Debug.Log("LINE");
                closure.Add(0);
                starts.Add(end);
                ends.Add(start);
                elements.Add(count);
            }
        }

        for (int i = 0; i < starts.Count; i++)
        {
            if (closure[i] > 0)
            {
                int index;
                Triangle t = starts[i];
                List<int> removeTri = new List<int>();
                while (t != ends[i] && t.GetLeftShoreNeighbour(myTriangles, out index))
                {
                    removeTri.Add(t.upperTri);
                    removeTri.Add(t.upperTri + 1);
                    removeTri.Add(t.upperTri + 2);
                    removeTri.Add(t.lowerTri);
                    removeTri.Add(t.lowerTri + 1);
                    removeTri.Add(t.lowerTri + 2);

                    t = myTriangles[index];
                }
                removeTri.Sort(); removeTri.Reverse();

                foreach (int idx in removeTri)
                    tri[idx] = 0;
            }
            else
            {
                float total = elements[i];
                int count = 0;
                bool fade = (total > 0);

                uv[starts[i].shoreVertexIdx1] = new Vector2(0, 0);
                uv[starts[i].shoreVertexIdx2] = new Vector2(0, 1);

                float factor = (fade) ? Mathf.Abs((count / total) - 0.5F) : 0;
                vert[starts[i].shoreVertexIdx2] -= starts[i].directionToSea * factor * 2 * scaling;

                int index;
                Triangle t = starts[i];
                while (count < 1000 && t != ends[i] && t.GetLeftShoreNeighbour(myTriangles, out index))
                {
                    count++;

                    Triangle n = myTriangles[index];
                    float d = 0.5F * (Vector3.Distance(t.shoreVertex1, n.shoreVertex1) + Vector3.Distance(t.shoreVertex2, n.shoreVertex2));
                    Vector2 tex = new Vector2(d / (4F * scaling), 0);

                    uv[n.shoreVertexIdx1] = uv[t.shoreVertexIdx1] + tex;
                    uv[n.shoreVertexIdx2] = uv[t.shoreVertexIdx2] + tex;

                    factor = (fade) ? Mathf.Abs((count / total) - 0.5F) : 0;
                    vert[n.shoreVertexIdx2] -= n.directionToSea * factor * 2 * scaling;
                    t = n;
                }
                if (count == 1000) Debug.LogError("Error in shore genereation");
            }
        }
    }

    //void refineBorder(ref List<Vector2> uv, ref List<Vector3> vert, ref List<int> tri, ref List<Triangle> shoreTriangles)
    //{
    //    RaycastHit hit;
    //    int layer = 1 << 30; //terrain
    //    foreach (Triangle t in shoreTriangles)
    //    {
    //        //do a raycast for every shore border point
    //        Vector3 shift = t.directionToSea * scaling / 3F;
    //        Vector3 p = vert[t.shoreVertexIdx1] - shift;
    //        if (Physics.Raycast(new Ray(p + new Vector3(0, 50, 0), Vector3.down), out hit, 100, layer))
    //        {
    //            vert[t.shoreVertexIdx1] = hit.point - Vector3.up / 10F; ;
    //        }
    //        else
    //        {
    //            vert[t.shoreVertexIdx1] += shift + Vector3.up / 3F;
    //        }

    //        vert[t.shoreVertexIdx2] += shift - Vector3.up * 0.1F;
    //    }
    //}

    //void adjustShoreVertices(ref List<Vector3> vert, ref List<Triangle> shoreTriangles)
    //{
    //    foreach (Triangle t in shoreTriangles)
    //    {
    //        int idx;
    //        if (t.getLeftShoreNeighbour(neighbourTriangles, out idx))
    //        {
    //            Triangle n = neighbourTriangles[idx];

    //            Vector3 d1 = (t.shoreVertex1 - n.shoreVertex1).normalized;
    //            Vector3 d2 = (t.shoreVertex2 - n.shoreVertex2).normalized;
    //            if (Vector3.Dot(d1, d2) < 0)
    //            {
    //                //int temp = t.shoreVertexIdx2;
    //                //t.shoreVertexIdx2 = n.shoreVertexIdx2;
    //                //n.shoreVertexIdx2 = temp;

    //                vert[t.shoreVertexIdx2] = n.shoreVertex2;
    //                vert[n.shoreVertexIdx2] = t.shoreVertex2;

    //                //Vector3 tempV = t.shoreVertex2;
    //                //t.shoreVertex2 = n.shoreVertex2;
    //                //n.shoreVertex2 = tempV;

    //                GameObject ob = GameObject.CreatePrimitive(PrimitiveType.Cube);
    //                ob.transform.position = (t.shoreVertex2 + n.shoreVertex2) / 2F;
    //                ob.transform.localScale = Vector3.one * 0.3F;
    //            }
    //        }
    //    }
    //}

    float areaTriangle(Vector3 A, Vector3 B, Vector3 C)
    {
        float a = Mathf.Pow(Vector3.Distance(A, B), 2);
        float b = Mathf.Pow(Vector3.Distance(B, C), 2);
        float c = Mathf.Pow(Vector3.Distance(C, A), 2);
        return 0.25F * Mathf.Sqrt(2 * (a * b + a * c + b * c) - (a * a + b * b + c * c));
    }

    /// <summary>
    /// We have a triangle that is part of the intersection between land and sea
    /// We define vertex 'a' to be alone on one side, the other two vertices 'b1', 'b2' are together on the other side
    /// To compute the exact borderpoint we scale the vectors (b1-a) and (b2-a) with the factors f1 and f2 respectivly
    /// such that the endpoints of the vectors are lying in the seaplane
    /// To compute the factors f1 and f2 we evaluate (a.y-h)/(b1.y-a.y) where h denotes the absolute height of the seaplane
    /// The borderpoint then is defined as 'a + (f1*(b1-a) + (f2*(b2-a))/2'
    /// 
    /// The direction to the sea is computed by the crossproduct between the (b1-b2) and the up-vector (reverse b2 and b1 when a is under sealevel)
    /// </summary>
    /// <param name="t"></param>
    void calculateShoreVertices(Triangle t)
    {
        Vector3 a = vertices[t.v1];
        Vector3 b1 = vertices[t.v2];
        Vector3 b2 = vertices[t.v3];

        if (a.y < height ^ b1.y < height)
        {
            if (b1.y < height ^ b2.y < height)
            {
                Vector3 temp = a;
                a = b1;
                b1 = b2;
                b2 = temp;
            }
        }
        else if (a.y < height ^ b2.y < height)
        {
            if (b1.y < height ^ b2.y < height)
            {
                Vector3 temp = a;
                a = b2;
                b2 = b1;
                b1 = temp;
            }
        }

        float factor1 = Mathf.Abs((a.y - height) / (b1.y - a.y));
        Vector3 p1 = factor1 * (b1 - a);

        float factor2 = Mathf.Abs((a.y - height) / (b2.y - a.y));
        Vector3 p2 = factor2 * (b2 - a);

        Vector3 borderVertex = (a + (p1 + p2) / 2F);

        Vector3 tangent = (p2 - p1).normalized;
        if (a.y < height)//b1.y)
        {
            tangent = (p1 - p2).normalized;
            t.shoreVertex1 = (b1 + b2) / 2F;
        }
        else
        {
            t.shoreVertex1 = a;
        }

        t.directionToSea = Vector3.Cross(tangent, Vector3.up).normalized;

        // [0,1] [flat,steep]
        float slope = Mathf.Clamp01(Mathf.Abs((b1 - a).normalized.y) + Mathf.Abs((b2 - a).normalized.y));
        //if very flat, take a point more on the landside
        //if steep instead, take the exact borderpoint
        t.shoreVertex1 = (1 - slope) * t.shoreVertex1 + slope * borderVertex;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="t"></param>
    /// <param name="degree"></param>
    private void averageDirectionToSeaOverNeighbourhood(Triangle t, int degree)
    {
        Vector3 dir = t.directionToSea;
        int num = 1;
        List<int> usedNeighbour = new List<int>();

        if (degree > 0)
        {
            averageDirectionToSea(t, degree - 1, ref num, ref dir, ref usedNeighbour);
        }

        t.directionToSea = dir / (float)num;

        t.shoreVertex2 = t.shoreVertex1 + t.directionToSea * scaling;
        t.shoreVertex2.y = height;
    }

    private void averageDirectionToSea(Triangle t, int degree, ref int num, ref Vector3 avgDir, ref List<int> used)
    {
        foreach (int n in t.neighbours)
        {
            try
            {
                if (myTriangles[n].shore && !used.Contains(n))
                {
                    used.Add(n);
                    avgDir += myTriangles[n].directionToSea;
                    num++;

                    if (degree > 0)
                    {
                        averageDirectionToSea(myTriangles[n], degree - 1, ref num, ref avgDir, ref used);
                    }
                }
            }
            catch
            {
                //Debug.Log(n + " too high " + neighbourTriangles.Length);
            }
        }
    }

    //private void correctOverlappingShoreVertices(Triangle t)
    //{
    //    Triangle l = neighbourTriangles[t.lowerTri];
    //    Triangle u = neighbourTriangles[t.upperTri];
    //}

    /// <summary>
    /// 
    ///  1____________
    ///   |\         |
    ///   |   \      |
    ///   |      \   |
    ///   |_________\|
    ///  2          0
    /// 
    /// 
    /// </summary>
    private class Triangle
    {
        public int idx;
        public bool shore = false;

        //public bool usedInShore = false;
        public Vector3 shoreVertex1;
        public Vector3 shoreVertex2;

        public int shoreVertexIdx1;
        public int shoreVertexIdx2;

        public List<int> neighbours = new List<int>();

        public Triangle(int idx, int i, int j, int resx, int resy, bool lower)
        {
            this.idx = idx;

            //add left
            if (!lower || (lower && j > 0))
                neighbours.Add(idx - 1);

            //add right
            if (lower || (!lower && j < resy - 2))
                neighbours.Add(idx + 1);

            //add upper/lower
            if (lower && i > 0)
            {
                neighbours.Add(idx + 1 - (2 * (resy - 1)));
            }
            else if (!lower && i < resx - 2)
            {
                neighbours.Add(idx - 1 + (2 * (resy - 1)));
            }
        }

        public int GetNeighbourhoodShore(Triangle[] list)
        {
            int num = 0;
            foreach (int i in neighbours)
                if (list[i].shore) num++;
            return num;
        }

        private int rightIdx = -1;
        public bool GetRightShoreNeighbour(Triangle[] list, out int idx)
        {
            if (rightIdx >= 0) { idx = rightIdx; return true; }

            idx = -1;

            Vector3 right = Vector3.Cross(directionToSea, Vector3.up);
            bool found = false;
            float dot = 0;
            foreach (int i in neighbours)
            {
                if (list[i].shore)
                {
                    Vector3 v = (list[i].shoreVertex1 - shoreVertex1).normalized;
                    if (Vector3.Dot(v, right) > dot)
                    {
                        found = true;
                        idx = rightIdx = i;
                        dot = Vector3.Dot(v, right);
                    }
                }
            }
            return found;
        }

        private int leftIdx = -1;
        public bool GetLeftShoreNeighbour(Triangle[] list, out int idx)
        {
            if (leftIdx >= 0) { idx = leftIdx; return true; }

            idx = -1;

            Vector3 left = Vector3.Cross(directionToSea, Vector3.up);
            bool found = false;
            float dot = 0;
            foreach (int i in neighbours)
            {
                if (list[i].shore)
                {
                    Vector3 v = (list[i].shoreVertex1 - shoreVertex1).normalized;
                    if (Vector3.Dot(v, left) < dot)
                    {
                        found = true;
                        idx = leftIdx = i;
                        dot = Vector3.Dot(v, left);
                    }
                }
            }
            return found;
        }

        public int GetLeftNeighbour(Triangle[] list)
        {
            Vector3 left = Vector3.Cross(directionToSea, Vector3.up);

            int bestNeighbour = neighbours[0];
            float dot = 1;
            foreach (int i in neighbours)
            {
                if (list[i].shore)
                {
                    Vector3 v = (list[i].shoreVertex1 - shoreVertex1).normalized;
                    if (Vector3.Dot(v, left) < dot)
                    {
                        bestNeighbour = i;
                        dot = Vector3.Dot(v, left);
                    }
                }
            }
            return bestNeighbour;
        }

        public void SetVertices(int v1, int v2, int v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }

        public int v1;
        public int v2;
        public int v3;

        public int upperTri;
        public int lowerTri;

        public Vector3 directionToSea;
    }
}