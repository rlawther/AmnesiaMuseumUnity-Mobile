using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using Assets.AVIE.Scripts.Graphics;
using Toolbelt;

public class CommandLineUsageException : Exception
{
	public CommandLineUsageException (string message)
        : base(message)
	{
	}
}

public class InitialisationScript : MonoBehaviour, icDebugGUI, StereoCameraInterface
{
	public delegate void SlicePostRenderCallbackHandler(SliceCameraDescription sliceCam);
	protected event SlicePostRenderCallbackHandler SlicePostRenderCallback;
	
	public delegate void AllPostRenderCallbackHandler(ProjectorDescription pd);
	protected event AllPostRenderCallbackHandler AllPostRenderCallback;
	
	
	public const string CONFIG_PATH = ProjectorDescription.CONFIG_PATH;
	[SerializeField]
	private bool
		InputEnabled = false;
	[Range(1, 6)]
	public int
		_numIGs = 6;
	[Range(1, 16)]
	public int
		_numSlicesPerIG = 4;
	public float eyeWidth = 0.6f;
	public float eyeHeight = 10.0f;
	private bool showGrid = false;
	public bool showBlend = true;
	public bool doWarp = true;
	public Texture leftEyeGrid;
	public Texture rightEyeGrid;
	public GameObject warpMeshPrefab;
	public Camera _masterCamera;
	public Camera _orthoCameraTemplate;
	public int _projectorRenderTextureWidth = 2048;
	public int _projectorRenderTextureHeight = 2048;
	public bool showLeftEye;
	public int modelLayer = 7;
	public float renderRadius = 20;
	public float renderHeight = 16;
	public Transform _followCamera;
	
	private float[] sliceSeams;
	private float sliceWidth;
	private float cameraAngle;
	private List<ProjectorMesh> _projectorMeshes = new List<ProjectorMesh> ();
	private List<ProjectorDescription> _projectors = new List<ProjectorDescription> ();
	private List<SliceCameraDescription> _sliceCameraDescriptions;
	private GameObject _sliceCameraParent;
	
	
	
	public List<ProjectorDescription> GetProjectorDescriptions() {
		return this._projectors;
	}
	
	public enum DisplayConfigurations
	{
		MASTER,
		MULTI_VIEW,
		DOUBLE_IG,
		TRIPLE_IG,
		LEFT_BLANK,
		BLANK_RIGHT,
		LEFT_RIGHT}
	;

	public DisplayConfigurations _displayConfiguraiton = DisplayConfigurations.MASTER;
	private DisplayConfigurations _internalDisplayConfiguration = DisplayConfigurations.MASTER;
	protected StereoMode _currentEye;
	
	public float RenderRadius {
		get {
			return renderRadius;
		}
		set {
			renderRadius = value;
			RecomputeCameraProjections ();
		}
	}

	public float RenderHeight { get { return renderHeight; } set { renderHeight = value; } }

	public bool isInitialized = false;
	//private int slaveIndex = -1;
	// Use this for initialization
	void Start ()
	{
		// AH: General Setup
		//Screen.showCursor = false;

		StartRenderSystem (); 
		StartCoroutine ("LateInitializeImageShaderScripts");
	}
	
	private IEnumerator LateInitializeImageShaderScripts ()
	{
		//Have to wait a frame before turning imageshaderscripts on. Otherwise a flash will occur.
		yield return new WaitForEndOfFrame ();
		
		Transform t = this.transform.FindChild ("Cluster Manager");
		
		if (t != null) {
			ImageShaderScript[] scripts = t.GetComponentsInChildren<ImageShaderScript> (true);
			
			foreach (ImageShaderScript iss in scripts) {
				if (!iss.enabled) {
					iss.enabled = true;
				}
			}
			
			this.isInitialized = true;

		} else {
			Debug.Log ("Avie Prefab destroyed before finished creating!");
		}
	}

	void ShutdownRenderSystem ()
	{
		//Debug.Log("Shutdown Render System");
		// Destroy Slice Camera root object
        
		Destroy (_sliceCameraParent);        
		_sliceCameraParent = null;

		// Destroy projectors objects
		foreach (ProjectorDescription projDesc in _projectors) {
			projDesc.destroyGameObjects ();
		}

		_projectors.Clear ();
		_projectorMeshes.Clear (); 
	}

	private void setDisplayConfiguration() {
		//only setDisplayConfiguration if we are running from EXE.		
		if (!Application.isEditor && ClusterManager.Instance.hadArgs)
		{
			int toShow = ClusterManager.Instance.NumSlavesToShow;
			if (toShow == 1) {
				this._displayConfiguraiton = DisplayConfigurations.LEFT_RIGHT;
			} else if (toShow == 2) {
				this._displayConfiguraiton = DisplayConfigurations.DOUBLE_IG;
			} else if (toShow == 3) {
				this._displayConfiguraiton = DisplayConfigurations.TRIPLE_IG;
			}
		}
		
		
	}
	void StartRenderSystem ()
	{
		// In case the render system already exists, detroy it
		ShutdownRenderSystem ();

		this.setDisplayConfiguration();
		// AH: Read in the warp mesh. This only reads in the 
		// warp meshes relevant for this IG? 
		ReadWarpMesh ();

		// Establish exactly how many and where the slices and seams are
		sliceSeams = new float[_numSlicesPerIG * _numIGs];
		sliceWidth = 1.0f / (_numSlicesPerIG * _numIGs);
		for (int i = 0; i < sliceSeams.Length; i++) {
			sliceSeams [i] = sliceWidth * (float)i;
		}
		cameraAngle = 360.0f / (float)(sliceSeams.Length);
//        Debug.Log("Each slice camera looks at " + (sliceSeams.Length).ToString() + "th of the AVIE, which is " + cameraAngle.ToString() + " degrees.");

		_sliceCameraParent = new GameObject ();
		CreateMeshesForSlaves ();

		//
		// Organize Meshes in appropriate manner 
		// based on flags
		// 
		// Arrange the projector displays in a grid 

		//
		// If displaying all displays. Arrange them appropriately
		//
		if (_internalDisplayConfiguration == DisplayConfigurations.MASTER) {
			// There should only be two projectors per slave
			ProjectorDescription leftProjector = _projectors [0];
			leftProjector._orthographicCamera.SetActive (false);
			leftProjector.warpMeshObject.SetActive (true);

			ProjectorDescription rightProjector = _projectors [1];
			rightProjector._orthographicCamera.SetActive (false);
			rightProjector.warpMeshObject.SetActive (true);

			_masterCamera.camera.enabled = true;
		} else if (_internalDisplayConfiguration == DisplayConfigurations.LEFT_BLANK) {
			// There should only be two projectors per slave
			ProjectorDescription leftProjector = _projectors [0];
			leftProjector._orthographicCamera.camera.rect = new Rect (0, 0, 0.5f, 1.0f);
			leftProjector._orthographicCamera.SetActive (true);
			leftProjector.warpMeshObject.SetActive (true);

			ProjectorDescription rightProjector = _projectors [1];
			rightProjector._orthographicCamera.SetActive (false);
			rightProjector.warpMeshObject.SetActive (false);

			_masterCamera.camera.enabled = false;
		} else if (_internalDisplayConfiguration == DisplayConfigurations.BLANK_RIGHT) {
			// There should only be two projectors per slave
			ProjectorDescription leftProjector = _projectors [0];
			leftProjector._orthographicCamera.SetActive (false);
			leftProjector.warpMeshObject.SetActive (false);

			ProjectorDescription rightProjector = _projectors [1];
			rightProjector._orthographicCamera.camera.rect = new Rect (0.5f, 0, 0.5f, 1.0f);
			rightProjector._orthographicCamera.SetActive (true);
			rightProjector.warpMeshObject.SetActive (true);

			_masterCamera.camera.enabled = false;
		} else if (_internalDisplayConfiguration == DisplayConfigurations.LEFT_RIGHT) {
			// There should only be two projectors per slave
			ProjectorDescription leftProjector = _projectors [0];
			leftProjector._orthographicCamera.camera.rect = new Rect (0, 0, 0.5f, 1.0f);
			leftProjector._orthographicCamera.SetActive (true);
			leftProjector.warpMeshObject.SetActive (true);

			ProjectorDescription rightProjector = _projectors [1];
			rightProjector._orthographicCamera.camera.rect = new Rect (0.5f, 0, 0.5f, 1.0f);
			rightProjector._orthographicCamera.SetActive (true);
			rightProjector.warpMeshObject.SetActive (true);
            
			_masterCamera.camera.enabled = false;
		} else if (_internalDisplayConfiguration == DisplayConfigurations.MULTI_VIEW) {
			float screenWidth = 1.0f / 6.0f;
			float screenHeight = 0.5f;

			for (int i = 0; i < 11; i += 2) {
				float x_offset = i / 2 * screenWidth;

				// There should only be two projectors per slave
				ProjectorDescription leftProjector = _projectors [i];
				leftProjector._orthographicCamera.camera.rect = new Rect (x_offset, 0, screenWidth, screenHeight);
				leftProjector._orthographicCamera.SetActive (true);
				leftProjector.warpMeshObject.SetActive (true);

				ProjectorDescription rightProjector = _projectors [i + 1];
				rightProjector._orthographicCamera.camera.rect = new Rect (x_offset, screenHeight, screenWidth, screenHeight);
				rightProjector._orthographicCamera.SetActive (true);
				rightProjector.warpMeshObject.SetActive (true);
			}

			_masterCamera.camera.enabled = false;
		} else if (_internalDisplayConfiguration == DisplayConfigurations.DOUBLE_IG) {			
			float screenWidth = 1.0f / 4.0f;
			float screenHeight = 1.0f;
			
			for (int i = 0; i < 4; i += 2) {
				float x_offset = i / 4f;				
				// There should only be two projectors per slave
				ProjectorDescription leftProjector = _projectors [i];
				
				leftProjector._orthographicCamera.camera.rect = new Rect (x_offset, 0, screenWidth, screenHeight);
				leftProjector._orthographicCamera.SetActive (true);
				leftProjector.warpMeshObject.SetActive (true);
				
				
				x_offset = (i + 1f) / 4f;
				
				ProjectorDescription rightProjector = _projectors [i + 1];
				rightProjector._orthographicCamera.camera.rect = new Rect (x_offset, 0, screenWidth, screenHeight);
				rightProjector._orthographicCamera.SetActive (true);
				rightProjector.warpMeshObject.SetActive (true);
			}
			
			_masterCamera.camera.enabled = false;
		} else if (_internalDisplayConfiguration == DisplayConfigurations.TRIPLE_IG) {			
			float screenWidth = 1.0f / 3.0f;
			float screenHeight = 0.5f;
			
			for (int i = 0; i < 5; i += 2) {
				float x_offset = i / 2 * screenWidth;
				
				// There should only be two projectors per slave
				ProjectorDescription leftProjector = _projectors [i];
				leftProjector._orthographicCamera.camera.rect = new Rect (x_offset, 0, screenWidth, screenHeight);
				leftProjector._orthographicCamera.SetActive (true);
				leftProjector.warpMeshObject.SetActive (true);
				
				ProjectorDescription rightProjector = _projectors [i + 1];
				rightProjector._orthographicCamera.camera.rect = new Rect (x_offset, screenHeight, screenWidth, screenHeight);
				rightProjector._orthographicCamera.SetActive (true);
				rightProjector.warpMeshObject.SetActive (true);
			}
			
			_masterCamera.camera.enabled = false;
		}
		InitialCameraSettings ics = this.GetComponent<InitialCameraSettings> ();
		if (ics != null)
			ics.ExecuteInitialCameraSettings ();
	}

	void ReadWarpMesh ()
	{
		TextReader file = new StreamReader (CONFIG_PATH + "Avie/Config/mesh.txt");
		while (true) {

			if (this.ParseWarpMeshProjector(file)) {
				break;
			}
			
		}
	}
	private bool isSlaveDisplayed(int projectorId) {
		bool result = true;
		if (!ClusterManager.Instance.IsMaster) {
			result = ((projectorId / 2) == ClusterManager.Instance.ClientID);
		}
		if (this._displayConfiguraiton == DisplayConfigurations.MULTI_VIEW) {
			result = true;
		}
		if (this._displayConfiguraiton == DisplayConfigurations.DOUBLE_IG) {
			result = ((projectorId / 2) == ClusterManager.Instance.ClientID) || 
				((projectorId / 2) == ClusterManager.Instance.ClientID + 1);
		}
		if (this._displayConfiguraiton == DisplayConfigurations.TRIPLE_IG) {
			result = (projectorId / 2 >= ClusterManager.Instance.ClientID && 
			                  projectorId / 2 <= ClusterManager.Instance.ClientID + 2);
		}
		return result;
	}
    private bool ParseWarpMeshProjector(TextReader file) {
    	//returns if no entries remaining.
		string line = file.ReadLine ();
		if (line == null) {
			return true;
		}
		string[] tokens = line.Trim ().Split (" ,():".ToCharArray (), StringSplitOptions.RemoveEmptyEntries);
		if (tokens.Length == 2 && tokens [0] == "Projector") {
			int projectorId = int.Parse (tokens [1]);
			
			
			// Start line of a new projector's info
			line = file.ReadLine ();
			if (line == null) {
				throw new Exception ("Could not create a mesh. File has ended before vertices list.");
			}
			bool isBeingDisplayed = this.isSlaveDisplayed(projectorId);
			
			tokens = line.Trim ().Split (" ,():".ToCharArray (), StringSplitOptions.RemoveEmptyEntries);
			int gridWidth = Int32.Parse (tokens [0]);
			int gridHeight = Int32.Parse (tokens [1]);
			float projectorStart = (float)Double.Parse (tokens [2]);
			float projectorEnd = (float)Double.Parse (tokens [3]);
			int numVertices = (gridWidth + 1) * (gridHeight + 1);
			
			ProjectorMesh projectorMesh = null;
			if (isBeingDisplayed) {				
				if (doWarp) {
					projectorMesh = new ProjectorMesh (projectorId, gridWidth, gridHeight, projectorStart, projectorEnd);
					this.processProjectorVertices(file, !isBeingDisplayed, projectorMesh,numVertices);
				} else {
					this.getFakeProjStartEnd(projectorId, out projectorStart, out projectorEnd);
					projectorMesh = new ProjectorMesh (projectorId, 1, 1, projectorStart, projectorEnd);
					this.fakeProjectorVertices(file, !isBeingDisplayed, projectorMesh,numVertices);
				}
				_projectorMeshes.Add (projectorMesh);
			}
			
		}
		return false;
    
    }
    private void getFakeProjStartEnd(int projectorId, out float projectorStart, out float projectorEnd) {
    	projectorId /= 2; //convert from projector to slaveID
    	int numProjs = this._numIGs;
    	float start = projectorId/(float)numProjs;
    	float end = start + 1.0f/numProjs;
    	if (start == 0.0f) {
    		start = 0.9999f;
    		end += 1.0f;
    	} else if (end == 1.0f) {
    		end += 0.0001f;
    	}
    	projectorStart = start;
    	projectorEnd = end;
    	
    }
    
    private void fakeProjectorVertices(TextReader file, bool skip, ProjectorMesh pm, int numVertices) {
    	if (!skip) { //create fake projector vertices
//			Projector 12:
//			1 1 0.0 1.0
//			
//			
//			

			string a = "( 0, 0) -0.500000 -0.500000  0.000000  1.000000";
			string b = "( 1, 0)  0.500000 -0.500000  1.000000  1.000000";
			string c = "( 0, 1) -0.500000  0.500000  0.000000  0.000000";
			string d = "( 1, 1)  0.500000  0.500000  1.000000  0.000000";
			this.parseProjectorVertex(a, pm);
			this.parseProjectorVertex(b, pm);
			this.parseProjectorVertex(c, pm);
			this.parseProjectorVertex(d, pm);
    	
    	}
		
    	this.processProjectorVertices(file,true,pm, numVertices); //move file forward.
    }
    
    public void processProjectorVertices(TextReader file, bool skip, ProjectorMesh pm, int numVertices) {
		for (int i = 0; i < numVertices; i++) {
			// Read one line and create a vertex
			string line = file.ReadLine ();
			if (line == null) {
				throw new Exception ("Could not create a mesh because there weren't enough vertices. File has ended");
			}
			

			if (!skip) {
				this.parseProjectorVertex(line, pm);
			}
		}
    }
    
    public void parseProjectorVertex(string line, ProjectorMesh pm) {
		string[] tokens = line.Trim ().Split (" ,():".ToCharArray (), StringSplitOptions.RemoveEmptyEntries);
		if (tokens.Length < 6) {
			throw new Exception ("Could not create a mesh because there weren't enough vertices. Not enough verts in this list.");
		}
		int xIndex = (int)int.Parse (tokens [0]);
		int yIndex = (int)int.Parse (tokens [1]);
		float cylinderX = (float)Double.Parse (tokens [2]);
		float cylinderY = (float)Double.Parse (tokens [3]);
		float cylinderU = (float)Double.Parse (tokens [4]);
		float cylinderV = (float)Double.Parse (tokens [5]);
		pm.SetCylinderPoint (xIndex, yIndex, new Vector2 (cylinderX, cylinderY), new Vector2 (cylinderU, cylinderV));
    
    }
	public void RecomputeCameraProjections ()
	{
		foreach (ProjectorDescription projector in _projectors) {
			foreach (SliceCameraDescription camera in projector.sliceCameras) {
				camera.ComputeProjection (cameraAngle, renderRadius, renderHeight, eyeWidth, eyeHeight, sliceWidth);
			}
		}
	}

	public void setGrid (bool enabled)
	{
		this.showGrid = enabled;
		foreach (ProjectorDescription projector in _projectors) {
			UpdateTextures (projector);
		}
	}
	
	public void setBlend (bool enabled)
	{
		if (enabled)
			this.EnableBlend ();
		else
			this.DisableBlend ();	
		this.showBlend = enabled;		
	}
	// Update is called once per frame
	void Update()
	{
    	
		if (this.InputEnabled) {
			if (Input.GetKeyDown (KeyCode.L)) {
				showLeftEye = !showLeftEye;
			} else if (Input.GetKeyDown (KeyCode.G)) {
				this.setGrid (!this.showGrid);            
			} else if (Input.GetKeyDown (KeyCode.Alpha1)) {
				eyeWidth -= 0.01f;
			} else if (Input.GetKeyDown (KeyCode.Alpha2)) {
				eyeWidth += 0.01f;
			} else if (Input.GetKeyDown (KeyCode.Alpha3)) {
				this.ShutdownRenderSystem ();
			} else if (Input.GetKeyDown (KeyCode.Alpha4)) {
				this.StartRenderSystem ();
			}
	
			////////////////////////////
			// Display Configurations //
			////////////////////////////
			if (Input.GetKeyDown (KeyCode.F5)) {
				this._displayConfiguraiton = DisplayConfigurations.MASTER;
			} else if (Input.GetKeyDown (KeyCode.F6)) {
	
			} else if (Input.GetKeyDown (KeyCode.F7)) {
	
			} else if (Input.GetKeyDown (KeyCode.F8)) {
	
			} else if (Input.GetKeyDown (KeyCode.F9)) {
	
			}
		}

		///////////////////////////////////////////////////
		// Update the display configuration if necessary //
		///////////////////////////////////////////////////
		if (_displayConfiguraiton != _internalDisplayConfiguration) {
			// Restart the render system
			_internalDisplayConfiguration = _displayConfiguraiton;
			this.StartRenderSystem (); 
		}

		/////////////////////////////////
		// Per Frame Parameter Updates //
		/////////////////////////////////
		foreach (ProjectorDescription projector in _projectors) {
			foreach (SliceCameraDescription camera in projector.sliceCameras) {
				if (camera.cameraObject != null) {
					camera.EyeWidth = eyeWidth;
					camera.cameraObject.camera.depthTextureMode = DepthTextureMode.Depth;
					camera.cameraObject.camera.Render();
					
					if (this.SlicePostRenderCallback != null)
						this.SlicePostRenderCallback(camera);
				}
			}
			if (this.AllPostRenderCallback != null)
				this.AllPostRenderCallback(projector);
		}

		////////////////////////////////////
		// Track the follow camera object //
		////////////////////////////////////
		if (_sliceCameraParent) {
			if (_followCamera) {
				Vector3 p = _followCamera.transform.position;
				p.y += 5.0f; 
				_sliceCameraParent.transform.position = p;
				_sliceCameraParent.transform.eulerAngles = _followCamera.transform.eulerAngles;
			} else {
				_sliceCameraParent.transform.position = _masterCamera.transform.position;
				_sliceCameraParent.transform.eulerAngles = _masterCamera.transform.eulerAngles;
			}
		}
	}

	void CreateMeshesForSlaves ()
	{
		// AH: Update this function so that it only creates meshes 
		// if this is the correct slave node. Or we are forcing
		// all slave nodes to render.
		_sliceCameraParent.name = "Slice Cameras";
		_sliceCameraParent.transform.parent = gameObject.transform;
		_sliceCameraParent.transform.localPosition = Vector3.zero;
//        Debug.Log("Number of projector meshes = " + _projectorMeshes.Count);
		foreach (ProjectorMesh projectorMesh in _projectorMeshes) {
			ProjectorDescription projector = new ProjectorDescription (projectorMesh, 
                _projectorRenderTextureWidth,
                _projectorRenderTextureHeight,
                warpMeshPrefab,
                this.transform.FindChild ("Cluster Manager"));
			_projectors.Add (projector);
            

			GameObject nodeParent = new GameObject ();
			string stereoTypeString = (projector.stereoType == ProjectorDescription.ProjectorStereoType.Left) ? "Left" : "Right";
			nodeParent.name = "Node:" + projector.nodeNumber + " " + stereoTypeString;
			nodeParent.transform.parent = _sliceCameraParent.transform;
			nodeParent.transform.localPosition = Vector3.zero;

			_sliceCameraDescriptions = projector.CreateSliceCameras (sliceSeams, 
																	nodeParent, 
																	this, 
																	_masterCamera);
																		
			foreach (SliceCameraDescription cameraDescription in _sliceCameraDescriptions) {
				if (ClusterManager.Instance.IsMaster) {
					cameraDescription.CameraObject.SetActive (false);
				}
			}
			UpdateTextures (projector);
			UpdateBlend (projector);
			
			// Create a camera for each slave that views the slave portion.
			projector.CreateDisplayCamera (gameObject, _orthoCameraTemplate);
			if (ClusterManager.Instance.IsMaster) {
				projector.ProjectorObject.SetActive (false);
			}
		}
		_masterCamera.camera.enabled = true;
	}
	
	private void UpdateBlend (ProjectorDescription projector)
	{			
		GameObject warpMeshObject = projector.warpMeshObject;
		if (this.showBlend) {			
			warpMeshObject.renderer.material.SetTexture ("_BlendTex", projector._targetBlendTexture);
		} else {
			warpMeshObject.renderer.material.SetTexture("_BlendTex",null);
		}
	}
	
	private void DisableBlend ()
	{
		foreach (ProjectorDescription p in this._projectors) {
			GameObject warpMeshObject = p.warpMeshObject;
			warpMeshObject.renderer.material.SetTexture ("_BlendTex", null);	
		}
	}
	
	private void EnableBlend ()
	{
		foreach (ProjectorDescription p in this._projectors) {
			GameObject warpMeshObject = p.warpMeshObject;
			warpMeshObject.renderer.material.SetTexture ("_BlendTex", p._targetBlendTexture);	
		}
		
	}

	private void UpdateTextures (ProjectorDescription projector)
	{
		GameObject warpMeshObject = projector.warpMeshObject;
		if (showGrid) {
			// Use grid instead of showing scene
			if (projector.stereoType == ProjectorDescription.ProjectorStereoType.Left) {
				//Debug.Log("Set Left Eye to Grid");
				warpMeshObject.GetComponent<RefreshTextureScript> ().toReplace = leftEyeGrid;
				Vector2 texScale = new Vector2 (projector.ProjectorMesh.MeshEnd - projector.ProjectorMesh.MeshStart, 1.0f);
				warpMeshObject.renderer.material.mainTextureScale = texScale;
				Vector2 texOffset = new Vector2 (projector.ProjectorMesh.MeshStart, 0.0f);
				warpMeshObject.renderer.material.mainTextureOffset = texOffset;
			} else {
				//Debug.Log("Set Right Eye to Grid");
				warpMeshObject.GetComponent<RefreshTextureScript> ().toReplace = rightEyeGrid;
				Vector2 texScale = new Vector2 (projector.ProjectorMesh.MeshEnd - projector.ProjectorMesh.MeshStart, 1.0f);
				Vector2 texOffset = new Vector2 (projector.ProjectorMesh.MeshStart, 0.0f);
				warpMeshObject.renderer.material.mainTextureScale = texScale;
				warpMeshObject.renderer.material.mainTextureOffset = texOffset;
			}
		} else {
			warpMeshObject.GetComponent<RefreshTextureScript> ().toReplace = projector._targetRenderTexture;
			warpMeshObject.renderer.material.mainTextureScale = new Vector2 (1.0f, 1.0f);
			warpMeshObject.renderer.material.mainTextureOffset = new Vector2 (0, 0);
			foreach (SliceCameraDescription cameraDescription in _sliceCameraDescriptions) {
//                Camera camera = cameraDescription.CameraObject.camera;
				cameraDescription.ComputeProjection (cameraAngle, renderRadius, renderHeight, eyeWidth, eyeHeight, sliceWidth);
				cameraDescription.ComputeViewport (Screen.width);
				//cameraDescription.SetRenderLayer(modelLayer);
			}
		}
	}
    
	public StereoMode GetCurrentEye ()
	{
		return this._currentEye;
	}
    	
	public void SetCurrentEye (StereoMode eye)
	{
		this._currentEye = eye;
		ToolbeltManager.FirstInstance.SetCurrentEye (eye);
	}
	
	public void AddSliceCameraRenderCallback(SlicePostRenderCallbackHandler func) {
		SlicePostRenderCallback += func;
	}
	
	public void AddAllCameraRenderCallback(AllPostRenderCallbackHandler func) {
		AllPostRenderCallback += func;
	}

	public void DrawDebugGUI ()
	{
		GUILayout.BeginVertical ();
		GUILayout.Label ("Eye Separation: " + this.eyeWidth.ToString ());
		this.eyeWidth = GUILayout.HorizontalSlider (this.eyeWidth, -1f, 1f);
    	
		
		bool swapGrid = GUILayout.Toggle (this.showGrid, "Show Calibration Grid");
		if (swapGrid != this.showGrid) {
			this.setGrid (swapGrid);
		}
		
		bool blend = GUILayout.Toggle (this.showBlend, "Enable Blend regions");
		if (blend != this.showBlend) {
			this.setBlend (blend);			
		}
    	
		GUILayout.EndVertical ();
	}
}
