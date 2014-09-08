using UnityEngine;
using System.Collections;
namespace Toolbelt{

public class BinkViewer : ToolbeltManager {
	public string[] binkFiles;
	public Transform sampleQuad;
	private int currentIndex = 0;
	private Transform currentQuad;
	protected override void StartSubclass ()
	{
		this.loadMovie();		
	}
	protected override void UpdateSubclass ()
	{
		if (Input.GetKey(KeyCode.N)) {
			currentIndex++;
			currentIndex %= binkFiles.Length;
			killQuad ();
			loadMovie ();
		} else if (Input.GetKey (KeyCode.B)) {
			currentIndex--;
			currentIndex += binkFiles.Length;
			currentIndex %= binkFiles.Length;
			
			killQuad ();
			loadMovie ();
		}
		
	}
	protected void killQuad() {
		if (currentQuad != null) {
			Destroy (currentQuad.gameObject);
			currentQuad = null;
		}
	}
	protected void loadMovie() {
		
		Transform t = GameObject.Instantiate(this.sampleQuad)  as Transform;
		t.gameObject.SetActive(true);
		
		icBinkMaterial ibm = t.gameObject.AddComponent<icBinkMaterial>();
		ibm.isGameSpeedAgnostic = true;
		ibm.filePath = this.binkFiles[currentIndex];
			
		t.GetComponent<BinkPlayOptions>().movieSpeed = 1.0f;
		this.currentQuad = t;
		t.SetParent(this.transform);
		
	}
}
}