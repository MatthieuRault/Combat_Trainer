using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Sensibilité")]
    public float sensitivityX = 2f;
    public float sensitivityY = 2f;

    [Header("Limite verticale")]
    public float minY = -60f;
    public float maxY = 60f;

    [Header("TPS / Zoom")]
    public float tpsHeight = 1.5f;
    public float tpsOffsetX = 0.5f;
    public float minDistance = 0f;
    public float maxDistance = 6f;
    public float scrollSpeed = 2f;
    public float switchToFPSThreshold = 0.3f;

    private float _rotationX = 0f;
    private Transform _playerBody;
    private bool _isTPS = false;
    private float _currentDistance = 4f;

    private Vector3 _fpsPosition = new Vector3(0f, 0.6f, 0f);

    [Header("Mesh joueur")]
    public MeshRenderer playerMesh;

    void Start()
    {
        _playerBody = transform.parent;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        transform.localPosition = _fpsPosition;
    }

    void Update()
    {
        HandleMouseLook();
        HandleZoom();

        if (_isTPS)
            UpdateTPSPosition();
    }

    void HandleMouseLook()
    {
        float mouseX = Mouse.current.delta.x.ReadValue() * sensitivityX;
        float mouseY = Mouse.current.delta.y.ReadValue() * sensitivityY;

        _rotationX -= mouseY;
        _rotationX = Mathf.Clamp(_rotationX, minY, maxY);
        transform.localRotation = Quaternion.Euler(_rotationX, 0f, 0f);
        _playerBody.Rotate(Vector3.up * mouseX);
    }

    void HandleZoom()
    {
        float scroll = Mouse.current.scroll.y.ReadValue();

        if (scroll != 0f)
        {
            _currentDistance -= scroll * scrollSpeed;
            _currentDistance = Mathf.Clamp(_currentDistance, minDistance, maxDistance);
        }

        if (Keyboard.current.vKey.wasPressedThisFrame)
        {
            if (_isTPS)
            {
                _currentDistance = 0f;
            }
            else
            {
                _currentDistance = 10f;
            }
        }

        if (_currentDistance <= switchToFPSThreshold)
        {
            if (_isTPS)
            {
                _isTPS = false;
                transform.localPosition = _fpsPosition;
            }
        }
        else
        {
            _isTPS = true;
        }
        if (playerMesh != null)
            playerMesh.enabled = _isTPS;
    }

    void UpdateTPSPosition()
    {
        Vector3 tpsOffset = new Vector3(tpsOffsetX, tpsHeight, -_currentDistance);
        transform.localPosition = tpsOffset;
    }
}