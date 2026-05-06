using UnityEngine;

// Les 4 directions d'attaque + Centre pour le bloc
public enum CombatDirection { Left, Right, Top, Bottom, Center }

public class CombatSystem : MonoBehaviour
{
    [Header("Stats de combat")]
    public float attackDamage = 20f;
    public float knockbackForce = 4f;
    public float attackCooldown = 0.6f;

    [Header("Coût en Stamina")]
    public float staminaCostAttack = 15f;
    public float staminaCostBlock = 5f;

    [Header("Références")]
    public StaminaSystem stamina;
    public HealthSystem health;

    // private bool _isAttacking = false;
    private bool _isBlocking = false;
    private CombatDirection _currentBlock = CombatDirection.Center;
    private float _attackTimer = 0f;

    void Update()
    {
        // Cooldown entre les attaques
        if (_attackTimer > 0)
            _attackTimer -= Time.deltaTime;
    }

    // Appelé quand le joueur ou l'IA attaque
    public void Attack(CombatDirection direction, CombatSystem target)
    {
        if (_attackTimer > 0) return;          // encore en cooldown
        if (!stamina.Use(staminaCostAttack)) return; // pas assez de stamina

        _attackTimer = attackCooldown;

        Debug.Log($"{gameObject.name} attaque en {direction}");

        // On envoie le coup à la cible
        if (target != null)
            target.ReceiveHit(direction, attackDamage, transform.position);
    }

    // Appelé quand on REÇOIT un coup
    public void ReceiveHit(CombatDirection attackDirection, float damage, Vector3 attackerPos)
    {
        bool blocked = CheckBlock(attackDirection);

        if (blocked)
        {
            stamina.Use(staminaCostBlock);
            Debug.Log($"{gameObject.name} a bloqué !");
        }
        else
        {
            health.TakeDamage(damage);
            ApplyKnockback(attackerPos);
            Debug.Log($"{gameObject.name} a pris {damage} dégâts !");
        }
    }

    private bool CheckBlock(CombatDirection attackDirection)
    {
        if (!_isBlocking) return false;

        // Le bloc Centre bloque tout
        if (_currentBlock == CombatDirection.Center) return true;

        // Sinon le bloc doit correspondre à la direction de l'attaque
        return _currentBlock == attackDirection;
    }

    private void ApplyKnockback(Vector3 attackerPos)
    {
        Vector3 direction = (transform.position - attackerPos).normalized;
        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null)
            cc.Move(direction * knockbackForce);
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