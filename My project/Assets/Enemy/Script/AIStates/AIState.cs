using UnityEngine;

/// <summary>
/// Abstract base class for all AI states. Provides a modular foundation for state-based AI behavior.
/// </summary>
public abstract class AIState : MonoBehaviour
{
    [Header("State Configuration")]
    [SerializeField] protected float stateDuration = 0f;
    [SerializeField] protected bool canTransitionToSelf = false;
    
    protected EnemyAI enemyAI;
    protected Transform player;
    protected float stateTimer;
    protected bool isActive;
    
    /// <summary>
    /// Called when the state is entered
    /// </summary>
    public virtual void EnterState(EnemyAI ai, Transform playerTransform)
    {
        enemyAI = ai;
        player = playerTransform;
        stateTimer = 0f;
        isActive = true;
        OnStateEnter();
    }
    
    /// <summary>
    /// Called every frame while the state is active
    /// </summary>
    public virtual void UpdateState()
    {
        if (!isActive) return;
        
        stateTimer += Time.deltaTime;
        OnStateUpdate();
        
        // Check if state should end based on duration
        if (stateDuration > 0 && stateTimer >= stateDuration)
        {
            OnStateTimeout();
        }
    }
    
    /// <summary>
    /// Called when the state is exited
    /// </summary>
    public virtual void ExitState()
    {
        isActive = false;
        OnStateExit();
    }
    
    /// <summary>
    /// Check if this state can transition to another state
    /// </summary>
    public virtual bool CanTransitionTo(AIState targetState)
    {
        return targetState != this || canTransitionToSelf;
    }
    
    /// <summary>
    /// Override this method to implement state-specific enter behavior
    /// </summary>
    protected virtual void OnStateEnter() { }
    
    /// <summary>
    /// Override this method to implement state-specific update behavior
    /// </summary>
    protected virtual void OnStateUpdate() { }
    
    /// <summary>
    /// Override this method to implement state-specific exit behavior
    /// </summary>
    protected virtual void OnStateExit() { }
    
    /// <summary>
    /// Override this method to implement state-specific timeout behavior
    /// </summary>
    protected virtual void OnStateTimeout() { }
    
    /// <summary>
    /// Get the current state name for debugging
    /// </summary>
    public virtual string GetStateName()
    {
        return GetType().Name;
    }
}
