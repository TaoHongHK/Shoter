using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(SuperStateController))]
public class SuperStateControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SuperStateController targetScript = (SuperStateController)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Actions:");
        if(targetScript.m_actions != null)
        {
            EditorGUI.indentLevel++;
            int i = 1;
            foreach (var action in targetScript.m_actions)
            {
                EditorGUILayout.ObjectField("Action" + (i++), action, action.ActionType, true);
            }
            EditorGUI.indentLevel--;
        }

        this.Repaint();
    }

}
