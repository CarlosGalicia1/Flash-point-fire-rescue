using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameOverScreen gameOverScreen; // Referencia al script GameOverScreen
    public static bool isGameOver = false;

    public void EndGame(string end)
    {
        isGameOver = true;
        // Pasar el valor del contador de da�o al GameOverScreen
        gameOverScreen.Setup(end);
        Time.timeScale = 0f; // Detener el tiempo del juego
    }

}
