using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Net.Sockets;

public class Client : MonoBehaviour {
    private bool socketReady;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;

    public string clientName;

    private void Start() {
        DontDestroyOnLoad(gameObject);
    }

    private void Update() {
        if (socketReady) {
            if (stream.DataAvailable) {
                string data = reader.ReadLine();

                if (data != null) {
                    OnIncomingData(data);
                }
            }
        }
    }

    private void OnApplicationQuit() {
        CloseSocket();
    }
    private void OnDisable()
    {
        CloseSocket();
    }
    private void CloseSocket() {
        if (!socketReady) {
            return;
        }
        writer.Close();
        reader.Close();
        socket.Close();
        socketReady = false;

    }
    public void Send(string data) {
        if (!socketReady) {
            return;
        }

        writer.WriteLine(data);
        writer.Flush();
    }
    private void OnIncomingData(string data) {
        Debug.Log(data);
    }
    public bool ConnectToServer(string host, int port) {
        if (socketReady) {
            return false;
        }
        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);

            socketReady = true;
        }
        catch (Exception e) {
            Debug.Log("Socket Error Occured: " + e.Message);
        }
        return socketReady;

    }
}

public class GameClient {
    public string name;
    public bool isHost;
}