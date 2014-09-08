using System.Text;
using UnityEngine;
using System.Collections.Generic;

namespace icExtensionMethods
{
	public static class MyExtensions
	{	

		public static List<T> GetComponentFromList<T> (this List<Transform> items) where T:Component
		{
			List<T> result = new List<T> ();
			
			foreach (Transform trans in items) {
				T component = trans.gameObject.GetComponent<T> ();
				if (component != null) {
					result.Add (component);
				}
			}
			return result;
		}
		
		public static List<T> GetComponentsInChildrenFromList<T> (this List<Transform> items) where T:Component
		{
			List<T> result = new List<T> ();
			
			foreach (Transform trans in items) {
				T[] components = trans.gameObject.GetComponentsInChildren<T> ();
				foreach (T component in components) {
					result.Add (component);
				}
			}
			return result;
		}
		
		public static Color MoveTowards (this Color orig, Color target, float amount)
		{
			Color result = new Color (orig.r, orig.g, orig.b, orig.a);
			
			result.r = Mathf.MoveTowards (result.r, target.r, amount);
			result.g = Mathf.MoveTowards (result.g, target.g, amount);
			result.b = Mathf.MoveTowards (result.b, target.b, amount);
			result.a = Mathf.MoveTowards (result.a, target.a, amount);
			return result;
		}
		
		public static float MoveTowards (float orig, float target, float amount)
		{
			//moves orig towards target by amount. Clamps to target if overshot.
			float result = orig;
			if (orig < target) {
				result = Mathf.Min (orig + amount, target);
			} else if (orig > target) {
				result = Mathf.Max (orig - amount, target);
			} else {
				result = target;
			}
			return result;
			
		}
		//a positive modulo for floats
		public static float PositiveMod (float x, float m)
		{
			float r = x % m;
			return r < 0f ? r + m : r;
		}
		// positive modulo for ints
		public static int PositiveMod (int x, int m)
		{
			int r = x % m;
			return r < 0 ? r + m : r;
		}
		
		public static float ModularDistance (float a, float b, float m)
		{
			//distance between a and b, given a modulo of m
			return Mathf.Min (PositiveMod (a - b, m), PositiveMod (b - a, m));
		}
		
		public static T GetOrAddComponent<T> (this GameObject go) where T:Component
		{
			T result = go.GetComponent<T> ();
			if (result == null) {
				result = go.AddComponent<T> ();
			}
			return result;
		}

		public static float ModularDirection (float a, float b, float m)
		{
			float dist1 = PositiveMod (b - a, m);
			float dist2 = PositiveMod (a - b, m);
			if (dist1 < dist2) {
				return 1;
			} else {
				return -1;
			}
		}
		
		public static string Multiply (this string source, int multiplier)
		{
			StringBuilder sb = new StringBuilder (multiplier * source.Length);
			for (int i = 0; i < multiplier; i++) {
				sb.Append (source);
			}
		
			return sb.ToString ();
		}
		
		public static int Contains<T> (this T[] source, T item)
		{
			for (int i = 0; i < source.Length; i++) {
				if (source [i].Equals (item)) {
					return i;
				}
			}
			return -1;
		}

		public static MovieTexture GetMovieTexture (this Transform t)
		{
			Texture texture = t.renderer.material.mainTexture;
			if (texture) {
				return texture as MovieTexture;
			}
			return null;
		}

		public static bool getRender (this Transform t)
		{
			if (t.renderer) {
				return t.renderer.enabled;
			}
			return false;
		}

		public static void setRender (this Transform t, bool render)
		{
			if (t.renderer) {
				t.renderer.enabled = render;
			}			
		}
		
		public static Vector3 getLocalPosition (this MonoBehaviour mb)
		{
			return mb.transform.localPosition;
		}

		public static Vector3 getPosition (this MonoBehaviour mb)
		{
			return mb.transform.position;
		}

		public static Vector3 getLocalScale (this MonoBehaviour mb)
		{
			return mb.transform.localScale;
		}
		
		public static Quaternion getLocalRotation (this MonoBehaviour mb)
		{
			return mb.transform.localRotation;
		}

		public static void setLocalPosition (this MonoBehaviour mb, Vector3 pos)
		{
			mb.transform.localPosition = pos;
		}
		
		public static void setPosition (this MonoBehaviour mb, Vector3 pos)
		{
			mb.transform.position = pos;
		}
		
		public static void setLocalScale (this MonoBehaviour mb, Vector3 scale)
		{
			mb.transform.localScale = scale;
		}

		public static void setLocalRotation (this MonoBehaviour mb, Quaternion q)
		{
			mb.transform.localRotation = q;
		}

		public static bool between (this float f, float from, float to)
		{
			return (f >= from && f <= to);
		}
		

	}
}