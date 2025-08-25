using UnityEngine;

public class Ladder : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent<PlayerMove>(out var player))
        {
            player.CurrentState = PlayerMove.State.Climbing;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent<PlayerMove>(out var player))
        {
            player.CurrentState = PlayerMove.State.Walking;
        }
    }
}
