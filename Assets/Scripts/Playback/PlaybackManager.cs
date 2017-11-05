using UnityEngine;

/// <summary>
/// Manages starting and playing songs.
/// </summary>
public sealed class PlaybackManager : MonoBehaviourSingleton<PlaybackManager> {
    /// <summary>
    /// The song to be played.
    /// </summary>
    [SerializeField] private SongData _song;

    /// <summary>
    /// Add the audio sources in the scene here.
    /// The first audio source will be filled with the song's main stem clip,
    /// and each additional audio source will be filled with a stem in the order that they're on the song.
    /// </summary>
    [SerializeField] private AudioSource[] _audioSources;

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

    /// <summary>
    /// Start playback.
    /// </summary>
    public void Play() {
        _playhead.Play();

        for (int i = 0; i < _audioSources.Length; ++i) {
            if (_audioSources[i]) _audioSources[i].Play();
        }
    }

    protected override void Awake() {
        base.Awake();

        _playhead = new Playhead();
        _playhead.Skip += OnPlayheadSkip;

        SetupSong();
    }

    private void Update() {
        _playhead.Update();
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
