using UnityEngine;
using UnityEngine.UI;

public class DirectionIndicator : MonoBehaviour
{
    public CombatInput combatInput;  // référence au script de combat
    public RectTransform arrowImage; // l'image de la flèche dans le Canvas

    void Update()
    {
        if (combatInput == null || arrowImage == null) return;

        // Tourne la flèche selon la direction actuelle
        float angle = GetAngleFromDirection(combatInput.GetCurrentDirection());
        arrowImage.rotation = Quaternion.Euler(0, 0, angle);
    }

    float GetAngleFromDirection(CombatDirection direction)
    {
        switch (direction)
        {
            case CombatDirection.Right: return 0f;
            case CombatDirection.Top: return 90f;
            case CombatDirection.Left: return 180f;
            case CombatDirection.Bottom: return 270f;
            default: return 0f;
        }
    }
}