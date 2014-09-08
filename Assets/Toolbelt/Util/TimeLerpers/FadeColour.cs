using UnityEngine;
using System.Collections;
namespace Toolbelt {
/*** Simple TimeLerper for fading to a colour
 * */
public class FadeColour : TimeLerper {
	
	public Color targetColour;
	
	protected Color startColour;
	
	// Use this for initialization
	protected override void SetStart ()
	{
		if (attachedIcMat)
			startColour = attachedIcMat.icColour;
	}
	
	// Update is called once per frame
	protected override void DoUpdate (float t)
	{
		if (attachedIcMat) {
			attachedIcMat.icColour = Color.Lerp ( startColour, targetColour, t );
		}
	}

}
}