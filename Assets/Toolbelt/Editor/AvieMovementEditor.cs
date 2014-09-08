using UnityEditor;
using UnityEngine; 


namespace Toolbelt {

[CustomEditor(typeof(AvieMovement))] 
public class AvieMovementEditor : Editor {

	public override void OnInspectorGUI() {
		AvieMovement myTarget = target as AvieMovement;
		myTarget.AviePosition = EditorGUILayout.Vector3Field("Avie Position", myTarget.AviePosition);
		myTarget.calculateAviePosition = EditorGUILayout.Toggle("Recalc Pos", myTarget.calculateAviePosition);
		myTarget.faceCentre = EditorGUILayout.Toggle ("Face Centre", myTarget.faceCentre);
		if (GUI.changed)
			EditorUtility.SetDirty(target);
	}
}
}