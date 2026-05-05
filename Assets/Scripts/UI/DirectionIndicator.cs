using TMPro;
using UnityEngine;

public class DirectionIndicator : MonoBehaviour
{
    public CombatInput combatInput;
    public RectTransform arrowImage;
    public Transform enemy;        // l'ennemi dans la scène
    public Camera mainCamera;
    public float radius = 60f;     // distance autour de l'ennemi en pixels

    void Update()
    {
        if (combatInput == null || arrowImage == null || enemy == null) return;

        // Convertit la position monde de l'ennemi en position écran
        Vector3 enemyScreenPos = mainCamera.WorldToScreenPoint(enemy.position);

        // Si l'ennemi est derrière la caméra on cache la flèche
        if (enemyScreenPos.z < 0)
        {
            arrowImage.gameObject.SetActive(false);
            return;
        }

        arrowImage.gameObject.SetActive(true);

        // Place la flèche autour de la position écran de l'ennemi
        CombatDirection dir = combatInput.GetCurrentDirection();
        Vector2 offset = GetOffset(dir);

        // Convertit la position écran en position Canvas
        arrowImage.position = new Vector3(
            enemyScreenPos.x + offset.x,
            enemyScreenPos.y + offset.y,
            0
        );

        // Tourne la flèche dans la bonne direction
        arrowImage.localRotation = Quaternion.Euler(0, 0, GetAngle(dir));
    }

    Vector2 GetOffset(CombatDirection dir)
    {
        switch (dir)
        {
            case CombatDirection.Left: return new Vector2(-radius, 0);
            case CombatDirection.Right: return new Vector2(radius, 0);
            case CombatDirection.Top: return new Vector2(0, radius);
            case CombatDirection.Bottom: return new Vector2(0, -radius);
            default: return new Vector2(radius, 0);
        }
    }

    float GetAngle(CombatDirection dir)
    {
        switch (dir)
        {
            case CombatDirection.Right: return 0f;
            case CombatDirection.Top: return 90f;
            case CombatDirection.Left: return 180f;
            case CombatDirection.Bottom: return 270f;
            default: return 0f;
        }
    }
}