using UnityEngine;
using System.Collections.Generic;

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
	
	protected override void ThreadFunction ()
	{
		Debug.Log ("Attempting connection to " + this.remoteString);
		IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(this.remoteIP), this.remotePort);
		this.tcpClient.Connect(endPoint);
		
		Debug.Log ("Connected to " + remoteString);
		
		NetworkStream clientStream = tcpClient.GetStream();
		
		// Will keep running until told to stop
		while (!this.stopRequested)
		{
			lock (messageLock)
			{
				// Grab messages from queue and send them
				while (this.messageQueue.Count > 0)
				{
					byte[] msg = this.messageQueue.Dequeue();
					
					clientStream.Write(msg, 0, msg.Length);
					clientStream.Flush();	
				}
			}
			
			Thread.Sleep(16);
		}
		
		Debug.Log ("Stopping connection to " + this.remoteString);
	}
}
}