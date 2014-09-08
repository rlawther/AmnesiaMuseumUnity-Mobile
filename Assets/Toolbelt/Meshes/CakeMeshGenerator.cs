using UnityEngine;
using System.Collections;
//using UnityEditor;
public class CakeMeshGenerator : MonoBehaviour {
	//makes slices of a circular birthday cake.
	
	public float percentageOfCircle = 0.125f;
	public int numSlices = 4;
	
	private int _numSlices;
	private float _percentageOfCircle;
	
	
	
	// Use this for initialization
	void Start () {
		this._numSlices = this.numSlices;
		this._percentageOfCircle = this.percentageOfCircle;
		this.buildMesh();
	}
	void checkAndRebuild() {
		bool needRebuild = false;
		if (_numSlices != numSlices) {
			if (numSlices < 1) {
				numSlices = 1;
			}
			_numSlices = numSlices;
			needRebuild = true;
		} 
		
		if (_percentageOfCircle != percentageOfCircle) {
			if (percentageOfCircle <= 0.0f || percentageOfCircle > 1.0f) {
				percentageOfCircle = 1.0f;
			}
			_percentageOfCircle = percentageOfCircle;
			needRebuild = true;
		}
		
		if (needRebuild) {
			this.buildMesh();
		}
	}
	
	// Update is called once per frame
	void Update () {
		this.checkAndRebuild();
	}
	/********************************************************************
		MESH BUILDING SECTION!
	*********************************************************************/
	public Vector2[] uvs;
	public Vector3[] vertices;
	public int[] triangles;
	//public Vector3[] normals;
	
	
	private void buildMesh() {
		MeshFilter mf = GetComponent<MeshFilter>();
		if (!mf) {
			mf = this.gameObject.AddComponent<MeshFilter>();
		}

		Mesh mesh = mf.mesh;
		
		mesh.Clear();
		
		this.calculateVerticesAndUVs();
		this.calculateTriangles();		
		
		mesh.vertices = this.vertices;				
		mesh.triangles = this.triangles;
		mesh.RecalculateNormals();
		mesh.uv = this.uvs;
		//AssetDatabase.CreateAsset(mesh,	"Assets/CakeSlice_45.asset");	//Uncomment this line to save this mesh.
	}
	private int getBottomSliceOffset() {
		return 8;
	}
	private int getTopSliceOffset() {
		return this.getBottomSliceOffset() + (numSlices+1);
	}
	
	private void calculateVerticesAndUVs() {	
		//super naive implementation 
		// Order of vertices:
		// *front of cake slice
		// *back of cake slice
		// * curved section of cake slice (bottom)
		// * curved section of cake slice (top)
				
		this.vertices = new Vector3[8+(numSlices+1)*2];
		this.uvs = new Vector2[8+(numSlices+1)*2];
		
		int bottomSliceOffset = this.getBottomSliceOffset();;
		int topSliceOffset = this.getTopSliceOffset();
		
		float totalAngle = percentageOfCircle * 360.0f * Mathf.Deg2Rad;
		float splitSize = totalAngle/numSlices;
		
		
		//front of slice - a quad
		this.vertices[0] = new Vector3(0.0f,0.0f,0.0f);		
		this.vertices[1] = new Vector3(0.0f,1.0f,0.0f);
		this.vertices[2] = new Vector3(Mathf.Sin (0.0f),1.0f,Mathf.Cos (0.0f));
		this.vertices[3] = new Vector3(Mathf.Sin (0.0f),0.0f,Mathf.Cos (0.0f));
		
		//back of slice - a quad
		this.vertices[4] = new Vector3(0.0f,0.0f,0.0f);
		this.vertices[5] = new Vector3(0.0f,1.0f,0.0f);
		this.vertices[6] = new Vector3(Mathf.Sin (totalAngle),1.0f,Mathf.Cos (totalAngle));
		this.vertices[7] = new Vector3(Mathf.Sin (totalAngle),0.0f,Mathf.Cos (totalAngle));
		
		//uvs for slices
		this.uvs[0] = this.uvs[4] = new Vector2(0.0f,0.0f);
		this.uvs[1] = this.uvs[5] = new Vector2(0.0f,1.0f);
		this.uvs[2] = this.uvs[6] = new Vector2(1.0f,1.0f);
		this.uvs[3] = this.uvs[7] = new Vector2(1.0f,0.0f);
		
		
		
		for (int i = 0; i <= numSlices; i++ ) {
			float x = Mathf.Sin(i * splitSize);
			float z = Mathf.Cos(i * splitSize);
			
			this.vertices[i + bottomSliceOffset] = new Vector3(x,0.0f,z);
			this.uvs[i+bottomSliceOffset] = new Vector2(0.0f,(float)i/numSlices);
			
			this.vertices[i + topSliceOffset] = new Vector3(x,1.0f,z);
			this.uvs[i+topSliceOffset] = new Vector2(1.0f,(float)i/numSlices);
		}
		
		//now we have to centre the model on (0,0,0)
		//To do this, we find the x/z of the first and last slice, an
		this.recentreVertices();
	}
	private void recentreVertices() {
		//recentre Y values first.		
		for (int i = 0; i < this.vertices.Length; i++) {
			Vector3 v = this.vertices[i];			
			v.y -= 0.5f;
			this.vertices[i] = v;
		}
		//if we have at least half a pie, the pivot point is 0,0,0. Otherwise, its the centre of the slice.
		if (this._percentageOfCircle >= 0.5f) {
			return;
		}
		//to find the centre of the slice, we find the middle Angle and associated x/z values;
		float totalAngle = percentageOfCircle * 360.0f * Mathf.Deg2Rad;
		float centreAngle = totalAngle/2.0f;
		float x = Mathf.Sin (centreAngle);
		float z = Mathf.Cos (centreAngle);
		
		//now shift everything by half that amount to get the whole slice centred nicely.
		x /= 2.0f;
		z /= 2.0f;
		
		for (int i = 0; i < this.vertices.Length; i++) {
			Vector3 v = this.vertices[i];
			v.x -= x;
			v.z -= z;
			this.vertices[i] = v;
		}

	}

	private void calculateTriangles() {
		//2 quads -> 4 triangles.
		//top and bottom -> numSlice * 2 triangles
		//curved area -> 2 triangles * numSlice
		
		int frontBackQuads = 2;
		int topSlices = numSlices;
		int bottomSlices = topSlices;
		int curveSlices = numSlices;
		
		int numTris = (frontBackQuads*2) + topSlices+bottomSlices + (curveSlices * 2);
		this.triangles = new int[numTris * 3];
		
		//front quad
		triangles[0] = 2;
		triangles[1] = 1;
		triangles[2] = 0;
		
		triangles[3] = 0;
		triangles[4] = 3;
		triangles[5] = 2;
		// back quad
		triangles[6] = 4;
		triangles[7] = 5;
		triangles[8] = 6;
		
		triangles[9] = 6;
		triangles[10] = 7;
		triangles[11] = 4;
		
		int firstBottomVertex = this.getBottomSliceOffset();
		int firstTopVertex = this.getTopSliceOffset();
		int triangleIndex = 12;
		
		for (int i = 0; i < numSlices; i++) {
			int bottomLeft = i + firstBottomVertex;
			int bottomRight = (i+1) + firstBottomVertex;
			int topLeft = i + firstTopVertex;
			int topRight = (i+1) + firstTopVertex;
			
			//Curved rectangles
			triangles[triangleIndex] = topRight;
			triangles[triangleIndex+1] = topLeft;
			triangles[triangleIndex+2] = bottomLeft;
			
			triangles[triangleIndex+3] = bottomLeft;
			triangles[triangleIndex+4] = bottomRight;
			triangles[triangleIndex+5] = topRight;
			triangleIndex += 6;
			//bottom slice
			triangles[triangleIndex] = 0;//index 0 is bottom pointy
			triangles[triangleIndex+1] = bottomRight;
			triangles[triangleIndex+2] = bottomLeft;
			triangleIndex += 3;
			//top slice
			triangles[triangleIndex] = topLeft;
			triangles[triangleIndex+1] = topRight;
			triangles[triangleIndex+2] = 1; //index 1 is top pointy
			triangleIndex += 3;
		}
		
		
	}
}
