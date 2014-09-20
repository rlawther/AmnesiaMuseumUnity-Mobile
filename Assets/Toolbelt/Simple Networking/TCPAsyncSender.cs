using UnityEngine;
using System.Collections.Generic;
using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;

using System.Text;

namespace Toolbelt {
/***
 * General class for sending strings through TCP
 */
public class TCPAsyncSender : ThreadedClass {
	
	private int remotePort;
	private string remoteIP;
	
	private TcpClient tcpClient;
	
	private string remoteString;
	
	private Queue<byte[]> messageQueue = new Queue<byte[]>();
	private object messageLock = new object();

	private int state;
	
	public TCPAsyncSender(string ip, int port)
	{
		remotePort = port;
		remoteIP = ip;
		
		this.tcpClient = new TcpClient();	
		this.remoteString = remoteIP + ":" + remotePort;
	}
	
	public void AddMessage(byte[] msg) {
		lock (messageLock) {
			this.messageQueue.Enqueue(msg);
		}
	}
	
	public bool isConnected()
	{
		if (state == 1)
			return true;
		else
			return false;
	}

	protected override void ThreadFunction ()
	{
		Debug.Log ("Attempting connection to " + this.remoteString);
		//IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(this.remoteIP), this.remotePort);

		// Will keep running until told to stop
		while (!this.stopRequested)
		{
			state = 0;
			var result = this.tcpClient.BeginConnect(this.remoteIP, this.remotePort, null, null);
			var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));
		
			if (!success)
			{
				Debug.Log ("Could not connect to " + this.remoteString + ". Retry in 5 seconds");
				Thread.Sleep(5000);
				continue;
			}
		
			// we have connected
			this.tcpClient.EndConnect(result);

			Debug.Log ("Connected to " + remoteString);
			state = 1;
			
			NetworkStream clientStream = tcpClient.GetStream();
			Socket sock = tcpClient.Client;
			sock.NoDelay = true;

			while (!this.stopRequested)
			{
				lock (messageLock)
				{
					// Grab messages from queue and send them
					while (this.messageQueue.Count > 0)
					{
						//Debug.Log ("async send");
						byte[] msg = this.messageQueue.Dequeue();
						
						/* FIXME : catch here and retry connect? */
						clientStream.Write(msg, 0, msg.Length);
						clientStream.Flush();	
					}
				}
			}
				
			Thread.Sleep(16);
		}
			
		Debug.Log ("Stopping connection to " + this.remoteString);
	}
}
}