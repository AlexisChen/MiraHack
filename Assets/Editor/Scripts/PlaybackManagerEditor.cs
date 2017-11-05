using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlaybackManager))]
sealed class PlaybackManagerEditor : Editor {
    private int _beatInput, _subdivInput;

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        var playback = (PlaybackManager) target;

        var isPlaying = playback.Playhead != null && playback.Playhead.IsPlaying;
        var playingString = isPlaying ? "Playing" : "Stopped";
        var timeString = "-";
        var beatString = "-";
        var measureString = "-";
        if (isPlaying) {
            timeString = string.Format("{0:F} s", playback.Playhead.CurrentTime);

            if (playback.Playhead.CurrentBeat < 0) {
                beatString = "Delayed";
                measureString = "Delayed";
            } else {
                beatString = string.Format("{0:00}.{1:0}", playback.Playhead.CurrentBeat,
                    playback.Playhead.CurrentSubdivision);
                measureString = string.Format("{0:00}:{1:0} {2}", playback.Playhead.CurrentMeasure + 1,
                    playback.Playhead.CurrentMeasureBeat + 1, playback.Playhead.CurrentMeasureBeat == 0 ? '*' : ' ');
            }
        }

        EditorGUILayout.Space();

        var restoreEnabled = GUI.enabled;
        GUI.enabled = Application.isPlaying && !isPlaying;
        if (GUILayout.Button("Play")) {
            playback.Play();
        }
        GUI.enabled = restoreEnabled;

        GUILayout.BeginVertical(GUI.skin.box);
        {
            GUILayout.Label(playingString, EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Time", timeString);
            EditorGUILayout.LabelField("Beat/Subdivision", beatString);
            EditorGUILayout.LabelField("Measure", measureString);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            {
                _beatInput = EditorGUILayout.IntField(_beatInput);
                GUILayout.Label(".");
                _subdivInput = EditorGUILayout.IntField(_subdivInput);
                GUILayout.Space(7);

                restoreEnabled = GUI.enabled;
                GUI.enabled = Application.isPlaying &&
                    playback.Playhead != null &&
                    (_beatInput > playback.Playhead.CurrentBeat ||
                    (_beatInput == playback.Playhead.CurrentBeat &&
                    _subdivInput > playback.Playhead.CurrentSubdivision));
                if (GUILayout.Button("Skip Forward", EditorStyles.miniButton) && playback.Playhead != null) {
                    playback.Playhead.SeekTo(_beatInput, _subdivInput);
                }
                GUI.enabled = restoreEnabled;
            }
            EditorGUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }

    public override bool RequiresConstantRepaint() {
        return ((PlaybackManager) target).Playhead != null;
    }
}
