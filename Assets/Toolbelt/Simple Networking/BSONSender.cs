using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Toolbelt {
public class BSONSender {
	
	private TCPAsyncSender tcpSender;
	
	// Use this for initialization
	public BSONSender(string ip, int port) {
		tcpSender = new TCPAsyncSender(ip, port);
		
		tcpSender.StartThread();
	}
	
	~BSONSender()
	{
		tcpSender.StopThread();
	}
	
	
	/*
	* Turns BSONObj into an array of bytes ready to transport.
	* -------------------------------------------------
	* | a |   b   |         c                         |
	* -------------------------------------------------
	* c => compressed data
	* b => length of uncompressed data
	* a => 1 byte header
	*
	*/	
	public void SendUncompressed(Kernys.Bson.BSONObject bsonObj)
	{
		byte[] raw = Kernys.Bson.SimpleBSON.Dump(bsonObj);
		//byte[] compressed = ZlibStream.CompressBuffer(raw);
		
		List<byte> b = intToByteString(raw.Length);
		b.AddRange(raw);
		
		List<byte> a = new List<byte>();
		a.Add(2);
		a.AddRange(b);
		
		//		string deb = "";
		//		foreach (byte oneB in a) {
		//			deb += oneB + " ";
		//		}
		//		Debug.Log (deb);
			
		tcpSender.AddMessage(a.ToArray());
			
	}
	
	private List<byte> intToByteString(int paramInt)
	{
		byte[] array = System.BitConverter.GetBytes(paramInt);
		
		// Our BSON interface expects big endian
		if (System.BitConverter.IsLittleEndian)
			System.Array.Reverse(array);

		return new List<byte>(array);
	}	
}
}