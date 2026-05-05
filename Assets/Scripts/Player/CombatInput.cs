using UnityEngine;
using UnityEngine.InputSystem;

public class CombatInput : MonoBehaviour
{
    public CombatSystem myCombat;
    public CombatSystem enemyCombat;

    // La direction actuelle pointée par la souris
    private CombatDirection _currentDirection = CombatDirection.Right;

    void Update()
    {
        var keyboard = Keyboard.current;
        var mouse = Mouse.current;
        if (keyboard == null || mouse == null) return;

        // Met à jour la direction en temps réel selon la souris
        UpdateDirectionFromMouse(mouse);

        // Attaque : clic gauche
        HandleMouseAttack(mouse);

        // Bloc : clic droit maintenu
        HandleMouseBlock(mouse);

        // Touches clavier directes
        HandleKeyboard(keyboard);
    }

    void UpdateDirectionFromMouse(Mouse mouse)
    {
        // On récupère le delta de la souris (mouvement relatif)
        Vector2 delta = mouse.delta.ReadValue();

        // On ne met à jour que si la souris bouge vraiment
        if (delta.magnitude < 0.5f) return;

        _currentDirection = GetDirectionFromDelta(delta);
    }

    void HandleMouseAttack(Mouse mouse)
    {
        // Clic gauche = attaque dans la direction actuelle
        if (mouse.leftButton.wasPressedThisFrame)
        {
            myCombat.ReleaseBlock();
            myCombat.Attack(_currentDirection, enemyCombat);
            Debug.Log($"Attaque : {_currentDirection}");
        }
    }

    void HandleMouseBlock(Mouse mouse)
    {
        // Clic droit maintenu = bloc dans la direction actuelle
        if (mouse.rightButton.isPressed)
        {
            myCombat.SetBlock(_currentDirection);
        }

        // Clic droit relâché = on arrête de bloquer
        if (mouse.rightButton.wasReleasedThisFrame)
        {
            myCombat.ReleaseBlock();
        }
    }

    void HandleKeyboard(Keyboard keyboard)
    {
        // Touches d'attaque directes (remappables plus tard dans les options)
        if (keyboard.leftArrowKey.wasPressedThisFrame)
            myCombat.Attack(CombatDirection.Left, enemyCombat);

        if (keyboard.rightArrowKey.wasPressedThisFrame)
            myCombat.Attack(CombatDirection.Right, enemyCombat);

        if (keyboard.upArrowKey.wasPressedThisFrame)
            myCombat.Attack(CombatDirection.Top, enemyCombat);

        if (keyboard.downArrowKey.wasPressedThisFrame)
            myCombat.Attack(CombatDirection.Bottom, enemyCombat);

        // Touches de bloc directes
        if (keyboard.numpad4Key.isPressed)
            myCombat.SetBlock(CombatDirection.Left);

        else if (keyboard.numpad6Key.isPressed)
            myCombat.SetBlock(CombatDirection.Right);

        else if (keyboard.numpad8Key.isPressed)
            myCombat.SetBlock(CombatDirection.Top);

        else if (keyboard.numpad2Key.isPressed)
            myCombat.SetBlock(CombatDirection.Bottom);

        // Relâche le bloc si aucune touche de bloc enfoncée
        if (!keyboard.numpad4Key.isPressed &&
            !keyboard.numpad6Key.isPressed &&
            !keyboard.numpad8Key.isPressed &&
            !keyboard.numpad2Key.isPressed)
        {
            // Ne relâche pas si clic droit maintenu
            if (!Mouse.current.rightButton.isPressed)
                myCombat.ReleaseBlock();
        }
    }

    CombatDirection GetDirectionFromDelta(Vector2 delta)
    {
        float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;

        if (angle > 45f && angle < 135f) return CombatDirection.Top;
        if (angle < -45f && angle > -135f) return CombatDirection.Bottom;
        if (Mathf.Abs(angle) > 135f) return CombatDirection.Left;
        return CombatDirection.Right;
    }

    // Accessible par l'UI pour afficher l'indicateur
    public CombatDirection GetCurrentDirection() => _currentDirection;
}