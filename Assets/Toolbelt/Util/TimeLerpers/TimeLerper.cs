using UnityEngine;
using System.Collections;

/*** Base class for anything you want to do an object over a set duration.
 * To use simply add to it an object with .enabled = false, and enable it
 * when you want it to start
 * Serves the same purpose as 'Animations' in Forge/Blacksmith
 * */
namespace Toolbelt {
abstract public class TimeLerper : MonoBehaviour {
	
	public float duration = 0.0f;
	public float startTime = 0.0f;
	public bool removeWhenFinished = false;
	
	protected float inverseDuration;
	protected float curTimer = 0.0f;
	protected bool bFinished = false;
	
	protected icMaterial attachedIcMat;
	
	void Awake() {
		attachedIcMat = this.GetComponent<icMaterial>() as icMaterial;
	}
	
	// Use this for initialization
	void Start () {
		if (this.duration != 0.0f) {
			this.inverseDuration = 1.0f / this.duration;
		}
		this.curTimer = this.startTime;
		this.bFinished = false;
		
		SetStart();
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Simply don't do anything if finished
		if (!this.bFinished)
		{
			this.curTimer += Time.deltaTime;
			
			if (this.curTimer < 0.0f) {
				return;
			} else if (this.curTimer >= this.duration) 
			{
				this.bFinished = true;
				DoUpdate(1.0f);	
				if (removeWhenFinished)
					Destroy(this);
				
			} else {
				float t = this.curTimer * this.inverseDuration;
				DoUpdate(t);
			}
		}
	}
	
	// Always disabled by default
	void Reset() 
	{
		ResetTimer();
		this.duration = 0.0f;
		this.startTime = 0.0f;
	}
	
	/// Stop and reset timer
	public void ResetTimer() 
	{
		this.enabled = false;
		this.inverseDuration = 0.0f;
		this.curTimer = 0.0f;
		this.bFinished = false;		
	}
	
	public bool IsFinished() {
		return this.bFinished;
	}
	
	protected abstract void SetStart();
	protected abstract void DoUpdate(float t);
}
}