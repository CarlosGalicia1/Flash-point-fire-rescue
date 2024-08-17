using UnityEngine;

public class DoorHealth : MonoBehaviour
{

    public static int damageCounter = 0; // Contador de daño
    public static int maxDamage = 24; // Daño máximo antes de terminar la simulación

    void Start()
    {
        damageCounter = 0; // Contador de daño
    }

    void OnMouseDown()
    {
        if (GameManager.isGameOver) return; // No hacer nada si el juego ha terminado

        SetDestroy();
    }

    private void SetDestroy()
    {
        damageCounter++;

        if (damageCounter >= maxDamage)
        {
            EndSimulation();
        }
        Destroy(gameObject);
    }

    public void EndSimulation()
    {
        Debug.Log("¡Simulación terminada! El contador de daño ha alcanzado el máximo permitido.");
        // Detiene el juego o realiza las acciones necesarias
        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.EndGame(); // Llamar a EndGame en lugar de Application.Quit()
    }
}
