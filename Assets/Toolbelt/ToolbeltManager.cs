using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Toolbelt {

	public enum StereoMode {
		LEFT = 1,
		RIGHT = 2,
		BOTH = 3
	};
	/*** The base manager that all scenes should have on their scene root 
	 * Subclass this for your own project code
	 * */
	public class ToolbeltManager : MonoBehaviour 
	{
		static ToolbeltManager instance;
//		static DebugPanel debugPanel;
		public bool useBink = true;
		public bool showCameraGhost = false;
		private bool _prevShowCameraGhost = false;

		public int concurrentMediaLoads = 1;
		public int imageCacheMBs = 0;
		public float imageCacheCleanupInterval = 5.0f;
		public float imageCacheGraceTime = 9.0f;

		public int leftEyeLayer = 11;
		public int rightEyeLayer = 12;
		
		// If being loaded by another scene, this should point to the ToolbeltManager of that scene
		public ToolbeltManager masterManager;
		//populate this with extra things that get killed if this is not a standalone scene. e.g cursor
		public Transform[] sceneExtras; 
		public Transform sceneParent;
		protected icMediaLoader mediaLoader;
		protected icMediaLoader soundLoader;
		protected icImageCacher imageCacher;
		
		public Transform sceneCamera;
		public bool killCameraIfNotFirstInstance = true;
		protected Transform toolbeltCamera;
		StereoMode currentEye;
		
		protected Camera masterCamera;
		
		private static int numToolBeltManagers = 0;
		[SerializeField]
		protected int uniqueID;
		private bool assignedUniqueID = false;
		
		public bool hasUniqueID() {
			return assignedUniqueID;
		}
		
		public int getUniqueID() {
			if (!this.assignedUniqueID) {
				throw new Exception("Attempted to get UniqueID before it was assigned!");
			}
			return this.uniqueID;
		}
		
		public static int GenerateUniqueID() {
			return ToolbeltManager.numToolBeltManagers++;
		}
		
		public void assignUniqueID(int id) {
			if (this.assignedUniqueID) {
				throw new Exception("Attempted to double-assign a unique id to toolbeltmanager");
			}
			this.assignedUniqueID = true;
			this.uniqueID = id;		
		}
		
		static public ToolbeltManager FirstInstance {
			get {
				return instance;
			}
		}
		
		// Get the manager that is actually in use
		public ToolbeltManager ActiveManager {
			get {
				if (masterManager != null) {
					return masterManager.ActiveManager;
				} else {
					return this;
				}
			}
		}
		
		public icMediaLoader MediaLoader {
			get {			
				return this.ActiveManager.mediaLoader;						
			}
		}
		
		public icMediaLoader SoundLoader {
			get {
				return this.ActiveManager.soundLoader;
			}
		}
		/// Override this in subclass to do other things before and after our Awake
		protected virtual void PreAwakeSubclass(){}
		protected virtual void AwakeSubclass(){}
		
		
		void Awake() 
		{
			this.PreAwakeSubclass();
			// The first ToolbeltManager to start up will be stored
			if (instance == null) {
				instance = this;
				this.mediaLoader = new icMediaLoader(this.concurrentMediaLoads, useBink);
				this.soundLoader = new icMediaLoader(this.concurrentMediaLoads, false);
				if (imageCacheMBs > 0)
					imageCacher = new icImageCacher(imageCacheMBs, imageCacheGraceTime);
				
				this.assignUniqueID(ToolbeltManager.GenerateUniqueID());
				this.ActivateSceneParent();
				
			}
			this.AwakeSubclass();
			// TODO: don't initialise singletons if being loaded by another scene
			// Scene loading code should look for ToolbeltManager and set
			// parameters appropriately
		}
		
		public void ActivateSceneParent() {
			if (this.sceneParent != null) {
				this.sceneParent.gameObject.SetActive(true);
			}
		}
		/// Use this for initialization
		void Start () 		
		{
			this.PreStartSubclass();
			if (this.imageCacher != null) {
				StartCoroutine( this.imageCacher.CleanupCoroutine(Mathf.Max (0, this.imageCacheCleanupInterval) ) );
			}
			
			if (ToolbeltManager.FirstInstance != this) {							
				//this scene was loaded. Use previous scene's camera, so have to kill this scene's one.
				if (this.killCameraIfNotFirstInstance)
					this.KillAttachedCamera();
				this.KillSceneExtras();
			} else {
				DebugPanel dp = gameObject.AddComponent<DebugPanel>();
				dp.dropSkin = Resources.Load("Skins/DropSkin") as GUISkin;
//				ToolbeltManager.debugPanel = dp;
				
			}
			
			this.toolbeltCamera = ToolbeltManager.FirstInstance.sceneCamera;
			
			this.StartSubclass();
		}
		
		public void KillAttachedCamera() {
			Destroy(this.sceneCamera.gameObject);
			this.sceneCamera = null;
		}
		
		protected virtual void KillSceneExtras() {
			foreach (Transform t in this.sceneExtras) {
				Destroy (t.gameObject);
			}
			this.sceneExtras = new Transform[0];
		}
		

		/// Override this in subclass to do other things before our Start()
		protected virtual void PreStartSubclass() {
			
		}
		/// Override this in subclass to do other things on Start()
		/// This will always be called at the end of Start() so singletons
		/// are guaranteed to be loaded already
		protected virtual void StartSubclass() {
			
		}
		
		// Update is called once per frame
		void Update () 
		{
			if (mediaLoader != null) mediaLoader.Update();

			// Showing/hiding the camera ghost outline
			if (this.showCameraGhost != this._prevShowCameraGhost) {
				this._prevShowCameraGhost = this.showCameraGhost;
				if (this.showCameraGhost) {
					Debug.Log("Showing camera ghost");
				} else {
					Debug.Log("Hiding camera ghost");
				}
				this.toolbeltCamera.SendMessage("MsgShowGhost", this.showCameraGhost);
			}

			UpdateSubclass();
		}
		
		protected virtual void UpdateSubclass() {		
		}
		
		void OnApplicationQuit()
		{
			
		}
		
		public void ParentToMe(GameObject go) {
			go.transform.parent = this.transform;	
		}
		
		/// Make a quad parented to this ToolbeltManager's root
		public GameObject CreatePrimitiveQuad() 
		{
			GameObject q = GameObject.CreatePrimitive(PrimitiveType.Quad);
			q.transform.parent = this.transform;
			
			// We don't use the mesh collider, so get rid of it
			UnityEngine.Object.Destroy(q.collider);	
			return q;
		}
		
		/// Convenience function for making a primitive quad from scratch with an icImageMaterial,
		/// parented to this ToolbeltManager
		public GameObject CreateImageQuad(string filePath) 
		{
			GameObject q = CreatePrimitiveQuad();
			this.MediaLoader.CreateImageMaterial(filePath, q);

			return q;
		}
		
		/// Convenience function for making a primitive quad from scratch with an icBinkMaterial,
		/// parented to this ToolbeltManager
		public GameObject CreateBinkQuad(string filePath)
		{
			GameObject q = CreatePrimitiveQuad();
			this.MediaLoader.CreateBinkMaterial(filePath, q);
			return q;
		}
		
		// AddImageShadingScript to gameobject.
		public ImageShaderScript AddImageShaderScript(GameObject go) {
			return this.AddImageShaderScript(go, false);
		}
		
		public ImageShaderScript AddImageShaderScript(GameObject go, bool separatePass) {
			return this.AddImageShaderScript(go, separatePass, ImageShaderScript.StereoTypes.none);
		}
			
		public ImageShaderScript AddImageShaderScript(GameObject go, bool separatePass, ImageShaderScript.StereoTypes stereoType)
		{
			ImageShaderScript script = go.AddComponent<ImageShaderScript>();
			
			script.separatePass = separatePass;
			script.stereoType = stereoType;
			
			return script;
		}
		
		protected int loadScene(string sceneName){	
			int id = GenerateUniqueID();
			StartCoroutine(loadSceneAsync(sceneName, id));
			return id;
		}
		
		private IEnumerator loadSceneAsync(string sceneName, int id){
			//load scene a
			AsyncOperation async = Application.LoadLevelAdditiveAsync(sceneName);		
			yield return async;						
			
			//asdfasdf
			
			ToolbeltManager[] tbms = FindObjectsOfType(typeof(ToolbeltManager)) as ToolbeltManager[];
			ToolbeltManager tm = null;
			
			foreach (ToolbeltManager tbm in tbms) {
				if (!tbm.assignedUniqueID && tbm.gameObject.name == "__"+sceneName) {
					tm = tbm;
					tm.assignUniqueID(id);
					break;
				}
			}
			
			if (tm == null) {
				throw new MissingReferenceException("Component incorrectly named or does not include a Scene script: " + sceneName);
			} else {
				tm.JustFinishedAsyncLoad(this);
				this.DoneLoadOtherScene(tm);
			} 		
		}
		
		protected void JustFinishedAsyncLoad(ToolbeltManager tm) { 
			//called when this object was loaded async by another tbm		
			this.masterManager = tm;
			this.transform.parent = tm.transform;
		}
		
		//called when another object finished loading by this tm
		protected virtual void DoneLoadOtherScene(ToolbeltManager tm) { 
			//you can identify the other tm via its uniqueID.
			
		}

		// TODO: hook up to the camera prefab or config to figure out how tall the screen is
		public float GetScreenHeight() {
			return 4.0f;
		}

		// TODO: as above
		public float GetScreenRadius() {
			return 5.0f;
		}

		public Transform GetCameraTransform() {
			return this.toolbeltCamera;
		}

		// TODO: return left/right eye depending on render pass
		public StereoMode GetCurrentEye()
		{
			return this.currentEye;
		}
		
		public void SetCurrentEye(StereoMode stereoMode) {
			this.currentEye = stereoMode;
		}
		

		public int GetStereoLayer(StereoMode stereoMode)
		{
			switch (stereoMode)
			{
			case StereoMode.LEFT:
				return this.leftEyeLayer;			

			case StereoMode.RIGHT:
				return this.rightEyeLayer;

			default:
				return 0;

			}
		}


	}
}