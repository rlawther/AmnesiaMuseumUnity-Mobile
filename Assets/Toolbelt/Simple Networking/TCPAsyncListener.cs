using UnityEngine;
using System.Collections.Generic;

using System.Net.Sockets;
using System.Net;
using System.Threading;

using System.Text;
using System;
namespace Toolbelt {

/***
 * General class for sending strings through TCP
 */
public class TCPAsyncListener : ThreadedClass {
	
	private int listenPort;
	private string remoteIP;
	
	private TcpClient tcpClient;
    private TcpListener tcpListener;
    private NetworkStream networkStream;
    private bool gotConnection = false;
	
	private string remoteString;
	
	private Queue<byte[]> messageQueue = new Queue<byte[]>();
	private object messageLock = new object();

    private int bufferSize = 4096;
    private byte[] buffer;
	
	public TCPAsyncListener(int port)
	{
		listenPort = port;
		Debug.Log("Starting listener on port " + port);
			
		this.tcpListener = new TcpListener(IPAddress.Any, listenPort);	
        this.tcpListener.Start();
        Debug.Log("Started listener on port " + port);
	}

    public int GetMessageCount()
    {
        return this.messageQueue.Count;
    }
	
	public byte[] GetMessage()
    {
        lock (messageLock)
        {
            if (this.messageQueue.Count > 0)
                return this.messageQueue.Dequeue();
            else
                return null;
        }
	}
	
	protected override void ThreadFunction ()
	{
        int bytesRead;
        buffer = new byte[bufferSize];
        byte [] newBuffer;

        //Debug.Log("listener thread");
		while (!this.stopRequested)
        {
            if ((!gotConnection) && (this.tcpListener.Pending()))
            {
                //Debug.Log("pending");
                tcpClient = this.tcpListener.AcceptTcpClient();
                networkStream = tcpClient.GetStream ();
                gotConnection = true;
            }
            else if ((gotConnection) && networkStream.DataAvailable)
            {
                bytesRead = networkStream.Read(buffer, 0, bufferSize);
    			//Debug.Log ("read " + bytesRead);
                //Debug.Log(System.Text.Encoding.UTF8.GetString(buffer));
                newBuffer = new byte[bytesRead];
                Array.Copy(buffer, 0, newBuffer, 0, bytesRead);
                lock (messageLock)
                {
                    this.messageQueue.Enqueue(newBuffer);
                }
            
            }
            //Debug.Log("sleeping");
			Thread.Sleep(16);
            
        }
        //Debug.Log("listener thread ending");
	}
}
}