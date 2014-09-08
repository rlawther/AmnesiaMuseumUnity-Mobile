using UnityEngine;
using System.Collections;

namespace Toolbelt {
public class WireFrameMode : MonoBehaviour, icDebugGUI {
	Vector3[] _wfVertices;
	int[] _wfTris;
	Vector2[] _wfUvs;
	Material _wfMaterial;
	
	Vector3[] _origVertices;
	int[] _origTris;
	Vector2[] _origUvs;
	Material _origMaterial;
	
	private const int MAX_VERTS = 65536;

	private void createUniqueVertices(Mesh m) {
		int[] oldTris = m.triangles;
		Vector3[] oldVerts = m.vertices;
		
		if (oldTris.Length > MAX_VERTS){
			Debug.Log ("Too many verts:" + (oldTris.Length).ToString() + ". Aborting");
			return;
		} else { 
			//Debug.Log ("Creating new mesh from old mesh:" + (oldTris.Length).ToString() + " triangles");
		}
		
		//New Vertices = oldTries * 3
		Vector3[] _vertices = new Vector3[oldTris.Length];
		int[] _tris = new int[oldTris.Length];
		
		for (int i = 0; i < oldTris.Length; i++) {
			//copy old vertex into new vertex array;
			_vertices[i] = oldVerts[oldTris[i]];
			_tris[i] = i;
		}
		
		Vector2[] _uvs = this.GetBarycentricFixed(_vertices);		
		
		this._origVertices = oldVerts;
		this._origUvs = m.uv;
		this._origTris = m.triangles;
		
		this._wfVertices = _vertices;
		this._wfTris = _tris;
		this._wfUvs = _uvs;
	}
	
	private Vector2[] GetBarycentricFixed(Vector3[] vertices)
	{
		Vector2[] uvs =
			new Vector2[vertices.Length];
		
		for (int i = 0; i < vertices.Length; i++) {
			if (i % 3 == 0) {
				uvs[i] = new Vector2(0,0);
			} else if (i % 3 == 1) {
				uvs[i] = new Vector2(0,1);
			} else if (i % 3 == 2) {
				uvs[i] = new Vector2(1,0);
			}
		}
		
		return uvs;
	}
	private void initializeWireFrames() {
		MeshFilter mf = this.GetComponent<MeshFilter>();
		this.createUniqueVertices(mf.mesh);
		this._origMaterial = this.renderer.material;
		this._wfMaterial = new Material(this._origMaterial);
		this._wfMaterial.shader = Shader.Find ("WireFrame/Transparent");
	}
	
	public void OnEnable() {
		if (this._wfVertices == null) {
			this.initializeWireFrames();	
		}
		//flip to wireframe mode
		MeshFilter mf = this.GetComponent<MeshFilter>();
		Mesh m = mf.mesh;
		m.Clear();
		m.vertices = this._wfVertices;
		m.triangles = this._wfTris;
		m.uv = this._wfUvs;
		m.RecalculateNormals();
		this.renderer.material = this._wfMaterial;
		
		
		
	}
	public void OnDisable() {
		//restore old stuff
		
		MeshFilter mf = this.GetComponent<MeshFilter>();
		Mesh m = mf.mesh;
		m.Clear();
		m.vertices = this._origVertices;
		m.triangles = this._origTris;
		m.uv = this._origUvs;
		m.RecalculateNormals();
		this.renderer.material = this._origMaterial;
	}
	public void DrawDebugGUI() {
		GUILayout.Label ("Enable/disable script to enable/disable wireframe");
	}
}
}