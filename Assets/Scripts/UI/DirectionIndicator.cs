using UnityEngine;

public class DirectionIndicator : MonoBehaviour
{
    public CombatInput combatInput;
    public RectTransform arrowImage;
    public Transform enemy;
    public Camera mainCamera;
    public float offsetX = 150f;
    public float offsetY = 150f;

    void Update()
    {
        if (combatInput == null || arrowImage == null || enemy == null) return;

        Vector3 enemyScreenPos = mainCamera.WorldToScreenPoint(enemy.position);
        if (enemyScreenPos.z < 0) { arrowImage.gameObject.SetActive(false); return; }

        arrowImage.gameObject.SetActive(true);

        CombatDirection dir = combatInput.GetCurrentDirection();
        Vector2 offset = GetOffset(dir);

        arrowImage.position = new Vector3(
            enemyScreenPos.x + offset.x,
            enemyScreenPos.y + offset.y,
            0
        );

        arrowImage.localRotation = Quaternion.Euler(0, 0, GetAngle(dir));
    }

    Vector2 GetOffset(CombatDirection dir)
    {
        switch (dir)
        {
            case CombatDirection.Left: return new Vector2(-offsetX, 0);
            case CombatDirection.Right: return new Vector2(offsetX, 0);
            case CombatDirection.Top: return new Vector2(0, offsetY);
            case CombatDirection.Bottom: return new Vector2(0, -offsetY);
            default: return new Vector2(offsetX, 0);
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