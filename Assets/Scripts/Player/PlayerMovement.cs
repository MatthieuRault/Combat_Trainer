using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Vitesse")]
    public float walkSpeed = 5f;
    public float gravity = -9.81f;

    private CharacterController _controller;
    private Vector3 _velocity;

    void Start()
    {
        // On récupère le CharacterController qu'on a ajouté sur la capsule
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Lire les touches clavier (ZQSD ou WASD)
        float horizontal = 0f;
        float vertical = 0f;

        if (Keyboard.current.qKey.isPressed) horizontal = -1f;
        if (Keyboard.current.dKey.isPressed) horizontal = 1f;
        if (Keyboard.current.zKey.isPressed) vertical = 1f;
        if (Keyboard.current.sKey.isPressed) vertical = -1f;

        // Calculer la direction de déplacement
        Vector3 direction = transform.right * horizontal + transform.forward * vertical;

        // Déplacer le personnage
        _controller.Move(direction * walkSpeed * Time.deltaTime);

        // Appliquer la gravité
        if (_controller.isGrounded)
            _velocity.y = -2f; // petite valeur pour rester collé au sol
        else
            _velocity.y += gravity * Time.deltaTime;

        _controller.Move(_velocity * Time.deltaTime);
    }
}