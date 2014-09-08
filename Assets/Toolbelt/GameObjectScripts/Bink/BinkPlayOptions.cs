using UnityEngine;
using System.Collections;

namespace Toolbelt {
public class BinkPlayOptions : MonoBehaviour {
	//A neat interface between icBinkMaterial and BinkPlugin, since it is difficult to save
	//the data of BinkPlugin.
	public bool alphaChannel = true;
	public float movieSpeed = 0.0f;	
	public float movieTime = 0f;	
	public bool loopMovie = true; 
	public bool canCatchup = false;	// render 2 frames when behind.
	public bool smartRender = true; // Do not render whenever video is not visible
	public float playSpeed = 1.0f; //temp variable so you can store what you want to play at
	
	// will not start playing until this is <= 0. Will only count down if movieSpeed > 0
	public float playAfterSeconds = 0.0f; 
}
}
