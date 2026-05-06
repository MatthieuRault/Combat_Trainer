using UnityEngine;
using UnityEngine.AI;

public enum AIDifficulty { Easy, Medium, Hard }
public enum AIState { Idle, Approach, Combat, Dead }

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("Difficulté")]
    public AIDifficulty difficulty = AIDifficulty.Medium;

    [Header("Détection")]
    public float detectionRange = 10f;
    public float combatRange = 2f;

    [Header("Références")]
    public CombatSystem myCombat;
    public CombatSystem playerCombat;
    public HealthSystem myHealth;

    private NavMeshAgent _agent;
    private Transform _player;
    private AIState _state = AIState.Idle;
    private float _reactionTime;
    private float _blockChance;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindWithTag("Player").transform;
        _agent.stoppingDistance = combatRange;
        _agent.autoBraking = true;
        ApplyDifficulty();
    }

    void ApplyDifficulty()
    {
        switch (difficulty)
        {
            case AIDifficulty.Easy:
                _reactionTime = 2f;
                _blockChance = 0.2f;
                _agent.speed = 2f;
                break;
            case AIDifficulty.Medium:
                _reactionTime = 1f;
                _blockChance = 0.5f;
                _agent.speed = 3.5f;
                break;
            case AIDifficulty.Hard:
                _reactionTime = 0.3f;
                _blockChance = 0.8f;
                _agent.speed = 5f;
                break;
        }

        InvokeRepeating(nameof(AIDecisionTick), _reactionTime, _reactionTime);
    }

    void Update()
    {
        if (_state == AIState.Dead) return;

        if (myHealth.Current <= 0)
        {
            Die();
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

        if (distanceToPlayer <= combatRange)
        {
            _state = AIState.Combat;
            _agent.isStopped = true;
            _agent.velocity = Vector3.zero;
            FacePlayer();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            _state = AIState.Approach;
            _agent.isStopped = false;
            _agent.SetDestination(_player.position);
        }
        else
        {
            _state = AIState.Idle;
            _agent.isStopped = true;
        }
    }

    void AIDecisionTick()
    {
        if (_state != AIState.Combat) return;

        float roll = Random.value;

        if (roll < _blockChance)
        {
            CombatDirection[] blocs = {
                CombatDirection.Left,
                CombatDirection.Center,
                CombatDirection.Right
            };
            myCombat.SetBlock(blocs[Random.Range(0, blocs.Length)]);
        }
        else
        {
            myCombat.ReleaseBlock();
            CombatDirection[] attaques = {
                CombatDirection.Left,
                CombatDirection.Right,
                CombatDirection.Top,
                CombatDirection.Bottom
            };
            myCombat.Attack(attaques[Random.Range(0, attaques.Length)], playerCombat);
        }
    }

    void FacePlayer()
    {
        Vector3 direction = (_player.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);
    }

    void Die()
    {
        _state = AIState.Dead;
        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;
        CancelInvoke(nameof(AIDecisionTick));
        Debug.Log($"{gameObject.name} est mort !");
        Destroy(gameObject, 2f);
    }
}