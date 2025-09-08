using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Chase state for enemy AI. Follows the player when detected but not in attack range.
/// </summary>
public class Chase : AIState
{
    [Header("Chase Configuration")]
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float losePlayerDistance = 10f;
    [SerializeField] private float updateDestinationInterval = 0.5f;
    
    private NavMeshAgent navAgent;
    private float lastDestinationUpdate = 0f;
    private Vector3 lastKnownPlayerPosition;
    
    protected override void OnStateEnter()
    {
        navAgent = enemyAI.GetComponent<NavMeshAgent>();
        if (navAgent != null)
        {
            navAgent.speed = chaseSpeed;
            navAgent.isStopped = false;
        }
        
        lastKnownPlayerPosition = player.position;
        UpdateDestination();
    }
    
    protected override void OnStateUpdate()
    {
        if (player == null || navAgent == null) return;
        
        // Check if player is still in range
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // If player is too far, we might lose them
        if (distanceToPlayer > losePlayerDistance)
        {
            enemyAI.HandlePlayerLost();
            return;
        }
        
        // Check if we're in attack range
        if (distanceToPlayer <= attackRange)
        {
            enemyAI.TransitionToAttack();
            return;
        }
        
        // Update destination periodically for better performance
        if (Time.time - lastDestinationUpdate >= updateDestinationInterval)
        {
            UpdateDestination();
            lastDestinationUpdate = Time.time;
        }
        
        FacePlayer();
    }
    
    protected override void OnStateExit()
    {
        if (navAgent != null)
        {
            navAgent.isStopped = true;
        }
    }
    
    private void UpdateDestination()
    {
        if (player != null && navAgent != null)
        {
            lastKnownPlayerPosition = player.position;
            navAgent.SetDestination(lastKnownPlayerPosition);
        }
    }
    
    private void FacePlayer()
    {
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0; 
            
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }
        }
    }
    
    /// <summary>
    /// Get the distance to the player
    /// </summary>
    public float GetDistanceToPlayer()
    {
        if (player != null)
        {
            return Vector3.Distance(transform.position, player.position);
        }
        return float.MaxValue;
    }
    
    /// <summary>
    /// Get the last known position of the player
    /// </summary>
    public Vector3 GetLastKnownPlayerPosition()
    {
        return lastKnownPlayerPosition;
    }
}
