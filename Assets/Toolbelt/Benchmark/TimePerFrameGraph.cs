using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimePerFrameGraph : MonoBehaviour {
	public int numPoints = 180;
	public float maxMilliseconds = 100;
	// Use this for initialization
	private ParticleSystem.Particle[] points;
	
	void Start () {
		this.numPoints = (int)Mathf.Clamp(this.numPoints,10,500);
		points = new ParticleSystem.Particle[numPoints];
		
		
		for (int i = 0; i < numPoints; i++) {
			ParticleSystem.Particle part = new ParticleSystem.Particle();
			part.position = new Vector3((i+1)*this.transform.localScale.x/numPoints,0f,0f);
			part.color = Color.red;
			part.size = 1.0f/numPoints * this.transform.localScale.x* 10.0f;
			points[i] = part;
		}
	}
	
	private void Cycle() {
		
		
		for (int i = 0; i < numPoints - 1; i++) {
			Vector3 oldpos = this.points[i].position;
			oldpos.y = this.points[i+1].position.y;
			this.points[i].position = oldpos;
			this.points[i].color = this.points[i+1].color;
			
		}
		
	}
	
	void AddToEnd(float timeInMilliseconds) {
		ParticleSystem.Particle input = this.points[this.points.Length - 1];
		input.position = new Vector3(this.transform.localScale.x,
									timeInMilliseconds/maxMilliseconds*this.transform.localScale.y,
									0f);
		input.color = new Color(timeInMilliseconds/this.maxMilliseconds,1.0f,0.0f,1.0f);
		input.size = 1.0f/numPoints* this.transform.localScale.x;		
		this.points[this.points.Length - 1] = input;
	}
	// Update is called once per frame
	void Update () {
				
		this.Cycle ();
		this.AddToEnd (Time.deltaTime*1000f);
		particleSystem.SetParticles(this.points,this.points.Length);
	}
}
