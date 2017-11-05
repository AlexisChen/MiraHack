using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StemData))]
sealed class StemDataEditor : Editor {
    private enum MetadataType {
        RMS
    }

    private MetadataType _typeInput;

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        var stemData = (StemData) target;

        EditorGUILayout.Space();
        GUILayout.Label("Audio Data", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        {
            _typeInput = (MetadataType) EditorGUILayout.EnumPopup(_typeInput);
            if (GUILayout.Button("Create", EditorStyles.miniButton)) {
                Load(_typeInput);
            }
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.BeginVertical(GUI.skin.box);
        {
            EditorGUILayout.LabelField("RMS",
                stemData.RmsData != null && stemData.RmsData.Length > 0 ? stemData.RmsData.Length + " points" : "-");

            if (GUILayout.Button("Clear All", EditorStyles.miniButton, GUILayout.ExpandWidth(false))) {
                ClearAll();
            }
        }
        GUILayout.EndVertical();
    }

    private void Load(MetadataType type) {
        var stemData = (StemData)target;

        EditorUtility.DisplayProgressBar("Stem Data", string.Format("Building {0}...", type), 0);

        switch (type) {
            case MetadataType.RMS:
                stemData.LoadRMS();
                break;
        }

        EditorUtility.ClearProgressBar();
    }

    private void ClearAll() {
        var stemData = (StemData)target;

        stemData.ClearData();
    }
}
