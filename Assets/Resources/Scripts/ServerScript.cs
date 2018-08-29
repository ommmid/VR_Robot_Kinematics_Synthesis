using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Net;
using System.Text;
using System.IO;

public class ServerScript : MonoBehaviour {

    // Use this for initialization
    TcpListener listener;
    String msg;

    IPAddress Host = IPAddress.Parse("127.0.0.1");
    Int32 Port = 20000;

    void Start()
    {
        listener = new TcpListener(Host, Port);
        listener.Start();
        print("is listening");
    }
    // Update is called once per frame
    void Update()
    {
        if (!listener.Pending())
        {
        }
        else
        {
            print("socket comes");
            TcpClient client = listener.AcceptTcpClient();
            NetworkStream ns = client.GetStream();
            StreamReader reader = new StreamReader(ns);
            msg = reader.ReadToEnd();
            print(msg);
        }
    }
}
