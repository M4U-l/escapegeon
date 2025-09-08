using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Main EnemyAI manager that handles state transitions and player detection.
/// This is the core component that manages the AI state machine.
/// </summary>
public class EnemyAI : MonoBehaviour
{
    [Header("AI Configuration")]
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float fieldOfView = 90f;
    [SerializeField] private LayerMask playerLayer = 1;
    [SerializeField] private LayerMask obstacleLayer = 1;
    
    [Header("State References")]
    [SerializeField] private Patrol patrolState;
    [SerializeField] private Chase chaseState;
    [SerializeField] private Attack attackState;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;
    [SerializeField] private Color detectionColor = Color.yellow;
    [SerializeField] private Color attackRangeColor = Color.red;
    
    private AIState currentState;
    private NavMeshAgent navAgent;
    private bool isPlayerDetected = false;
    private Vector3 lastKnownPlayerPosition;
    
    // Events for external systems
    public System.Action<AIState> OnStateChanged;
    public System.Action<Transform> OnPlayerDetected;
    public System.Action OnPlayerLost;
    
    private void Awake()
    {
        // Get or add NavMeshAgent component
        navAgent = GetComponent<NavMeshAgent>();
        if (navAgent == null)
        {
            navAgent = gameObject.AddComponent<NavMeshAgent>();
        }
        
        // Initialize states if not assigned
        InitializeStates();
    }
    
    private void Start()
    {
        // Find player if not assigned
        if (player == null)
        {
            FindPlayer();
        }
        
        // Start with patrol state
        TransitionToState(patrolState);
    }
    
    private void Update()
    {
        // Update current state
        if (currentState != null)
        {
            currentState.UpdateState();
        }
        
        // Check for player detection
        CheckPlayerDetection();
    }
    
    private void InitializeStates()
    {
        // Get state components if not assigned
        if (patrolState == null)
            patrolState = GetComponent<Patrol>();
        if (chaseState == null)
            chaseState = GetComponent<Chase>();
        if (attackState == null)
            attackState = GetComponent<Attack>();
        
        // Add state components if they don't exist
        if (patrolState == null)
            patrolState = gameObject.AddComponent<Patrol>();
        if (chaseState == null)
            chaseState = gameObject.AddComponent<Chase>();
        if (attackState == null)
            attackState = gameObject.AddComponent<Attack>();
    }
    
    private void FindPlayer()
    {
        // Try to find player by tag
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player not found! Please assign the player transform or ensure player has 'Player' tag.");
        }
    }
    
    private void CheckPlayerDetection()
    {
        if (player == null) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool wasPlayerDetected = isPlayerDetected;
        
        // Check if player is in detection range
        if (distanceToPlayer <= detectionRange)
        {
            // Check if player is in field of view
            if (IsPlayerInFieldOfView())
            {
                // Check if there's a clear line of sight
                if (HasLineOfSightToPlayer())
                {
                    if (!isPlayerDetected)
                    {
                        isPlayerDetected = true;
                        lastKnownPlayerPosition = player.position;
                        OnPlayerDetected?.Invoke(player);
                        
                        // Transition to chase state
                        TransitionToChase();
                    }
                    else
                    {
                        lastKnownPlayerPosition = player.position;
                    }
                }
            }
        }
        else
        {
            // Player is out of detection range
            if (isPlayerDetected)
            {
                isPlayerDetected = false;
                OnPlayerLost?.Invoke();
            }
        }
    }
    
    private bool IsPlayerInFieldOfView()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        return angleToPlayer <= fieldOfView / 2f;
    }
    
    private bool HasLineOfSightToPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, detectionRange, obstacleLayer | playerLayer))
        {
            return hit.transform == player;
        }
        
        return false;
    }
    
    public void TransitionToState(AIState newState)
    {
        if (newState == null) return;
        
        // Exit current state
        if (currentState != null)
        {
            currentState.ExitState();
        }
        
        // Enter new state
        currentState = newState;
        currentState.EnterState(this, player);
        
        // Invoke state change event
        OnStateChanged?.Invoke(currentState);
        
        Debug.Log($"EnemyAI: Transitioned to {currentState.GetStateName()} state");
    }
    
    public void TransitionToPatrol()
    {
        TransitionToState(patrolState);
    }
    
    public void TransitionToChase()
    {
        TransitionToState(chaseState);
    }
    
    public void TransitionToAttack()
    {
        TransitionToState(attackState);
    }
    
    public void HandlePlayerLost()
    {
        isPlayerDetected = false;
        OnPlayerLost?.Invoke();
        TransitionToPatrol();
    }
    
    // Public getters for external access
    public AIState GetCurrentState() => currentState;
    public bool IsPlayerDetected() => isPlayerDetected;
    public Transform GetPlayer() => player;
    public Vector3 GetLastKnownPlayerPosition() => lastKnownPlayerPosition;
    public float GetDetectionRange() => detectionRange;
    public float GetFieldOfView() => fieldOfView;
    
    // Public setters for runtime configuration
    public void SetPlayer(Transform newPlayer) => player = newPlayer;
    public void SetDetectionRange(float range) => detectionRange = range;
    public void SetFieldOfView(float fov) => fieldOfView = fov;
    
    private void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;
        
        // Draw detection range
        Gizmos.color = detectionColor;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Draw field of view
        Vector3 leftBoundary = Quaternion.AngleAxis(-fieldOfView / 2f, Vector3.up) * transform.forward * detectionRange;
        Vector3 rightBoundary = Quaternion.AngleAxis(fieldOfView / 2f, Vector3.up) * transform.forward * detectionRange;
        
        Gizmos.color = detectionColor;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
        
        // Draw attack range if in attack state
        if (currentState is Attack attack)
        {
            Gizmos.color = attackRangeColor;
            Gizmos.DrawWireSphere(transform.position, attack.GetAttackRange());
        }
        
        // Draw line to player if detected
        if (isPlayerDetected && player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}
