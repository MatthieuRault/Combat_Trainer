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

    private float _rotationX = 0f; // rotation verticale (haut/bas)
    private Transform _playerBody;  // le corps du joueur tourne sur Y

    void Start()
    {
        // Le script est sur la caméra, le joueur est son parent
        _playerBody = transform.parent;

        // Cache et bloque la souris au centre de l'écran
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Lire le mouvement de la souris
        float mouseX = Mouse.current.delta.x.ReadValue() * sensitivityX;
        float mouseY = Mouse.current.delta.y.ReadValue() * sensitivityY;

        // Rotation verticale (caméra seulement)
        _rotationX -= mouseY;
        _rotationX = Mathf.Clamp(_rotationX, minY, maxY);
        transform.localRotation = Quaternion.Euler(_rotationX, 0f, 0f);

        // Rotation horizontale (tout le corps du joueur)
        _playerBody.Rotate(Vector3.up * mouseX);
    }
}