using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;


[ExecuteInEditMode]
public class CubeTestScript : MonoBehaviour {

    public GameObject cb;
    public Convertion con;
    
	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        transform.rotation = Quaternion.AngleAxis(30, new Vector3(0.8994f, -0.4301f, -0.0782f));
        Debug.Log("cube rotation in quaternion form in Unity =====>>>>" + transform.rotation.ToString("F4"));

        Quaternion quatM =  con.conU2M_quaternion(transform.rotation);
        Debug.Log("cube rotation in quaternion form in MATLAB =====>>>>" + quatM.ToString("F4"));

        Vector3 vc = con.conU2M_translation(transform.position);
        Debug.Log(vc.ToString("F4"));

        // Here, I make HM converted to MATLAB: 
        float[,] M = con.conU2M_H(Matrix4x4.TRS(transform.position, transform.rotation, new Vector3(1, 1, 1)));
        Debug.Log("the object transformation converted to MATLAB by my functions: ");
        Debug.Log(M[0, 0].ToString("F4") + " " + M[0, 1].ToString("F4") + " " + M[0, 2].ToString("F4") + " " + M[0, 3].ToString("F4"));
        Debug.Log(M[1, 0].ToString("F4") + " " + M[1, 1].ToString("F4") + " " + M[1, 2].ToString("F4") + " " + M[1, 3].ToString("F4"));
        Debug.Log(M[2, 0].ToString("F4") + " " + M[2, 1].ToString("F4") + " " + M[2, 2].ToString("F4") + " " + M[2, 3].ToString("F4"));
        Debug.Log(M[3, 0].ToString("F4") + " " + M[3, 1].ToString("F4") + " " + M[3, 2].ToString("F4") + " " + M[3, 3].ToString("F4"));
        // then I use Alba's funtion in Mathematica to convert this HM to DQ.
        // For the same transformation (the transformation of the object that
        // this script is attached to), I calculate the DQ for MATLAB by my function and compare it with the Mathematica's result 
        float[] dq = con.qv2DQ(quatM, vc);
        Debug.Log("the object transformation converted to dq (in MATLAB) by my functions: ");
        Debug.Log(dq[0].ToString("F4") + ", " + dq[1].ToString("F4") + ", " + dq[2].ToString("F4") + ", " + dq[3].ToString("F4") +
              ", " +  dq[4].ToString("F4") + ", " + dq[5].ToString("F4") + ", " + dq[6].ToString("F4") + ", " + dq[7].ToString("F4"));

        //Debug.Log("position: " + transform.position.ToString("F4"));
        //Debug.Log("rotation: " + transform.rotation.ToString("F4"));

        //Vector3 vec = new Vector3(1, 4, 6);
        //Debug.Log(con.vecNorm(vec));
        //Debug.Log(con.vecUnit(vec).ToString("F4"));
        //Vector3 a = new Vector3(1, 5, 7);
        //Vector3 b = new Vector3(5, 4, 1);
        //Debug.Log(con.DotProduct(a, b).ToString("F4"));
        //Debug.Log(con.CrossProduct(a, b).ToString("F4"));

        //float[,] m1 = new float[3, 3];
        //m1[0, 0] = 1; m1[0, 1] = 5; m1[0, 2] = 6;
        //m1[1, 0] = 8; m1[1, 1] = 5; m1[1, 2] = 6;
        //m1[2, 0] = 51; m1[2, 1] = 7; m1[2, 2] = 8;

        //float[,] m2 = new float[3, 3];
        //m2[0, 0] = 1; m2[0, 1] = 3; m2[0, 2] = 2;
        //m2[1, 0] = 5; m2[1, 1] = 6; m2[1, 2] = 29;
        //m2[2, 0] = 4; m2[2, 1] = 1; m2[2, 2] = 6;

        //float[,] m12 = con.mat3Mult(m1, m2);
        //Debug.Log(m12[0,0] + " " + m12[0,1] + " " + m12[0,2]);
        //Debug.Log(m12[1, 0] + " " + m12[1, 1] + " " + m12[1, 2]);
        //Debug.Log(m12[2, 0] + " " + m12[2, 1] + " " + m12[2, 2]);

        //Debug.Log("position in Unity Coordinates: " + transform.position.ToString("F4"));
        //Debug.Log("same position in MATLAB Coordinate: " + con.conU2M_translation(transform.position).ToString("F4"));

        //Debug.Log("rotation in Unity Coordinates: " + transform.rotation.ToString("F4"));
        //Debug.Log("same rotation in MATLAB Coordinate: " + con.conU2M_quaternion(transform.rotation).ToString("F4"));

        //Matrix4x4 M4 = Matrix4x4.TRS(transform.position, transform.rotation, new Vector3(1,1,1) );
        //Debug.Log("------------------ M4 in Unity Coordinates:");
        //Debug.Log( M4.GetRow(0).ToString("F4"));
        //Debug.Log( M4.GetRow(1).ToString("F4"));
        //Debug.Log( M4.GetRow(2).ToString("F4"));
        //Debug.Log( M4.GetRow(3).ToString("F4"));
        //float[,] N4 = con.conU2M_H(M4);
        //Debug.Log("------------------ M4 in MATLAB Coordinates:");
        //Debug.Log(N4[0, 0].ToString("F4") + " " + N4[0, 1].ToString("F4") + " " + N4[0, 2].ToString("F4") + " " + N4[0, 3].ToString("F4"));
        //Debug.Log(N4[1, 0].ToString("F4") + " " + N4[1, 1].ToString("F4") + " " + N4[1, 2].ToString("F4") + " " + N4[1, 3].ToString("F4"));
        //Debug.Log(N4[2, 0].ToString("F4") + " " + N4[2, 1].ToString("F4") + " " + N4[2, 2].ToString("F4") + " " + N4[2, 3].ToString("F4"));
        //Debug.Log(N4[3, 0].ToString("F4") + " " + N4[3, 1].ToString("F4") + " " + N4[3, 2].ToString("F4") + " " + N4[3, 3].ToString("F4"));

        //Debug.Log((float)8/(float)3);

        //Quaternion q1 = new Quaternion(0.36515f, 0.54772f, 0.7303f, 0.18257f);
        //Quaternion q2 = new Quaternion(0.46829f, 0.65561f, 0.56195f, 0.18732f);
        //Debug.Log(con.qMult(q1, q2).ToString("F4"));

        //float[,] hm = { { 0.1488f,   0.0405f,   -0.9880f,   -0.2642f }, 
        //                { -0.9775f,    0.1571f,   -0.1408f,   -0.7337f},
        //                { 0.1495f,    0.9868f,    0.0629f,    2.1545f}, 
        //                { 0, 0, 0, 1} };
        //float[] dq = con.conHM2DQ(hm);
        //Debug.Log(dq[0].ToString("F4") + " " + dq[1].ToString("F4") + " " + dq[2].ToString("F4") + " " + dq[3].ToString("F4") + " " +
        //          dq[4].ToString("F4") + " " + dq[5].ToString("F4") + " " + dq[6].ToString("F4") + " " + dq[7].ToString("F4"));

        ///*
        //string text = File.ReadAllText(@".\Assets\Resources\Scripts\test.lua");
        //text = text.Replace("Unity_numPose", (5).ToString("F4"));
        //File.WriteAllText(@".\Assets\Resources\Scripts\test2.lua", text);
        //*/

        ///*
        //string str = "";
        //for (int i=1; i<=3; i++)
        //{
        //    str = "omid \n" + str;
        //}
        //File.WriteAllText(@".\Assets\Resources\Scripts\testString.lua", str);
        //*/
        //string str1 = " print(\"test from Unity okljlkgjl dfgkdfjglkj sdlgkdfjglf\") \n";
        //string str2 = "print(\"ommmmid\")";
        //var sb = new StringBuilder();
        //sb.Append(str1);
        //sb.Append(str2);
        //Debug.Log(sb);

        ////string str = "print(\"test from Unity\") \n print(\" ommid \")";
        //string str = "print(\"test from Unity hshshshs\")";
        //Debug.Log(ASCIIEncoding.Unicode.GetByteCount(str));
        //Debug.Log(ASCIIEncoding.ASCII.GetByteCount(str));
        //int strNum = ASCIIEncoding.ASCII.GetByteCount(str);
        ////File.WriteAllText(@"C:\Users\Omid\Desktop\Files\msgBytes.txt", strNum.ToString());

        // -------------- extract calculated joint information by ArtTreeKS
        List<int> InxS, InxE;
        int a;
        string tx;
        Vector3 vecD = new Vector3();
        Vector3 vecM = new Vector3();
        string[] words1, words2, words3;
        List<float> joint_value = new List<float>();
        List<List<float>> calculatedJointValues = new List<List<float>>();
        List<Vector3> calculatedJointDirection = new List<Vector3>();
        List<Vector3> calculatedJointMoment = new List<Vector3>();

        string data = File.ReadAllText(@".\Assets\Resources\Scripts\outResult.lua");
        InxS = findString(data, "{");
        InxE = findString(data, "}");

        calculatedJointValues.RemoveRange(0, calculatedJointValues.Count);
        calculatedJointDirection.RemoveRange(0, calculatedJointDirection.Count);
        calculatedJointMoment.RemoveRange(0, calculatedJointMoment.Count);
        for (int k = 0; k < 3*9; k+=9)
        {
            Debug.Log("--------------------------- k : " + k );
            a = 0;
            tx = data.Substring(InxS[k] + 2, (InxE[k] - 2) - (InxS[k] + 2));
            Debug.Log("================> joint values: " + tx);
            words1 = tx.Split(',');
            joint_value.RemoveRange(0, joint_value.Count);
            foreach (string word in words1)
            {
                joint_value.Add( float.Parse(word));
                Debug.Log(joint_value[a]);
                a = a + 1;
            }
            calculatedJointValues.Add(new List<float>(joint_value));
            
            a = 0;
            tx = data.Substring(InxS[k+3] + 2, (InxE[k+3] - 1) - (InxS[k+3] + 2));
            Debug.Log("================> joint direction: " + tx);
            words2 = tx.Split(',');
            foreach (string word in words2)
            {
                vecD[a] = float.Parse(word);
                Debug.Log(vecD[a]);
                a = a + 1;
            }
            calculatedJointDirection.Add(vecD);

            a = 0;
            tx = data.Substring(InxS[k+4] + 2, (InxE[k+4] - 1) - (InxS[k+4] + 2));
            Debug.Log("================> joint moment: " + tx);
            words3 = tx.Split(',');
            foreach (string word in words3)
            {
                vecM[a] = float.Parse(word);
                Debug.Log(vecM[a]);
                a = a + 1;
            }
            calculatedJointMoment.Add(vecM);
        }
        
        Debug.Log("============== joint Values ============" + calculatedJointValues.Count);
        for (int j = 0; j < calculatedJointValues.Count; j++)
        {
            Debug.Log("Joint Values => " + j + " => " + calculatedJointValues[j][0] + "  " + calculatedJointValues[j][1] + "  " + calculatedJointValues[j][2] + "  " + calculatedJointValues[j][3]);
        }
        Debug.Log("============== joint Direction ============" + calculatedJointDirection.Count);
        for (int i = 0; i < calculatedJointDirection.Count; i++)
        {
            Debug.Log("Joint Direction => " + i + " => " + calculatedJointDirection[i].ToString("F"));
        }
        Debug.Log("============== joint Moment ============" + calculatedJointMoment.Count);
        for (int i = 0; i < calculatedJointMoment.Count; i++)
        {
            Debug.Log("Joint Moment => " + i + " => " + calculatedJointMoment[i].ToString("F"));
        }

        // how to nest a list in another list
        Debug.Log("*****************************************************");
        List<List<float>> list = new List<List<float>>();
        List<float> list1 = new List<float>();
        float[] num = {4,5,6,7,8,9,1,2,3 };
        int x = 0;
        for (int m =0; m<2; m++)
        {
            list1.RemoveRange(0, list1.Count);
            for(int n =0; n < 4; n++)
            {
                Debug.Log(x);
                list1.Add(num[x]);
                x = x + 1;
            }
            list.Add(new List<float>(list1)); // here is the key point. doing "list.add(list1)" is wrong
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Debug.Log( i + j + " => " + list[i][j]);
                }
            }
        }

        Debug.Log(-375 % 360);
    }


    // functions
    List<int> findString(string str, string val) // we want to find "val" in "str"
    {
        int startIndex = 0;
        int st;
        int ed;
        int stT = 0;
        List<int> inx = new List<int>();

        while (str.IndexOf(val) > -1)
        {
            startIndex = str.IndexOf(val);
            st = startIndex + val.Length;
            ed = str.Length;
            str = str.Substring(st, ed - st);

            inx.Add(startIndex + stT);
            stT = st + stT;
        }
        return inx;
    }
}
