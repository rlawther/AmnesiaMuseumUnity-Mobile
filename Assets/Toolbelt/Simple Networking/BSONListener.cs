using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using Ionic.Zlib;
namespace Toolbelt {

public class BSONListener {

	private const int HEADER_BIT_COMPRESS = 1 << 0;
	private const int HEADER_BIT_JSONBSON = 1 << 1;
	
	private TCPAsyncListener mTcpListener;

	private int mMsgOffset;
	private bool mPartwayThroughMessage;
	private byte[] mMsg;
	
	// Use this for initialization
	public BSONListener(int port)
    {
		mTcpListener = new TCPAsyncListener(port);
		mTcpListener.StartThread();
		mPartwayThroughMessage = false;
	}
	
	~BSONListener()
	{
		mTcpListener.StopThread();
	}
	
	/*
 	 * Grabs a byte[] from the listener and turns it into a BSONObject
     *
     * Returns null if no messages
     *
	 * ---------------------------------------------------------
	 * | a |   b   |   c   |         d                         |
	 * ---------------------------------------------------------
	 * d => compressed data
	 * c => length of uncompressed data
	 * b => length of c + compressed data (aka compressed data + 4)
	 * a => 1 byte header
	 *
	 */	
	public Kernys.Bson.BSONObject Receive()
	{
        if (mTcpListener.GetMessageCount() == 0)
        {
            return null;
        }
        else
        {
            byte a;
            int b;
            //int c;
            byte [] compressedData;
            byte [] uncompressedData;
			int offset;

			if (!mPartwayThroughMessage)
			{
				mMsgOffset = 0;	
				mMsg = mTcpListener.GetMessage();
			}

			a = mMsg[mMsgOffset];
			b = getInt(mMsg, mMsgOffset + 1);
            //c = getInt(msg, 5);

            if ((a & HEADER_BIT_COMPRESS) != 0)
            {
                //Debug.Log("Compressed");
                compressedData = new byte [b - 4];
				Array.Copy(mMsg, mMsgOffset + 9, compressedData, 0, b - 4);
                uncompressedData = ZlibStream.UncompressBuffer(compressedData);
            }
            else
            {
                //Debug.Log("Not Compressed");
                uncompressedData = new byte [b];
				Array.Copy(mMsg, mMsgOffset + 5, uncompressedData, 0, b);
				//Debug.Log("msg len : " + b + ", " + mMsg.Length + ", " + mMsgOffset);
            }            
            Kernys.Bson.BSONObject obj = Kernys.Bson.SimpleBSON.Load(uncompressedData);

			/* The current message is (b + 5) bytes long */
			if ((mMsgOffset + b + 5) < mMsg.Length)
			{
				mPartwayThroughMessage = true;
				mMsgOffset += b + 5;
			}
			else
			{
				mPartwayThroughMessage = false;
				mMsgOffset = 0;
			}

            return obj;
        }

	}

    /*
     * Returns 4 bytes from a byte[] at the given offset as an int
     */
    private int getInt(byte [] array, int offset)
    {
        int i;

        i = (array[offset + 0] << 24) + 
            (array[offset + 1] << 16) + 
            (array[offset + 2] <<  8) + 
            (array[offset + 3]);

        return i;
    }
	
}
}
