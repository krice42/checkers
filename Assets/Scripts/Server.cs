using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;
using System.Net.Sockets;
using UnityEngine;
using System.IO;

public class Server : MonoBehaviour {
    public int port = 6321;

    private List<ServerClient> clients;
    private List<ServerClient> disconnectList;

    private TcpListener server;
    private bool serverStarted;

    private void Update()
    {
        if (!serverStarted)
        {
            return;
        }

        foreach (ServerClient sc in clients)
        {
            //check if the client is still connected
            if (!IsConnected(sc.tcp))
            {
                sc.tcp.Close();
                disconnectList.Add(sc);
                continue;
            }
            else {
                NetworkStream s = sc.tcp.GetStream();
                if (s.DataAvailable)
                {
                    StreamReader reader = new StreamReader(s, true);
                    string data = reader.ReadLine();

                    if (data != null)
                    {
                        OnIncomingData(sc, data);
                    }
                }
            }
        }
        for (int i = 0; i < disconnectList.Count; i++)
        {
            // disconnection message goes here

            clients.Remove(disconnectList[i]);
            disconnectList.RemoveAt(i);
        }
    }

    public void Init() {
        DontDestroyOnLoad(gameObject);
        clients = new List<ServerClient>();
        disconnectList = new List<ServerClient>();

        try {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();

            StartListening();
            serverStarted = true;
        }
        catch (Exception e) {
            Debug.Log("Socket Error Occured: " + e.Message);
        }
    }
    private void Broadcast(string data, List<ServerClient> cl) {
        foreach (ServerClient sc in cl) {
            try
            {
                StreamWriter writer = new StreamWriter(sc.tcp.GetStream());
                writer.WriteLine(data);
                writer.Flush();
            }
            catch (Exception e) {
                Debug.Log("Write Error Occured: " + e.Message);
            }
        }
    }
    private void Broadcast(string data, ServerClient c)
    {
        foreach (ServerClient sc in cl)
        {
            try
            {
                StreamWriter writer = new StreamWriter(sc.tcp.GetStream());
                writer.WriteLine(data);
                writer.Flush();
            }
            catch (Exception e)
            {
                Debug.Log("Write Error Occured: " + e.Message);
            }
        }
    }

    private void OnIncomingData(ServerClient c, string data) {
        Debug.Log(c.clientName + " : " + data);
    }
    private bool IsConnected(TcpClient c) {
        try
        {
            if (c != null && c.Client != null && c.Client.Connected)
            {
                if (c.Client.Poll(0, SelectMode.SelectRead))
                {
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                }
                return true;
            }
            else {
                return false;
            }
        }
        catch {
            return false;
        }
    }
    public void StartListening() {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
    }
    private void AcceptTcpClient(IAsyncResult ar) {
        TcpListener listener = (TcpListener)ar.AsyncState;
        ServerClient sc = new ServerClient(listener.EndAcceptTcpClient(ar));
        clients.Add(sc);

        StartListening();

        Debug.Log("A Player has connected!");

        Broadcast("", );
    }
}

public class ServerClient {
    public string clientName;
    public TcpClient tcp;

    public ServerClient(TcpClient tcp) {
        this.tcp = tcp;
    }
}
