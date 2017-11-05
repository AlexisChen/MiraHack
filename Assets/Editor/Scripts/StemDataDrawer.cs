using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// A custom dropdown for StemData which only includes stems attached to SongData assets.
/// </summary>
[CustomPropertyDrawer(typeof(StemData))]
sealed class StemDataDrawer : PropertyDrawer {
    private List<GUIContent> _optionsList;
    private GUIContent[] _options;
    private List<StemData> _stemDataIndexes;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        // Allow choosing any Stem on Songs
        if (property.serializedObject.targetObject is SongData) {
            EditorGUI.PropertyField(position, property, label);
            return;
        }

        BuildOptionsList();

        int index = _stemDataIndexes.IndexOf(property.objectReferenceValue as StemData);
        bool wasUnknown = index < 0;

        // None is at 0, so that shifts everything right (and -1 -> 0)
        ++index;
        index = EditorGUI.Popup(position, label, index, _options);
        --index;

        // If the current value hasn't changed and it wasn't in the list originally,
        // then don't change it from that unknown value
        if (!wasUnknown || index >= 0) {
            property.objectReferenceValue = index >= 0 ? _stemDataIndexes[index] : null;
        }
    }

    private void BuildOptionsList() {
        var songs = AssetDatabase.FindAssets("t:" + typeof(SongData).Name)
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<SongData>)
            .ToArray();

        if (_optionsList == null) {
            _optionsList = new List<GUIContent> {
                new GUIContent("None")
            };
            _stemDataIndexes = new List<StemData>();
        } else {
            _optionsList.Clear();
            _optionsList.Add(new GUIContent("None"));
            _stemDataIndexes.Clear();
        }

        for (int songIndex = 0; songIndex < songs.Length; ++songIndex) {
            var song = songs[songIndex];
            if (song.MainStem != null) {
                _optionsList.Add(new GUIContent(song.name + "/(Main)"));
                _stemDataIndexes.Add(song.MainStem);
            }

            for (int stemIndex = 0; stemIndex < song.Stems.Length; ++stemIndex) {
                var stemName = song.Stems[stemIndex].name
                    .Replace(song.name, "")
                    .Trim();

                _optionsList.Add(new GUIContent(song.name + "/" + stemName));
                _stemDataIndexes.Add(song.Stems[stemIndex]);
            }
        }

        _options = _optionsList.ToArray();
    }
}
