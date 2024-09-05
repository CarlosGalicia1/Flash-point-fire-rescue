using UnityEngine;

public class WallHealth : MonoBehaviour
{
    private int currentHealth; // Resistencia actual
    private Renderer objectRenderer; // Renderer del objeto para cambiar materiales

    void Start()
    {
        currentHealth = GameConstants.maxHealth;
        objectRenderer = GetComponent<Renderer>();
    }

    void OnMouseDown()
    {
        if (GameManager.isGameOver) return; // No hacer nada si el juego ha terminado

        TakeDamage(GameConstants.explosionDamage);
    }

    private void TakeDamage(int damage)
    {
        // Disminuir la resistencia al hacer clic
        currentHealth -= damage;
        GameConstants.damageCounter++;

        GridElementManager gridElementManager = FindObjectOfType<GridElementManager>();
        if (gridElementManager != null)
        {
            gridElementManager.UpdateGridElements(GameConstants.damageCounter);
        }

        // Verificar si la resistencia es igual a 2
        if (currentHealth == 2)
        {
            objectRenderer.material = GameConstants.damagedMaterial;
        }

        // Verificar si la resistencia es 0 o menos
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }

        // Terminar la simulaci�n si el contador de da�o alcanza el valor maximo permitido
        if (GameConstants.damageCounter >= GameConstants.maxDamage)
        {
            EndSimulation();
        }
    }
    public void EndSimulation()
    {
        return;
    }
}
