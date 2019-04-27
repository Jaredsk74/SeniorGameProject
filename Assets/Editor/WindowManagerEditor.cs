using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

// Associate this with the WindowManager
[CustomEditor(typeof(WindowManager))]
public class WindowManagerEditor : Editor {

	// Allow reordering of items
	private ReorderableList list;

	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		list.DoLayoutList();

		serializedObject.ApplyModifiedProperties();

		EditorGUILayout.HelpBox("Always regenerate the labels if the list is modified.", MessageType.Warning);

		// Custom button code
		if (GUILayout.Button("Generate Window Labels")) {
			// Grab the window array
			var windows = ((WindowManager)target).windows;

			// Build the enum list
			StringBuilder sb = new StringBuilder();
			sb.Append("public enum Windows {");
			sb.Append("None,");
			// Add each window name
			for (int i = 0; i < windows.Length; i++) {
				sb.Append(windows[i].name.Replace(" ", ""));
				if (i < windows.Length - 1) {
					sb.Append(",");
				}
			}
			sb.Append("}");

			// Write the file
			var path = EditorUtility.SaveFilePanel("Save Script", "", "WindowLabels", "cs");
			using(FileStream fs = new FileStream(path, FileMode.Create)) {
				using(StreamWriter sw = new StreamWriter(fs)) {
					sw.Write(sb.ToString());
				}
			}

			// Refresh Unity
			AssetDatabase.Refresh();
		}
	}

	// Method is automatically called when the editor gets focus
	private void OnEnable() {
		// Set the list equal to the array of windows
		list = new ReorderableList(serializedObject, serializedObject.FindProperty("windows"), true, true, true, true);

		// Edit the title
		list.drawHeaderCallback = (Rect rect) => {
			EditorGUI.LabelField(rect, "Windows");
		};

		list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
			// The element inside the array
			var element = list.serializedProperty.GetArrayElementAtIndex(index);
			// Give each element a field in Unity
			EditorGUI.PropertyField(new Rect(rect.x, rect.y, Screen.width - 75, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
		};
	}
}
