using UnityEngine;
using System.Collections;
public static class ZBufferConversion {

//http://www.sjbaker.org/steve/omniv/love_your_z_buffer.html
	public static float metresToRangeZBufferValue01(int bits, float zNear, float zFar, float distance) 
	{
		// z_buffer_value = (1<<N) * ( a + b / z )		
		//	Where:
		//			
		//			N = number of bits of Z precision
		//				a = zFar / ( zFar - zNear )
		//				b = zFar * zNear / ( zNear - zFar )
		//				z = distance from the eye to the object
		//				
		//				...and z_buffer_value is an integer.
		if (distance < zNear) {
			return 0.0f;
		} else if (distance > zFar) {
			return 1.0f;
		}
		double a = zFar / (zFar - zNear);
		double b = zFar * zNear / (zNear - zFar);
		uint z_buffer_value =(uint)( (1 << bits) * (a + b / distance));
		//now put z_buffervalue between 0 and 1...
		float result = ((float)z_buffer_value / (1 << bits));
		Debug.Log(distance);
		Debug.Log (result);
		return result;
	}
	public static float metresToZBufferValue(Camera c, float distance) {
		//returns value from 0 to 1. 0 = nearclipplane, 1 = farclipplane. Linearly interpolated.
		float result = c.WorldToViewportPoint((distance-c.nearClipPlane) * c.transform.forward + c.transform.position).z / (c.farClipPlane-c.nearClipPlane);		
		return result;
	}
}

