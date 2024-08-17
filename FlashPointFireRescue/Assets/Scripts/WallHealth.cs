using UnityEngine;

public class WallHealth : MonoBehaviour
{
    public int maxHealth = 4; // Puntos de resistencia máxima
    public Material damagedMaterial; // Material para cuando la resistencia es 2

    private int currentHealth; // Resistencia actual
    private Renderer objectRenderer; // Renderer del objeto para cambiar materiales

    void Start()
    {
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
        DoorHealth.damageCounter++;

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
        if (DoorHealth.damageCounter >= DoorHealth.maxDamage)
        {
            DoorHealth doorHealth = FindObjectOfType<DoorHealth>();

            if (doorHealth != null)
            {
                doorHealth.EndSimulation();
            }
        }
    }
}
