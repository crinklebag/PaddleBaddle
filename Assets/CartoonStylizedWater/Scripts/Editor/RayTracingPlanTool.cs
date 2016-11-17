using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class RayTracingPlanTool : EditorWindow{

    public Material waterMaterial;
    [MenuItem("CartoonStylizedWater/Tool")]
    public static void ShowWindow()
    {
        var window=GetWindow<RayTracingPlanTool>();
        window.title="RayTracingPlanTool";
    }

    public void OnSeceneGUI(SceneView sceneView)
    {
        
    }
    void CreateBezierObject()
    {
        if( GameObject.Find("MidPointSplineLine") )
        {
            
        }
        else
        {
            GameObject bezzierObj =  new GameObject("MidPointSplineLine");
            bezzierObj.AddComponent<BezierSpline>();
        }
    }
    void BuildMesh()
    {
        if(GameObject.Find("WaterPlaneRayTracingTool") != null)
        {
            GameObject.DestroyImmediate(GameObject.Find("WaterPlaneRayTracingTool"));
        }
        GameObject parent = new GameObject("WaterPlaneRayTracingTool");

        parent.AddComponent<MeshFilter>();
        parent.AddComponent<MeshRenderer>();

        Mesh m = new Mesh();
        m.name = "ScriptedMesh";

        int distanceOfObject=20;

        List<int> trianglesList = new List<int>();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();

        for(int i = 0 ; i < distanceOfObject; i++)
        {
            vertices.Add(new Vector3(i*0.5f,0,-1));
            vertices.Add(new Vector3(i*0.5f,0,1));

            float xValue =   vertices[vertices.Count-1].x / vertices[0].x;
            float yValue =   vertices[vertices.Count-1].z / vertices[0].z;
            uv.Add(new Vector2(xValue,yValue));
            normals.Add(Vector3.up);

            float xValue2 =   vertices[vertices.Count-2].x / vertices[0].x;
            float yValue2 =   vertices[vertices.Count-2].z / vertices[0].z;
            uv.Add(new Vector2(xValue2,yValue2));
            normals.Add(Vector3.up);

        }

        for(int i = 0 ; i < vertices.Count-3; i++)
        {
            trianglesList.Add(i);
            trianglesList.Add(i+1);
            trianglesList.Add(i+2);
            trianglesList.Add(i+2);
            trianglesList.Add(i+1);
            trianglesList.Add(i);
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.parent = parent.transform;
            go.transform.position = vertices[i];
            go.transform.localScale /=2;
        }

        m.vertices=vertices.ToArray();
        m.uv=uv.ToArray();
        m.triangles=trianglesList.ToArray();
        m.normals = normals.ToArray();
        m.RecalculateNormals();

        parent.GetComponent<MeshFilter>().mesh = m;

    }
    Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, Quaternion angle)
    {
        return angle * ( point - pivot) + pivot;
    }
    void BuildPointsFromLine()
    {
        if( GameObject.Find("GeneratedVertices") )
        {
            GameObject.DestroyImmediate(GameObject.Find("GeneratedVertices"));
        }
        GameObject parent = new GameObject("GeneratedVertices");

        if( GameObject.Find("GeneratedForwardDirection") )
        {
            GameObject.DestroyImmediate(GameObject.Find("GeneratedForwardDirection"));
        }
        GameObject parentForward = new GameObject("GeneratedForwardDirection");

        GameObject parentMidLine = GameObject.Find("MidLine");
        Vector3 waterDirection = (parentMidLine.transform.GetChild(1).position - parentMidLine.transform.GetChild(0).position).normalized;
        for(int i = 0 ; i < parentMidLine.transform.childCount; i++)
        {
            
            if(i != 0 && i < parentMidLine.transform.childCount-1)
                waterDirection = (parentMidLine.transform.GetChild(i+1).position - parentMidLine.transform.GetChild(i).position).normalized;

            Vector3 midPosition = parentMidLine.transform.GetChild(i).transform.position;

            GameObject go3 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go3.transform.parent = parentForward.transform;
            go3.transform.position = midPosition+waterDirection/2;
            go3.transform.localScale /=5;

            Vector3 leftSideVertice = RotateAroundPoint(midPosition+ waterDirection, midPosition, Quaternion.Euler(0, 90, 0) );
            Vector3 rightSideVertice = RotateAroundPoint(midPosition+waterDirection, midPosition, Quaternion.Euler(0, -90, 0) );

            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.parent = parent.transform;
            go.transform.position = leftSideVertice;
            go.transform.localScale /=2;

            GameObject go2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go2.transform.parent = parent.transform;
            go2.transform.position = rightSideVertice;
            go2.transform.localScale /=2;

        }
    }
    void BuildPointsFromBezierLine()
    {
        if( GameObject.Find("GeneratedVertices") )
        {
            GameObject.DestroyImmediate(GameObject.Find("GeneratedVertices"));
        }
        GameObject parent = new GameObject("GeneratedVertices");

        if( GameObject.Find("GeneratedForwardDirection") )
        {
            GameObject.DestroyImmediate(GameObject.Find("GeneratedForwardDirection"));
        }
        GameObject parentForward = new GameObject("GeneratedForwardDirection");


        GameObject parentExtras = GameObject.Find("GeneratedExtras");
        if(parentExtras != null)
            DestroyImmediate(parentExtras);


        BezierSpline MidLineBeizer = GameObject.Find("MidPointSplineLine").GetComponent<BezierSpline>();
        GameObject goLastFrame = null;
        Vector3 waterDirection = ( MidLineBeizer.GetPoint(0.01f)-MidLineBeizer.GetPoint(0f)).normalized;
        for(int i = 0 ; i < 100; i++)
        {

            if(i != 0 && i < 99)
                waterDirection = (MidLineBeizer.GetPoint((float)i/100f + 0.05f) - MidLineBeizer.GetPoint((float)i/100f)).normalized;

            Vector3 midPosition = MidLineBeizer.GetPoint((float)i/100f);

            GameObject go3 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go3.transform.parent = parentForward.transform;
            go3.transform.position = midPosition+waterDirection/2;
            go3.transform.localScale /=5;
            if(go3.GetComponent<SphereCollider>() != null)
                DestroyImmediate(go3.GetComponent<SphereCollider>());

            go3.transform.forward = waterDirection;

            Vector3 workingPos = go3.transform.position;
            workingPos.y = 0;
            //go3.transform.position = workingPos;
//            Vector3 leftSideVertice = RotateAroundPoint(midPosition + waterDirection.normalized, midPosition, Quaternion.Euler(0, 90, 0) );
//            Vector3 rightSideVertice = RotateAroundPoint(midPosition + waterDirection.normalized, midPosition, Quaternion.Euler(0, -90, 0) );
          //  Vector3 leftSideVertice = RotateAroundPoint(midPosition + waterDirection, midPosition, Quaternion.Euler(go3.transform.up.x*90, go3.transform.up.y*90, go3.transform.up.z*90) );
           // Vector3 rightSideVertice = RotateAroundPoint(midPosition + waterDirection, midPosition, Quaternion.Euler(go3.transform.up.x*-90, go3.transform.up.y*-90, go3.transform.up.z*-90) );

            Vector3 leftSideVertice = RotateAroundPoint(workingPos + waterDirection, workingPos, Quaternion.Euler(0, 90, 0) );
            Vector3 rightSideVertice = RotateAroundPoint(workingPos + waterDirection, workingPos, Quaternion.Euler(0, -90, 0) );


            leftSideVertice.y = go3.transform.position.y;
            rightSideVertice.y = go3.transform.position.y;

            leftSideVertice = go3.transform.position + (leftSideVertice - go3.transform.position).normalized;
            rightSideVertice = go3.transform.position + (rightSideVertice - go3.transform.position).normalized;




            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.parent = parent.transform;
            go.transform.position = leftSideVertice;
            go.transform.localScale /=2;
            if(go.GetComponent<SphereCollider>() != null)
                DestroyImmediate(go.GetComponent<SphereCollider>());
            go.AddComponent<VertexInfo>();
            go.GetComponent<VertexInfo>().ConnectedMidPoint = goLastFrame;
   


            GameObject go2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go2.transform.parent = parent.transform;
            go2.transform.position = rightSideVertice;
            go2.transform.localScale /=2;
            if(go2.GetComponent<SphereCollider>() != null)
                DestroyImmediate(go2.GetComponent<SphereCollider>());
            go2.AddComponent<VertexInfo>();
            go2.GetComponent<VertexInfo>().ConnectedMidPoint = goLastFrame;

            go.GetComponent<VertexInfo>().OtherSidePoint = go2;
            go2.GetComponent<VertexInfo>().OtherSidePoint = go;

            goLastFrame = go3;
        } 
    }

    void BuildMeshFromPoints()
    {
       
        if(GameObject.Find("GeneratedMeshFromPoints") != null)
        {
            GameObject.DestroyImmediate(GameObject.Find("GeneratedMeshFromPoints"));
        }
        GameObject parent = new GameObject("GeneratedMeshFromPoints");

        GameObject verticesParent = GameObject.Find("GeneratedVertices");

        parent.AddComponent<MeshFilter>();
        parent.AddComponent<MeshRenderer>();

        Mesh m = new Mesh();
        m.name = "ScriptedMesh";

        List<int> trianglesList = new List<int>();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();

        float totalDistance = 0;
        int side = 0;

        for(int i = 2 ; i < verticesParent.transform.childCount; i++)
        {
            if(side==0)
            {
                
            }
            if(side==1)
            {
                
            }
            totalDistance += Vector3.Distance( verticesParent.transform.GetChild(i).position , verticesParent.transform.GetChild(i-2).position);
            side++;
            if(side==2)
                side=0;
            

        }

        side = 0;
        float currentDist = 0;
        for(int i = 0 ; i < verticesParent.transform.childCount; i++)
        {
            vertices.Add(verticesParent.transform.GetChild(i).position);
            float xValue = 0;
            float yValue = 0;

            float amountX = verticesParent.transform.childCount;
            
            if(side==0)
            {
                xValue=0; 
            }
            if(side==1)
            {
                xValue=1;
            }

            if(i>1)
                currentDist += Vector3.Distance( verticesParent.transform.GetChild(i).position , verticesParent.transform.GetChild(i-2).position);
            
            yValue= currentDist / totalDistance;
            //yValue= (float)i / amountX;
            side++;
            if(side==2)
                side=0;
            uv.Add(new Vector2(xValue,yValue));

            normals.Add(Vector3.up);
        }

        for(int i = 0 ; i < vertices.Count-2; i++)
        {
            trianglesList.Add(i);
            trianglesList.Add(i+1);
            trianglesList.Add(i+2);
            trianglesList.Add(i+2);
            trianglesList.Add(i+1);
            trianglesList.Add(i);
        }

        m.vertices=vertices.ToArray();
        m.uv=uv.ToArray();
        m.triangles=trianglesList.ToArray();
        m.normals = normals.ToArray();
        m.RecalculateNormals();


        parent.GetComponent<MeshRenderer>().material = Instantiate((Material)AssetDatabase.LoadAssetAtPath("Assets/CartoonStylizedWater/Materials/WaterDynamicCreation.mat",typeof(Material)));

        WaterScroll sc = parent.AddComponent<WaterScroll>();
        sc.scrollSpeed = 0;
        sc.scrollSpeedY = -2.0f;

        parent.GetComponent<MeshFilter>().mesh = m;
    }
    Vector3 GetClosestMidLinePoint(GameObject aObj)
    {
        if(aObj.GetComponent<VertexInfo>().ConnectedMidPoint != null)
            return aObj.GetComponent<VertexInfo>().ConnectedMidPoint.transform.position;
        
        GameObject parentMidLine = GameObject.Find("GeneratedForwardDirection");

        int closestIndex=0;
        float closestDistance = 9999;
        for(int i = 0 ; i < parentMidLine.transform.childCount; i++)
        {
            GameObject currentPoint = parentMidLine.transform.GetChild(i).gameObject;
            float dist = Vector3.Distance(currentPoint.transform.position,aObj.transform.position);
            if(dist < closestDistance)
            {
                closestIndex = i;
                closestDistance = dist;
            }
        }
        return parentMidLine.transform.GetChild(closestIndex).transform.position;
    }
    Vector3[] GetTwoClosestMidLinePoint(GameObject aObj)
    {
//        if(aObj.GetComponent<VertexInfo>().ConnectedMidPoint != null)
//            return aObj.GetComponent<VertexInfo>().ConnectedMidPoint.transform.position;

        GameObject parentMidLine = GameObject.Find("GeneratedForwardDirection");

        int closestIndex=0;
        float closestDistance = 9999;
        int closestIndex2=0;
        float closestDistance2 = 9999;
        for(int i = 0 ; i < parentMidLine.transform.childCount; i++)
        {
            GameObject currentPoint = parentMidLine.transform.GetChild(i).gameObject;
            float dist = Vector3.Distance(currentPoint.transform.position,aObj.transform.position);
            if(dist < closestDistance)
            {
                closestIndex = i;
                closestDistance = dist;
            }
        }
        for(int i = 0 ; i < parentMidLine.transform.childCount; i++)
        {
            GameObject currentPoint = parentMidLine.transform.GetChild(i).gameObject;
            float dist = Vector3.Distance(currentPoint.transform.position,aObj.transform.position);
            if(dist < closestDistance2 && i != closestIndex)
            {
                closestIndex2 = i;
                closestDistance2 = dist;
            }
        }
        Vector3[] points = new Vector3[2]; 
        points[0]=parentMidLine.transform.GetChild(closestIndex).transform.position;
        points[1]=parentMidLine.transform.GetChild(closestIndex2).transform.position;

        return points;
    }

    void MoveVerticesToSide()
    {
        GameObject parentVertices = GameObject.Find("GeneratedVertices");
        GameObject parentExtras = GameObject.Find("GeneratedExtras");
       
        if(parentExtras != null)
            DestroyImmediate(parentExtras);
        
        parentExtras =  new GameObject("GeneratedExtras");

        for(int i = 0 ; i < parentVertices.transform.childCount; i++)
        {
            GameObject currentVertice = parentVertices.transform.GetChild(i).gameObject;
            Vector3 closestMidPoint = GetClosestMidLinePoint(currentVertice);
            Vector3[] twoClosestMidPoint = GetTwoClosestMidLinePoint(currentVertice);


            Vector3 calculatedMidPoint = ClosestPointOnLine(twoClosestMidPoint[0],twoClosestMidPoint[1],currentVertice.transform.position);

            closestMidPoint = calculatedMidPoint;
            closestMidPoint.y=0;
            Vector3 pos = currentVertice.transform.position;
            pos.y=0;
            Vector3 dirToRaytrace = (pos - closestMidPoint).normalized;

            dirToRaytrace = (currentVertice.transform.position - currentVertice.GetComponent<VertexInfo>().OtherSidePoint.transform.position);

            //dirToRaytrace = 
            Ray ray = new Ray(currentVertice.transform.position,dirToRaytrace);
            RaycastHit hit;
            if( Physics.Raycast(ray,out hit) )
            {
                GameObject obj =  (GameObject)GameObject.CreatePrimitive(PrimitiveType.Cube);
                obj.name = "WaterToEdgePlane";
                obj.transform.parent = parentExtras.transform;
                obj.transform.position = parentVertices.transform.GetChild(i).position;
                Vector3 vecToEdge = hit.point - parentVertices.transform.GetChild(i).position;
                obj.transform.forward = vecToEdge.normalized;
                obj.transform.localScale = new Vector3(0.01f,obj.transform.localScale.y,vecToEdge.magnitude);
                obj.transform.position = obj.transform.position + vecToEdge/2;

                // If hitting the plane we use the point from the last hit
                if(hit.collider.name == "WaterToEdgePlane")
                {
                    parentVertices.transform.GetChild(i).position = parentVertices.transform.GetChild(i-2).position;
                }
                else
                {
                    parentVertices.transform.GetChild(i).position = hit.point;
                }
            }

        }

        for(int i = 0 ; i < parentVertices.transform.childCount; i++)
        {
            for(int j = parentVertices.transform.childCount-1 ; j >= 0; j--)
            {
                if(i!=j)
                if( Vector3.Distance( parentVertices.transform.GetChild(i).position, parentVertices.transform.GetChild(j).position) < 0.1f)
                {
                    //DestroyImmediate(parentVertices.transform.GetChild(j).gameObject);
                    //DestroyImmediate(parentVertices.transform.GetChild(j-1).gameObject);
                }
            }
        }

    }
    Vector3 ClosestPointOnLine(Vector3 vA,Vector3 vB,Vector3 vPoint)
    {
        Vector3 vVector1 = vPoint - vA;
        Vector3 vVector2 = (vB - vA).normalized;

        float d = Vector3.Distance(vA, vB);
        float t = Vector3.Dot(vVector2, vVector1);

        if (t <= 0)
            return vA;

        if (t >= d)
            return vB;

        Vector3 vVector3 = vVector2 * t;

        Vector3 vClosestPoint = vA + vVector3;

        return vClosestPoint;
    }
    void BuildMeshVerticalIndividual()
    {
        
        GameObject parentVertices = GameObject.Find("GeneratedVertices");
        List<GameObject> waterVerticesLeft = new List<GameObject>();
        List<GameObject> waterVerticesRight = new List<GameObject>();
        List<GameObject> waterVerticesAll = new List<GameObject>();
        for(int i = 0 ; i < parentVertices.transform.childCount;i+=4)
        {
            waterVerticesRight.Add( parentVertices.transform.GetChild(i).gameObject);
            parentVertices.transform.GetChild(i).gameObject.name = "Right"+waterVerticesRight.Count.ToString();

            waterVerticesRight.Add( parentVertices.transform.GetChild(i+2).gameObject);
            parentVertices.transform.GetChild(i+2).gameObject.name = "Right"+waterVerticesRight.Count.ToString();

            waterVerticesLeft.Add( parentVertices.transform.GetChild(i+1).gameObject);
            parentVertices.transform.GetChild(i+1).gameObject.name = "Left"+waterVerticesLeft.Count.ToString();

            waterVerticesLeft.Add( parentVertices.transform.GetChild(i+3).gameObject);
            parentVertices.transform.GetChild(i+3).gameObject.name = "Left"+waterVerticesLeft.Count.ToString();
        }

        for(int i = waterVerticesLeft.Count-2 ; i >= 0;i--)
        {
            if(Vector3.Distance( waterVerticesLeft[i].transform.position, waterVerticesLeft[i+1].transform.position) < 0.01f)
            {
                waterVerticesLeft.RemoveAt(i+1);
            }
        }
        for(int i = waterVerticesRight.Count-2 ; i >= 0;i--)
        {
            if(Vector3.Distance( waterVerticesRight[i].transform.position, waterVerticesRight[i+1].transform.position) < 0.01f)
            {
                waterVerticesRight.RemoveAt(i+1);
            }
        }
        float distanceLeft = 0;
        float distanceRight = 0;
        for(int i = 0 ; i < waterVerticesLeft.Count;i++)
        {
            waterVerticesAll.Add(waterVerticesLeft[i]);
            if(i < waterVerticesLeft.Count-1)
                distanceLeft += Vector3.Distance(waterVerticesLeft[i].transform.position , waterVerticesLeft[i+1].transform.position);
        }
        for(int i = 0 ; i < waterVerticesRight.Count;i++)
        {
            waterVerticesAll.Add(waterVerticesRight[i]);
            if(i < waterVerticesRight.Count-1)
                distanceRight += Vector3.Distance(waterVerticesRight[i].transform.position , waterVerticesRight[i+1].transform.position);
        }
        
   



        if( GameObject.Find("VerticalEdge") )
        {
            GameObject.DestroyImmediate(GameObject.Find("VerticalEdge"));
        }
        GameObject parent = new GameObject("VerticalEdge");

        for(int i = 0 ; i < waterVerticesAll.Count-1;i++)
        {
            if(i == waterVerticesLeft.Count-1)
              continue;
        
            Vector3 pointOne =waterVerticesAll[i].transform.position;
            Vector3 pointTwo =waterVerticesAll[i+1].transform.position;


            pointOne += new Vector3(0,-0.2f,0);
            pointTwo += new Vector3(0,-0.2f,0);

            GameObject edge = new GameObject("edge");
            edge.transform.parent = parent.transform;
            edge.AddComponent<MeshFilter>();
            edge.AddComponent<MeshRenderer>();

            Mesh m = new Mesh();
            m.name = "EdgeMesh";
            Vector3 [] vertices = new Vector3[4];
            Vector3 [] normals = new Vector3[4];
            Vector2 [] uv = new Vector2[4];
            int [] triangles = new int[12];

            Vector3 closestMidPointOne = GetClosestMidLinePoint(waterVerticesAll[i].gameObject);
            Vector3 closestMidPointTwo = GetClosestMidLinePoint(waterVerticesAll[i+1].gameObject);

            Plane waterPlane = new Plane(pointOne,pointTwo,closestMidPointOne);

            Vector3 dir = (pointOne-pointTwo).normalized;

            Vector3 myVector = Quaternion.AngleAxis(90, dir) * waterPlane.normal;

            Vector3 oneOffset = (closestMidPointOne - pointOne).normalized;
            Vector3 twoOffset = (closestMidPointTwo - pointTwo).normalized;

            pointOne+=oneOffset*0.1f;
            pointTwo+=twoOffset*0.1f;


            vertices[0] = pointOne;
            vertices[1] = pointOne+new Vector3(0,1,0);
            vertices[2] = pointTwo;
            vertices[3] = pointTwo+new Vector3(0,1,0);;

            normals[0] = Vector3.one;
            normals[1] = Vector3.one;
            normals[2] = Vector3.one;
            normals[3] = Vector3.one;

            uv[0] = new Vector2(0,0);
            uv[1] = new Vector2(0,1);
            uv[2] = new Vector2(1,0);
            uv[3] = new Vector2(1,1);

            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;

            triangles[3] = 2;
            triangles[4] = 1;
            triangles[5] = 0;

            triangles[6] = 3;
            triangles[7] = 2;
            triangles[8] = 1;

            triangles[9] = 1;
            triangles[10] = 2;
            triangles[11] = 3;

            m.vertices=vertices;
            m.uv=uv;
            m.triangles=triangles;
            m.normals = normals;
            m.RecalculateNormals();

            edge.GetComponent<MeshFilter>().mesh = m;

            edge.GetComponent<MeshRenderer>().material = Instantiate((Material)AssetDatabase.LoadAssetAtPath("Assets/CartoonStylizedWater/Materials/edgeV.mat",typeof(Material)));
            WaterScroll sc = edge.AddComponent<WaterScroll>();
            sc.scrollSpeed = -1.0f;
            sc.scrollSpeedY = 0;
            sc.scrollSpeed2 = -0.31f;
            sc.scrollSpeedY2 = 0;
            sc.Tiling = true;
            sc.TilingX=1;
            sc.amplitudeY = 3.1f;
            sc.amplitudeY2 = 2.6f;
            sc.max = 0.2f;

        }
    }
    void BuildMeshVerticalOneMesh()
    {

        GameObject parentVertices = GameObject.Find("GeneratedVertices");
        List<GameObject> waterVerticesLeft = new List<GameObject>();
        List<GameObject> waterVerticesRight = new List<GameObject>();
        List<GameObject> waterVerticesAll = new List<GameObject>();
        for(int i = 0 ; i < parentVertices.transform.childCount;i+=4)
        {
            waterVerticesRight.Add( parentVertices.transform.GetChild(i).gameObject);
            parentVertices.transform.GetChild(i).gameObject.name = "Right"+waterVerticesRight.Count.ToString();

            waterVerticesRight.Add( parentVertices.transform.GetChild(i+2).gameObject);
            parentVertices.transform.GetChild(i+2).gameObject.name = "Right"+waterVerticesRight.Count.ToString();

            waterVerticesLeft.Add( parentVertices.transform.GetChild(i+1).gameObject);
            parentVertices.transform.GetChild(i+1).gameObject.name = "Left"+waterVerticesLeft.Count.ToString();

            waterVerticesLeft.Add( parentVertices.transform.GetChild(i+3).gameObject);
            parentVertices.transform.GetChild(i+3).gameObject.name = "Left"+waterVerticesLeft.Count.ToString();
        }

        for(int i = waterVerticesLeft.Count-2 ; i >= 0;i--)
        {
            if(Vector3.Distance( waterVerticesLeft[i].transform.position, waterVerticesLeft[i+1].transform.position) < 0.01f)
            {
                waterVerticesLeft.RemoveAt(i+1);
            }
        }
        for(int i = waterVerticesRight.Count-2 ; i >= 0;i--)
        {
            if(Vector3.Distance( waterVerticesRight[i].transform.position, waterVerticesRight[i+1].transform.position) < 0.01f)
            {
                waterVerticesRight.RemoveAt(i+1);
            }
        }
        float distanceLeft = 0;
        float distanceRight = 0;
        for(int i = 0 ; i < waterVerticesLeft.Count;i++)
        {
            waterVerticesAll.Add(waterVerticesLeft[i]);
            if(i < waterVerticesLeft.Count-1)
                distanceLeft += Vector3.Distance(waterVerticesLeft[i].transform.position , waterVerticesLeft[i+1].transform.position);
        }
        for(int i = 0 ; i < waterVerticesRight.Count;i++)
        {
            waterVerticesAll.Add(waterVerticesRight[i]);
            if(i < waterVerticesRight.Count-1)
                distanceRight += Vector3.Distance(waterVerticesRight[i].transform.position , waterVerticesRight[i+1].transform.position);
        }





        if( GameObject.Find("VerticalEdge") )
        {
            GameObject.DestroyImmediate(GameObject.Find("VerticalEdge"));
        }
        GameObject parent = new GameObject("VerticalEdge");

        List<int> triangles = new List<int>();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();

        float currentDistance = 0;

        for(int i = 0 ; i < waterVerticesAll.Count-1;i++)
        {
            if(i == waterVerticesLeft.Count-1)
                continue;
            Vector3 pointOne =waterVerticesAll[i].transform.position;
            Vector3 pointTwo =waterVerticesAll[i+1].transform.position;

            pointOne += new Vector3(0,-0.25f,0);
            pointTwo += new Vector3(0,-0.25f,0);


            Vector3 closestMidPointOne = GetClosestMidLinePoint(waterVerticesAll[i].gameObject);
            Vector3 closestMidPointTwo = GetClosestMidLinePoint(waterVerticesAll[i+1].gameObject);

            Plane waterPlane = new Plane(pointOne,pointTwo,closestMidPointOne);

            Vector3 dir = (pointOne-pointTwo).normalized;

            Vector3 myVector = Quaternion.AngleAxis(90, dir) * waterPlane.normal;

            Vector3 oneOffset = (closestMidPointOne - pointOne).normalized;
            Vector3 twoOffset = (closestMidPointTwo - pointTwo).normalized;

            pointOne+=oneOffset*0.1f;
            pointTwo+=twoOffset*0.1f;

            vertices.Add(pointOne);
            vertices.Add(pointOne+new Vector3(0,1,0));
            vertices.Add(pointTwo);
            vertices.Add(pointTwo+new Vector3(0,1,0));

            float uvX = (float)i / (float)waterVerticesAll.Count;
            float uvX2 = ((float)i+1f) / (float)waterVerticesAll.Count;

            uvX = currentDistance / distanceLeft;
            currentDistance+=Vector3.Distance(pointOne,pointTwo);
            uvX2 = currentDistance / distanceLeft;

            uv.Add(new Vector2(uvX,0));
            uv.Add(new Vector2(uvX,1));
            uv.Add(new Vector2(uvX2,0));
            uv.Add(new Vector2(uvX2,1));


            triangles.Add(vertices.Count-4);
            triangles.Add(vertices.Count-3);
            triangles.Add(vertices.Count-2);

            triangles.Add(vertices.Count-2);
            triangles.Add(vertices.Count-3);
            triangles.Add(vertices.Count-4);

            triangles.Add(vertices.Count-1);
            triangles.Add(vertices.Count-2);
            triangles.Add(vertices.Count-3);

            triangles.Add(vertices.Count-3);
            triangles.Add(vertices.Count-2);
            triangles.Add(vertices.Count-1);

        }

        GameObject edge = new GameObject("edge");
        edge.transform.parent = parent.transform;
        edge.AddComponent<MeshFilter>();
        edge.AddComponent<MeshRenderer>();

        Mesh m = new Mesh();
        m.name = "EdgeMesh";
        m.vertices=vertices.ToArray();
        m.uv=uv.ToArray();
        m.triangles=triangles.ToArray();
        //m.normals = normals;
        m.RecalculateNormals();

        edge.GetComponent<MeshFilter>().mesh = m;
        edge.GetComponent<MeshRenderer>().material = Instantiate((Material)AssetDatabase.LoadAssetAtPath("Assets/CartoonStylizedWater/Materials/edgeVDynamic.mat",typeof(Material)));
        WaterScroll sc = edge.AddComponent<WaterScroll>();
        sc.scrollSpeed = 0.5f;
        sc.scrollSpeedY = 0;
        sc.scrollSpeed2 = 0.31f;
        sc.scrollSpeedY2 = 0;
        sc.Tiling = true;
        sc.TilingValue = 0.48f;
        sc.TilingX=-20f;
        sc.amplitudeY = 3.1f;
        sc.amplitudeY2 = 2.6f;
        sc.max = 0.1f;

        edge.GetComponent<MeshRenderer>().sharedMaterial.SetTextureScale("_MainTex",new Vector2(-100,1));
        edge.GetComponent<MeshRenderer>().sharedMaterial.SetTextureScale("_MainTex2",new Vector2(-100,1));   




    }
    void BuildMeshHorizontalIndividual()
    {
        GameObject parentVertices = GameObject.Find("GeneratedVertices");

        List<GameObject> waterVerticesLeft = new List<GameObject>();
        List<GameObject> waterVerticesRight = new List<GameObject>();
        List<GameObject> waterVerticesAll = new List<GameObject>();
        for(int i = 0 ; i < parentVertices.transform.childCount;i+=4)
        {
            waterVerticesRight.Add( parentVertices.transform.GetChild(i).gameObject);
            waterVerticesRight.Add( parentVertices.transform.GetChild(i+2).gameObject);
            waterVerticesLeft.Add( parentVertices.transform.GetChild(i+1).gameObject);
            waterVerticesLeft.Add( parentVertices.transform.GetChild(i+3).gameObject);
        }

        for(int i = waterVerticesLeft.Count-2 ; i >= 0;i--)
        {
            if(Vector3.Distance( waterVerticesLeft[i].transform.position, waterVerticesLeft[i+1].transform.position) < 0.01f)
            {
                waterVerticesLeft.RemoveAt(i+1);
            }
        }
        for(int i = waterVerticesRight.Count-2 ; i >= 0;i--)
        {
            if(Vector3.Distance( waterVerticesRight[i].transform.position, waterVerticesRight[i+1].transform.position) < 0.01f)
            {
                waterVerticesRight.RemoveAt(i+1);
            }
        }

        float distanceLeft = 0;
        float distanceRight = 0;
        for(int i = 0 ; i < waterVerticesLeft.Count;i++)
        {
            waterVerticesAll.Add(waterVerticesLeft[i]);
            if(i < waterVerticesLeft.Count-1)
                distanceLeft += Vector3.Distance(waterVerticesLeft[i].transform.position , waterVerticesLeft[i+1].transform.position);
        }
        for(int i = 0 ; i < waterVerticesRight.Count;i++)
        {
            waterVerticesAll.Add(waterVerticesRight[i]);
            if(i < waterVerticesRight.Count-1)
                distanceRight += Vector3.Distance(waterVerticesRight[i].transform.position , waterVerticesRight[i+1].transform.position);
        }

        if( GameObject.Find("HorizontalEdge") )
        {
            GameObject.DestroyImmediate(GameObject.Find("HorizontalEdge"));
        }
        GameObject parent = new GameObject("HorizontalEdge");

        for(int i = 0 ; i < waterVerticesAll.Count-1;i++)
        {
            if(i == waterVerticesLeft.Count-1)
                    continue;
        Vector3 pointOne =waterVerticesAll[i].transform.position;
        Vector3 pointTwo =waterVerticesAll[i+1].transform.position;


            pointOne.y+=_EditorSettingHeightOffset;
            pointTwo.y+=_EditorSettingHeightOffset;

        GameObject edge = new GameObject("edge");
            edge.transform.parent = parent.transform;
        edge.AddComponent<MeshFilter>();
        edge.AddComponent<MeshRenderer>();

        Mesh m = new Mesh();
        m.name = "EdgeMesh";
        Vector3 [] vertices = new Vector3[4];
        Vector3 [] normals = new Vector3[4];
        Vector2 [] uv = new Vector2[4];
        int [] triangles = new int[12];


            Vector3 closestMidPointOne = GetClosestMidLinePoint(waterVerticesAll[i].gameObject);
            Vector3 closestMidPointTwo = GetClosestMidLinePoint(waterVerticesAll[i+1].gameObject);

            Plane waterPlane = new Plane(pointOne,pointTwo,closestMidPointOne);

        Vector3 dir = (pointOne-pointTwo).normalized;



        Vector3 myVector = Quaternion.AngleAxis(90, dir) * waterPlane.normal;

            Vector3 oneOffset = (closestMidPointOne - pointOne).normalized;
            Vector3 twoOffset = (closestMidPointTwo - pointTwo).normalized;

        vertices[0] = pointOne;
            vertices[1] = pointOne + oneOffset;
        vertices[2] = pointTwo;
            vertices[3] = pointTwo + twoOffset;

        normals[0] = Vector3.one;
        normals[1] = Vector3.one;
        normals[2] = Vector3.one;
        normals[3] = Vector3.one;

        uv[0] = new Vector2(0,0);
        uv[1] = new Vector2(0,1);
        uv[2] = new Vector2(1,0);
        uv[3] = new Vector2(1,1);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        triangles[3] = 2;
        triangles[4] = 1;
        triangles[5] = 0;

        triangles[6] = 3;
        triangles[7] = 2;
        triangles[8] = 1;

        triangles[9] = 1;
        triangles[10] = 2;
        triangles[11] = 3;

        m.vertices=vertices;
        m.uv=uv;
        m.triangles=triangles;
        m.normals = normals;
        m.RecalculateNormals();

        edge.GetComponent<MeshFilter>().mesh = m;

        edge.GetComponent<MeshRenderer>().material = Instantiate((Material)AssetDatabase.LoadAssetAtPath("Assets/CartoonStylizedWater/Materials/edgeHDynamic.mat",typeof(Material)));
        WaterScroll sc = edge.AddComponent<WaterScroll>();
        sc.scrollSpeed = -0.2f;
        sc.scrollSpeedY = 0;
        sc.scrollSpeed2 = -0.3f;
        sc.scrollSpeedY2 = 0;
        sc.TilingOnlyY = true;
        sc.amplitudeY = 1.0f;
        sc.amplitudeY2 = 1.5f;
        sc.max = 0.2f;
        sc.TilingX=1f;
        }
    }
    void BuildMeshHorizontalOneMesh()
    {
        GameObject parentVertices = GameObject.Find("GeneratedVertices");

        List<GameObject> waterVerticesLeft = new List<GameObject>();
        List<GameObject> waterVerticesRight = new List<GameObject>();
        List<GameObject> waterVerticesAll = new List<GameObject>();
        for(int i = 0 ; i < parentVertices.transform.childCount;i+=4)
        {
            waterVerticesRight.Add( parentVertices.transform.GetChild(i).gameObject);
            waterVerticesRight.Add( parentVertices.transform.GetChild(i+2).gameObject);
            waterVerticesLeft.Add( parentVertices.transform.GetChild(i+1).gameObject);
            waterVerticesLeft.Add( parentVertices.transform.GetChild(i+3).gameObject);
        }

        for(int i = waterVerticesLeft.Count-2 ; i >= 0;i--)
        {
            if(Vector3.Distance( waterVerticesLeft[i].transform.position, waterVerticesLeft[i+1].transform.position) < 0.01f)
            {
                waterVerticesLeft.RemoveAt(i+1);
            }
        }
        for(int i = waterVerticesRight.Count-2 ; i >= 0;i--)
        {
            if(Vector3.Distance( waterVerticesRight[i].transform.position, waterVerticesRight[i+1].transform.position) < 0.01f)
            {
                waterVerticesRight.RemoveAt(i+1);
            }
        }

        float distanceLeft = 0;
        float distanceRight = 0;
        for(int i = 0 ; i < waterVerticesLeft.Count;i++)
        {
            waterVerticesAll.Add(waterVerticesLeft[i]);
            if(i < waterVerticesLeft.Count-1)
                distanceLeft += Vector3.Distance(waterVerticesLeft[i].transform.position , waterVerticesLeft[i+1].transform.position);
        }
        for(int i = 0 ; i < waterVerticesRight.Count;i++)
        {
            waterVerticesAll.Add(waterVerticesRight[i]);
            if(i < waterVerticesRight.Count-1)
                distanceRight += Vector3.Distance(waterVerticesRight[i].transform.position , waterVerticesRight[i+1].transform.position);
        }

        if( GameObject.Find("HorizontalEdge") )
        {
            GameObject.DestroyImmediate(GameObject.Find("HorizontalEdge"));
        }
        GameObject parent = new GameObject("HorizontalEdge");

        List<int> triangles = new List<int>();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();

        float currentDistance = 0;

        for(int i = 0 ; i < waterVerticesAll.Count-1;i++)
        {
            if(i == waterVerticesLeft.Count-1)
                continue;
            Vector3 pointOne =waterVerticesAll[i].transform.position;
            Vector3 pointTwo =waterVerticesAll[i+1].transform.position;


            pointOne.y+=_EditorSettingHeightOffset;
            pointTwo.y+=_EditorSettingHeightOffset;




            Vector3 closestMidPointOne = GetClosestMidLinePoint(waterVerticesAll[i].gameObject);
            Vector3 closestMidPointTwo = GetClosestMidLinePoint(waterVerticesAll[i+1].gameObject);

            Plane waterPlane = new Plane(pointOne,pointTwo,closestMidPointOne);

            Vector3 dir = (pointOne-pointTwo).normalized;

            Vector3 myVector = Quaternion.AngleAxis(90, dir) * waterPlane.normal;

            Vector3 oneOffset = (closestMidPointOne - pointOne).normalized;
            Vector3 twoOffset = (closestMidPointTwo - pointTwo).normalized;

            vertices.Add(pointOne);
            vertices.Add(pointOne+oneOffset);
            vertices.Add(pointTwo);
            vertices.Add(pointTwo+twoOffset);

            float uvX = (float)i / (float)waterVerticesAll.Count;
            float uvX2 = ((float)i+1f) / (float)waterVerticesAll.Count;

            uvX = currentDistance / distanceLeft;
            currentDistance+=Vector3.Distance(pointOne,pointTwo);
            uvX2 = currentDistance / distanceLeft;

            uv.Add(new Vector2(uvX,0));
            uv.Add(new Vector2(uvX,1));
            uv.Add(new Vector2(uvX2,0));
            uv.Add(new Vector2(uvX2,1));


            triangles.Add(vertices.Count-4);
            triangles.Add(vertices.Count-3);
            triangles.Add(vertices.Count-2);

            triangles.Add(vertices.Count-2);
            triangles.Add(vertices.Count-3);
            triangles.Add(vertices.Count-4);

            triangles.Add(vertices.Count-1);
            triangles.Add(vertices.Count-2);
            triangles.Add(vertices.Count-3);

            triangles.Add(vertices.Count-3);
            triangles.Add(vertices.Count-2);
            triangles.Add(vertices.Count-1);


        }

        GameObject edge = new GameObject("edge");
        edge.transform.parent = parent.transform;
        edge.AddComponent<MeshFilter>();
        edge.AddComponent<MeshRenderer>();

        Mesh m = new Mesh();
        m.name = "EdgeMesh";

        m.vertices=vertices.ToArray();
        m.uv=uv.ToArray();
        m.triangles=triangles.ToArray();
        m.RecalculateNormals();

        edge.GetComponent<MeshFilter>().mesh = m;

        edge.GetComponent<MeshRenderer>().material = Instantiate((Material)AssetDatabase.LoadAssetAtPath("Assets/CartoonStylizedWater/Materials/edgeHDynamic.mat",typeof(Material)));
        WaterScroll sc = edge.AddComponent<WaterScroll>();
        sc.scrollSpeed = 0.7f;
        sc.scrollSpeedY = 0;
        sc.scrollSpeed2 = 0.3f;
        sc.scrollSpeedY2 = 0;
        sc.TilingOnlyY = true;
        sc.amplitudeY = 1.0f;
        sc.amplitudeY2 = 1.5f;
        sc.max = 0.2f;
        sc.TilingX=1f;

        edge.GetComponent<MeshRenderer>().sharedMaterial.SetTextureScale("_MainTex",new Vector2(-20,1));
        edge.GetComponent<MeshRenderer>().sharedMaterial.SetTextureScale("_MainTex2",new Vector2(-20,1));   

    }
    void BuildMeshHorizontalShadowIndividual()
    {
        GameObject parentVertice = GameObject.Find("GeneratedVertices");
        List<GameObject> waterVerticesLeft = new List<GameObject>();
        List<GameObject> waterVerticesRight = new List<GameObject>();
        List<GameObject> waterVerticesAll = new List<GameObject>();
        for(int i = 0 ; i < parentVertice.transform.childCount;i+=4)
        {
            waterVerticesRight.Add( parentVertice.transform.GetChild(i).gameObject);
            waterVerticesRight.Add( parentVertice.transform.GetChild(i+2).gameObject);
            waterVerticesLeft.Add( parentVertice.transform.GetChild(i+1).gameObject);
            waterVerticesLeft.Add( parentVertice.transform.GetChild(i+3).gameObject);
        }
        for(int i = waterVerticesLeft.Count-2 ; i >= 0;i--)
        {
            if(Vector3.Distance( waterVerticesLeft[i].transform.position, waterVerticesLeft[i+1].transform.position) < 0.01f)
            {
                waterVerticesLeft.RemoveAt(i+1);
            }
        }
        for(int i = waterVerticesRight.Count-2 ; i >= 0;i--)
        {
            if(Vector3.Distance( waterVerticesRight[i].transform.position, waterVerticesRight[i+1].transform.position) < 0.01f)
            {
                waterVerticesRight.RemoveAt(i+1);
            }
        }
        for(int i = 0 ; i < waterVerticesLeft.Count;i++)
            waterVerticesAll.Add(waterVerticesLeft[i]);
        for(int i = 0 ; i < waterVerticesRight.Count;i++)
            waterVerticesAll.Add(waterVerticesRight[i]);

        if( GameObject.Find("HorizontalEdgeShadow") )
        {
            GameObject.DestroyImmediate(GameObject.Find("HorizontalEdgeShadow"));
        }
        GameObject parent = new GameObject("HorizontalEdgeShadow");

        for(int i = 0 ; i < waterVerticesAll.Count-1;i++)
        {
            if(i == waterVerticesLeft.Count-1)
                continue;
            
            int pointTwoIndex = i+1;
            Vector3 pointOne =waterVerticesAll[i].transform.position;
            Vector3 pointTwo =waterVerticesAll[pointTwoIndex].transform.position;

            pointOne.y+=_EditorSettingHeightOffsetShadows;
            pointTwo.y+=_EditorSettingHeightOffsetShadows;

            GameObject edge = new GameObject("edge");
            edge.transform.parent = parent.transform;
            edge.AddComponent<MeshFilter>();
            edge.AddComponent<MeshRenderer>();

            Mesh m = new Mesh();
            m.name = "EdgeMesh";
            Vector3 [] vertices = new Vector3[4];
            Vector3 [] normals = new Vector3[4];
            Vector2 [] uv = new Vector2[4];
            int [] triangles = new int[12];


            Vector3 closestMidPointOne = GetClosestMidLinePoint(waterVerticesAll[i].gameObject);
            Vector3 closestMidPointTwo = GetClosestMidLinePoint(waterVerticesAll[pointTwoIndex].gameObject);

            Plane waterPlane = new Plane(pointOne,pointTwo,closestMidPointOne);

            Vector3 dir = (pointOne-pointTwo).normalized;

            Vector3 myVector = Quaternion.AngleAxis(90, dir) * waterPlane.normal;

            Vector3 oneOffset = (closestMidPointOne - pointOne).normalized;
            Vector3 twoOffset = (closestMidPointTwo - pointTwo).normalized;


            vertices[0] = pointOne-oneOffset*0.2f;
            vertices[1] = pointOne + oneOffset*1.2f;
            vertices[2] = pointTwo-twoOffset*0.2f;
            vertices[3] = pointTwo + twoOffset*1.2f;

            normals[0] = Vector3.one;
            normals[1] = Vector3.one;
            normals[2] = Vector3.one;
            normals[3] = Vector3.one;

            uv[0] = new Vector2(0,0);
            uv[1] = new Vector2(0,1);
            uv[2] = new Vector2(1,0);
            uv[3] = new Vector2(1,1);

            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;

            triangles[3] = 2;
            triangles[4] = 1;
            triangles[5] = 0;

            triangles[6] = 3;
            triangles[7] = 2;
            triangles[8] = 1;

            triangles[9] = 1;
            triangles[10] = 2;
            triangles[11] = 3;

            m.vertices=vertices;
            m.uv=uv;
            m.triangles=triangles;
            m.normals = normals;
            m.RecalculateNormals();

            edge.GetComponent<MeshFilter>().mesh = m;

            edge.GetComponent<MeshRenderer>().material = Instantiate((Material)AssetDatabase.LoadAssetAtPath("Assets/CartoonStylizedWater/Materials/ShadowForGeneration.mat",typeof(Material)));
            WaterScroll sc = edge.AddComponent<WaterScroll>();
            sc.scrollSpeed = -0.7f;
            sc.scrollSpeedY = 0;
            sc.scrollSpeed2 = -0.3f;
            sc.scrollSpeedY2 = 0;
            sc.Tiling = true;
            sc.amplitudeY = 0f;
            sc.amplitudeY2 = 0f;
            sc.max = 0.48f;
            sc.TilingX=1;
        }
    }
    void BuildMeshHorizontalShadowOneMesh()
    {
        GameObject parentVertice = GameObject.Find("GeneratedVertices");
        List<GameObject> waterVerticesLeft = new List<GameObject>();
        List<GameObject> waterVerticesRight = new List<GameObject>();
        List<GameObject> waterVerticesAll = new List<GameObject>();
        for(int i = 0 ; i < parentVertice.transform.childCount;i+=4)
        {
            waterVerticesRight.Add( parentVertice.transform.GetChild(i).gameObject);
            waterVerticesRight.Add( parentVertice.transform.GetChild(i+2).gameObject);
            waterVerticesLeft.Add( parentVertice.transform.GetChild(i+1).gameObject);
            waterVerticesLeft.Add( parentVertice.transform.GetChild(i+3).gameObject);
        }
        for(int i = waterVerticesLeft.Count-2 ; i >= 0;i--)
        {
            if(Vector3.Distance( waterVerticesLeft[i].transform.position, waterVerticesLeft[i+1].transform.position) < 0.01f)
            {
                waterVerticesLeft.RemoveAt(i+1);
            }
        }
        for(int i = waterVerticesRight.Count-2 ; i >= 0;i--)
        {
            if(Vector3.Distance( waterVerticesRight[i].transform.position, waterVerticesRight[i+1].transform.position) < 0.01f)
            {
                waterVerticesRight.RemoveAt(i+1);
            }
        }
        float distanceLeft = 0;
        float distanceRight = 0;
        for(int i = 0 ; i < waterVerticesLeft.Count;i++)
        {
            waterVerticesAll.Add(waterVerticesLeft[i]);
            if(i < waterVerticesLeft.Count-1)
                distanceLeft += Vector3.Distance(waterVerticesLeft[i].transform.position , waterVerticesLeft[i+1].transform.position);
        }
        for(int i = 0 ; i < waterVerticesRight.Count;i++)
        {
            waterVerticesAll.Add(waterVerticesRight[i]);
            if(i < waterVerticesRight.Count-1)
                distanceRight += Vector3.Distance(waterVerticesRight[i].transform.position , waterVerticesRight[i+1].transform.position);
        }

        if( GameObject.Find("HorizontalEdgeShadow") )
        {
            GameObject.DestroyImmediate(GameObject.Find("HorizontalEdgeShadow"));
        }
        GameObject parent = new GameObject("HorizontalEdgeShadow");


        List<int> triangles = new List<int>();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();

        float currentDistance = 0;


        for(int i = 0 ; i < waterVerticesAll.Count-1;i++)
        {
            if(i == waterVerticesLeft.Count-1)
                continue;

            int pointTwoIndex = i+1;
            Vector3 pointOne =waterVerticesAll[i].transform.position;
            Vector3 pointTwo =waterVerticesAll[pointTwoIndex].transform.position;

            pointOne.y+=_EditorSettingHeightOffsetShadows;
            pointTwo.y+=_EditorSettingHeightOffsetShadows;



            Vector3 closestMidPointOne = GetClosestMidLinePoint(waterVerticesAll[i].gameObject);
            Vector3 closestMidPointTwo = GetClosestMidLinePoint(waterVerticesAll[pointTwoIndex].gameObject);

            Plane waterPlane = new Plane(pointOne,pointTwo,closestMidPointOne);

            Vector3 dir = (pointOne-pointTwo).normalized;

            Vector3 myVector = Quaternion.AngleAxis(90, dir) * waterPlane.normal;

            Vector3 oneOffset = (closestMidPointOne - pointOne).normalized;
            Vector3 twoOffset = (closestMidPointTwo - pointTwo).normalized;

            vertices.Add(pointOne-oneOffset*0.2f);
            vertices.Add(pointOne+ oneOffset*1.2f);
            vertices.Add(pointTwo-twoOffset*0.2f);
            vertices.Add(pointTwo+ twoOffset*1.2f);

            float uvX = (float)i / (float)waterVerticesAll.Count;
            float uvX2 = ((float)i+1f) / (float)waterVerticesAll.Count;

            uvX = currentDistance / distanceLeft;
            currentDistance+=Vector3.Distance(pointOne,pointTwo);
            uvX2 = currentDistance / distanceLeft;

            uv.Add(new Vector2(uvX,0));
            uv.Add(new Vector2(uvX,1));
            uv.Add(new Vector2(uvX2,0));
            uv.Add(new Vector2(uvX2,1));


            triangles.Add(vertices.Count-4);
            triangles.Add(vertices.Count-3);
            triangles.Add(vertices.Count-2);

            triangles.Add(vertices.Count-2);
            triangles.Add(vertices.Count-3);
            triangles.Add(vertices.Count-4);

            triangles.Add(vertices.Count-1);
            triangles.Add(vertices.Count-2);
            triangles.Add(vertices.Count-3);

            triangles.Add(vertices.Count-3);
            triangles.Add(vertices.Count-2);
            triangles.Add(vertices.Count-1);

        }

        GameObject edge = new GameObject("edge");
        edge.transform.parent = parent.transform;
        edge.AddComponent<MeshFilter>();
        edge.AddComponent<MeshRenderer>();

        Mesh m = new Mesh();
        m.name = "EdgeMesh";

        m.vertices=vertices.ToArray();
        m.uv=uv.ToArray();
        m.triangles=triangles.ToArray();
        m.RecalculateNormals();

        edge.GetComponent<MeshFilter>().mesh = m;

        edge.GetComponent<MeshRenderer>().material = Instantiate((Material)AssetDatabase.LoadAssetAtPath("Assets/CartoonStylizedWater/Materials/ShadowForGeneration.mat",typeof(Material)));
        WaterScroll sc = edge.AddComponent<WaterScroll>();
        sc.scrollSpeed = 0.9f;
        sc.scrollSpeedY = 0;
        sc.scrollSpeed2 = 0.3f;
        sc.scrollSpeedY2 = 0;
        sc.Tiling = true;
        sc.TilingOnlyY = true;
        sc.TilingX = -30;
        sc.TilingValue = 0.47f;
        sc.amplitudeY = 1f;
        sc.amplitudeY2 = 1f;
        sc.max = 0.4f;


        edge.GetComponent<MeshRenderer>().sharedMaterial.SetTextureScale("_MainTex",new Vector2(-30,1));
        edge.GetComponent<MeshRenderer>().sharedMaterial.SetTextureScale("_MainTex2",new Vector2(-30,1));   

    }
    void BuildWatterFallEdge()
    {
        GameObject parentVertice = GameObject.Find("GeneratedVertices");
        List<GameObject> waterVerticesLeft = new List<GameObject>();
        List<GameObject> waterVerticesRight = new List<GameObject>();
        List<GameObject> waterVerticesAll = new List<GameObject>();
        List<int> waterFallTopIndexes = new List<int>();
        List<int> waterFallBottomIndexes = new List<int>();

        for(int i = 0 ; i < parentVertice.transform.childCount-4;i+=4)
        {
            waterVerticesRight.Add( parentVertice.transform.GetChild(i).gameObject);
            waterVerticesRight.Add( parentVertice.transform.GetChild(i+2).gameObject);
            waterVerticesLeft.Add( parentVertice.transform.GetChild(i+1).gameObject);
            waterVerticesLeft.Add( parentVertice.transform.GetChild(i+3).gameObject);
        }

        float waterDir=0;
        float waterDir1=0;
        float waterDir2=0;
        float waterDir3=0;
        for(int i = 1 ; i < waterVerticesLeft.Count ;i++)
        {
            float distY = waterVerticesLeft[i].transform.position.y - waterVerticesLeft[i-1].transform.position.y;
            if(distY > 0.1)
                waterDir =1;
            else if(distY < -0.1)
                waterDir = -1;
            else
                waterDir =0;

     
   
            if(waterDir1 == waterDir2 && waterDir1 ==waterDir3)
            {
                if(waterDir != waterDir1)
                {
                    Debug.Log(i +" "+waterDir+" "+waterDir2+" " +waterDir3);
                    if(waterDir1 == 0)
                    {
                        if(waterFallTopIndexes.Contains(i) == false && i > _EditorSettingWatterFallOffset+_EditorSettingWatterFallUp)
                            waterFallTopIndexes.Add(i);
                    }
                   // if(waterDir1 < 0)
                     //   waterFallBottomIndexes.Add(i);
                }
            }

            waterDir3 = waterDir2;
            waterDir2 = waterDir1;
            waterDir1 = waterDir;
     
        }

        for(int i = 0 ; i < waterFallTopIndexes.Count ;i++)
        {
            List<Vector3> pointForWaterFallUp = new List<Vector3>();
            List<Vector3> pointForWaterFallDown = new List<Vector3>();
            int offset = -1;
            for(int j = 0; j < _EditorSettingWatterFallUp ; j++)
            {
              
                pointForWaterFallUp.Add(waterVerticesLeft[ waterFallTopIndexes[i]-j+_EditorSettingWatterFallOffset ].transform.position);
                pointForWaterFallUp.Add(waterVerticesRight[ waterFallTopIndexes[i]-j+_EditorSettingWatterFallOffset ].transform.position);
              
            }
            for(int j = 0; j < _EditorSettingWatterFallDown ; j++)
            {
                
                pointForWaterFallDown.Add(waterVerticesLeft[ waterFallTopIndexes[i]+j+_EditorSettingWatterFallOffset ].transform.position);
                pointForWaterFallDown.Add(waterVerticesRight[ waterFallTopIndexes[i]+j+_EditorSettingWatterFallOffset ].transform.position);
               
            }
                

            GameObject go1 = BuildTopWaterfallWithPointsUp(pointForWaterFallUp,"GeneratedWaterFallTopUp"+i.ToString());
            GameObject go2 = BuildTopWaterfallWithPointsDown(pointForWaterFallDown,"GeneratedWaterFallTopDown"+i.ToString());
            go1.transform.position +=new Vector3(0,0.3f,0);
            go2.transform.position +=new Vector3(0,0.3f,0);
        }

        //GameObject.Find("GeneratedWaterFallTopDown").transform.position +=new Vector3(0,0.3f,0);
       // GameObject.Find("GeneratedWaterFallTopUp").transform.position +=new Vector3(0,0.3f,0);
    }
    GameObject BuildTopWaterfallWithPointsUp(List<Vector3> points,string aName)
    {

        if(GameObject.Find(aName) != null)
        {
            GameObject.DestroyImmediate(GameObject.Find(aName));
        }
        GameObject parent = new GameObject(aName);


        parent.AddComponent<MeshFilter>();
        parent.AddComponent<MeshRenderer>();

        Mesh m = new Mesh();
        m.name = "ScriptedMesh";

        List<int> trianglesList = new List<int>();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();

        float totalDistance = 0;
        int side = 0;

        for(int i = 2 ; i < points.Count; i++)
        {
            if(side==0)
            {

            }
            if(side==1)
            {

            }
            totalDistance += Vector3.Distance( points[i] , points[i-2]);
            side++;
            if(side==2)
                side=0;
        }

        side = 0;
        float currentDist = 0;
        for(int i = 0 ; i < points.Count; i++)
        {
            vertices.Add(points[i]);
            float xValue = 0;
            float yValue = 0;

            float amountX = points.Count;

            if(side==0)
            {
                xValue=0; 
            }
            if(side==1)
            {
                xValue=1;
            }

            if(i>1)
                currentDist += Vector3.Distance( points[i] ,points[i-2]);

            yValue= currentDist / totalDistance;
            yValue = yValue;
            side++;
            if(side==2)
                side=0;
            uv.Add(new Vector2(xValue,yValue));

            normals.Add(Vector3.up);
        }

        for(int i = 0 ; i < vertices.Count-2; i++)
        {
            trianglesList.Add(i);
            trianglesList.Add(i+1);
            trianglesList.Add(i+2);
            trianglesList.Add(i+2);
            trianglesList.Add(i+1);
            trianglesList.Add(i);
        }

        m.vertices=vertices.ToArray();
        m.uv=uv.ToArray();
        m.triangles=trianglesList.ToArray();
        m.normals = normals.ToArray();
        m.RecalculateNormals();


        parent.GetComponent<MeshRenderer>().material = Instantiate((Material)AssetDatabase.LoadAssetAtPath("Assets/CartoonStylizedWater/Materials/edgeVDarkDynamic.mat",typeof(Material)));

        WaterScroll sc = parent.AddComponent<WaterScroll>();
        sc.scrollSpeed = 0.31f;
        sc.scrollSpeedY = 0;
        sc.scrollSpeed2 = -0.41f;
        sc.scrollSpeedY2 = 0;
        sc.Tiling = true;
        sc.TilingOnlyY = false;
        sc.TilingX = 2;
        sc.TilingValue = 0.47f;
        sc.amplitudeY = 0.1f;
        sc.amplitudeY2 = 0.1f;
        sc.max = 0.5f;

        parent.GetComponent<MeshFilter>().mesh = m;
        return parent;
    }
    GameObject BuildTopWaterfallWithPointsDown(List<Vector3> points,string aName)
    {

        if(GameObject.Find(aName) != null)
        {
            GameObject.DestroyImmediate(GameObject.Find(aName));
        }
        GameObject parent = new GameObject(aName);


        parent.AddComponent<MeshFilter>();
        parent.AddComponent<MeshRenderer>();

        Mesh m = new Mesh();
        m.name = "ScriptedMesh";

        List<int> trianglesList = new List<int>();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();

        float totalDistance = 0;
        int side = 0;

        for(int i = 2 ; i < points.Count; i++)
        {
            if(side==0)
            {

            }
            if(side==1)
            {

            }
            totalDistance += Vector3.Distance( points[i] , points[i-2]);
            side++;
            if(side==2)
                side=0;
        }

        side = 0;
        float currentDist = 0;
        for(int i = 0 ; i < points.Count; i++)
        {
            vertices.Add(points[i]);
            float xValue = 0;
            float yValue = 0;

            float amountX = points.Count;

            if(side==0)
            {
                xValue=0; 
            }
            if(side==1)
            {
                xValue=1;
            }

            if(i>1)
                currentDist += Vector3.Distance( points[i] ,points[i-2]);

            yValue= currentDist / totalDistance;
            side++;
            if(side==2)
                side=0;
            uv.Add(new Vector2(xValue,yValue));

            normals.Add(Vector3.up);
        }

        for(int i = 0 ; i < vertices.Count-2; i++)
        {
            trianglesList.Add(i);
            trianglesList.Add(i+1);
            trianglesList.Add(i+2);
            trianglesList.Add(i+2);
            trianglesList.Add(i+1);
            trianglesList.Add(i);
        }

        m.vertices=vertices.ToArray();
        m.uv=uv.ToArray();
        m.triangles=trianglesList.ToArray();
        m.normals = normals.ToArray();
        m.RecalculateNormals();


        parent.GetComponent<MeshRenderer>().material = Instantiate((Material)AssetDatabase.LoadAssetAtPath("Assets/CartoonStylizedWater/Materials/edgeVDarkDynamic.mat",typeof(Material)));

        WaterScroll sc = parent.AddComponent<WaterScroll>();
        sc.scrollSpeed = 0.31f;
        sc.scrollSpeedY = 0;
        sc.scrollSpeed2 = -0.41f;
        sc.scrollSpeedY2 = 0;
        sc.Tiling = true;
        sc.TilingOnlyY = false;
        sc.TilingX = 2;
        sc.TilingValue = 0.47f;
        sc.amplitudeY = 0f;
        sc.amplitudeY2 = 0f;
        sc.max = 0f;

        parent.GetComponent<MeshFilter>().mesh = m;
        return parent;
    }
    void BuildWatterFallSplash()
    {
        GameObject parentVertice = GameObject.Find("GeneratedVertices");
        List<GameObject> waterVerticesLeft = new List<GameObject>();
        List<GameObject> waterVerticesRight = new List<GameObject>();
        List<GameObject> waterVerticesAll = new List<GameObject>();
        List<int> waterFallTopIndexes = new List<int>();
        List<int> waterFallBottomIndexes = new List<int>();

        for(int i = 0 ; i < parentVertice.transform.childCount-4;i+=4)
        {
            waterVerticesRight.Add( parentVertice.transform.GetChild(i).gameObject);
            waterVerticesRight.Add( parentVertice.transform.GetChild(i+2).gameObject);
            waterVerticesLeft.Add( parentVertice.transform.GetChild(i+1).gameObject);
            waterVerticesLeft.Add( parentVertice.transform.GetChild(i+3).gameObject);
        }

        float waterDir=0;
        float waterDir1=0;
        float waterDir2=0;
        float waterDir3=0;
        float waterDir4=0;
        float waterDir5=0;
        for(int i = 1 ; i < waterVerticesLeft.Count ;i++)
        {
            float distY = waterVerticesLeft[i].transform.position.y - waterVerticesLeft[i-1].transform.position.y;
            if(distY > 0.1)
                waterDir =1;
            else if(distY < -0.1)
                waterDir = -1;
            else
                waterDir =0;



            if(waterDir1 == waterDir2 && waterDir1 ==waterDir3 && waterDir1 ==waterDir4 && waterDir1 ==waterDir5)
            {
                if(waterDir != waterDir1)
                {
                    Debug.Log(i +" "+waterDir+" "+waterDir2+" " +waterDir3 + " "+waterDir4+" "+waterDir5);
                    if(waterDir1 == -1)
                    {
                        if(waterFallTopIndexes.Contains(i) == false)
                            waterFallTopIndexes.Add(i);
                    }
                    // if(waterDir1 < 0)
                    //   waterFallBottomIndexes.Add(i);
                }
            }
            waterDir5 = waterDir4;
            waterDir4 = waterDir3;
            waterDir3 = waterDir2;
            waterDir2 = waterDir1;
            waterDir1 = waterDir;

        }

        for(int i = 0 ; i < waterFallTopIndexes.Count ;i++)
        {
            List<Vector3> pointForWaterFallUp = new List<Vector3>();
            List<Vector3> pointForWaterFallDown = new List<Vector3>();
            int offset = -1;
            for(int j = 0; j < _EditorSettingWatterFallSplashUp ; j++)
            {

                pointForWaterFallUp.Add(waterVerticesLeft[ waterFallTopIndexes[i]-j+_EditorSettingWatterFallSplashOffset ].transform.position);
                pointForWaterFallUp.Add(waterVerticesRight[ waterFallTopIndexes[i]-j+_EditorSettingWatterFallSplashOffset ].transform.position);

            }
            for(int j = 0; j < _EditorSettingWatterFallSplashDown ; j++)
            {

                pointForWaterFallDown.Add(waterVerticesLeft[ waterFallTopIndexes[i]+j+_EditorSettingWatterFallSplashOffset ].transform.position);
                pointForWaterFallDown.Add(waterVerticesRight[ waterFallTopIndexes[i]+j+_EditorSettingWatterFallSplashOffset ].transform.position);

            }


            GameObject go1 = BuildTopWaterfallSplashWithPointsUp(pointForWaterFallUp,"GeneratedWaterFallSplashTopUp"+i.ToString());
            GameObject go2 = BuildTopWaterfallSplashWithPointsDown(pointForWaterFallDown,"GeneratedWaterFallSplashTopDown"+i.ToString());

       

          //  go1.transform.position +=new Vector3(0,0.01f,0);
            go2.transform.position +=new Vector3(0,0.3f,0);
        }

        //GameObject.Find("GeneratedWaterFallTopDown").transform.position +=new Vector3(0,0.3f,0);
        // GameObject.Find("GeneratedWaterFallTopUp").transform.position +=new Vector3(0,0.3f,0);
    }
    GameObject BuildTopWaterfallSplashWithPointsUp(List<Vector3> points,string aName)
    {

        if(GameObject.Find(aName) != null)
        {
            GameObject.DestroyImmediate(GameObject.Find(aName));
        }
        GameObject parent = new GameObject(aName);


        parent.AddComponent<MeshFilter>();
        parent.AddComponent<MeshRenderer>();

        Mesh m = new Mesh();
        m.name = "ScriptedMesh";

        List<int> trianglesList = new List<int>();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();

        float totalDistance = 0;
        int side = 0;

        for(int i = 2 ; i < points.Count; i++)
        {
            if(side==0)
            {

            }
            if(side==1)
            {

            }
            totalDistance += Vector3.Distance( points[i] , points[i-2]);
            side++;
            if(side==2)
                side=0;
        }

        side = 0;
        float currentDist = 0;
        for(int i = 0 ; i < points.Count; i++)
        {
            vertices.Add(points[i]);
            float xValue = 0;
            float yValue = 0;

            float amountX = points.Count;

            if(side==0)
            {
                xValue=0; 
            }
            if(side==1)
            {
                xValue=1;
            }

            if(i>1)
                currentDist += Vector3.Distance( points[i] ,points[i-2]);

            yValue= currentDist / totalDistance;
            yValue = yValue;
            side++;
            if(side==2)
                side=0;
            uv.Add(new Vector2(xValue,yValue));

            normals.Add(Vector3.up);
        }

        for(int i = 0 ; i < vertices.Count-2; i++)
        {
            trianglesList.Add(i);
            trianglesList.Add(i+1);
            trianglesList.Add(i+2);
            trianglesList.Add(i+2);
            trianglesList.Add(i+1);
            trianglesList.Add(i);
        }

        m.vertices=vertices.ToArray();
        m.uv=uv.ToArray();
        m.triangles=trianglesList.ToArray();
        m.normals = normals.ToArray();
        m.RecalculateNormals();


        parent.GetComponent<MeshRenderer>().material = Instantiate((Material)AssetDatabase.LoadAssetAtPath("Assets/CartoonStylizedWater/Materials/edgeVDynamicWatterFallBottom.mat",typeof(Material)));

        WaterScroll sc = parent.AddComponent<WaterScroll>();
        sc.scrollSpeed = 0.31f;
        sc.scrollSpeedY = 0;
        sc.scrollSpeed2 = -0.41f;
        sc.scrollSpeedY2 = 0;
        sc.Tiling = true;
        sc.TilingOnlyY = false;
        sc.TilingX = 2;
        sc.TilingValue = 0.7f;
        sc.amplitudeY = 0f;
        sc.amplitudeY2 = 0f;
        sc.max = 0f;

        parent.GetComponent<MeshFilter>().mesh = m;
        return parent;
    }
    GameObject BuildTopWaterfallSplashWithPointsDown(List<Vector3> points,string aName)
    {

        if(GameObject.Find(aName) != null)
        {
            GameObject.DestroyImmediate(GameObject.Find(aName));
        }
        GameObject parent = new GameObject(aName);


        parent.AddComponent<MeshFilter>();
        parent.AddComponent<MeshRenderer>();

        Mesh m = new Mesh();
        m.name = "ScriptedMesh";

        List<int> trianglesList = new List<int>();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();

        float totalDistance = 0;
        int side = 0;

        for(int i = 2 ; i < points.Count; i++)
        {
            if(side==0)
            {

            }
            if(side==1)
            {

            }
            totalDistance += Vector3.Distance( points[i] , points[i-2]);
            side++;
            if(side==2)
                side=0;
        }

        side = 0;
        float currentDist = 0;
        for(int i = 0 ; i < points.Count; i++)
        {
            vertices.Add(points[i]);
            float xValue = 0;
            float yValue = 0;

            float amountX = points.Count;

            if(side==0)
            {
                xValue=0; 
            }
            if(side==1)
            {
                xValue=1;
            }

            if(i>1)
                currentDist += Vector3.Distance( points[i] ,points[i-2]);

            yValue= currentDist / totalDistance;
            side++;
            if(side==2)
                side=0;
            uv.Add(new Vector2(xValue,yValue));

            normals.Add(Vector3.up);
        }

        for(int i = 0 ; i < vertices.Count-2; i++)
        {
            trianglesList.Add(i);
            trianglesList.Add(i+1);
            trianglesList.Add(i+2);
            trianglesList.Add(i+2);
            trianglesList.Add(i+1);
            trianglesList.Add(i);
        }

        m.vertices=vertices.ToArray();
        m.uv=uv.ToArray();
        m.triangles=trianglesList.ToArray();
        m.normals = normals.ToArray();
        m.RecalculateNormals();


        GameObject particleEffect1 = (GameObject)Instantiate((GameObject)AssetDatabase.LoadAssetAtPath("Assets/CartoonStylizedWater/Prefabs/RipplesPlane.prefab",typeof(GameObject)));
        GameObject particleEffect2 = (GameObject)Instantiate((GameObject)AssetDatabase.LoadAssetAtPath("Assets/CartoonStylizedWater/Prefabs/RipplesPlane.prefab",typeof(GameObject)));



        Vector3 effectForward =  ( points[points.Count-3] - points[points.Count-1] );
        Vector3 effectForward2 =  ( points[points.Count-2] - points[points.Count-1] );

        Vector3 effectPos =  (points[0+_EditorSettingWatterFallSplashEffectOffset] + effectForward2/4)+ new Vector3(0,0.1f,0);
        Vector3 effectPos2 =  (points[0+_EditorSettingWatterFallSplashEffectOffset] + (effectForward2/4) * 3) + new Vector3(0,0.1f,0);
        float dist =  (points[points.Count-1] - points[points.Count-2]).magnitude;

        particleEffect1.transform.position = effectPos ;
        particleEffect1.transform.parent = parent.transform;
        particleEffect1.transform.forward = effectForward;



        particleEffect2.transform.position = effectPos2;
        particleEffect2.transform.parent = parent.transform;
        particleEffect2.transform.forward = effectForward;


        parent.GetComponent<MeshRenderer>().material = Instantiate((Material)AssetDatabase.LoadAssetAtPath("Assets/CartoonStylizedWater/Materials/edgeVDynamicWatterFallBottom.mat",typeof(Material)));

        WaterScroll sc = parent.AddComponent<WaterScroll>();
        sc.scrollSpeed = 0.31f;
        sc.scrollSpeedY = 0;
        sc.scrollSpeed2 = -0.41f;
        sc.scrollSpeedY2 = 0;
        sc.Tiling = true;
        sc.TilingOnlyY = false;
        sc.TilingX = 2;
        sc.TilingValue = 0.49f;
        sc.amplitudeY = 0.1f;
        sc.amplitudeY2 = 0.1f;
        sc.max = 0.5f;

        parent.GetComponent<MeshFilter>().mesh = m;
        return parent;
    }
    void ToggleSpheres()
    {
        GameObject parent = GameObject.Find("MidLine");
        if(parent != null)
        for(int i = 0 ; i < parent.transform.childCount;i++)
        {
            if(parent.transform.GetChild(i).GetComponent<MeshRenderer>().enabled)
                parent.transform.GetChild(i).GetComponent<MeshRenderer>().enabled = false;
            else
                parent.transform.GetChild(i).GetComponent<MeshRenderer>().enabled = true;
        }

        parent = GameObject.Find("GeneratedForwardDirection");
        for(int i = 0 ; i < parent.transform.childCount;i++)
        {
            if(parent.transform.GetChild(i).GetComponent<MeshRenderer>().enabled)
                parent.transform.GetChild(i).GetComponent<MeshRenderer>().enabled = false;
            else
                parent.transform.GetChild(i).GetComponent<MeshRenderer>().enabled = true;
        }

        parent = GameObject.Find("GeneratedVertices");
        for(int i = 0 ; i < parent.transform.childCount;i++)
        {
            if(parent.transform.GetChild(i).GetComponent<MeshRenderer>().enabled)
                parent.transform.GetChild(i).GetComponent<MeshRenderer>().enabled = false;
            else
                parent.transform.GetChild(i).GetComponent<MeshRenderer>().enabled = true;
        }



    }
    void ToggleLines()
    {
        GameObject parent = GameObject.Find("MidLine");
        parent = GameObject.Find("GeneratedExtras");
        for(int i = 0 ; i < parent.transform.childCount;i++)
        {
            if(parent.transform.GetChild(i).GetComponent<MeshRenderer>().enabled)
                parent.transform.GetChild(i).GetComponent<MeshRenderer>().enabled = false;
            else
                parent.transform.GetChild(i).GetComponent<MeshRenderer>().enabled = true;
        }
    }
    void DeleteAll()
    {
        GameObject obj = GameObject.Find("HorizontalEdgeShadow");
        if(obj != null)
            DestroyImmediate(obj);

        obj = GameObject.Find("HorizontalEdge");
        if(obj != null)
            DestroyImmediate(obj);

        obj = GameObject.Find("VerticalEdge");
        if(obj != null)
            DestroyImmediate(obj);

        obj = GameObject.Find("GeneratedMeshFromPoints");
        if(obj != null)
            DestroyImmediate(obj);

        obj = GameObject.Find("GeneratedForwardDirection");
        if(obj != null)
            DestroyImmediate(obj);

        obj = GameObject.Find("GeneratedVertices");
        if(obj != null)
            DestroyImmediate(obj);

        obj = GameObject.Find("GeneratedExtras");
        if(obj != null)
            DestroyImmediate(obj);

        obj = GameObject.Find("GeneratedWaterFallTopUp");
        if(obj != null)
            DestroyImmediate(obj);

        obj = GameObject.Find("GeneratedWaterFallTopDown");
        if(obj != null)
            DestroyImmediate(obj);

        for(int i = 0; i<  20;i++)
        {
            if(GameObject.Find("GeneratedWaterFallTopUp"+i.ToString()) != null)
                DestroyImmediate(GameObject.Find("GeneratedWaterFallTopUp"+i.ToString()));
            if( GameObject.Find("GeneratedWaterFallTopDown"+i.ToString()) != null)
                DestroyImmediate(GameObject.Find("GeneratedWaterFallTopDown"+i.ToString()) );
        }
        for(int i = 0; i<  20;i++)
        {
            if(GameObject.Find("GeneratedWaterFallSplashTopUp"+i.ToString()) != null)
                DestroyImmediate(GameObject.Find("GeneratedWaterFallSplashTopUp"+i.ToString()));
            if( GameObject.Find("GeneratedWaterFallSplashTopDown"+i.ToString()) != null)
                DestroyImmediate(GameObject.Find("GeneratedWaterFallSplashTopDown"+i.ToString()) );
        }
        
    }
    void PlaceUnderObject()
    {
        Transform parent = GameObject.Find("MidPointSplineLine").transform;
        DestroyImmediate( GameObject.Find("GeneratedVertices"));
        DestroyImmediate(GameObject.Find("GeneratedForwardDirection"));
        DestroyImmediate(GameObject.Find("GeneratedExtras"));
        GameObject.Find("GeneratedMeshFromPoints").transform.parent = parent;
        GameObject.Find("VerticalEdge").transform.parent = parent;
        GameObject.Find("HorizontalEdge").transform.parent = parent;
        GameObject.Find("HorizontalEdgeShadow").transform.parent = parent;

        for(int i = 0; i<  20;i++)
        {
            if(GameObject.Find("GeneratedWaterFallTopUp"+i.ToString()) != null)
            GameObject.Find("GeneratedWaterFallTopUp"+i.ToString()).transform.parent = parent;
            if( GameObject.Find("GeneratedWaterFallTopDown"+i.ToString()) != null)
            GameObject.Find("GeneratedWaterFallTopDown"+i.ToString()).transform.parent = parent;
        }
        for(int i = 0; i<  20;i++)
        {
            if(GameObject.Find("GeneratedWaterFallSplashTopUp"+i.ToString()) != null)
                GameObject.Find("GeneratedWaterFallSplashTopUp"+i.ToString()).transform.parent = parent;
            if( GameObject.Find("GeneratedWaterFallSplashTopDown"+i.ToString()) != null)
                GameObject.Find("GeneratedWaterFallSplashTopDown"+i.ToString()).transform.parent = parent;
        }
    }
    int _EditorSettingWatterFallUp=8;
    int _EditorSettingWatterFallDown=5;
    int _EditorSettingWatterFallSplashUp=8;
    int _EditorSettingWatterFallSplashDown=5;
    int _EditorSettingWatterFallOffset=0;
    int _EditorSettingWatterFallSplashOffset=0;
    int _EditorSettingWatterFallSplashEffectOffset=0;

    void LoadDefaultValues()
    {
        _EditorSettingWatterFallUp = 3;
        _EditorSettingWatterFallDown = 10;
        _EditorSettingWatterFallSplashUp = 3;
        _EditorSettingWatterFallSplashDown = 10;
        _EditorSettingWatterFallOffset = 0;
        _EditorSettingWatterFallSplashOffset = -4;
        _EditorSettingWatterFallSplashEffectOffset = 7;
        _EditorSettingHeightOffset = 0.03f;
        _EditorSettingHeightOffsetShadows = 0.02f;
    }

    float _EditorSettingHeightOffset=0.03f;
    float _EditorSettingHeightOffsetShadows = 0.02f;
    Vector2 _scroll = Vector2.zero;
    bool _showAdvanced = false;
    void OnGUI()
    {
        Color originalColor = GUI.color;
        Color originalBGColor = GUI.backgroundColor;
        _scroll = GUILayout.BeginScrollView(_scroll);


        if( GUILayout.Button("0:CreateBezierObject",GUILayout.Width(400)))
        {
            CreateBezierObject(); 
            Selection.activeGameObject = GameObject.Find("MidPointSplineLine");
        }
    
        GameObject MidPointSplineLine = GameObject.Find("MidPointSplineLine");
        if(MidPointSplineLine != null)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(MidPointSplineLine,typeof(GameObject),GUILayout.Width(197));
            GUI.color = Color.red;
            if( GUILayout.Button("Delete",GUILayout.Width(197)))
            {
                DestroyImmediate(MidPointSplineLine);
            }
            GUI.color = originalColor;
            GUILayout.EndHorizontal();

            if( GUILayout.Button("Add Point",GUILayout.Width(400)))
            {
                MidPointSplineLine.GetComponent<BezierSpline>().AddCurve();
                EditorUtility.SetDirty(MidPointSplineLine.GetComponent<BezierSpline>());

            }
        }


        GUILayout.Space(1);


        if(_showAdvanced)
            GUI.color = Color.green;
        GUILayout.BeginHorizontal();
        if( GUILayout.Button("Show Advanced Settings",GUILayout.Width(350)))
        {
            _showAdvanced = !_showAdvanced;
        }
        if( GUILayout.Button("Reset",GUILayout.Width(50)))
        {
            LoadDefaultValues();
        }
        GUILayout.EndHorizontal();
        GUI.color = originalColor;
       
        if(_showAdvanced)
        {
            
            GUI.backgroundColor = Color.green;
            GUI.Box(new Rect(0,GUILayoutUtility.GetLastRect().y-2,407,353),"");
            GUI.backgroundColor = originalBGColor;
            GUI.color = originalColor * new Color(1,1.2f,1,1);
        if( GUILayout.Button("1:BuildPointsFromBezierLine",GUILayout.Width(400)))
        {
            BuildPointsFromBezierLine(); 
        }
        GUILayout.Space(1);
        if( GUILayout.Button("2:MoveVerticesToSide",GUILayout.Width(400)))
        {
            MoveVerticesToSide(); 
        }
        GUILayout.Space(1);
        if( GUILayout.Button("3:BuildMeshFromPoints",GUILayout.Width(400)))
        {
            BuildMeshFromPoints(); 
        }
        GUILayout.Space(1);
        if( GUILayout.Button("4:BuildMeshVertical",GUILayout.Width(400)))
        {
            BuildMeshVerticalOneMesh(); 
        }
        GUILayout.Space(1);
        if( GUILayout.Button("5:BuildMeshHorizontal",GUILayout.Width(400)))
        {
            BuildMeshHorizontalOneMesh(); 
        }
  
        GUILayout.BeginHorizontal();
        _EditorSettingHeightOffset= EditorGUILayout.FloatField("HeighOffset",_EditorSettingHeightOffset,GUILayout.Width(400));
        GUILayout.EndHorizontal();
        GUILayout.Space(5);
        if( GUILayout.Button("6:BuildMeshHorizontalShadow",GUILayout.Width(400)))
        {
            BuildMeshHorizontalShadowOneMesh(); 
        }
            GUILayout.BeginHorizontal();
        _EditorSettingHeightOffsetShadows= EditorGUILayout.FloatField("HeighOffset",_EditorSettingHeightOffsetShadows,GUILayout.Width(400));
            GUILayout.EndHorizontal();
        GUILayout.Space(5);
        if( GUILayout.Button("7:BuildWatterFallEdge",GUILayout.Width(400)))
        {
            BuildWatterFallEdge(); 
        }
        GUILayout.BeginHorizontal();
            _EditorSettingWatterFallUp= EditorGUILayout.IntField("WatterFallEdgeUp",_EditorSettingWatterFallUp,GUILayout.Width(197));
            _EditorSettingWatterFallDown =EditorGUILayout.IntField("WatterFallEdgeDown",_EditorSettingWatterFallDown,GUILayout.Width(197));
        GUILayout.EndHorizontal();
            _EditorSettingWatterFallOffset =EditorGUILayout.IntField("Offset",_EditorSettingWatterFallOffset,GUILayout.Width(197));
        GUILayout.Space(5);
        if( GUILayout.Button("8:BuildWatterFallSplash",GUILayout.Width(400)))
        {
            BuildWatterFallSplash(); 
        }
        GUILayout.BeginHorizontal();
            _EditorSettingWatterFallSplashUp= EditorGUILayout.IntField("WatterFallSplashUp",_EditorSettingWatterFallSplashUp,GUILayout.Width(197));
            _EditorSettingWatterFallSplashDown =EditorGUILayout.IntField("WatterFallSplashDown",_EditorSettingWatterFallSplashDown,GUILayout.Width(197));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
            _EditorSettingWatterFallSplashOffset =EditorGUILayout.IntField("Offset",_EditorSettingWatterFallSplashOffset,GUILayout.Width(197));
            _EditorSettingWatterFallSplashEffectOffset =EditorGUILayout.IntField("Effect",_EditorSettingWatterFallSplashEffectOffset,GUILayout.Width(197));
        GUILayout.EndHorizontal();

        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
            if( GUILayout.Button("9:Toggle Spheres",GUILayout.Width(197)))
        {
            ToggleSpheres(); 
        }
            if( GUILayout.Button("10:Toggle Lines",GUILayout.Width(197)))
        {
            ToggleLines(); 
        }
        GUILayout.EndHorizontal();
      
      
        }
        GUILayout.Space(5);
        GUILayout.Label("Controlls");
        GUILayout.BeginHorizontal();
        GUI.color = Color.red;
        if( GUILayout.Button("8:Delete All",GUILayout.Width(200)))
        {
            DeleteAll(); 
        }
        GUI.color = Color.green;
        if( GUILayout.Button("8: Run All",GUILayout.Width(200)))
        {
            BuildPointsFromBezierLine(); 
            MoveVerticesToSide();
            BuildMeshFromPoints();
            BuildMeshVerticalOneMesh();
            BuildMeshHorizontalOneMesh();
            BuildMeshHorizontalShadowOneMesh();
            BuildWatterFallEdge();
            BuildWatterFallSplash();

            ToggleSpheres();
            ToggleLines();
            PlaceUnderObject();

        }
        GUILayout.EndHorizontal();
        GUI.color = originalColor;

        GUILayout.EndScrollView();

//        GUILayout.Label("Legacy");
//        if( GUILayout.Button("BuildMesh"))
//        {
//            BuildMesh(); 
//        }
//        if( GUILayout.Button("1:BuildPointsFromManualLine"))
//        {
//            BuildPointsFromLine(); 
//        }
    }

    void OnEnable(){
        SceneView.onSceneGUIDelegate += OnSeceneGUI;
    }
    void OnDisable(){
        SceneView.onSceneGUIDelegate -= OnSeceneGUI;
    }
}