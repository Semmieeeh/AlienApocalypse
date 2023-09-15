using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;

[CustomEditor(typeof(UIButton),true)]
[CanEditMultipleObjects]
public class VCRButtonEditor : UISelectableEditor
{
    UIButton button;
    Vector2 scrollPosition;

    bool colorFoldOut;

    private SerializedProperty defaultColor;
    private SerializedProperty hoveredColor;
    private SerializedProperty clickedColor;
    private SerializedProperty selectedColor;
    private SerializedProperty disabledColor;

    private void OnEnable()
    {
        defaultColor = serializedObject.FindProperty("Colors.DefaultColor");
        hoveredColor = serializedObject.FindProperty("Colors.HoveredColor");
        clickedColor = serializedObject.FindProperty("Colors.ClickedColor");
        selectedColor = serializedObject.FindProperty("Colors.SelectedColor");
        disabledColor = serializedObject.FindProperty("Colors.DisabledColor");
    }
    public override void OnInspectorGUI()
    {
        button = target as UIButton;
        
        base.OnInspectorGUI();
        EditorGUILayout.LabelField("Button Settings", EditorStyles.boldLabel);

        button.TextField = (TextMeshProUGUI)EditorGUILayout.ObjectField("Text Element", button.TextField, typeof(TextMeshProUGUI), true);

        button.Background = (Image)EditorGUILayout.ObjectField("Background", button.Background, typeof(Image), true);

        button.Theme = (ButtonStyle)EditorGUILayout.EnumPopup("Theme" , button.Theme);

        if(button.Theme != ButtonStyle.Default)
        {
            button.Icon = (Sprite)EditorGUILayout.ObjectField("Icon", button.Icon, typeof(Sprite), true);
        }

        if (button.Theme != ButtonStyle.IconOnly)
        {

            EditorGUILayout.LabelField("Button Text");

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            button.Text = EditorGUILayout.TextField(button.Text, GUILayout.Height(50));

            button.TextAlignment = (ButtonTextAlignment)EditorGUILayout.EnumPopup("Text Alignment", button.TextAlignment);


            EditorGUILayout.EndScrollView();


        }

        colorFoldOut = EditorGUILayout.Foldout(colorFoldOut, "Colors");

        if (colorFoldOut)
        {
            EditorGUI.indentLevel++;
            serializedObject.Update();

            EditorGUILayout.PropertyField(defaultColor, new GUIContent("Default Color"), true);
            EditorGUILayout.PropertyField(hoveredColor, new GUIContent("Hovered Color"), true);
            EditorGUILayout.PropertyField(clickedColor, new GUIContent("Clicked Color"), true);
            EditorGUILayout.PropertyField(selectedColor, new GUIContent("Selected Color"), true);
            EditorGUILayout.PropertyField(disabledColor, new GUIContent("Disabled Color"), true );

            EditorGUI.indentLevel--;

            serializedObject.ApplyModifiedProperties();

        }

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("On Click",EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("OnClickEvent"));

        if(GUILayout.Button("Reset Button"))
        {
            button.Reset();
        }

        serializedObject.ApplyModifiedProperties();

    }
}
