using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Attack state for enemy AI. Attacks the player when in range.
/// </summary>
public class Attack : AIState
{
    [Header("Attack Configuration")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float losePlayerDistance = 15f;
    [SerializeField] private float rotationSpeed = 5f;
    
    private NavMeshAgent navAgent;
    private float lastAttackTime = 0f;
    private bool isAttacking = false;
    
    protected override void OnStateEnter()
    {
        navAgent = enemyAI.GetComponent<NavMeshAgent>();
        if (navAgent != null)
        {
            navAgent.isStopped = true; // Stop moving when attacking
        }
        
        lastAttackTime = -attackCooldown; // Allow immediate first attack
    }
    
    protected override void OnStateUpdate()
    {
        if (player == null || navAgent == null) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Check if player is too far away
        if (distanceToPlayer > losePlayerDistance)
        {
            // Player lost - transition back to patrol
            enemyAI.HandlePlayerLost();
            return;
        }
        
        // Check if player is out of attack range
        if (distanceToPlayer > attackRange)
        {
            // Transition back to chase
            enemyAI.TransitionToChase();
            return;
        }
        
        // Face the player
        FacePlayer();
        
        // Check if we can attack
        if (CanAttack())
        {
            PerformAttack();
        }
    }
    
    protected override void OnStateExit()
    {
        isAttacking = false;
        if (navAgent != null)
        {
            navAgent.isStopped = false;
        }
    }
    
    private bool CanAttack()
    {
        return !isAttacking && Time.time - lastAttackTime >= attackCooldown;
    }
    
    private void PerformAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        
        // Face the player before attacking
        FacePlayer();
        
        // Perform the attack
        StartCoroutine(AttackCoroutine());
    }
    
    private System.Collections.IEnumerator AttackCoroutine()
    {
        // Attack animation/effect duration
        yield return new WaitForSeconds(0.5f);
        
        // Deal damage to player
        DealDamageToPlayer();
        
        // Reset attack state
        isAttacking = false;
    }
    
    private void DealDamageToPlayer()
    {
        if (player != null)
        {
            // Try to get player health component
            var playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
            else
            {
                // Fallback: try to find any component with TakeDamage method
                var healthComponent = player.GetComponent<MonoBehaviour>();
                if (healthComponent != null)
                {
                    var takeDamageMethod = healthComponent.GetType().GetMethod("TakeDamage");
                    if (takeDamageMethod != null)
                    {
                        takeDamageMethod.Invoke(healthComponent, new object[] { attackDamage });
                    }
                }
            }
            
            Debug.Log($"Enemy attacked player for {attackDamage} damage!");
        }
    }
    
    private void FacePlayer()
    {
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0; // Keep the enemy upright
            
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
    
    /// <summary>
    /// Check if the enemy is currently attacking
    /// </summary>
    public bool IsAttacking()
    {
        return isAttacking;
    }
    
    /// <summary>
    /// Get the time until the next attack is available
    /// </summary>
    public float GetTimeUntilNextAttack()
    {
        float timeSinceLastAttack = Time.time - lastAttackTime;
        return Mathf.Max(0, attackCooldown - timeSinceLastAttack);
    }
    
    /// <summary>
    /// Get the current attack range
    /// </summary>
    public float GetAttackRange()
    {
        return attackRange;
    }
}
