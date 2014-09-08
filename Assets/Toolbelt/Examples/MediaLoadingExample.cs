using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Toolbelt;

/*** Examples of how you can load images and Bink movies through Toolbelt
 * 
 * */ 
public class MediaLoadingExample : ToolbeltManager 
{
	public string myImageUrl;
	public string imageUrl;
	public string binkUrl;
	public string dupImageUrl;
	
	protected GameObject myImageCube;
	protected GameObject imageQuad;
	protected GameObject binkQuad;
	
	protected List<GameObject> duplicateQuads = new List<GameObject>();
	
	int counter = 0;
	
	// Use this for initialization
	protected override void StartSubclass()
	{
		Debug.Log("1. Making my own GameObject and putting an icImageMaterial on it with image: " + myImageUrl);
		
		// Make a standard cube that starts off transparent
		myImageCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		myImageCube.renderer.material.SetColor( "_Color", new Color(1.0f, 1.0f, 1.0f, 0.0f) );
		myImageCube.renderer.material.shader = Shader.Find ("Transparent/Diffuse");
		
		// Parent it to this ToolbeltManager
		this.ParentToMe(myImageCube);
		
		// Put an icImageMaterial on it
		icMediaLoader icml = this.MediaLoader;
		
		icImageMaterial myImageMat = icml.CreateImageMaterial(myImageUrl, myImageCube);
		
		// Give it a mediaReady callback so it fades to white after the image loads.
		FadeColour fadeScript = myImageMat.CreateMediaReadyCallback<FadeColour>();
		fadeScript.targetColour = Color.white;
		fadeScript.duration = 2.0f;
		
		myImageCube.transform.localPosition = new Vector3(-2.0f, 0.0f, 5.0f);
		myImageCube.transform.Rotate(0, 90, 0);
		
		/////////////////////////////////////////////////////
		// CreateImageQuad() and CreateBinkQuad() are convenience functions
		// that create the quad for you with the appropriate icMaterial,
		// with the quad already parented to the ToolbeltManager
		Debug.Log("2. Making image quad with image: " + imageUrl);
		imageQuad = this.CreateImageQuad(imageUrl);
		imageQuad.transform.localPosition = new Vector3(-1.0f, 0.0f, 5.0f);
		AddImageShaderScript(imageQuad);
		
		Debug.Log ("3. Making bink quad with file: " + binkUrl);
		binkQuad = this.CreateBinkQuad(binkUrl);
		binkQuad.transform.localPosition = new Vector3(1.0f, 0.0f, 5.0f);
		
		Debug.Log ("4. Making multiple quads with same image to show image caching");
		// Attach them all to a parent
		GameObject dupParent = new GameObject("dupParent");
		dupParent.transform.parent = this.gameObject.transform;

		// Give the empty GameObject an icMaterial so it can be used for colour parenting
		dupParent.AddComponent<icMaterial>();

		const int count = 12;
		float step = 10.0f / count;
		
		for (int i = 0; i < count; ++i)
		{
			GameObject q = this.CreateImageQuad(dupImageUrl);
			q.name = "Dup Quad " + i;
			q.transform.localPosition = new Vector3(-4.0f + i * step, -1.0f, 6.0f);
			q.transform.parent = dupParent.transform;
			AddImageShaderScript(q);
			duplicateQuads.Add(q);
		}
	}
	
	protected override void UpdateSubclass ()
	{
		// A few examples of how to use MovieSpeed and MovieTime
		++counter;
		
		// Start playing after 60 frames have passed
		if (counter == 60) {
			binkQuad.GetComponent<icBinkMaterial>().getBinkPlayOptions().movieSpeed = 1.0f;
		}
		
		// Skip to 200 seconds after 180 frames have passed
		if (counter == 180) {
			BinkPlayOptions bpo = binkQuad.GetComponent<icBinkMaterial>().getBinkPlayOptions();
			bpo.loopMovie = true;
			bpo.movieTime = 200.0f;
		}
	}
	
}
