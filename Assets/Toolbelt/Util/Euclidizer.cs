//
// Helper functions to place quads in a circle around Avie.
//
using UnityEngine;
using System.Collections;

namespace Toolbelt {

// There are two coordinate systems
// Euclidean, where xyz is the standard cube
// UV, where x is a cylinder with values 0 <= x <= 1, y is height and z is distance from (0,0,0) in euclidean

public static class Euclidizer
{
	private const float  TWO_PI = Mathf.PI * 2.0F;
	public static void AvieToEuclid(Vector3 position, ref Vector3 outVector)
	{
		//fast version, writes the output to outVector
	    float angle = position.x;
	    float length = position.z; //in meters already. The length is the hypotenuse
	    
	    angle = angle % 1.0F; //get angle in range 0.0->1.0
	    angle *= TWO_PI; // degrees to radians
	
	    float euclidx = Mathf.Sin(angle) * length; 
	    float euclidz = Mathf.Cos(angle) * length;
	    
	    outVector.Set (euclidx,position.y,euclidz);
	    
	}
	
	public static Vector3 AvieToEuclid(float x, float y, float z) 
	{
		return AvieToEuclid (new Vector3(x,y,z));
	}
	
	public static Vector3 AvieToEuclid(Vector3 position)		
	{
		// Slow version, creates and returns the result
		Vector3 outVector = new Vector3();
		AvieToEuclid (position,ref outVector);
		return outVector;	    
	}
	
	public static Vector3 EuclidToAvie(float x, float y, float z) {
		return EuclidToAvie (new Vector3(x,y,z));
	}
	
	public static void EuclidToAvie(Vector3 position, ref Vector3 outVector)
	{
		float xlength = position.x;
	    float zlength = position.z;
	    
	    float hypotenuse = Mathf.Sqrt(xlength * xlength + zlength * zlength);
	    float angle = Mathf.Atan2(zlength,xlength);
	    
	    angle += Mathf.PI; //range of angle is -pi to pi...
	    angle /= TWO_PI;
	    angle = 0.75F - angle;
	    angle %= 1.0F;
	    outVector.Set(angle,position.y,hypotenuse);
	    
	}
	
	public static Vector3 EuclidToAvie(Vector3 position)		
	{
		// Slow version, creates and returns the result
		Vector3 outVector = new Vector3();
		EuclidToAvie (position,ref outVector);
		return outVector;	    
	}
	
}
    
}
    
