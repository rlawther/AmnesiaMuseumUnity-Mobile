using UnityEngine;
using System.Collections;

//ATTACH THIS SCRIPT TO AVIE PREFAB
//Requires standard assets/ImageEffects
namespace Toolbelt
{
	public class AvieMotionBlur : MonoBehaviour, icDebugGUI
	{
		private bool initialized = false;
		// Use this for initialization
		private Transform clusterManager = null;
		
		public float blurAmount = 0.8f;
		private float _blurAmount = 0.8f;
		public bool extraBlur = false;
		private bool _extraBlur = false;
		
		private icMotionBlur[] myBlur;
		icGUIHelpers.FloatPicker blurPicker;
		void Start ()
		{
			this.blurPicker = new icGUIHelpers.FloatPicker("Blur Amount", 0.0f, 0.92f);
			
			this.clusterManager = this.transform.FindChild ("Cluster Manager");
			
		}

		// Update is called once per frame
		void Update ()
		{
			if (!this.initialized) {
				this.tryInitialize ();
			} else {
				if (this.blurAmount != this._blurAmount) {
					this._blurAmount = this.blurAmount;
					this.updateBlurAmount();
				} else if (this._extraBlur != this.extraBlur) {
					this._extraBlur = this.extraBlur;				
					this.updateExtraBlur();
				}
			}
		}

		private delegate void updateBlur(icMotionBlur mb);
		
		private void propagateChange(updateBlur change) {
			foreach (icMotionBlur mb in this.myBlur) {
				change(mb);			
			}
		}

		private void updateBlurAmount() {
			updateBlur blurAmt = (x) => { x.blurAmount = this.blurAmount; };
			this.propagateChange(blurAmt);		
		}
		private void updateExtraBlur() {
			updateBlur blurAmt = (x) => { x.extraBlur = this.extraBlur; };
			this.propagateChange(blurAmt);		
		}
		
		
		private void tryInitialize ()
		{

			if (this.clusterManager.childCount == 0)
				return;
			this.initialized = true;
			StartCoroutine(this._initialize());
		}
	
		private IEnumerator _initialize () 
		{
			//we have to wait till end of frame before doing all our stuff - otherwise it breaks.
			yield return new WaitForEndOfFrame();
			this.myBlur = new icMotionBlur[this.clusterManager.childCount];
			int index = 0;
			foreach (Transform child in this.clusterManager) {			
				foreach (Transform child2 in child) {
					if (child2.name.Contains (").Projector(")) {
						icMotionBlur mb = child2.gameObject.AddComponent<icMotionBlur> ();
						mb.shader = Shader.Find ("Hidden/icMotionBlur");
						this.myBlur[index] = mb;
						index++;
					}
				}
			
			
			}
		}
		public void DrawDebugGUI () {
			GUILayout.BeginVertical();
			this.blurAmount = this.blurPicker.DrawGUI(this.blurAmount);			
			this.extraBlur = GUILayout.Toggle(this.extraBlur, "Extra Blur");
			GUILayout.EndVertical();
		}
	}
}