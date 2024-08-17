using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameOverScreen gameOverScreen; // Referencia al script GameOverScreen
    public static bool isGameOver = false;

    public void EndGame()
    {
        isGameOver = true;
        // Pasar el valor del contador de daño al GameOverScreen
        gameOverScreen.Setup(DoorHealth.damageCounter);
        Time.timeScale = 0f; // Detener el tiempo del juego
    }

}
