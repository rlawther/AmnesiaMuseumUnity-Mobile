using UnityEngine;
using System.Collections;
namespace Toolbelt {
/*** Script for easier use of cyli-quads relative to the camera
 * All screen-space positioning and scaling is done as if the cylinder is 
 * at radius 1 away from the camera
 * */
public class icScreenSpaceCyli : MonoBehaviour {

	public Vector3 screenSpacePosition;
	private Vector3 prevScreenSpacePosition;

	public Vector2 screenSpaceScale;
	private Vector2 prevScreenSpaceScale;

	private Vector3 positioningScale;

	public bool parentToCamera = true;

	// Use these tessellation parameters if you want to keep the same density
	// of vertex tessellation regardless of cylinder size
	public float tessellationPerCircle = 0;
	private float prevTessellationPerCircle = 0;

	public float tessellationPerY = 0;
	private float prevTessellationPerY = 0;

	CyliQuadMaker cyliMaker;

	void Awake()
	{
		// Make a CyliQuadMaker if there isn't one
		cyliMaker = GetComponent<CyliQuadMaker>();
		if (!cyliMaker) {
			cyliMaker = this.gameObject.AddComponent<CyliQuadMaker>();
		}
	}

	void Start()
	{
		if (this.parentToCamera) {
			// Set it to null for now so we can parent it to the camera on the first Update()
			this.transform.parent = null;
		}

		this.UpdatePositionFromSS();
	}
		
	void Update () 
	{
		if (this.transform.parent == null && this.parentToCamera) {
			// Parent to camera. Can't do it on Start() because the camera may not
			// be contructed yet
			this.transform.parent = ToolbeltManager.FirstInstance.GetCameraTransform();
			this.UpdatePositionFromSS();
		}

		if ( this.tessellationPerCircle != this.prevTessellationPerCircle ) {
			this.UpdateXTessellation();
			this.prevTessellationPerCircle = this.tessellationPerCircle;
		}

		if ( this.tessellationPerY != this.prevTessellationPerY ) {
			this.UpdateYTessellation();
			this.prevTessellationPerY = this.tessellationPerY;
		}

		if (this.screenSpacePosition != this.prevScreenSpacePosition) {
			UpdatePositionFromSS();
		} else if (this.screenSpaceScale != this.prevScreenSpaceScale) {
			// UpdatePositionFromSS() already calls UpdateScaleFromSS()
			UpdateScaleFromSS();
		}
	}

	protected void UpdateXTessellation() {
		if (this.tessellationPerCircle > 0) {
			this.cyliMaker.SetMeshTessellationX( Mathf.CeilToInt( this.tessellationPerCircle * this.screenSpaceScale.x ) );
		}
	}

	protected void UpdateYTessellation() {
		if (this.tessellationPerY > 0)
			this.cyliMaker.SetMeshTessellationY( Mathf.CeilToInt( this.tessellationPerY * this.screenSpaceScale.y ) );
	}

	protected void UpdatePositionFromSS()
	{
		float radius = this.screenSpacePosition.z;

		// Cylinder origin is always the center of the cylinder
		// All y positions are as if the cylinder was at radius 1, so we multiply by radius
		this.transform.localPosition = new Vector3(0, this.screenSpacePosition.y * radius, 0);

		// Screen space x position is just a rotation
		this.transform.eulerAngles = new Vector3(0, this.screenSpacePosition.x * 360.0f, 0);

		// Because origin is the center, 
		// scaling is used to make the cylinder further away
		this.positioningScale.Set(radius, radius, radius);

		this.prevScreenSpacePosition = this.screenSpacePosition;

		this.UpdateScaleFromSS();
	}

	protected void UpdateScaleFromSS()
	{
		this.screenSpaceScale.x = Mathf.Clamp01(this.screenSpaceScale.x);

		// Percentage of circle is just x scale
		this.cyliMaker.percentageOfCircle = this.screenSpaceScale.x;

		this.transform.localScale = new Vector3(this.positioningScale.x, 
		                                        this.positioningScale.y * this.screenSpaceScale.y,
		                                        this.positioningScale.z);

		this.UpdateXTessellation();
		this.UpdateYTessellation();
		this.prevScreenSpaceScale = this.screenSpaceScale;
	}
	
}
}