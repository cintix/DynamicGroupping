using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GroupDynamics))]
[CanEditMultipleObjects]
public class GroupDynamicsEditor : Editor {

	public override void OnInspectorGUI() {

		GroupDynamics script = (GroupDynamics) target;
		this.DrawDefaultInspectorWithoutScriptField () ;

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Dynamics Model Options", EditorStyles.boldLabel);

		if (script.dynamicModel == GroupDynamics.GroupModel.Circle) {
			var currentStyle = EditorStyles.label.fontStyle;
			EditorStyles.label.fontStyle = FontStyle.Normal;
			script.width = EditorGUILayout.FloatField(new GUIContent("Radius"), script.width, EditorStyles.numberField);
			if (script.width < 0) script.width = 0;
			EditorStyles.label.fontStyle = currentStyle;
		} else if (script.dynamicModel == GroupDynamics.GroupModel.Box) {
			var currentStyle = EditorStyles.label.fontStyle;
			EditorStyles.label.fontStyle = FontStyle.Normal;
			script.height = EditorGUILayout.FloatField(new GUIContent("Height"), script.height, EditorStyles.numberField);
			script.width = EditorGUILayout.FloatField(new GUIContent("Width"), script.width, EditorStyles.numberField);
			EditorStyles.label.fontStyle = currentStyle;
		} else if (script.dynamicModel == GroupDynamics.GroupModel.Rectangel) {
			var currentStyle = EditorStyles.label.fontStyle;
			EditorStyles.label.fontStyle = FontStyle.Normal;
			script.width = EditorGUILayout.FloatField(new GUIContent("Radius"), script.width, EditorStyles.numberField);
			EditorStyles.label.fontStyle = currentStyle;
		} else if (script.dynamicModel == GroupDynamics.GroupModel.Triangle) {
			var currentStyle = EditorStyles.label.fontStyle;
			EditorStyles.label.fontStyle = FontStyle.Normal;
			script.height = EditorGUILayout.FloatField(new GUIContent("Height"), script.height, EditorStyles.numberField);
			script.width = EditorGUILayout.FloatField(new GUIContent("Width"), script.width, EditorStyles.numberField);
			EditorStyles.label.fontStyle = currentStyle;
		}	

		EditorGUILayout.Space(); 
		EditorGUILayout.LabelField("Spawn options ", EditorStyles.boldLabel);

		if (script.spawnTypes.Count > 1) {
			var currentStyle = EditorStyles.label.fontStyle;
			EditorStyles.label.fontStyle = FontStyle.Normal;
			script.randomizeFollowers = EditorGUILayout.Toggle(new GUIContent("Randomize Spawners"), script.randomizeFollowers, EditorStyles.toggle);
			EditorStyles.label.fontStyle = currentStyle;
		} else {		
			script.randomizeFollowers = false;
		}		script.followTheLeader = EditorGUILayout.Toggle(new GUIContent("Follow Leader"), script.followTheLeader, EditorStyles.toggle);

		if (script.followTheLeader) {
			script.grouppingMode = (GroupDynamics.GrouppingMode) EditorGUILayout.EnumPopup(new GUIContent("Follow Group Mode"), script.grouppingMode);
		}

		if (script.height < 0) script.height = 0;
		if (script.width < 0) script.width = 0;


		if (script.spawnTypes.Count < 1) {
			EditorGUILayout.HelpBox("No follower type is set, so type is set to attached GameObject", MessageType.Info);
		}

		SceneView.RepaintAll();

	}
}

public static class GroupDynamicsEditor_EditorExtension {
	
	public static bool DrawDefaultInspectorWithoutScriptField ( this Editor Inspector ) {
		EditorGUI.BeginChangeCheck() ;
		Inspector.serializedObject.Update() ;

		SerializedProperty Iterator = Inspector.serializedObject.GetIterator() ;
		Iterator.NextVisible(true) ;

		while (Iterator.NextVisible(false)) {
			var currentStyle = EditorStyles.label.fontStyle;
			EditorStyles.label.fontStyle = FontStyle.Normal;
			EditorGUILayout.PropertyField(Iterator, true) ;
			EditorStyles.label.fontStyle = currentStyle;
		}

		Inspector.serializedObject.ApplyModifiedProperties();
		return (EditorGUI.EndChangeCheck());
	}
}