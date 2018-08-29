using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class cllBack : MonoBehaviour {

    // public GameObject cnvas;
     public GameObject link;
     public GameObject gob;
     public robotSynthesis cRS;
     public GameObject txtBox;
     public GameObject jCylinder;
     public bool addAxes = false;
     int numJoints = 1;


    public bool jointInformation = false;
    public string jointName;

    
    public void addJoint()
    {
        Debug.Log("adddddd Joiiiintttt");
        addAxes = true;

        numJoints = numJoints + 1;
        //txtBox.GetComponent<Text>().text = "Number of Joints: " + numJoints.ToString();
        // add an axes between the end-effector axis and the last joint axis
        var newJointAxis = Instantiate(gob, (cRS.jointAxes[cRS.jointAxes.Count-1].transform.position + cRS.jointAxes[cRS.jointAxes.Count - 2].transform.position) /2, Quaternion.identity);
        cRS.jointAxes.Insert(cRS.jointAxes.Count - 1, newJointAxis);
        // add two cylinders
        var newJointCylinder = Instantiate(jCylinder, newJointAxis.transform.position, Quaternion.identity);
        var newJointCylinderp = Instantiate(jCylinder, newJointAxis.transform.position, Quaternion.identity);
        GameObject[] newJointCylinderObjects = new GameObject[2];
        newJointCylinderObjects[0] = newJointCylinder;
        newJointCylinderObjects[1] = newJointCylinderp;
        cRS.jointCylinders.Add(newJointCylinderObjects);

        var newLink = Instantiate(link, Vector3.up, Quaternion.identity); 
        cRS.links.Add(newLink);
        /*
        var newJointText = Instantiate(cnvas.transform.GetChild(0).gameObject, new Vector3(cnvas.transform.GetChild(0).transform.position.x, cRS.jointTexts[numJoints-2].transform.position.y-35, cnvas.transform.GetChild(0).transform.position.z), Quaternion.identity);
        newJointText.transform.SetParent(cnvas.transform);
        newJointText.GetComponent<Text>().text = "Joint " + numJoints;
        newJointText.name = "Joint" + numJoints;
        cRS.jointTexts.Add(newJointText);*/
    }

    

    public void jointInfo()
    {
        jointInformation = true;
        jointName = EventSystem.current.currentSelectedGameObject.name;
    }


   

}
