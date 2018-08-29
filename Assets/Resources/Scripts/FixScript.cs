using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FixScript : MonoBehaviour {

    public GameObject fixedCubeYAxis;
    public string pushedSliderName;
    public GameObject sliderObject;
    public GameObject btn;
    public GameObject gob;
    public GameObject cubeObject;
    //public Vector3[] posePosition; // Unity will use the inspector version of the array for any public arrays, and since you probably didn't fill values in there, it'll assign a 0 length array
    //public Quaternion[] poseRotation;
    //public Matrix4x4 poseMatrix;
    [HideInInspector]
    public int numJ = 0;
    public bool ArtTreeKS = false; // static because I want to change them in another script
    public bool counter = false;
    public bool makeRobot = false;

    public bool bRobot = false; // shoudl be false but here the synthesis part is already done by the previous scene

    [HideInInspector]
    public int pn = 0;

    // Most of the time it is easier to use List instead of Array
    public List<Vector3> poseTranslation = new List<Vector3>();
    public List<Quaternion> poseRotation = new List<Quaternion>();
    public List<Matrix4x4> poseNatrix = new List<Matrix4x4>();
    //public List<GameObject> fixedCube = new List<GameObject>();

    EventSystem es;
    private void Awake()
    {
        // Don't initialize class variables at declaration time, doing it in Awake() or Startup() is the proper way to do it.
        //posePosition = new Vector3[5]; 
        //poseRotation = new Quaternion[5];

        es = EventSystem.current;

    }

    void Start()
    {
       // Debug.Log("------------------ poseTranslation ------> " + poseTranslation.Count);
    }

    public void FixCallBack()
    {
        counter = true;

        var newGOB = Instantiate(gob, cubeObject.transform.position, cubeObject.transform.rotation);
        newGOB.GetComponent<Renderer>().material.color = Color.blue;
        //fixedCube.Add(newGOB);
        newGOB.transform.SetParent(GameObject.Find("saveFixedCubes").transform);

        poseTranslation.Add(cubeObject.transform.position); 
        poseRotation.Add(cubeObject.transform.rotation);

        Matrix4x4 M4 = Matrix4x4.TRS(cubeObject.transform.position, cubeObject.transform.rotation, new Vector3(1, 1, 1));
        poseNatrix.Add(M4);

        //var newYAxis = Instantiate(fixedCubeYAxis, cubeObject.transform.position, cubeObject.transform.rotation);
        //newYAxis.transform.rotation = Quaternion.FromToRotation(Vector3.up, newGOB.transform.up);
        //newYAxis.transform.SetParent(GameObject.Find("saveFixedAxes").transform);

    }

    public void ATRKCallBack()
    {
        ArtTreeKS = true;
        GameObject.Find("waitText").GetComponent<Text>().text = "wait ...";
    }

    public void robotCallBack()
    {
        makeRobot = true;
        
    }

    public void jointsNumCallBack()
    {
        numJ = numJ + 1;
        btn.GetComponentInChildren<Text>().text = "Number of Joints: " + numJ.ToString();
        Debug.Log("numJ: " + numJ);
        var newSlider = Instantiate(sliderObject, GameObject.Find("SliderPanel").transform.position, GameObject.Find("SliderPanel").transform.rotation);
        newSlider.name = "UI Slider " + numJ;
        newSlider.transform.SetParent(GameObject.Find("SliderPanel").transform);
        newSlider.transform.localPosition = new Vector3(0,0,0);
        newSlider.transform.Translate(new Vector3(0, (numJ-1) * 0.1f, 0), Space.Self);
    }


    public void buildRobot()
    {
        bRobot = true;
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void poseNumber()
    {
        if (GameObject.Find("createRobot").GetComponent<robotMake>().poseBool == true)
        {
            GameObject.Find("PoseTxt").GetComponent<Text>().text = "Pose: " + (float)(pn + 1);

            Debug.Log("hhhhhhhiiiiiiiiiiiiiiiiiieeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee");
            
                for (int j = 0; j < GameObject.Find("ClientObject").GetComponent<ClientScript>().numOfJoints; j++)
                {

                if (pn == 0)
                {
                    float fl0 = GameObject.Find("ClientObject").GetComponent<ClientScript>().calculatedJointValues[j][0];
                    Debug.Log("joint values from ArtTreeKS " + "pose: " + pn + " joint: " + j + " => " + fl0);
                    
                    //Transform jointTransform0 = GameObject.Find("createRobot").GetComponent<robotSynthesis>().jointCylinders[j][1].transform;
                    //jointTransform0.Rotate(jointTransform0.up, fl0, Space.World); // fl0 is directly from Unity then we do not change it

                    HingeJoint hingeJoint0 = GameObject.Find("createRobot").GetComponent<robotSynthesis>().jointCylinders[j][1].transform.GetComponent<HingeJoint>();
                    JointSpring spr0 = hingeJoint0.spring;
                    spr0.targetPosition = fl0;
                    hingeJoint0.spring = spr0;
                }
                else
                {
                    float fl = GameObject.Find("ClientObject").GetComponent<ClientScript>().calculatedJointValues[j][pn];
                    float fl_U = conA2U_angle(fl); // convert the angle from ArtTreeKS to Unity hinge joint
                    Debug.Log("joint values from ArtTreeKS " + "pose: " + pn + " joint: " + j + " in ArtTreeKS => " + fl + " in Unity => " + fl_U);
                    // --- using object transform:
                    //Transform jointTransform = GameObject.Find("createRobot").GetComponent<robotSynthesis>().jointCylinders[j][1].transform;
                    //jointTransform.Rotate(jointTransform.up, -fl * 180 / Mathf.PI, Space.World);
                    // --- using joint spring:
                    HingeJoint hingeJoint = GameObject.Find("createRobot").GetComponent<robotSynthesis>().jointCylinders[j][1].transform.GetComponent<HingeJoint>();
                    JointSpring spr = hingeJoint.spring;
                    spr.targetPosition = - fl_U;
                    hingeJoint.spring = spr;
                }

            }

            pn = pn + 1;
            if (pn == poseNatrix.Count)
            {
                pn = 0;
            }
        }
    }


    public float conA2U_angle(float angle_ArtTreeKS)
    {
        // angle in ArtTreeKs is in radian and can be more than 2PI or less than -2PI
        // the angle of the hinge joint in unity is  -180 < t < 180
        
        float fl = angle_ArtTreeKS  * 180 / Mathf.PI;
        if (fl > 360 || fl < -360)
        {
            fl = fl % 360;
        }

        if (fl > 180)
        {
            fl = fl - 360;
        }
        else if (fl < -180)
        {
            fl = 360 + fl;
        }
        
        return fl;
    }

}
