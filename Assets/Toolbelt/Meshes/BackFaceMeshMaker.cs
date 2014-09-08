using UnityEngine;
using System.Collections;
//using UnityEditor;
namespace Toolbelt {
public class BackFaceMeshMaker : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Vector3[] verts = new Vector3[8];
		Vector2[] uvs = new Vector2[8];		
		int[] triangles = new int[12];
		
		// 0  1
		// 3  2
		
		verts[0] = new Vector3(-0.5f,0.5f,0.0f);
		verts[1] = new Vector3(0.5f,0.5f,0.0f);
		verts[2] = new Vector3(0.5f,-0.5f,0.0f);
		verts[3] = new Vector3(-0.5f,-0.5f,0.0f);
		
		// back face.
		verts[4] = new Vector3(-0.5f,0.5f,0.0f);
		verts[5] = new Vector3(0.5f,0.5f,0.0f);
		verts[6] = new Vector3(0.5f,-0.5f,0.0f);
		verts[7] = new Vector3(-0.5f,-0.5f,0.0f);
		
		uvs[0] = new Vector2(0.0f,1.0f);
		uvs[1] = new Vector2(1.0f,1.0f);
		uvs[2] = new Vector2(1.0f,0.0f);
		uvs[3] = new Vector2(0.0f,0.0f);
		
		uvs[4] = new Vector2(0.0f,1.0f);
		uvs[5] = new Vector2(1.0f,1.0f);
		uvs[6] = new Vector2(1.0f,0.0f);
		uvs[7] = new Vector2(0.0f,0.0f);
		
		triangles[0] = 0;
		triangles[1] = 1;
		triangles[2] = 3;
		
		triangles[3] = 1;
		triangles[4] = 2;
		triangles[5] = 3;
		
		triangles[6] = 7;		
		triangles[7] = 5;
		triangles[8] = 4;
		
		triangles[9] = 7;
		triangles[10] = 6;
		triangles[11] = 5;
		
		
		Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
		mesh.vertices = verts;
		mesh.uv = uvs;
		mesh.triangles = triangles;		
		mesh.RecalculateNormals();
		
		//AssetDatabase.CreateAsset(mesh,	"Assets/DoubleSidedQuad.asset");	//Uncomment this line to save this mesh.
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
}