using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class robotSynthesis : MonoBehaviour {

    public GameObject j0;
    public GameObject c0;
    public GameObject j1;
    public GameObject c1;
    public GameObject c1p;
    public GameObject je;
    public GameObject ce;
    public GameObject link; // link base or link parent
    public GameObject Jtext; // joint1 text

    public Convertion cv;
   // public cllBack callBack;
   [HideInInspector]
    public int k;
    [HideInInspector]
    public int numberOfJoint;

    struct Line
    {
        public Vector3 lineDirection;
        public Vector3 lineMoment;
    }

    [HideInInspector]    
    public List<GameObject[]> jointCylinders = new List<GameObject[]>();
    public List<GameObject> jointAxes = new List<GameObject>();
    List<Line> jointLines = new List<Line>(); // right-handed
    public List<float[]> jointValues = new List<float[]>();
    public List<Vector3[]> perpendicularPoints = new List<Vector3[]>(); // left-handed
    public List<GameObject> links = new List<GameObject>();
    [HideInInspector]
    public GameObject newlinkEE, newlinkBase;
    [HideInInspector]
    public Vector3 linkBase, linkEE, linkBaseDir, linkEEDir;
    [HideInInspector]
    public List<GameObject> jointTexts = new List<GameObject>();
    
    cllBack callBack;

    // Use this for initialization
    void Start () {
        callBack = GameObject.Find("OriginSpot").GetComponent<cllBack>();

        // objects
        jointAxes.Add(j0); jointAxes.Add(j1); jointAxes.Add(je);
        GameObject[] gb = new GameObject[2];
        gb[0] = c1; gb[1] = c1p;
        jointCylinders.Add(gb);
        // jointCylinders = [c1 c1p ; c2 c2p; ...], c0 and ce should be treated separately

        // difference between Vector3.up and transform.up
        // Vector3.up is constant and corresponds to the world up so(0, 1, 0).It is the yellow arrow on hte gizmo in the scene view.
        // tranform.up corresponds to the yellow arrow on the object and is related to it. As you rotate your object, 
        // the gizmo on the object moves and all three arrows change direction (not magnitude, always 1). transform.up, 
        // right and forward gives you the vector to which they correspond in world space.

        newlinkBase = Instantiate(link, Vector3.up, Quaternion.identity); 
        //links.Add(newlinkBase);
        newlinkEE = Instantiate(link, Vector3.up, Quaternion.identity); 
        //links.Add(newlinkEE);

        // text
        jointTexts.Add(Jtext);

        // feed from output of the ArtTreeKS (read from ClientScript.cs)
        /* Debug.Log("run Add joint");
         GameObject.Find("OriginSpot").GetComponent<cllBack>().addJoint();
         jointAxes[1].transform.position = new Vector3(0.5f, 0.3f, 0);
         jointAxes[1].transform.rotation = Quaternion.AngleAxis(45f, new Vector3(0.5f, 0.5f, 0.5f));
         jointAxes[2].transform.position = new Vector3(0.7f, 0.5f, 0);
         jointAxes[2].transform.rotation = Quaternion.AngleAxis(30f, new Vector3(0.3f, 0.1f, 0.8f));*/

        // add joints the scene. the base joint, ee joint and one joint are there by default
        numberOfJoint = GameObject.Find("ClientObject").GetComponent<ClientScript>().numOfJoints;
        Debug.Log("number of joint ===> " + numberOfJoint);
        for (int i=0; i< numberOfJoint-1; i++)
        {
            GameObject.Find("OriginSpot").GetComponent<cllBack>().addJoint();
        }

        for (int i=0; i < jointAxes.Count-2; i++)
        {
            Vector3 vcDir_M = GameObject.Find("ClientObject").GetComponent<ClientScript>().calculatedJointDirection[i];
            Vector3 vcDir_U = cv.conU2M_translation(vcDir_M);
            jointAxes[i+1].transform.rotation = Quaternion.FromToRotation(Vector3.up, vcDir_U) ;
            Vector3 vcMom_M = GameObject.Find("ClientObject").GetComponent<ClientScript>().calculatedJointMoment[i];
            Vector3 vcLoc_U =cv.conU2M_translation( cv.CrossProduct(vcDir_M, vcMom_M));
            jointAxes[i + 1].transform.position = vcLoc_U;

        }
        // EE
        jointAxes[jointAxes.Count - 1].transform.rotation = GameObject.Find("FixedObject").GetComponent<FixScript>().poseRotation[0];
        jointAxes[jointAxes.Count - 1].transform.position = GameObject.Find("FixedObject").GetComponent<FixScript>().poseTranslation[0];
        
    }

    // Update is called once per frame
    void Update () {

        // Make lines in right-handed
        // the structure jointLines should be emptied for each frame otherwise it is going to add all the dirctions and moments
        // of lines of all frames in it which would make it hard to access becuase I would need to cound the frame too
        jointLines.RemoveRange(0, jointLines.Count);
        for (int i = 0; i< jointAxes.Count; i++)
        {
            Vector3 zAxis_M = cv.conU2M_translation(jointAxes[i].transform.up);
            Vector3 position_M =cv.conU2M_translation(jointAxes[i].transform.position );
            Line line; line.lineDirection = zAxis_M; line.lineMoment = cv.CrossProduct(position_M, zAxis_M);
            jointLines.Add(line);
        }

        // calculate the points that common normal intersects two lines in left-handed coordinates
        perpendicularPoints.RemoveRange(0, perpendicularPoints.Count);
        for (int i = 0; i <= jointCylinders.Count; i++)
        {
            Debug.Log(i);
            Vector3[] perpPs_M = perpPoints(jointLines[i], jointLines[i + 1]); Debug.Log("lineDirection" + jointLines[i].lineDirection);
            Vector3[] perpPs_U = new Vector3[2];
            perpPs_U[0] = cv.conU2M_translation(perpPs_M[0]);
            perpPs_U[1] = cv.conU2M_translation(perpPs_M[1]);
            perpendicularPoints.Add(perpPs_U);
            // the structure of perpendicularPoints is:
            // [c0 c1; c1p c2; c2p c3; c3p ce]
        }

        // joints cylinder should be aligned with the joints axes
        c0.transform.rotation = Quaternion.FromToRotation(Vector3.up, j0.transform.up); 
        c0.transform.position = perpendicularPoints[0][0];
        for (int i = 0; i < jointCylinders.Count; i++)
        {
            jointCylinders[i][0].transform.rotation = Quaternion.FromToRotation(Vector3.up, jointAxes[i+1].transform.up); // c1 
            jointCylinders[i][1].transform.rotation = Quaternion.FromToRotation(Vector3.up, jointAxes[i+1].transform.up); // c1p

            jointCylinders[i][0].transform.position = perpendicularPoints[i][1]; // c1 
            jointCylinders[i][1].transform.position = perpendicularPoints[i+1][0]; // c1p
        }
        ce.transform.rotation = Quaternion.FromToRotation(Vector3.up, je.transform.up);  
        ce.transform.position = perpendicularPoints[perpendicularPoints.Count-1][1];

        // ---------------------- Add links
        linkBase = c1.transform.position - c0.transform.position;
        float linkBaseNorm = cv.vecNorm(linkBase);
        linkBaseDir = linkBase / linkBaseNorm;
        newlinkBase.transform.position = c0.transform.position;
        newlinkBase.transform.rotation = Quaternion.FromToRotation(Vector3.up, linkBaseDir);
        Vector3 lcsB = newlinkBase.transform.localScale;
        lcsB.y = linkBaseNorm / 2;
        newlinkBase.transform.localScale =  lcsB;

        for (int i=0; i<jointCylinders.Count-1; i++)
        {
            Vector3 linkI = jointCylinders[i + 1][0].transform.position - jointCylinders[i][1].transform.position;
            float linkINorm = cv.vecNorm(linkI);
            Vector3 linkIDir = linkI / linkINorm;
            links[i].transform.position = jointCylinders[i][1].transform.position;
            links[i].transform.rotation = Quaternion.FromToRotation(Vector3.up, linkIDir);
            Vector3 lcsI = links[i].transform.localScale;
            lcsI.y = linkINorm / 2;
            links[i].transform.localScale =  lcsI;
        }

        linkEE = ce.transform.position - jointCylinders[jointCylinders.Count-1][1].transform.position;
        float linkEENorm = cv.vecNorm(linkEE);
        linkEEDir = linkEE / linkEENorm;
        newlinkEE.transform.position = jointCylinders[jointCylinders.Count - 1][1].transform.position;
        newlinkEE.transform.rotation = Quaternion.FromToRotation(Vector3.up, linkEEDir);
        Vector3 lcsE = newlinkEE.transform.localScale;
        lcsE.y = linkEENorm / 2;
        newlinkEE.transform.localScale =  lcsE;
        
      

        if (callBack.jointInformation == true)
        {
            if (callBack.jointName == "endE")
            {
                k = jointAxes.Count - 1;
            }
            else
            {
                string nm = callBack.jointName;
                Debug.Log(nm[5]);
                k = (int)Char.GetNumericValue(nm[5]);
            }
            Debug.Log("jointAxes Index: " + k);
            callBack.jointInformation = false;
        }

        

        

        if (GameObject.Find("FixedObject").GetComponent<FixScript>().bRobot == true)
        {
            Debug.Log("--------------------------- trueeeeeeeeeeeeeeeeeeeeeeeeeee");
            // disable this script (synthesis part) and enable makeRobot script (make part)
            gameObject.GetComponent<robotMake>().enabled = true;
            gameObject.GetComponent<robotSynthesis>().enabled = false;

        }
    }


    //----------------------------- functions
    //---------------------------------------

    // find the perpendicular line with respect to two lines
    Line perpLine(Line L1, Line L2)
    {
        Line L3;
        float dt = cv.DotProduct(L1.lineDirection, L2.lineDirection);

        if (0.99 < dt && dt < 1.01)
        {
            // if the two lines are parallel. get the perpendicular points on both lines and calculate the 
            // difference vector. That should give the pependicular vector
            Vector3 pp1 = cv.CrossProduct(L1.lineDirection, L1.lineMoment);
            Vector3 pp2 = cv.CrossProduct(L2.lineDirection, L2.lineMoment);
            L3.lineDirection = (pp2 - pp1) / cv.vecNorm((pp2 - pp1));
            L3.lineMoment = cv.CrossProduct(pp1, L3.lineDirection);
        }
        else
        {
            // if the two lines are not parallel
            
            Vector3 crP = cv.CrossProduct(L1.lineDirection, L2.lineDirection);
            L3.lineDirection = crP / cv.vecNorm(crP);
            L3.lineMoment = (cv.CrossProduct(L1.lineDirection, L2.lineMoment) + cv.CrossProduct(L1.lineMoment, L2.lineDirection)) / cv.vecNorm(crP);
        }

        return L3;
    } // ------------ correct

    // find two points intersecting the common lines between two lines
    Vector3[] perpPoints(Line L1, Line L2)
    {
        
        Line L3 = perpLine(L1, L2);
        Vector3 P1 = cv.CrossProduct(L1.lineDirection, L1.lineMoment) + (cv.DotProduct(cv.CrossProduct(L3.lineDirection, L3.lineMoment), L1.lineDirection)) * L1.lineDirection;
        Vector3 P2 = cv.CrossProduct(L2.lineDirection, L2.lineMoment) + (cv.DotProduct(cv.CrossProduct(L3.lineDirection, L3.lineMoment), L2.lineDirection)) * L2.lineDirection;

        Vector3[] perp = new Vector3[2];
        perp[0] = P1;
        perp[1] = P2;
        return perp;
    } // ---------- correct

    // find the perpendicular point on the line
    Vector3 linePoint(Line L1)
    {
        Vector3 p = cv.CrossProduct(L1.lineDirection, L1.lineMoment); ;
        return p;
    }

    float[] Vector4(float f1, float f2, float f3, float f4)
    {
        float[] vec = new float[4];
        vec[0] = f1;
        vec[1] = f2;
        vec[2] = f3;
        vec[3] = f4;

        return vec;
    }

    //------------------- change joint position
    public void sliderTX(float xValue)
    {
        Vector3 ps = jointAxes[k].transform.position;
        ps.x = xValue;
        jointAxes[k].transform.position = ps;
    }
    public void sliderTY(float yValue)
    {
        Vector3 ps = jointAxes[k].transform.position;
        ps.y = yValue;
        jointAxes[k].transform.position = ps;
    }
    public void sliderTZ(float zValue)
    {
        Vector3 ps = jointAxes[k].transform.position;
        ps.z = zValue;
        jointAxes[k].transform.position = ps;
    }
    public void sliderRX(float xValue)
    {
        Vector3 ps = jointAxes[k].transform.eulerAngles;
        ps.x = xValue;
        jointAxes[k].transform.eulerAngles = ps;
    }
    public void sliderRY(float yValue)
    {
        Vector3 ps = jointAxes[k].transform.eulerAngles;
        ps.y = yValue;
        jointAxes[k].transform.eulerAngles = ps;
    }
    public void sliderRZ(float zValue)
    {
        Vector3 ps = jointAxes[k].transform.eulerAngles;
        ps.z = zValue;
        jointAxes[k].transform.eulerAngles = ps;
    }
    // 
}
