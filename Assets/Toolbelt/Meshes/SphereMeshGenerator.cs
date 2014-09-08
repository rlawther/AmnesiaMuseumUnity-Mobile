using UnityEngine;
using System.Collections;
using System;
using System.IO;
//using UnityEditor;
namespace Toolbelt {
public class SphereMeshGenerator : MonoBehaviour {
	public Vector2[] uvs;
	public Vector3[] vertices;
	public Vector3[] normals;
	public int[] triangles;	
	public float width = 1.0F/2.0F;
	private float _width = 1.0F/2.0F;
	public Vector2 UV_range = new Vector2(0.0f,1.0f);
	private Vector2 _UV_range = new Vector2(0.0f,1.0f);
	
	private const int MESH_GRANULARITY_X = 250;
	private const int MESH_GRANULARITY_Y = 250;
	// Use this for initialization
	void Start () {
		this.RecalculateMesh();
//		UnityEditor.AssetDatabase.CreateAsset(this.GetComponent<MeshFilter>().mesh, "Assets/SimpleSphere_250_250.asset");
	}
	
	void calculateVertices() {
		this.vertices = new Vector3[MESH_GRANULARITY_X * (MESH_GRANULARITY_Y+1)];
		
		int i = 0;
		for (int y = 0; y <= MESH_GRANULARITY_Y; y++) {
			float yVal = (y - MESH_GRANULARITY_Y/2.0F)/MESH_GRANULARITY_Y*2.0f; //-1.0f->1.0F
			float radius = Mathf.Sqrt(1 - yVal*yVal);
			for (int j = MESH_GRANULARITY_X; j > 0; j--) {
				float amount = (float)j/MESH_GRANULARITY_X; //from 0 -> 1;				
				float xVal = Mathf.Sin (amount*Mathf.PI*2.0F) * radius;
				float zVal = Mathf.Cos (amount*Mathf.PI*2.0F) * radius;
				this.vertices[i] = new Vector3(xVal,yVal,zVal);	
				i++;
			}
		}
	}
	void calculateNormals() {
		this.normals = new Vector3[MESH_GRANULARITY_X * (MESH_GRANULARITY_Y+1)];
		for (int i = 0; i < MESH_GRANULARITY_X*MESH_GRANULARITY_Y; i++) {
			this.normals[i] = this.vertices[i].normalized;
		}
		
	}
	void calculateTriangles() {
		this.triangles = new int[(MESH_GRANULARITY_X+1) * (MESH_GRANULARITY_Y+1) * 3 * 2];
		int i = 0;
		int meshx = MESH_GRANULARITY_X - 1;
		int meshy = MESH_GRANULARITY_Y;
		int curry = 0;
		int currx = 0;
		for (curry = 0; curry < meshy; curry++)
		{
			for (currx = 0; currx <= meshx; currx++)
			{
				if (currx == meshx) {
					triangles[i+2] = (curry * (meshx + 1)) + currx;
					triangles[i+1] = (curry * (meshx + 1));
					triangles[i+0] = ((curry + 1) * (meshx + 1)) + currx;
					
					triangles[i+5] = (curry * (meshx + 1)) + (0);
					triangles[i+4] = ((curry + 1) * (meshx + 1)) + (0);
					triangles[i+3] = ((curry + 1) * (meshx + 1)) + currx;
					i+=6;
				} else {
				
					triangles[i+2] = (curry * (meshx + 1)) + currx;
					triangles[i+1] = (curry * (meshx + 1)) + (currx + 1);
					triangles[i+0] = ((curry + 1) * (meshx + 1)) + currx;
					
					triangles[i+5] = (curry * (meshx + 1)) + (currx + 1);
					triangles[i+4] = ((curry + 1) * (meshx + 1)) + (currx + 1);
					triangles[i+3] = ((curry + 1) * (meshx + 1)) + currx;
				}
				i += 6;
				
				
			}			
			
		}
		
		
	}
	void calculateUVsGrid()
	{
		//simplest UVs. They are a grid, but its wrong.
		this.uvs = new Vector2[MESH_GRANULARITY_X*(MESH_GRANULARITY_Y+1)];
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
		
	void calculateUVs()
	{
	
	this.uvs = new Vector2[MESH_GRANULARITY_X*(MESH_GRANULARITY_Y+1)];
	int i = 0;
	for (int y = 0; y <= MESH_GRANULARITY_Y;y++) 
	{
//		float yVal = y/(float)(MESH_GRANULARITY_Y);
		for (int x = 0; x < MESH_GRANULARITY_X; x++) {
			//y from -0.5 to 0.5... convert to 0 to 1					
			Vector3 vertex = this.vertices[i];					
			float yVal2 = vertex.y;
			yVal2 += 1.0F;
			yVal2 *= 0.5F;
			yVal2 = Mathf.Lerp(this.UV_range.x,this.UV_range.y,yVal2); //Set Uv's between this.UV_range
				
			if ((float)x <= width * MESH_GRANULARITY_X) {
				//float xVal = x/(width*MESH_GRANULARITY_X);

				//z from 1 to -1... convert to 0 to 1
				float zval = vertex.z;
				zval -= 1.0F; //0->-2
				zval *= -0.5F; // 0->1;
				
				this.uvs[i] = new Vector2(zval,yVal2);
			} else {					
				this.uvs[i] = new Vector2(0.0F,yVal2);
			}
			
			
			i++;
		}
	}

		
	}
	void RecalculateMesh() {
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		
		mesh.Clear();
		
		this.calculateVertices();
		this.calculateNormals();
		
		this.calculateTriangles();
		this.calculateUVs();
		//this.calculateUVsGrid();
		

		mesh.vertices = this.vertices;		
		
		mesh.triangles = this.triangles;
		mesh.normals = this.normals;
		mesh.uv = this.uvs;
		mesh.RecalculateBounds();
		
	}
	
	// Update is called once per frame
	void Update () {
	
		if (width != _width) {
			_width = width;
			this.RecalculateMesh();
		} else if ( this.UV_range != this._UV_range) {
			this._UV_range = this.UV_range;
			this.RecalculateMesh();
		}
	}
}
}