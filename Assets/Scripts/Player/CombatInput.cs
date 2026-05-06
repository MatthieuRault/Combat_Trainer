using UnityEngine;
using UnityEngine.InputSystem;

public class CombatInput : MonoBehaviour
{
    public CombatSystem myCombat;
    public CombatSystem enemyCombat;

    private CombatDirection _currentDirection = CombatDirection.Right;

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
            myCombat.ReleaseBlock();
            myCombat.Attack(_currentDirection, enemyCombat);
            Debug.Log($"Attaque : {_currentDirection}");
        }
    }

    void HandleMouseBlock(Mouse mouse)
    {
        if (mouse.rightButton.isPressed)
        {
            myCombat.SetBlock(_currentDirection);
        }

        if (mouse.rightButton.wasReleasedThisFrame)
        {
            myCombat.ReleaseBlock();
        }
    }

    void HandleKeyboard(Keyboard keyboard)
    {
        if (keyboard.leftArrowKey.wasPressedThisFrame)
            myCombat.Attack(CombatDirection.Left, enemyCombat);

        if (keyboard.rightArrowKey.wasPressedThisFrame)
            myCombat.Attack(CombatDirection.Right, enemyCombat);

        if (keyboard.upArrowKey.wasPressedThisFrame)
            myCombat.Attack(CombatDirection.Top, enemyCombat);

        if (keyboard.downArrowKey.wasPressedThisFrame)
            myCombat.Attack(CombatDirection.Bottom, enemyCombat);

        if (keyboard.numpad4Key.isPressed)
            myCombat.SetBlock(CombatDirection.Left);

        else if (keyboard.numpad6Key.isPressed)
            myCombat.SetBlock(CombatDirection.Right);

        else if (keyboard.numpad8Key.isPressed)
            myCombat.SetBlock(CombatDirection.Top);

        else if (keyboard.numpad2Key.isPressed)
            myCombat.SetBlock(CombatDirection.Bottom);

        if (!keyboard.numpad4Key.isPressed &&
            !keyboard.numpad6Key.isPressed &&
            !keyboard.numpad8Key.isPressed &&
            !keyboard.numpad2Key.isPressed)
        {
            if (!Mouse.current.rightButton.isPressed)
                myCombat.ReleaseBlock();
        }
        if (keyboard.fKey.wasPressedThisFrame)
            myCombat.Kick(enemyCombat);
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