using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SoundPlaybackData))]
public class SoundPlaybackDataPropertyDrawer : PropertyDrawer
{
    private SoundManager SoundManager => ProjectContext.Instance.SoundManager;
    private bool _isExpanded;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(foldoutRect, property.isExpanded, label);

        if (_isExpanded = property.isExpanded)
        {
            var clipRect = new Rect(position.x + 20, position.y + 20f, position.width - 20, EditorGUIUtility.singleLineHeight);
            var volumeRect = new Rect(position.x + 20, position.y + 40f, position.width - 20, EditorGUIUtility.singleLineHeight);
            var pitchRect = new Rect(position.x + 20, position.y + 60f, position.width - 20, EditorGUIUtility.singleLineHeight);
            var typeRect = new Rect(position.x + 20, position.y + 80f, position.width - 20, EditorGUIUtility.singleLineHeight);
            var playRect = new Rect(position.x + 20, position.y + 100f, position.width - 20, EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(clipRect, property.FindPropertyRelative("clip"), new GUIContent("Audio Clip"));
            EditorGUI.PropertyField(volumeRect, property.FindPropertyRelative("volume"), new GUIContent("Volume"));
            EditorGUI.PropertyField(pitchRect, property.FindPropertyRelative("pitchRange"), new GUIContent("Pitch Range"));
            EditorGUI.PropertyField(typeRect, property.FindPropertyRelative("type"), new GUIContent("Type"));

            if (GUI.Button(playRect, "Preview"))
            {
                if (EditorApplication.isPlaying)
                {
                    var sound = fieldInfo.GetValue(property.serializedObject.targetObject) as SoundPlaybackData;
                    SoundManager.PlaySound(sound, Camera.main.transform.position);
                }
                else
                {
                    Debug.LogError("Sound can be previewed during play mode ONLY!");
                }
            }
        }

        EditorGUI.EndFoldoutHeaderGroup();
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int lines = _isExpanded ? 7 : 1;
        return (20 - EditorGUIUtility.singleLineHeight) + (EditorGUIUtility.singleLineHeight * lines);
    }
}