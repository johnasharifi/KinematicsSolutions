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

        if (EditorGUI.EndChangeCheck())
        {
            ((IKManager)target).UpdateSolveMode((IKManager.SolveMode) solveMode.enumValueIndex);
        }

        serializedObject.ApplyModifiedProperties();
    }
}

#endif