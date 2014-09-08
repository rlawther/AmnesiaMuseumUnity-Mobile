using UnityEngine;
using System.Collections;

namespace Toolbelt {

/**
 * 
 * Note: When rotating this object in editor, make sure "Pivot" and "global" options are set, 
 * as opposed to "Center" and "local"
 * 
 * To rotate (like in forge), use this.transform.Rotate(Vector3.up*amount);
 * 
 **/
public class CyliQuadMaker : MonoBehaviour, icDebugGUI {
	public Vector2[] uvs;
	public Vector3[] vertices;
	public Vector3[] normals;
	
	public int[] triangles;
	
	private float _percentageOfCircle = 1.0f;
	public float percentageOfCircle = 1.0f;
	public bool realtimeUpdate = true;
	
	
	public int MESH_GRANULARITY_X = 40;
	public int MESH_GRANULARITY_Y = 5;

	protected bool requireRecalculate = false;
	protected Mesh originalMesh = null;
	
	protected icGUIHelpers.FloatPicker percPicker;
	// Enable/disable script to toggle between cyliquad and previous mesh.
	
	void OnEnable() {
		MeshFilter mf = GetComponent<MeshFilter>();
		if (mf != null) {
			this.originalMesh = mf.mesh;
		}
		this.requireRecalculate = true;
	}
	public void OnDisable() {
		this.restoreOriginalMesh();
	}
	public void Awake() {
		this.percPicker = new icGUIHelpers.FloatPicker("Percentage of Circle", 0f, 1f);
	}
	public void restoreOriginalMesh() {
		MeshFilter currentMeshFilter = this.GetComponent<MeshFilter>();				
		currentMeshFilter.mesh = originalMesh;
	}
	// Update is called once per frame
	void Update () 
	{
		if (this.realtimeUpdate) 
		{
			if (_percentageOfCircle != percentageOfCircle) 
			{
				_percentageOfCircle = percentageOfCircle;
				this.requireRecalculate = true;
			}
		}

		if (this.requireRecalculate) {
			this.RecalculateMesh();
		}
	}
	
	void setPercentageOfCircle(float newPerc) {
		this.percentageOfCircle = newPerc;		
	}
	
	public void SetMeshTessellationX(int x) 
	{
		int newGran = 1 + Mathf.Max(1, x);
		if (newGran != this.MESH_GRANULARITY_X)
		{
			this.MESH_GRANULARITY_X = newGran;
			this.requireRecalculate = true;
		}
	}

	public void SetMeshTessellationY(int y)
	{
		int newGran = 1 + Mathf.Max(1, y);
		if (newGran != this.MESH_GRANULARITY_Y)
		{
			this.MESH_GRANULARITY_Y = newGran;
			this.requireRecalculate = true;
		}
	}
	
	
	private void calculateVerticesAndUVs() {
		this.vertices = new Vector3[MESH_GRANULARITY_X * MESH_GRANULARITY_Y];
		this.uvs = new Vector2[MESH_GRANULARITY_X*MESH_GRANULARITY_Y];
		int i = 0;
		for (int y = 0; y < MESH_GRANULARITY_Y; y++) {
			float yVal = (float)(y)/(MESH_GRANULARITY_Y-1); //-1.0f->1.0F			
			for (int j = 0; j < MESH_GRANULARITY_X; j++) {
				float amount;
				if (j == MESH_GRANULARITY_X - 1 && percentageOfCircle >= 1.0f) {
					amount = 0; //deal with wraparound
				} else {
					amount = (float)j*percentageOfCircle/(MESH_GRANULARITY_X-1);	
				}
				float xVal = Mathf.Sin (amount*Mathf.PI*2.0F) ;
				float zVal = Mathf.Cos (amount*Mathf.PI*2.0F) ;
				this.vertices[i] = new Vector3(xVal,
											   yVal-0.5f, //centre from -0.5f to 0.5f;
											   zVal);	
				this.uvs[i] = new Vector2((float)j/(MESH_GRANULARITY_X-1),yVal);
				i++;
			}
		}
	}
	private void calculateNormals() {
		this.normals = new Vector3[MESH_GRANULARITY_X * MESH_GRANULARITY_Y];
		for (int i = 0; i < MESH_GRANULARITY_X*MESH_GRANULARITY_Y; i++) {
			this.normals[i] = -this.vertices[i].normalized;
		}
		
	}
	private void calculateTriangles() {
		this.triangles = new int[(MESH_GRANULARITY_X * MESH_GRANULARITY_Y * 3 * 2)];
		int i = 0;
		int meshx = MESH_GRANULARITY_X - 1;
		int meshy = MESH_GRANULARITY_Y - 1;
		int curry = 0;
		int currx = 0;
		for (curry = 0; curry < meshy; curry++)
		{
			for (currx = 0; currx < meshx; currx++)
			{
					int currentRow = curry * (MESH_GRANULARITY_X);
					int nextRow = (curry + 1) * (MESH_GRANULARITY_X);
				
					int currentCol = currx;
					int nextCol = currx + 1;
				
					triangles[i+2] = currentRow + currentCol;
					triangles[i+1] = currentRow + nextCol;
					triangles[i+0] = nextRow + currentCol;
					
					triangles[i+5] = currentRow + nextCol;
					triangles[i+4] = nextRow + nextCol;
					triangles[i+3] = nextRow + currentCol;
					i+=6;

				
				
			}			
			
		}
		
		
	}
	private void calculateUVsGrid()
	{
		//simplest UVs. They are a grid, but its wrong.
		this.uvs = new Vector2[MESH_GRANULARITY_X*MESH_GRANULARITY_Y];
		int i = 0;
		for (int y = 0; y <= MESH_GRANULARITY_Y;y++) 
		{
			float yVal = y/(float)(MESH_GRANULARITY_Y);
			for (int x = 0; x < MESH_GRANULARITY_X; x++) {
				float xVal = x/(float)MESH_GRANULARITY_X;
				this.uvs[i] = new Vector2(xVal,yVal);
				i++;
			}
		}
	}
		

	protected void RecalculateMesh() 
	{
		MeshFilter mf = GetComponent<MeshFilter>();
		if (!mf) {
			mf = this.gameObject.AddComponent<MeshFilter>();
		}
			
		
		if (mf.mesh == this.originalMesh) {
			mf.mesh = new Mesh();
		}
		Mesh mesh = mf.mesh;

		mesh.Clear();
		
		this.calculateVerticesAndUVs();
		this.calculateNormals();
		
		this.calculateTriangles();		
		//this.calculateUVsGrid();

		mesh.vertices = this.vertices;				
		mesh.triangles = this.triangles;
		mesh.normals = this.normals;
		mesh.uv = this.uvs;
		mesh.RecalculateBounds();
		
		
		
	}
	public void DrawDebugGUI() {
		GUILayout.Label ("Enable/disable script to enable/disable cylinder");	
		this.percentageOfCircle = this.percPicker.DrawGUI(this.percentageOfCircle);
		
	}

}

}