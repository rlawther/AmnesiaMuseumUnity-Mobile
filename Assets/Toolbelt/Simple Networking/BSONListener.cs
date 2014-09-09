using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Toolbelt {

public class BSONListener {

	private const int HEADER_BIT_COMPRESS = 1 << 0;
	private const int HEADER_BIT_JSONBSON = 1 << 1;
	
	private TCPAsyncListener tcpListener;
	
	// Use this for initialization
	public BSONListener(int port)
    {
		tcpListener = new TCPAsyncListener(port);
		tcpListener.StartThread();
	}
	
	~BSONListener()
	{
		tcpListener.StopThread();
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
        byte[] msg;

        if (tcpListener.GetMessageCount() == 0)
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

            msg = tcpListener.GetMessage();

            a = msg[0];
            b = getInt(msg, 1);
            //c = getInt(msg, 5);

			/* FIXME : assumes exactly one BSON object per packet - can't do this */

            if ((a & HEADER_BIT_COMPRESS) != 0)
            {
                Debug.Log("Compressed packet - skipping (Android can't use zlib DLL)");
				return null;
            }
            else
            {
                //Debug.Log("Not Compressed");
                uncompressedData = new byte [b];
                Array.Copy(msg, 5, uncompressedData, 0, b);
            }            
            Kernys.Bson.BSONObject obj = Kernys.Bson.SimpleBSON.Load(uncompressedData);


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
