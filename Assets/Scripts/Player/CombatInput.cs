using UnityEngine;
using UnityEngine.InputSystem;

public class CombatInput : MonoBehaviour
{
    public CombatSystem myCombat;
    public CombatSystem enemyCombat;

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // ATTAQUES
        if (keyboard.numpad4Key.wasPressedThisFrame)
            myCombat.Attack(CombatDirection.Left, enemyCombat);

        if (keyboard.numpad6Key.wasPressedThisFrame)
            myCombat.Attack(CombatDirection.Right, enemyCombat);

        if (keyboard.numpad8Key.wasPressedThisFrame)
            myCombat.Attack(CombatDirection.Top, enemyCombat);

        if (keyboard.numpad2Key.wasPressedThisFrame)
            myCombat.Attack(CombatDirection.Bottom, enemyCombat);

        // BLOCS
        if (keyboard.uKey.wasPressedThisFrame)
            myCombat.SetBlock(CombatDirection.Left);

        if (keyboard.iKey.wasPressedThisFrame)
            myCombat.SetBlock(CombatDirection.Center);

        if (keyboard.oKey.wasPressedThisFrame)
            myCombat.SetBlock(CombatDirection.Right);

        if (keyboard.uKey.wasReleasedThisFrame ||
            keyboard.iKey.wasReleasedThisFrame ||
            keyboard.oKey.wasReleasedThisFrame)
            myCombat.ReleaseBlock();
    }
}