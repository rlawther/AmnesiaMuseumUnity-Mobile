using UnityEngine;

//Utility functions
namespace Toolbelt {
public class Util : MonoBehaviour {
	public static byte randomByte() {
		return (byte)(Random.value * 255);
	}
	public static Color32 getRandomColour() {
		return new Color32(randomByte(),
						   randomByte(),
						   randomByte(),
						   0xFF);
	}
	
}
}