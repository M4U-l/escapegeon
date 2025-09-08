using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Patrol state for enemy AI. Moves between waypoints when player is not detected.
/// </summary>
public class Patrol : AIState
{
    [Header("Patrol Configuration")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float waitTimeAtWaypoint = 1.5f;
    [SerializeField] private float patrolSpeed = 3f;
    [SerializeField] private float waypointReachDistance = 1f;
    
    private int currentWaypointIndex = 0;
    private NavMeshAgent navAgent;
    private bool isWaiting = false;
    private float waitTimer = 0f;
    
    protected override void OnStateEnter()
    {
        navAgent = enemyAI.GetComponent<NavMeshAgent>();
        if (navAgent != null)
        {
            navAgent.speed = patrolSpeed;
            navAgent.isStopped = false;
        }
        
        // Set initial waypoint
        if (waypoints != null && waypoints.Length > 0)
        {
            MoveToWaypoint();
        }
        else
        {
            Debug.LogWarning("No waypoints assigned to Patrol state!");
        }
    }
    
    protected override void OnStateUpdate()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        
        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtWaypoint)
            {
                isWaiting = false;
                waitTimer = 0f;
                MoveToNextWaypoint();
            }
        }
        else if (navAgent != null && !navAgent.pathPending)
        {
            if (navAgent.remainingDistance < waypointReachDistance)
            {
                isWaiting = true;
                navAgent.isStopped = true;
            }
        }
    }
    
    protected override void OnStateExit()
    {
        if (navAgent != null)
        {
            navAgent.isStopped = true;
        }
    }
    
    private void MoveToWaypoint()
    {
        if (navAgent != null && waypoints[currentWaypointIndex] != null)
        {
            navAgent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }
    
    private void MoveToNextWaypoint()
    {
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        navAgent.isStopped = false;
        MoveToWaypoint();
    }
    
    /// <summary>
    /// Set the waypoints for this patrol state
    /// </summary>
    public void SetWaypoints(Transform[] newWaypoints)
    {
        waypoints = newWaypoints;
        currentWaypointIndex = 0;
    }
    
    /// <summary>
    /// Get the current waypoint the enemy is moving towards
    /// </summary>
    public Transform GetCurrentWaypoint()
    {
        if (waypoints != null && waypoints.Length > 0)
        {
            return waypoints[currentWaypointIndex];
        }
        return null;
    }
}
