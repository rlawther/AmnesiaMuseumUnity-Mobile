using UnityEngine;
using System.Collections;
namespace Toolbelt {
public class RefreshTextureScript : icRenderPass {
	public Texture toReplace = null;
	public override void RenderPass ()
	{
		this.renderer.material.mainTexture = toReplace;
	}
	
	protected override void DoAwake() {
	
	}
	
}
}