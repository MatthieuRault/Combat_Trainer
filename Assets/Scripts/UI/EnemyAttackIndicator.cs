using TMPro;
using UnityEngine;

public class EnemyAttackIndicator : MonoBehaviour
{
    [Header("Références")]
    public RectTransform attackArrow;
    public CombatSystem enemyCombat;

    [Header("Réglages")]
    public float radius = 40f;

    private TextMeshProUGUI _arrowText;

    void Start()
    {
        _arrowText = attackArrow.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (enemyCombat == null || attackArrow == null) return;

        if (enemyCombat.IsWindingUp)
        {
            attackArrow.gameObject.SetActive(true);

            CombatDirection dir = enemyCombat.CurrentAttackDirection;
            attackArrow.localPosition = GetOffset(dir);
            attackArrow.localRotation = Quaternion.Euler(0, 0, GetAngle(dir));

            float progress = enemyCombat.WindupProgress;
            _arrowText.color = Color.Lerp(Color.red, Color.yellow, progress);
        }
        else
        {
            attackArrow.gameObject.SetActive(false);
        }

        transform.LookAt(transform.position + Camera.main.transform.forward);
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