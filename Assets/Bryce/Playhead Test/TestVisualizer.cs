using UnityEngine;

namespace Bryce {
    public class TestVisualizer : MonoBehaviour {
        // Add StemData fields to let stems be selected in the inspector
        [SerializeField] private StemData _stem;

        [SerializeField] private Transform _beatTransform;
        [SerializeField] private Transform _noteTransform;
        [SerializeField] private Transform _volumeTransform;
        [SerializeField] private TextMesh _stemText;
        [SerializeField] private MeshRenderer[] _subdivisions;

        private StemPlayhead _stemPlayhead;
        private MaterialPropertyBlock _propertyBlock;
        private float _volumeDamped;

        private void Start() {
            // Get the stem playheads for the stems configured in the inspector
            _stemPlayhead = PlaybackManager.Instance.Playhead.GetStemPlayhead(_stem);

            // Subscribe to beat and note events from the Playhead and stems
            // OnBeat will be called on each song beat
            // OnSubdivision will be called on each beat subdivision
            // OnNote will be called when each stem has a note
            PlaybackManager.Instance.Playhead.Beat += OnBeat;
            PlaybackManager.Instance.Playhead.Subdivision += OnSubdivision;
            PlaybackManager.Instance.Playhead.SongEnd += OnSongEnd;
            _stemPlayhead.Note += OnNote;

            // Play shouldn't really be called from a visualizer
            PlaybackManager.Instance.Play();

            _propertyBlock = new MaterialPropertyBlock();
        }

        private void OnDestroy() {
            // Make sure to unsubscribe from each event when the script is destroyed,
            // otherwise we might get null reference exceptions
            if (PlaybackManager.HasInstance) {
                PlaybackManager.Instance.Playhead.Beat -= OnBeat;
                PlaybackManager.Instance.Playhead.Subdivision -= OnSubdivision;
                PlaybackManager.Instance.Playhead.SongEnd -= OnSongEnd;
            }
            _stemPlayhead.Note -= OnNote;
        }

        private void Update() {
            // Animate the transforms shrinking back to normal
            var beatScale = _beatTransform.localScale.x;
            beatScale -= 5 * Time.deltaTime;
            if (beatScale < 1) beatScale = 1;
            _beatTransform.localScale = new Vector3(beatScale, beatScale, beatScale);

            var noteScale = _noteTransform.localScale.x;
            noteScale -= 5 * Time.deltaTime;
            if (noteScale < 1) noteScale = 1;
            _noteTransform.localScale = new Vector3(noteScale, noteScale, noteScale);

            // Get the volume of stem 2 and let it jump up but damp it going down so it lasts longer
            var volume = _stemPlayhead.GetVolume();
            if (volume > _volumeDamped) {
                _volumeDamped = volume;
            } else {
                _volumeDamped = Mathf.Lerp(volume, _volumeDamped, 0.5f);
            }

            // Scale the cube with the volume
            var volumeScale = _volumeTransform.localScale;
            volumeScale.y = _volumeDamped * 10;
            _volumeTransform.localScale = volumeScale;
        }

        private void OnBeat(int beat) {
            var isFirstBeat = PlaybackManager.Instance.Playhead.CurrentMeasureBeat == 0;

            // Scale larger on the first beat of each measure
            var scale = isFirstBeat ? 2f : 1.5f;
            _beatTransform.localScale = new Vector3(scale, scale, scale);
        }

        private void OnSubdivision(int beat, int subdivision) {
            // Set the current subdivision block color
            for (int i = 0; i < _subdivisions.Length; ++i) {
                if (i == subdivision) {
                    _propertyBlock.SetColor("_Color", Color.red);
                } else {
                    _propertyBlock.Clear();
                }
                _subdivisions[i].SetPropertyBlock(_propertyBlock);
            }
        }

        private void OnSongEnd() {
            Debug.Log("[TestVisualizer] Song ended :(");
        }

        private void OnNote(StemPlayhead playhead, int beat, int subdiv) {
            // Set the text to the name of the stem and scale the transform on each note
            _stemText.text = playhead.Stem.name;
            _noteTransform.localScale = Vector3.one * 1.5f;

//            var color = Color.HSVToRGB(Random.value, 1, 1);
//            _noteTransform.GetComponent<Renderer>().material.color = color;
        }
    }
}
