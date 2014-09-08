using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Toolbelt {
//@todo: Threaded possibly?
public enum InputType {
	Tap = 0,
	Swipe = 1,
	Pinch = 2,
	FingerDown = 3,
	FingerUp = 4
}

public interface IiPadInput {
	InputType inputType
	{
		get;
	}
	
}

public class iPadTapInput : IiPadInput {
	public InputType inputType
	{
		get
		{
			return InputType.Tap;
		}
	} 
}

//public class iPadSwipeInput : IiPadInput {
//	public InputType inputType = InputType.Swipe;
//	public Vector2 centre;
//	public float distance;
//}

public class iPadBSONParser {
	
	private enum messageType
	{
		move,
		up,
		down
	}

	public Queue<IiPadInput> messageQueue = new Queue<IiPadInput>();
	private int lastFingers = 0;
	private messageType lastMessage = messageType.move;
	
	public void ParseBSON(Kernys.Bson.BSONObject bsonObj) {
		try {
			if (bsonObj["ipad"]["type"].stringValue == "up") {						
				if (lastMessage == messageType.down) {
					//its a tap!
					if (lastFingers == 1) {
						messageQueue.Enqueue(new iPadTapInput());
					} else {
						//double tap here
					}
							
				}
				lastMessage = messageType.up;
				this.lastFingers = 0;
			} else if (bsonObj["ipad"]["type"].stringValue == "down") {
				
				this.lastFingers = bsonObj["ipad"]["n"].int32Value;			
				lastMessage = messageType.down;
			} else {
				this.lastMessage = messageType.move;
			}
		} catch (KeyNotFoundException) {
			//unhandled keynotfoundexception. e.g. recieving letter inputs.
		}
		//TODO: Parse other types of messages, e.g. swipe, fingerdown, finger up...
	}
	
}

}