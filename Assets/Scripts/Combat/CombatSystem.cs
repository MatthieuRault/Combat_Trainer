using UnityEngine;

public enum CombatDirection { Left, Right, Top, Bottom, Center }

public class CombatSystem : MonoBehaviour
{
    [Header("Stats de combat")]
    public float attackDamage = 20f;
    public float heavyAttackMultiplier = 1.5f;
    public float attackCooldown = 1.1f;
    public float attackRange = 2f;

    [Header("Windup")]
    public float lightWindup = 0.6f;
    public float heavyWindup = 1.2f;

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

    // Windup en cours
    private bool _isWindingUp = false;
    private float _windupTimer = 0f;
    private float _windupDuration = 0f;
    private CombatDirection _windupDirection;
    private CombatSystem _windupTarget;
    private bool _isHeavy = false;

    // Direction d'attaque visible (pour la flèche)
    public CombatDirection CurrentAttackDirection => _windupDirection;
    public bool IsWindingUp => _isWindingUp;
    public float WindupProgress => _isWindingUp ? _windupTimer / _windupDuration : 0f;

    void Update()
    {
        if (_attackTimer > 0) _attackTimer -= Time.deltaTime;
        if (_kickTimer > 0) _kickTimer -= Time.deltaTime;

        // Windup en cours
        if (_isWindingUp)
        {
            _windupTimer += Time.deltaTime;
            if (_windupTimer >= _windupDuration)
                ReleaseAttack();
        }
    }

    // ── ATTAQUE ──────────────────────────────────────────

    // Démarre le windup
    public void StartAttack(CombatDirection direction, CombatSystem target, bool heavy)
    {
        if (_attackTimer > 0)
        {
            Debug.Log("Can't attack so fast !");
            return;
        }
        if (_isWindingUp) return;
        if (!stamina.Use(staminaCostAttack)) return;

        _isWindingUp = true;
        _windupTimer = 0f;
        _windupDirection = direction;
        _windupTarget = target;
        _isHeavy = heavy;
        _windupDuration = heavy ? heavyWindup : lightWindup;

        Debug.Log($"{gameObject.name} prépare une attaque {(heavy ? "LOURDE" : "légère")} en {direction}");
    }

    // Relâche l'attaque après le windup
    void ReleaseAttack()
    {
        _isWindingUp = false;
        _attackTimer = attackCooldown;

        if (_windupTarget == null) return;

        float distance = Vector3.Distance(transform.position, _windupTarget.transform.position);
        if (distance > attackRange)
        {
            Debug.Log($"{gameObject.name} trop loin !");
            return;
        }

        float damage = _isHeavy ? attackDamage * heavyAttackMultiplier : attackDamage;
        _windupTarget.ReceiveHit(_windupDirection, damage);
    }

    // Annule le windup (si on relâche trop tôt)
    public void CancelAttack()
    {
        if (_isWindingUp)
        {
            _isWindingUp = false;
            _windupTimer = 0f;
            Debug.Log($"{gameObject.name} annule son attaque");
        }
    }

    public void ReceiveHit(CombatDirection attackDirection, float damage)
    {
        if (CheckBlock(attackDirection))
        {
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
            stamina.Use(staminaCostBlock);
            attacker.stamina.Use(staminaCostBlock);
            Debug.Log($"{gameObject.name} a bloqué le kick !");
        }
        else
        {
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