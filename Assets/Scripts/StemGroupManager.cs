using UnityEngine;

public sealed class StemGroupManager : MonoBehaviourSingleton<StemGroupManager> {
    [System.Serializable]
    private struct Group {
        public StemData[] Stems;
    }

    private struct ClipInfo {
        public AudioSource AudioSource;
        public StemPlayhead Playhead;
    }

    [SerializeField] private Group[] _groups;
    [SerializeField] private int[] _activeClips;

    public int[] debugClips;
    public bool debugApply;

    private ClipInfo[][] _liveClipInfo;

    public int GroupCount {
        get { return _groups.Length; }
    }

    public int[] ActiveClips {
        get { return _activeClips; }
    }

    private void Start() {
        _liveClipInfo = new ClipInfo[_groups.Length][];
        for (int groupIndex = 0; groupIndex < _groups.Length; ++groupIndex) {
            var groupSources = new ClipInfo[_groups[groupIndex].Stems.Length];
            for (int stemIndex = 0; stemIndex < groupSources.Length; ++stemIndex) {
                var stem = _groups[groupIndex].Stems[stemIndex];
                groupSources[stemIndex] = new ClipInfo {
                    AudioSource = PlaybackManager.Instance.GetAudioSourceForStem(stem),
                    Playhead = PlaybackManager.Instance.Playhead.GetStemPlayhead(stem)
                };
            }
            _liveClipInfo[groupIndex] = groupSources;
        }

        // Set none active initially
        _activeClips = new int[_groups.Length];
        for (int i = 0; i < _activeClips.Length; ++i) {
            _activeClips[i] = -1;
        }
    }

    private void Update() {
        if (debugApply) {
            debugApply = false;
            for (int i = 0; i < _groups.Length; ++i) {
                SetGroupClip(i, debugClips[i]);
            }
        }
    }

    public void SetGroupClip(int group, int clip) {
        if (clip == _activeClips[group]) return;

        // Mute previous
        var lastClip = _activeClips[group];
        if (lastClip >= 0) {
            SetStemMute(group, lastClip, true);
        }

        _activeClips[group] = clip;

        // Unmute new
        if (clip >= 0) {
            SetStemMute(group, clip, false);
        }
    }

    public StemPlayhead GetGroupPlayhead(int group) {
        return _activeClips[group] >= 0
            ? _liveClipInfo[group][_activeClips[group]].Playhead
            : null;
    }

    private void SetStemMute(int group, int clip, bool muted) {
        var stemInfo = _liveClipInfo[group][clip];
        stemInfo.AudioSource.volume = muted ? 0 : 1;

        if (muted) {
            stemInfo.Playhead.Stop();
        } else {
            stemInfo.Playhead.Play();
            stemInfo.Playhead.SeekTo(PlaybackManager.Instance.Playhead.CurrentTime %
                                     _groups[group].Stems[clip].Clip.length);
        }
    }
}
