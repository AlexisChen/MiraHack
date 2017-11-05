using UnityEngine;

/// <summary>
/// Manages starting and playing songs.
/// </summary>
public sealed class PlaybackManager : MonoBehaviourSingleton<PlaybackManager> {
    /// <summary>
    /// The song to be played.
    /// </summary>
    [SerializeField]
    private SongData _song;

    /// <summary>
    /// Add the audio sources in the scene here.
    /// The first audio source will be filled with the song's main stem clip,
    /// and each additional audio source will be filled with a stem in the order that they're on the song.
    /// </summary>
    [SerializeField]
    private AudioSource[] _audioSources;

    [SerializeField]
    private bool _createAudioSources;

    [SerializeField]
    private bool _autoplay;

    private Playhead _playhead;

    public SongData Song {
        get { return _song; }
        set {
            _song = value;
            SetupSong();
        }
    }

    /// <summary>
    /// Get the song playhead.
    /// </summary>
    public Playhead Playhead {
        get { return _playhead; }
    }

    protected override void Awake() {
        base.Awake();

        _playhead = new Playhead();
        _playhead.Skip += OnPlayheadSkip;

        if (_createAudioSources) {
            _audioSources = new AudioSource[1 + _song.Stems.Length];
            for (int i = 0; i < _audioSources.Length; ++i) {
                var playbackObject = new GameObject("Playback Source " + i);
                playbackObject.transform.parent = transform;
                var audioSource = playbackObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                _audioSources[i] = audioSource;
            }
        }

        SetupSong();

        if (_autoplay) {
            Play();
        }
    }

    private void Update() {
        _playhead.Update();
    }

    /// <summary>
    /// Start playback.
    /// </summary>
    public void Play() {
        _playhead.Play();

        for (int i = 0; i < _audioSources.Length; ++i) {
            if (!_audioSources[i]) continue;

            _audioSources[i].Play();

            _audioSources[i].volume = (i == 0 && _song.MainStem && _song.MainStem.Autoplay) ||
                                      (i > 0 && i - 1 < _song.Stems.Length && _song.Stems[i - 1] && _song.Stems[i - 1].Autoplay)
                ? 1
                : 0;
        }
    }

    /// <summary>
    /// Get the AudioSource which is playing a certain stem.
    /// </summary>
    public AudioSource GetAudioSourceForStem(StemData stem) {
        int index;
        if (stem == _song.MainStem) {
            index = 0;
        } else {
            index = System.Array.IndexOf(_song.Stems, stem);
            if (index < 0) return null;
            ++index;
        }

        return index >= _audioSources.Length ? null : _audioSources[index];
    }

    private void OnPlayheadSkip(float timeDelta) {
        // Skip forward in each AudioSource to match the Playhead
        for (int i = 0; i < _audioSources.Length; ++i) {
            if (_audioSources[i] && _audioSources[i].clip) {
                var sampleDelta = (int) (timeDelta * _audioSources[i].clip.frequency);
                _audioSources[i].timeSamples += sampleDelta;
            }
        }
    }

    private void SetupSong() {
        _playhead.Song = _song;

        // Fill the AudioSources with the stem clips
        for (int i = 0; i < _audioSources.Length; ++i) {
            if (!_audioSources[i]) continue;

            if (i == 0) {
                if (_song.MainStem) {
                    _audioSources[i].clip = _song.MainStem.Clip;
                    _audioSources[i].loop = _song.MainStem.Loop;
                }
            } else if (i - 1 < _song.Stems.Length) {
                if (_song.Stems[i - 1]) {
                    _audioSources[i].clip = _song.Stems[i - 1].Clip;
                    _audioSources[i].loop = _song.Stems[i - 1].Loop;
                }
            }
        }
    }
}
