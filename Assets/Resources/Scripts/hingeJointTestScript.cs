using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hingeJointTestScript : MonoBehaviour {

	// Use this for initialization
	void Start () {

    }

    // Update is called once per frame
    void Update () {
        //float angle;
        //Vector3 vecAxis;
        //transform.localRotation.ToAngleAxis(out angle, out vecAxis);
        //Debug.Log(angle + " " + vecAxis );

        //Debug.Log(transform.localEulerAngles);

        //transform.rotation = Quaternion.Euler(0, 20 ,0);
        //transform.rotation = Quaternion.AngleAxis(1, transform.up) * transform.rotation; // angleaxis create a relative rotation so we have to multiply that by the previous one
        //transform.Rotate(transform.up, 0.1f, Space.World); // transform.up meams local y-axis expressed in world coordinate, that is why we have to use space.world for the last argument

        //float angle = 10f;
        //JointLimits jl = new JointLimits();
        //jl.min = angle;
        //jl.max = angle;
        //transform.GetComponent<HingeJoint>().limits = jl;

        JointSpring spr = transform.GetComponent<HingeJoint>().spring;
        spr.targetPosition = -45; // myInputData is the input from my sensor
        spr.spring = 100;
        transform.GetComponent<HingeJoint>().spring = spr;

        Debug.Log(transform.GetComponent<HingeJoint>().angle);
    }
}
