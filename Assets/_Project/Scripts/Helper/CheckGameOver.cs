using UnityEngine;

public class CheckGameOver : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Game Over");
            ActionCommands.OnGameOver();
        }
    }
}
