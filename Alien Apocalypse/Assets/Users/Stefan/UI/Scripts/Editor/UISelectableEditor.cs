using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UISelectable),true)]
[CanEditMultipleObjects]
public class UISelectableEditor : Editor
{
    UISelectable selectable;
    
    bool foldout;
    public override void OnInspectorGUI()
    {
        selectable = target as UISelectable;

        EditorGUILayout.BeginVertical(GUI.skin.box);
        foldout = EditorGUILayout.Foldout(foldout,"Interaction Settings",EditorStyles.foldoutHeader);
        if (foldout)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("interactionEvents"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("interactionSoundEffects"));

            EditorGUILayout.Space(8);
            selectable.Interactable = EditorGUILayout.ToggleLeft("Interactable", selectable.Interactable);
            selectable.DeriveFromParentSelectable = EditorGUILayout.ToggleLeft("Derive From Parent Selectable", selectable.DeriveFromParentSelectable);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.Space(8);
        EditorGUILayout.EndVertical();
        
        serializedObject.ApplyModifiedProperties();
    }
}
