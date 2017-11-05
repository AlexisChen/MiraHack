using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Dancer : MonoBehaviour {
    [SerializeField] private bool _dancing;
    [SerializeField] private Renderer _renderer;

    private bool _currentDancing;
    private Animator _animator;
    private MaterialPropertyBlock _propertyBlock;

    public bool Dancing {
        get { return _dancing; }
        set {
            _dancing = value;
            SetDancing();
        }
    }

    private void Start() {
        _animator = GetComponent<Animator>();
        _propertyBlock = new MaterialPropertyBlock();

        SetDancing();
    }

    private void Update() {
        if (_dancing != _currentDancing) {
            _currentDancing = _dancing;
            SetDancing();
        }
    }

    private void SetDancing() {
        _animator.SetBool("Dancing", _dancing);
        if (_renderer) {
            _propertyBlock.SetColor("_Color", _dancing ? Color.white : Color.gray);
            _renderer.SetPropertyBlock(_propertyBlock);
        }
    }
}
