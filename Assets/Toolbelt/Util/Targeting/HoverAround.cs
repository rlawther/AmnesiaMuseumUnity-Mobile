using UnityEngine;
using System.Collections;

namespace Toolbelt 
{
public class HoverAround : MonoBehaviour {
	public Transform toFollow;
	private Vector3 leaderLastPosition;
	private Quaternion leaderLastRotation;
	
	public float positionSpeed = 1.0f;
	public float rotationSpeed = 1.0f;
	public float hoverSpeed = 0.01f;
	public Vector3 positionOffset;
	public Vector3 targetLookAt;
	public Vector3 rotationOffset;
	
	
	// Use this for initialization
	public enum HoverState {
		FOLLOWING,
		HOVERING		
	}
	
	public HoverState currentState = HoverState.FOLLOWING;
	
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.checkLeaderChanged();	
		if (this.currentState == HoverState.FOLLOWING) {
			this.followLeader();
		} else {
			this.hover();
		}
	}
	
	private void hover() {
		
		Vector3 target = toFollow.position + positionOffset;
		target += new Vector3(Random.Range(-0.2f,0.2f),Random.Range (-0.2f,0.2f),Random.Range (-0.2f,0.2f));
		this.transform.position = Vector3.MoveTowards(this.transform.position,target,this.hoverSpeed);
	}
	
	private void followLeader() {
		Transform t = this.transform;
		t.position = Vector3.MoveTowards(t.position,toFollow.position + positionOffset,positionSpeed);		
		t.LookAt(targetLookAt);
		t.Rotate (rotationOffset);
		if (t.position == toFollow.position + positionOffset) {
			this.currentState = HoverState.HOVERING;
		}
		
	}
	private void checkLeaderChanged() {
		
		if (this.leaderLastPosition != toFollow.position || 
			this.leaderLastRotation != this.toFollow.rotation) {
			this.leaderLastPosition = toFollow.position;
			this.leaderLastRotation = toFollow.rotation;
			this.currentState = HoverState.FOLLOWING;
		}
	}
	
}
}