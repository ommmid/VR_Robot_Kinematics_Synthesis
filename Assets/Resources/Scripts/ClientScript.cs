using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.UI;

public class ClientScript : MonoBehaviour
{
    public Convertion conv; //This will get the script of a gameobject (not the gameobject itself). But we have to drag the gameobject into the slot  though
    public FixScript robotIns;
   // public createRobotScript mkRobot;
       
    public String host = "localhost";
    public Int32 port = 20000;

    internal Boolean socket_ready = false;
    internal String input_buffer = "";
    TcpClient tcp_socket;
    NetworkStream net_stream;

    StreamWriter socket_writer;
    StreamReader socket_reader;

    public List<Vector3> jointAxisDirection = new List<Vector3>();
    public List<Vector3> jointAxisLocation = new List<Vector3>();

    [HideInInspector]
    public int numOfJoints;

    int numBytes;
    string text;

    // to extract joint information from ArtTreeKS output
    List<int> InxS, InxE;
    int a;
    string tx;
    string data;
    string[] words;
    Vector3 vecD = new Vector3();
    Vector3 vecM = new Vector3();
    List<float> jv = new List<float>();
    public List<List<float>> calculatedJointValues = new List<List<float>>();
    public List<Vector3> calculatedJointDirection = new List<Vector3>();
    public List<Vector3> calculatedJointMoment = new List<Vector3>();

    void Awake()
    {
            setupSocket();
            Debug.Log("The Socket is set up");
    }

    void Start()
    {
        // this should be false but I keep it true as I want to see what happens for now
      //  mkRobot.enabled = true;
    }

    void Update()
    {
       

        string received_data = readSocket();
      //  Debug.Log("received message: " + received_data);

        if (robotIns.ArtTreeKS)
        {
            numOfJoints = robotIns.numJ;
            int numPose = robotIns.poseNatrix.Count;
         //   Debug.Log("number of selected poses: " + numPose);


            List<float[]> DQ_ArtTreeKS = new List<float[]>();
            string Poses1 = "";
            string Poses2 = "";
            string Poses = "";

            for (int i = 0; i < numPose; i++)
            {
                Debug.Log("----------------------- i = " + i);

                // convert a homogenous matrix from Unity to Normal
                //float[,] M4 = conv.conU2M_H(robotIns.poseNatrix[i]); // 
                // convert Homogeneous matrix to Dual Quaternion in ArtTreeKS form => DQ = (a+V) + e (V0+a0)
                //float[] DQ = conv.conHM2DQ(M4);
                //DQ_ArtTreeKS.Add(DQ);

                // convert quaternion in Unity to MATLAB:
                Quaternion qt = conv.conU2M_quaternion(robotIns.poseRotation[i]);
                // convert the location from Unity to MATLAB:
                Vector3 b = conv.conU2M_translation(robotIns.poseTranslation[i]);
                // calculate the Dual Quaterion in ArtTreeKS form by the converted quaternion and converted location
                float[] DQ = conv.qv2DQ(qt, b);
                DQ_ArtTreeKS.Add(DQ);

                Poses1 = Poses1 + " P[" + (i + 1).ToString() + "] = {} \n";
                Poses2 = Poses2 + " P[" + (i + 1).ToString() + "][1] = luadq.raw({" + DQ_ArtTreeKS[i][0].ToString() + "," + DQ_ArtTreeKS[i][1].ToString() + "," + DQ_ArtTreeKS[i][2].ToString() + "," + DQ_ArtTreeKS[i][3].ToString() + "," + DQ_ArtTreeKS[i][4].ToString() + "," + DQ_ArtTreeKS[i][5].ToString() + "," + DQ_ArtTreeKS[i][6].ToString() + "," + DQ_ArtTreeKS[i][7].ToString() + "}) \n";
            }

            text = File.ReadAllText(@".\Assets\Resources\Scripts\test3R.lua"); 
            text = text.Replace("Unity_Tree", (robotIns.numJ-1).ToString());
            text = text.Replace("Unity_numPose", numPose.ToString());
            Poses = Poses1 + "\n" + Poses2;
            text = text.Replace("Unity_Poses", Poses);
            File.WriteAllText(@".\Assets\Resources\Scripts\testPoses.lua", text); // I do not need to save it but just for the sake of debugging
            
            numBytes = ASCIIEncoding.ASCII.GetByteCount(text);
            //File.WriteAllText(@"C:\Users\Omid\Desktop\Files\msgBytes.txt", numBytes.ToString());
            //Thread.Sleep(5000);

            // text = "print(\"test from Unity\") \n print(\" ommid \")";
            //Byte[] sendBytes = Encoding.UTF8.GetBytes(text);
            //tcp_socket.GetStream().Write(sendBytes, 0, sendBytes.Length);

            //text = "print(\"test from Unity\") \nprint('heidari') \nprint('salam')";
            //numBytes = ASCIIEncoding.ASCII.GetByteCount(text);

            robotIns.ArtTreeKS = false;

            input_buffer = numBytes.ToString();
            writeSocket(input_buffer);
            input_buffer = "";
            Debug.Log("the number of bytes is sent");
            
            StartCoroutine(Example());
            input_buffer = text;
            writeSocket(input_buffer);
            input_buffer = "";
        }

            if (received_data != "")
            {
            // ======================================================
            // save the joint information for robotMake.cs to use
            // extract calculated joint information by ArtTreeKS
            File.WriteAllText(@".\Assets\Resources\Scripts\outResult.lua", received_data);

            //data = File.ReadAllText(@".\Assets\Resources\Scripts\outResult.lua");
            data = received_data;
            InxS = findString(data, "{");
            InxE = findString(data, "}");
            calculatedJointValues.RemoveRange(0, calculatedJointValues.Count);
            calculatedJointDirection.RemoveRange(0, calculatedJointDirection.Count);
            calculatedJointMoment.RemoveRange(0, calculatedJointMoment.Count);
            Debug.Log("*********-----------++++++++++++++ number of joints: " + robotIns.numJ);
            for (int k = 0; k < ((robotIns.numJ) * 9)-8; k += 9)
            {
                Debug.Log("--------------------------- k : " + k);
                a = 0;
                tx = data.Substring(InxS[k] + 2, (InxE[k] - 2) - (InxS[k] + 2));
                Debug.Log("================> joint values: " + tx);
                words = tx.Split(',');
                jv.RemoveRange(0, jv.Count);
                foreach (string word in words)
                {
                    jv.Add( float.Parse(word));
                    Debug.Log(jv[a]);
                    a = a + 1;
                }
                calculatedJointValues.Add(new List<float>(jv));

                a = 0;
                tx = data.Substring(InxS[k + 3] + 2, (InxE[k + 3] - 1) - (InxS[k + 3] + 2));
                Debug.Log("================> joint direction: " + tx);
                words = tx.Split(',');
                foreach (string word in words)
                {
                    vecD[a] = float.Parse(word);
                    Debug.Log(vecD[a]);
                    a = a + 1;
                }
                calculatedJointDirection.Add(vecD);

                a = 0;
                tx = data.Substring(InxS[k + 4] + 2, (InxE[k + 4] - 1) - (InxS[k + 4] + 2));
                Debug.Log("================> joint moment: " + tx);
                words = tx.Split(',');
                foreach (string word in words)
                {
                    vecM[a] = float.Parse(word);
                    Debug.Log(vecM[a]);
                    a = a + 1;
                }
                calculatedJointMoment.Add(vecM);
            }
            /*
            Debug.Log("============== joint Values ============" + calculatedJointValues.Count);
            for (int i = 0; i < calculatedJointValues.Count; i++)
            {
                Debug.Log(i);
                Debug.Log(calculatedJointValues[i][0] + " " + calculatedJointValues[i][1] + " " + calculatedJointValues[i][2] + " " + calculatedJointValues[i][3]);
            }
            Debug.Log("============== joint Direction ============" + calculatedJointDirection.Count);
            for (int i = 0; i < calculatedJointDirection.Count; i++)
            {
                Debug.Log(calculatedJointDirection[i].ToString("F"));
            }
            Debug.Log("============== joint Moment ============" + calculatedJointMoment.Count);
            for (int i = 0; i < calculatedJointMoment.Count; i++)
            {
                Debug.Log(calculatedJointMoment[i].ToString("F"));
            }
            */
            // =========================================================
            closeSocket();

            GameObject.Find("waitText").GetComponent<Text>().text = "The robot is ready now";

        }
    }

    IEnumerator Example()
    {
        Debug.Log(Time.time);
        yield return new WaitForSeconds(5);
        Debug.Log(Time.time);
    }

    // -------------------------------------------------------
    // -------------------------------------------------------
    // --------------------- TCP Socket Functions ------------

    void OnApplicationQuit()
    {
        closeSocket();
    }

    public void setupSocket()
    {
        try
        {
            tcp_socket = new TcpClient(host, port);

            net_stream = tcp_socket.GetStream();
            socket_writer = new StreamWriter(net_stream);
            socket_reader = new StreamReader(net_stream);

            socket_ready = true;
        }
        catch (Exception e)
        {
            // Something went wrong
            Debug.Log("Socket error: " + e);
        }
    }

    public void writeSocket(string line)
    {
        if (!socket_ready)
            return;

        line = line + "\r\n";
        socket_writer.Write(line);
        socket_writer.Flush();
    }

    public String readSocket()
    {
        if (!socket_ready)
            return "";

        if (net_stream.DataAvailable)
           
            return socket_reader.ReadToEnd();

        return "";
    }

    public void closeSocket()
    {
        if (!socket_ready)
            return;

        socket_writer.Close();
        socket_reader.Close();
        tcp_socket.Close();
        socket_ready = false;
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