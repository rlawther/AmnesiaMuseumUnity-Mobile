using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System;

namespace Toolbelt {
public class AVIETrackInterface : MonoBehaviour {

	public int remotePort;
	public string remoteIP;
	public GameObject[] targets;

	private TcpClient tcpClient;
	private NetworkStream clientStream;
	private bool connected = false;
	int reverseInt(int i) {
		#pragma warning disable 0675
		//The operator `|' used on the sign-extended type `int'. Consider casting to a smaller unsigned type first
			
		return (int)(((i & 0x000000ff) << 24) |
			   ((i & 0x0000ff00) << 8) | 
			   ((i & 0x00ff0000) >> 8) | 
			   ((i & 0xff000000) >> 24)); 
		#pragma warning restore 0675
	}

	void swapIntEndianness(byte[] data, int offset)
	{
		byte tmp1, tmp2;

		tmp1 = data[offset];
		tmp2 = data[offset + 1];
		data[offset] = data[offset + 3];
		data[offset + 1] = data[offset + 2];
		data[offset + 2] = tmp2;
		data[offset + 3] = tmp1;
	}

	void swapEndianness(byte[] data, int length)
	{
		int i;

		for (i = 0; i <= (length - 4); i += 4)
			swapIntEndianness(data, i);
	}

	// Use this for initialization
	void Start () {

		Application.runInBackground = true;

		this.tcpClient = new TcpClient();	
		IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(this.remoteIP), this.remotePort);
		try {
			this.tcpClient.Connect(endPoint);
			this.clientStream = tcpClient.GetStream();
			this.connected = true;
		} catch (SocketException e) {
			Debug.Log (e);
		}
		

	}

	void unpackRigidBody(byte[] data, int offset)
	{

		int rbID = BitConverter.ToInt32(data, offset);
		if (targets[rbID] == null)
			return;

		//		int dataType = BitConverter.ToInt32(data, offset + 4);
		float x = (BitConverter.ToSingle(data, offset + 8) / 100.0f) - 5.0f;
		float z = (BitConverter.ToSingle(data, offset + 12) / 100.0f) - 5.0f;
		float y = (BitConverter.ToSingle(data, offset + 16) / 100.0f);

		targets[rbID].transform.position = new Vector3(x, y, z);

		float qx = BitConverter.ToSingle(data, offset + 32);
		float qy = BitConverter.ToSingle(data, offset + 36);
		float qz = BitConverter.ToSingle(data, offset + 40);
		float qw = BitConverter.ToSingle(data, offset + 44);

		targets[rbID].transform.rotation = new Quaternion(qx, -1.0f * qy, -1.0f * qz, qw);
	}
	
	// Update is called once per frame
	void Update () {
		if (!this.connected) return;
		int bytesRead;
		int i;
		int numRigidBodies;
		byte[] buffer = new byte[4096];
		byte[] msg = new byte[4];
		clientStream.Write(msg, 0, msg.Length);
		clientStream.Flush();	

		bytesRead = clientStream.Read (buffer, 0, buffer.Length);
		swapEndianness(buffer, bytesRead);
		//Debug.Log (BitConverter.ToInt32(buffer, 12));

		numRigidBodies = BitConverter.ToInt32(buffer, 12);

		for (i = 0; i < numRigidBodies; i++)
		{
			unpackRigidBody(buffer, 20 + (i * 56));
		}

	}
}
}
