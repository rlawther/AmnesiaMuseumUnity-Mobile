using UnityEngine;
using System.Collections;

public class DepthSceneExample : MonoBehaviour {
	
	public RenderTexture depthTexture;
	
	public Camera camera;
	public Shader depthToColourShader;
	public Material fullScreenMat;
	
	protected Material depthToColourMat;

	
	void Start()
	{	
		this.camera.enabled = false;
		this.camera.targetTexture = new RenderTexture((int)this.camera.pixelWidth, (int)this.camera.pixelHeight, 0);
		
		if (!this.depthToColourShader) {
			this.depthToColourShader = Shader.Find("Custom/DepthToColour");
		}
		
		this.depthToColourMat = new Material(this.depthToColourShader);
	}	
	
	// Update is called once per frame
	void Update () 
	{
		if (camera) {
			this.camera.depthTextureMode = DepthTextureMode.Depth;
			if (!this.depthTexture) {
				this.depthTexture = new RenderTexture((int)this.camera.pixelWidth, (int)this.camera.pixelHeight, 0);
			}

			this.camera.Render();
			
			// Blit the depth onto the destination texture.
			Graphics.Blit(null, this.depthTexture, this.depthToColourMat);
		}
	}

	void OnGUI()
	{
		// It seems this is the only place where we can manually render
		// something to screen.
		if (Event.current.type == EventType.Repaint) {
			RenderTexture old = RenderTexture.active;
			RenderTexture.active = this.camera.targetTexture;
			Graphics.DrawTexture(new Rect(0, 0, this.camera.aspect * 100, 100), this.depthTexture);	
			Graphics.Blit (this.camera.targetTexture, null, this.fullScreenMat);
			RenderTexture.active = old;
		}
	}
}
