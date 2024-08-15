using UnityEngine;

public class WallHealth : MonoBehaviour
{
    public int maxHealth = 4; // Puntos de resistencia máxima
    public Material damagedMaterial; // Material para cuando la resistencia es 2

    public static int damageCounter = 0; // Contador de daño
    public static int maxDamage = 24; // Daño máximo antes de terminar la simulación

    private int currentHealth; // Resistencia actual
    private Renderer objectRenderer; // Renderer del objeto para cambiar materiales


    void Start()
    {
        damageCounter = 0; // Contador de daño
        currentHealth = maxHealth;
        objectRenderer = GetComponent<Renderer>();
    }

    void OnMouseDown()
    {
        if (GameManager.isGameOver) return; // No hacer nada si el juego ha terminado

        TakeDamage(2);

    }

    private void TakeDamage(int damage)
    {
        // Disminuir la resistencia en 2 al hacer clic
        currentHealth -= 2;
        damageCounter++;

        // Verificar si la resistencia es igual a 2
        if (currentHealth == 2)
        {
            objectRenderer.material = damagedMaterial;
        }

        // Verificar si la resistencia es 0 o menos
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }

        // Terminar la simulación si el contador de daño alcanza 24
        if (damageCounter >= maxDamage)
        {
            EndSimulation();
        }
    }

    void EndSimulation()
    {
        Debug.Log("¡Simulación terminada! El contador de daño ha alcanzado el máximo permitido.");
        // Detiene el juego o realiza las acciones necesarias
        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.EndGame(); // Llamar a EndGame en lugar de Application.Quit()
    }
}
