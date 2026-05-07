using UnityEngine;
using UnityEngine.InputSystem;

public class CombatInput : MonoBehaviour
{
    public CombatSystem myCombat;
    public CombatSystem enemyCombat;

    private CombatDirection _currentDirection = CombatDirection.Right;
    private float _mouseHoldTime = 0f;
    private bool _mouseHeld = false;
    public float heavyAttackThreshold = 0.4f;

    void Update()
    {
        var keyboard = Keyboard.current;
        var mouse = Mouse.current;
        if (keyboard == null || mouse == null) return;

        UpdateDirectionFromMouse(mouse);
        HandleMouseAttack(mouse);
        HandleMouseBlock(mouse);
        HandleKeyboard(keyboard);
    }

    void UpdateDirectionFromMouse(Mouse mouse)
    {
        Vector2 delta = mouse.delta.ReadValue();
        if (delta.magnitude < 0.5f) return;
        _currentDirection = GetDirectionFromDelta(delta);
    }

    void HandleMouseAttack(Mouse mouse)
    {
        if (mouse.leftButton.wasPressedThisFrame)
        {
            _mouseHeld = true;
            _mouseHoldTime = 0f;
            myCombat.ReleaseBlock();
            myCombat.StartAttack(_currentDirection, enemyCombat, false);
        }

        if (_mouseHeld && mouse.leftButton.isPressed)
        {
            _mouseHoldTime += Time.deltaTime;

            if (_mouseHoldTime >= heavyAttackThreshold && !myCombat.IsWindingUp)
                myCombat.StartAttack(_currentDirection, enemyCombat, true);
        }

        if (mouse.leftButton.wasReleasedThisFrame)
        {
            _mouseHeld = false;
            _mouseHoldTime = 0f;
        }
    }

    void HandleMouseBlock(Mouse mouse)
    {
        if (mouse.rightButton.isPressed)
            myCombat.SetBlock(_currentDirection);

        if (mouse.rightButton.wasReleasedThisFrame)
            myCombat.ReleaseBlock();
    }

    void HandleKeyboard(Keyboard keyboard)
    {
        // Kick
        if (keyboard.fKey.wasPressedThisFrame)
            myCombat.Kick(enemyCombat);

        // Attaques au clavier
        if (keyboard.leftArrowKey.wasPressedThisFrame)
            myCombat.StartAttack(CombatDirection.Left, enemyCombat, false);
        if (keyboard.rightArrowKey.wasPressedThisFrame)
            myCombat.StartAttack(CombatDirection.Right, enemyCombat, false);
        if (keyboard.upArrowKey.wasPressedThisFrame)
            myCombat.StartAttack(CombatDirection.Top, enemyCombat, false);
        if (keyboard.downArrowKey.wasPressedThisFrame)
            myCombat.StartAttack(CombatDirection.Bottom, enemyCombat, false);

        // Blocage clavier
        if (keyboard.numpad4Key.isPressed)
            myCombat.SetBlock(CombatDirection.Left);
        else if (keyboard.numpad6Key.isPressed)
            myCombat.SetBlock(CombatDirection.Right);
        else if (keyboard.numpad8Key.isPressed)
            myCombat.SetBlock(CombatDirection.Top);
        else if (keyboard.numpad2Key.isPressed)
            myCombat.SetBlock(CombatDirection.Bottom);

        // Relâche le bloc si aucune touche
        if (!keyboard.numpad4Key.isPressed &&
            !keyboard.numpad6Key.isPressed &&
            !keyboard.numpad8Key.isPressed &&
            !keyboard.numpad2Key.isPressed &&
            !Mouse.current.rightButton.isPressed)
            myCombat.ReleaseBlock();
    }

    CombatDirection GetDirectionFromDelta(Vector2 delta)
    {
        float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        if (angle > 45f && angle < 135f) return CombatDirection.Top;
        if (angle < -45f && angle > -135f) return CombatDirection.Bottom;
        if (Mathf.Abs(angle) > 135f) return CombatDirection.Left;
        return CombatDirection.Right;
    }

    public CombatDirection GetCurrentDirection() => _currentDirection;
}