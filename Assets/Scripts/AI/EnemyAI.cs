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

    // Stats selon difficulté
    private float _reactionTime;
    private float _blockChance;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindWithTag("Player").transform;
        ApplyDifficulty();
    }

    void ApplyDifficulty()
    {
        switch (difficulty)
        {
            case AIDifficulty.Easy:
                _reactionTime = 2f;    // réagit lentement
                _blockChance = 0.2f;   // bloque rarement
                _agent.speed = 2f;
                break;

            case AIDifficulty.Medium:
                _reactionTime = 1f;
                _blockChance = 0.5f;
                _agent.speed = 3.5f;
                break;

            case AIDifficulty.Hard:
                _reactionTime = 0.3f;  // réagit très vite
                _blockChance = 0.8f;   // bloque souvent
                _agent.speed = 5f;
                break;
        }

        // Lance la boucle de décision selon le temps de réaction
        InvokeRepeating(nameof(AIDecisionTick), _reactionTime, _reactionTime);
    }

    void Update()
    {
        if (_state == AIState.Dead) return;

        // Vérifie si le joueur est mort
        if (myHealth.Current <= 0)
        {
            Die();
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

        // Machine à états
        if (distanceToPlayer <= combatRange)
        {
            _state = AIState.Combat;
            _agent.isStopped = true; // arrête de bouger en combat
            FacePlayer();            // regarde toujours le joueur
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
        // Ne prend des décisions qu'en combat
        if (_state != AIState.Combat) return;

        float roll = Random.value;

        if (roll < _blockChance)
        {
            // Choisit une direction de bloc aléatoire
            CombatDirection[] blocs = {
                CombatDirection.Left,
                CombatDirection.Center,
                CombatDirection.Right
            };
            myCombat.SetBlock(blocs[Random.Range(0, blocs.Length)]);
        }
        else
        {
            // Attaque dans une direction aléatoire
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
        // L'ennemi regarde toujours vers le joueur
        Vector3 direction = (_player.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);
    }

    void Die()
    {
        _state = AIState.Dead;
        _agent.isStopped = true;
        CancelInvoke(nameof(AIDecisionTick));
        Debug.Log($"{gameObject.name} est mort !");
        Destroy(gameObject, 2f); // disparaît après 2 secondes
    }
}