using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Leap;
using Leap.Unity;
using Leap.Unity.Interaction;

public class robotMake : MonoBehaviour {

    public List<List<float>> hingeJointAngles = new List<List<float>>();
    public bool poseBool = false;
    public float xValueGlobal;
    public GameObject canvs;
    public GameObject jtSlider;
    public Convertion convert;
    public robotSynthesis rbtSynthesis;
    FixedJoint bc0_fixedJoint, c0l_fixedJoint, lc1_fixedJoint, cnl_fixedJoint, lce_fixedJoint;
    float connectedMS;
    List<GameObject> jointSliders = new List<GameObject>();
    public bool jointSliderBool;

    // Use this for initialization
    void Start () {
        connectedMS = 0.001f; // connectedMassScale

        GameObject.Find("PoseBtn").transform.SetParent(GameObject.Find("ButtonPanel").transform);
        GameObject.Find("PoseTxt").transform.SetParent(GameObject.Find("TextPanel").transform);

        for (int i=0; i<rbtSynthesis.jointAxes.Count; i++)
        {
            Destroy(rbtSynthesis.jointAxes[i]);
        }

        // ------------------------ Add rigidbody component to the jointCylinders
        rbtSynthesis.c0.AddComponent<Rigidbody>(); rbtSynthesis.c0.GetComponent<Rigidbody>().useGravity = false;
        rbtSynthesis.newlinkBase.AddComponent<Rigidbody>(); rbtSynthesis.newlinkBase.GetComponent<Rigidbody>().useGravity = false;
        for (int i=0; i< rbtSynthesis.jointCylinders.Count; i++)
        {
            rbtSynthesis.jointCylinders[i][0].AddComponent<Rigidbody>(); rbtSynthesis.jointCylinders[i][0].GetComponent<Rigidbody>().useGravity = false;
            rbtSynthesis.jointCylinders[i][1].AddComponent<Rigidbody>(); rbtSynthesis.jointCylinders[i][1].GetComponent<Rigidbody>().useGravity = false;
            // make the c1 joints longer so that it touches c1p
            Vector3 ccVec = rbtSynthesis.jointCylinders[i][1].transform.position - rbtSynthesis.jointCylinders[i][0].transform.position;
            float ccNorm = convert.vecNorm(ccVec);
            float cChildScale = rbtSynthesis.jointCylinders[i][0].transform.GetChild(0).localScale.y;
            float sg = convert.DotProduct(convert.conU2M_translation(rbtSynthesis.jointAxes[i + 1].transform.up), convert.conU2M_translation(ccVec));
            if(sg > 0)
            {
                rbtSynthesis.jointCylinders[i][0].transform.localScale = rbtSynthesis.jointCylinders[i][0].transform.localScale + new Vector3(0, ccNorm* cChildScale, 0); 
            }else if(sg < 0)
            {
                rbtSynthesis.jointCylinders[i][0].transform.localScale = rbtSynthesis.jointCylinders[i][0].transform.localScale + new Vector3(0, - ccNorm* cChildScale, 0); 
            }
            
        }
        rbtSynthesis.newlinkEE.AddComponent<Rigidbody>(); rbtSynthesis.newlinkEE.GetComponent<Rigidbody>().useGravity = false;
        rbtSynthesis.ce.AddComponent<Rigidbody>(); rbtSynthesis.ce.GetComponent<Rigidbody>().useGravity = false;

        // ------------------------ Add fixed joint and links
        // c0 to world
        bc0_fixedJoint =  rbtSynthesis.c0.AddComponent<FixedJoint>();
        // c0 to newLinkBase
        c0l_fixedJoint = rbtSynthesis.newlinkBase.AddComponent<FixedJoint>();
        c0l_fixedJoint.connectedBody = rbtSynthesis.c0.GetComponent<Rigidbody>();
        c0l_fixedJoint.connectedMassScale = connectedMS; // to stop the fixed joint from wobbling
        // newLinkBase to c1
        lc1_fixedJoint = rbtSynthesis.c1.AddComponent<FixedJoint>();
        lc1_fixedJoint.connectedBody = rbtSynthesis.newlinkBase.GetComponent<Rigidbody>();
        lc1_fixedJoint.connectedMassScale = connectedMS; // to stop the fixed joint from wobbling
        for (int i = 0; i < rbtSynthesis.jointCylinders.Count; i++)
            {
            
            if (i > 0)
            {
                rbtSynthesis.links[i-1].AddComponent<Rigidbody>(); rbtSynthesis.links[i - 1].GetComponent<Rigidbody>().useGravity = false;
                FixedJoint cl_fixedJoint;
                cl_fixedJoint = rbtSynthesis.links[i-1].AddComponent<FixedJoint>();
                cl_fixedJoint.connectedBody = rbtSynthesis.jointCylinders[i-1][1].GetComponent<Rigidbody>();
                cl_fixedJoint.connectedMassScale = connectedMS;

                FixedJoint lc_fixedJoint;
                lc_fixedJoint = rbtSynthesis.jointCylinders[i][0].AddComponent<FixedJoint>();
                lc_fixedJoint.connectedBody = rbtSynthesis.links[i-1].GetComponent<Rigidbody>();
                lc_fixedJoint.connectedMassScale = connectedMS;
            }
            
            }
        // cn(last c) to newLinkEE
        cnl_fixedJoint = rbtSynthesis.newlinkEE.AddComponent<FixedJoint>();
        cnl_fixedJoint.connectedBody = rbtSynthesis.jointCylinders[rbtSynthesis.jointCylinders.Count - 1][1].GetComponent<Rigidbody>();
        cnl_fixedJoint.connectedMassScale = connectedMS; 
        // newLinkEE to ce
        lce_fixedJoint = rbtSynthesis.ce.AddComponent<FixedJoint>();
        lce_fixedJoint.connectedBody = rbtSynthesis.newlinkEE.GetComponent<Rigidbody>();
        lce_fixedJoint.connectedMassScale = connectedMS;
                 
        // ------------------------ Add hinge joint    

        for (int i =0; i< rbtSynthesis.jointCylinders.Count; i++)
        {
            HingeJoint hingeJoint;
            hingeJoint = rbtSynthesis.jointCylinders[i][1].AddComponent<HingeJoint>();
            hingeJoint.connectedBody = rbtSynthesis.jointCylinders[i][0].GetComponent<Rigidbody>();
            hingeJoint.axis = rbtSynthesis.jointCylinders[i][1].transform.InverseTransformDirection(rbtSynthesis.jointCylinders[i][0].transform.up);
            // ".axis" The direction is defined in local space.
            // obj1.transform.InverseTransformDirection(obj2.transform.up) : convert the y-axis of obj2 from world frame to
            // the local frame of obj1
            hingeJoint.connectedMassScale = connectedMS;
            JointMotor JM = hingeJoint.motor;
            JM.force = 500;
            hingeJoint.motor = JM;
           // hingeJoint.useMotor = true;
            JointSpring spr = hingeJoint.spring;
            spr.spring = 1000;
            hingeJoint.spring = spr;
            hingeJoint.useSpring = enabled;
        }

        // --------------------- sleep
        /*      rbtSynthesis.c0.GetComponent<Rigidbody>().Sleep();
              for (int i = 0; i < rbtSynthesis.jointCylinders.Count; i++)
              {
                  rbtSynthesis.jointCylinders[i][0].GetComponent<Rigidbody>().Sleep();
                  rbtSynthesis.jointCylinders[i][1].GetComponent<Rigidbody>().Sleep();
              }
              rbtSynthesis.ce.GetComponent<Rigidbody>().Sleep();*/

        // add joint slider
  /*      for (int i=0; i<rbtSynthesis.jointTexts.Count; i++)
        {
            var newJointSlider = Instantiate(jtSlider,  new Vector3(0.5f, -0.2f, 0.6f), Quaternion.identity);
            newJointSlider.transform.SetParent(GameObject.Find("Canvas").transform);
            newJointSlider.GetComponent<Slider>().onValueChanged.AddListener(jointSliderCallBack);
            newJointSlider.name = "jointSlider" + (i + 1);
            jointSliders.Add(newJointSlider);
        }*/
        
        // find the sliders in hirarchy and assign the call back funtion
        for (int i=0; i < rbtSynthesis.jointAxes.Count-2 ; i++)
        {
            float numb = i + 1;
            GameObject.Find("UI Slider " + numb).transform.GetChild(1).GetComponent<InteractionSlider>().HorizontalSlideEvent = jointSliderCallBack;
            
        }

        // joint values of the reference configuration
        for (int p = 0; p < GameObject.Find("FixedObject").GetComponent<FixScript>().numJ; p++)
        {
//            GameObject.Find("ClientObject").GetComponent<ClientScript>().calculatedJointValues[p].Insert(0,GameObject.Find("createRobot").GetComponent<robotSynthesis>().jointCylinders[p][1].transform.localEulerAngles.y);
            GameObject.Find("ClientObject").GetComponent<ClientScript>().calculatedJointValues[p].Insert(0, GameObject.Find("createRobot").GetComponent<robotSynthesis>().jointCylinders[p][1].transform.GetComponent<HingeJoint>().angle);
        }


        Debug.Log("***************---------------------**********************");
        for (int i = 0; i < GameObject.Find("createRobot").GetComponent<robotSynthesis>().numberOfJoint; i++)
        {
            for (int j = 0; j < GameObject.Find("FixedObject").GetComponent<FixScript>().poseNatrix.Count; j++)
            {
                Debug.Log(i + " " + j + " " + GameObject.Find("ClientObject").GetComponent<ClientScript>().calculatedJointValues[i][j]);
            }
        }


        poseBool = true;

    }

    // Update is called once per frame
    void Update () {

        if (jointSliderBool == true)
        {

            jointSliderBool = false;
        }

    }

    public void jointSliderCallBack(float jointV)
    {
        int idx = 0;
        int numb = 0;
        for (int i = 0; i < rbtSynthesis.jointAxes.Count - 2; i++)
        {
            numb = i + 1;
            Transform sliderTransform = GameObject.Find("UI Slider " + numb).transform.GetChild(1);
            //float xValue = sliderTransform.localPosition.x;
            if (sliderTransform.GetComponent<Rigidbody>().IsSleeping() == false)
            {
                idx = numb - 1;
                //Transform trans = rbtSynthesis.jointCylinders[idx][1].transform;
                //trans.RotateAround(trans.position, trans.up, jointV);

                HingeJoint hJoint = rbtSynthesis.jointCylinders[idx][1].transform.GetComponent<HingeJoint>();
                JointSpring sprg = hJoint.spring;
                sprg.targetPosition = jointV;
                hJoint.spring = sprg;

            }
        }

     }

   
}
