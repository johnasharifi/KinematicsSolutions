#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IKManager))]
[CanEditMultipleObjects]
public class IKManagerEditor : Editor
{
    SerializedProperty solveMode;

    private void OnEnable()
    {
        solveMode = serializedObject.FindProperty("_solveMode");
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        if (solveMode != null)
        {
            EditorGUILayout.PropertyField(solveMode);
        }
        
        if (solveMode.enumValueIndex == (int) IKManager.SolveMode.step)
        {
            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            if (GUILayout.Button("Step"))
                ((IKManager)serializedObject.targetObject).UpdateSolveModeOnNodes();
            EditorGUI.EndDisabledGroup();
        }
        
        serializedObject.ApplyModifiedProperties();

        if (EditorGUI.EndChangeCheck())
        {
            ((IKManager)target).UpdateSolveModeOnNodes();
        }

        EditorGUI.BeginChangeCheck();

        bool nodesCanSway = GUILayout.Toggle(((IKManager)target).NodesCanSway, "Nodes can sway?");

        if (EditorGUI.EndChangeCheck())
        {
            // trigger IKManager -> IKNode set
            ((IKManager)target).NodesCanSway = nodesCanSway;
        }
    }
}

#endif