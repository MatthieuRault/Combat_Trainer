using UnityEngine;

public enum CombatDirection { Left, Right, Top, Bottom, Center }

public class CombatSystem : MonoBehaviour
{
    [Header("Stats de combat")]
    public float attackDamage = 20f;
    public float attackCooldown = 0.6f;
    public float attackRange = 2f;

    [Header("Kick")]
    public float kickCooldown = 3f;
    public float kickStunDuration = 1f;
    public float kickKnockback = 1.5f;
    public float kickStaminaCost = 20f;

    [Header("Coût en Stamina")]
    public float staminaCostAttack = 15f;
    public float staminaCostBlock = 5f;

    [Header("Références")]
    public StaminaSystem stamina;
    public HealthSystem health;

    private bool _isBlocking = false;
    private CombatDirection _currentBlock = CombatDirection.Center;
    private float _attackTimer = 0f;
    private float _kickTimer = 0f;

    void Update()
    {
        if (_attackTimer > 0) _attackTimer -= Time.deltaTime;
        if (_kickTimer > 0) _kickTimer -= Time.deltaTime;
    }

    // ── ATTAQUE ──────────────────────────────────────────
    public void Attack(CombatDirection direction, CombatSystem target)
    {
        if (_attackTimer > 0) return;
        if (!stamina.Use(staminaCostAttack)) return;

        _attackTimer = attackCooldown;

        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance > attackRange)
        {
            Debug.Log($"{gameObject.name} trop loin ! ({distance:F1}m)");
            return;
        }

        target.ReceiveHit(direction, attackDamage);
        Debug.Log($"{gameObject.name} attaque en {direction}");
    }

    public void ReceiveHit(CombatDirection attackDirection, float damage)
    {
        if (CheckBlock(attackDirection))
        {
            // Bloqué — coût stamina pour les deux
            stamina.Use(staminaCostBlock);
            Debug.Log($"{gameObject.name} a bloqué !");
        }
        else
        {
            health.TakeDamage(damage);
            Debug.Log($"{gameObject.name} a pris {damage} dégâts !");
        }
    }

    // ── KICK ─────────────────────────────────────────────
    public void Kick(CombatSystem target)
    {
        if (_kickTimer > 0) return;
        if (target == null) return;
        if (!stamina.Use(kickStaminaCost)) return;

        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance > attackRange) return;

        _kickTimer = kickCooldown;
        target.ReceiveKick(transform.position, this);
        Debug.Log($"{gameObject.name} donne un kick !");
    }

    public void ReceiveKick(Vector3 attackerPos, CombatSystem attacker)
    {
        if (CheckBlock(CombatDirection.Center))
        {
            // Kick bloqué — coût stamina pour les deux
            stamina.Use(staminaCostBlock);
            attacker.stamina.Use(staminaCostBlock);
            Debug.Log($"{gameObject.name} a bloqué le kick !");
        }
        else
        {
            // Kick réussi — stun + léger recul
            StartCoroutine(StunRoutine());
            CharacterController cc = GetComponent<CharacterController>();
            if (cc != null)
            {
                Vector3 dir = (transform.position - attackerPos).normalized;
                cc.Move(dir * kickKnockback);
            }
            Debug.Log($"{gameObject.name} est déstabilisé !");
        }
    }

    private System.Collections.IEnumerator StunRoutine()
    {
        _attackTimer = kickStunDuration;
        _kickTimer = kickStunDuration;
        yield return new WaitForSeconds(kickStunDuration);
    }

    // ── BLOCK ─────────────────────────────────────────────
    private bool CheckBlock(CombatDirection attackDirection)
    {
        if (!_isBlocking) return false;
        if (_currentBlock == CombatDirection.Center) return true;
        return _currentBlock == attackDirection;
    }

    public void SetBlock(CombatDirection direction)
    {
        _isBlocking = true;
        _currentBlock = direction;
    }

    public void ReleaseBlock()
    {
        _isBlocking = false;
    }
}