using UnityEngine;
using System.Collections;

namespace Toolbelt {
abstract public class ClickHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	protected abstract void itemHit(RaycastHit hit); //override with behaviour when something hits.
	protected abstract void itemHover(RaycastHit hit); //override with behaviour when user hovers over something.
	protected abstract void itemMiss(); //override with behaviour when user clicks nothing.
	protected abstract void itemUnhover(); //override with behaviour when user hovers nothing.
	
	void Update()
	{		
	    //empty RaycastHit object which raycast puts the hit details into
        RaycastHit hit;
        //ray shooting out of the camera from where the mouse is
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);	 		
        if (Physics.Raycast(ray, out hit))
        {
			if (Input.GetMouseButtonDown(0))
				this.itemHit (hit);
			else 
				this.itemHover(hit);
        } else {
			if (Input.GetMouseButtonDown (0)) {				
				this.itemMiss();
			} else {
				this.itemUnhover ();
			}
		}
	    
	}
}
}