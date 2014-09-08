using UnityEngine;
using System.Collections;
using System;

namespace icGUIHelpers {
	public static class FORMAT {
		public const string AUTO = "G";
		public const string THREE_DP = "0.000";
		public const string SIX_DP = "0.000000";
	}

	public class icGUI {
		public static float FloatSlider(float val,float low, float high, int? sliderWidth = null) {
			//Adds a horizontal slider using GUILayout
			if (sliderWidth != null)
				return  GUILayout.HorizontalSlider(val,low, high,GUILayout.Width ((float)sliderWidth));
			else 
				return  GUILayout.HorizontalSlider(val,low, high);
		}
		public static bool BoolHeading(bool input, string headingName) 
		{
			GUILayout.BeginHorizontal(GUI.skin.box);
			bool result = GUILayout.Toggle(input, "");
			GUILayout.FlexibleSpace();
			GUILayout.Label(headingName);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			return result;
		}
	}


	public abstract class TogglePicker<T> {
		protected string caption;
		protected int? sliderWidth;
		public bool enabled = false;
		public TogglePicker (string caption, int? sliderWidth = null) {
			this.caption = caption;
			this.sliderWidth = sliderWidth;
		}

		public T DrawGUI(T input) {
			GUILayout.BeginVertical();
			GUILayout.BeginHorizontal ();
			if(GUILayout.Button (caption)) {
				enabled = !enabled;
			}

			this.drawLabels(input);
			GUILayout.EndHorizontal();
			if (enabled)
				input = this.drawSliders(input);
			GUILayout.EndVertical();
			return input;
		}
		
		protected abstract void drawLabels(T input);
		protected abstract T drawSliders(T input);

	}

	public class RotationEditor
	{
		public bool edit = false;
		public float angle = 0.0f;
		public Vector3 axis = new Vector3(0,0,0);
		
		public Transform target = null;
		
		public void initialize(float angle, Vector3 axis, Transform target) {
			if (this.target != target) {
				this.angle = angle;
				this.axis = axis;
				this.target = target;				
			}
		}
		
	}

	
	public class FloatPicker : TogglePicker<float> {
		protected float min;
		protected float max;
		public FloatPicker(string caption, float min, float max, int? sliderWidth = null)
			: base(caption, sliderWidth)			
		{
			this.min = min;
			this.max = max;
		}
		protected override void drawLabels(float f) {
			GUILayout.Label (f.ToString (FORMAT.AUTO));
		}
		protected override float drawSliders(float f) {
			f = icGUI.FloatSlider (f, this.min, this.max, this.sliderWidth);
			return f;
		}
	}
	public class RotationPicker : TogglePicker<AngleAxis> {
		public RotationPicker(string caption, int? sliderWidth = null) 
			: base(caption,sliderWidth)
		{

		}
		protected override void drawLabels(AngleAxis aa) {
			GUILayout.Label ("Angle: " + aa.angle.ToString(FORMAT.THREE_DP));
			GUILayout.Label ("x: " + aa.axis.x.ToString(FORMAT.THREE_DP));
			GUILayout.Label ("y: " + aa.axis.y.ToString(FORMAT.THREE_DP));
			GUILayout.Label ("z: " + aa.axis.z.ToString(FORMAT.THREE_DP));
		}

		protected override AngleAxis drawSliders (AngleAxis input)
		{
			input.angle = icGUI.FloatSlider (input.angle, 0, 360f, this.sliderWidth);
			input.axis.x = icGUI.FloatSlider (input.axis.x, 0, 1f, this.sliderWidth);
			input.axis.y = icGUI.FloatSlider (input.axis.y, 0, 1f, this.sliderWidth);
			input.axis.z = icGUI.FloatSlider (input.axis.z, 0, 1f, this.sliderWidth);
			return input;
		}

	}
	public class Vector3Picker : TogglePicker<Vector3> {
		protected float low;
		protected float high;
		
		protected string[] labels = new string[] {"X", "Y", "Z"};
		public Vector3Picker (string caption, float low, float high, int? sliderWidth = null) 
			: base(caption,sliderWidth)
		{
			this.low = low;
			this.high = high;
		}
		public void setLabels(string[] newLabels) {
			if (newLabels.Length != 3) {
				throw new ArgumentException("Argument must be an array of 3 strings!");
			}
			this.labels = newLabels;
		}
		
		protected override void drawLabels(Vector3 v) 
		{
			GUILayout.Label (this.labels[0]+": " + v.x.ToString (FORMAT.THREE_DP));
			GUILayout.Label (this.labels[1]+": " + v.y.ToString (FORMAT.THREE_DP));
			GUILayout.Label (this.labels[2]+": " + v.z.ToString (FORMAT.THREE_DP));
		}

		protected override Vector3 drawSliders(Vector3 v) 
		{
			v.x = icGUI.FloatSlider(v.x, this.low, this.high, this.sliderWidth);
			v.y = icGUI.FloatSlider(v.y, this.low, this.high, this.sliderWidth);
			v.z = icGUI.FloatSlider(v.z, this.low, this.high, this.sliderWidth);
			return v;
		}
	}
	public class ColourPicker : TogglePicker<Color> {
		private bool monochrome = false;
		private bool alpha = true;
		private bool processMono = true;
		public ColourPicker(string caption, bool alpha = true, bool processMono = true,int? sliderWidth = null) 
			: base(caption,sliderWidth) 
		{
			this.alpha = alpha;
			this.processMono = processMono;
		}

		protected override void drawLabels(Color c) {
			GUILayout.Label ("R: " + c.r.ToString (FORMAT.THREE_DP));
			GUILayout.Label ("G: " + c.g.ToString (FORMAT.THREE_DP));
			GUILayout.Label ("B: " + c.b.ToString (FORMAT.THREE_DP));
			if (this.alpha)	GUILayout.Label ("A: " + c.a.ToString (FORMAT.THREE_DP));
		}

		protected override Color drawSliders(Color c) {
			Color result = c;

			result.r = icGUI.FloatSlider(c.r, 0, 1f, sliderWidth);
			result.g = icGUI.FloatSlider(c.g, 0, 1f, sliderWidth);
			result.b = icGUI.FloatSlider(c.b, 0, 1f, sliderWidth);
			if (this.alpha) result.a = icGUI.FloatSlider(c.a, 0, 1f, sliderWidth);
			if (this.processMono) 
				this.monochrome = GUILayout.Toggle(this.monochrome,"Monochrome");
			
			if (this.monochrome) {
				float val = result.r;
				
				if (result.g != c.g) val = result.g;
				else if (result.b != c.b) val = result.b;
				result.r = result.b = result.g = val;
			}

			return result;
		}

	}
}
