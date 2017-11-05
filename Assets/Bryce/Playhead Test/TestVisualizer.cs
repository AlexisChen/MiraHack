using UnityEngine;

namespace Bryce {
    public class TestVisualizer : MonoBehaviour {
        [SerializeField] private Transform _beatTransform;
        [SerializeField] private Transform _noteTransform;
        [SerializeField] private Transform[] _groupVolumeTransforms;
        [SerializeField] private TextMesh _stemText;
        [SerializeField] private MeshRenderer[] _subdivisions;

        private MaterialPropertyBlock _propertyBlock;
        private float[] _volumeDamped;

        private void Start() {
            // Subscribe to beat and note events from the Playhead and stems
            // OnBeat will be called on each song beat
            // OnSubdivision will be called on each beat subdivision
            // OnNote will be called when each stem has a note
            PlaybackManager.Instance.Playhead.Beat += OnBeat;
            PlaybackManager.Instance.Playhead.Subdivision += OnSubdivision;
            PlaybackManager.Instance.Playhead.SongEnd += OnSongEnd;

            // Play shouldn't really be called from a visualizer
            PlaybackManager.Instance.Play();

            _propertyBlock = new MaterialPropertyBlock();

            _volumeDamped = new float[StemGroupManager.Instance.GroupCount];
        }

        private void OnDestroy() {
            // Make sure to unsubscribe from each event when the script is destroyed,
            // otherwise we might get null reference exceptions
            if (PlaybackManager.HasInstance) {
                PlaybackManager.Instance.Playhead.Beat -= OnBeat;
                PlaybackManager.Instance.Playhead.Subdivision -= OnSubdivision;
                PlaybackManager.Instance.Playhead.SongEnd -= OnSongEnd;
            }
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

            for (int g = 0; g < StemGroupManager.Instance.GroupCount; ++g) {
                // Get the volume of the stem and let it jump up but damp it going down so it lasts longer
                var playhead = StemGroupManager.Instance.GetGroupPlayhead(g);
                var volume = playhead != null ? playhead.GetVolume() : 0;
                if (volume > _volumeDamped[g]) {
                    _volumeDamped[g] = volume;
                } else {
                    _volumeDamped[g] = Mathf.Lerp(volume, _volumeDamped[g], 0.5f);
                }

                // Scale the cube with the volume
                var volumeScale = _groupVolumeTransforms[g].localScale;
                volumeScale.y = _volumeDamped[g] * 10;
                _groupVolumeTransforms[g].localScale = volumeScale;
            }
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
