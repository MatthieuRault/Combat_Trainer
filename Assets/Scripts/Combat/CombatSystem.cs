using TMPro;
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

    private bool _isWindingUp = false;
    private float _windupTimer = 0f;
    private float _windupDuration = 0f;
    private CombatDirection _windupDirection;
    private CombatSystem _windupTarget;
    private bool _isHeavy = false;

    public CombatDirection CurrentAttackDirection => _windupDirection;
    public bool IsWindingUp => _isWindingUp;
    public float WindupProgress => _isWindingUp ? _windupTimer / _windupDuration : 0f;

    void Update()
    {
        if (_attackTimer > 0) _attackTimer -= Time.deltaTime;
        if (_kickTimer > 0) _kickTimer -= Time.deltaTime;

        if (_isWindingUp)
        {
            _windupTimer += Time.deltaTime;
            if (_windupTimer >= _windupDuration)
                ReleaseAttack();
        }
    }

    // ── ATTAQUE ──────────────────────────────────────────
    public void StartAttack(CombatDirection direction, CombatSystem target, bool heavy)
    {
        if (_attackTimer > 0)
        {
            SpawnPopup("Can't attack so fast!", Color.white, transform.position + Vector3.up * 2f);
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
    }

    void ReleaseAttack()
    {
        _isWindingUp = false;
        _attackTimer = attackCooldown;

        if (_windupTarget == null) return;

        float distance = Vector3.Distance(transform.position, _windupTarget.transform.position);
        if (distance > attackRange) return;

        float damage = _isHeavy ? attackDamage * heavyAttackMultiplier : attackDamage;
        _windupTarget.ReceiveHit(_windupDirection, damage);
    }

    public void CancelAttack()
    {
        if (_isWindingUp)
        {
            _isWindingUp = false;
            _windupTimer = 0f;
        }
    }

    // ── RECEIVE HIT ──────────────────────────────────────
    public void ReceiveHit(CombatDirection attackDirection, float damage)
    {
        if (CheckBlock(attackDirection))
        {
            stamina.Use(staminaCostBlock);
            SpawnPopup($"{gameObject.name} BLOCK: {(int)damage}", Color.green, transform.position + Vector3.up * 2f);
        }
        else
        {
            health.TakeDamage(damage);
            SpawnPopup($"{gameObject.name} -{(int)damage}", Color.red, transform.position + Vector3.up * 2f);
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
    }

    public void ReceiveKick(Vector3 attackerPos, CombatSystem attacker)
    {
        if (CheckBlock(CombatDirection.Center))
        {
            stamina.Use(staminaCostBlock);
            attacker.stamina.Use(staminaCostBlock);
            SpawnPopup($"{gameObject.name} KICK BLOCKED!", Color.green, transform.position + Vector3.up * 2f);
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
            SpawnPopup($"{gameObject.name} STUNNED!", Color.yellow, transform.position + Vector3.up * 2f);
        }
    }

    private System.Collections.IEnumerator StunRoutine()
    {
        _attackTimer = kickStunDuration;
        _kickTimer = kickStunDuration;
        yield return new WaitForSeconds(kickStunDuration);
    }

    // ── POPUP ─────────────────────────────────────────────
    void SpawnPopup(string message, Color color, Vector3 position)
    {
        GameObject popup = new GameObject("DamagePopup");
        popup.transform.position = position;

        TextMeshPro tmp = popup.AddComponent<TextMeshPro>();
        tmp.text = message;
        tmp.color = color;
        tmp.fontSize = 4f;
        tmp.alignment = TextAlignmentOptions.Center;

        DamagePopup dp = popup.AddComponent<DamagePopup>();
        dp.textComponent = tmp;

        Destroy(popup, 1.5f);
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