using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Importante para reiniciar la escena

public class GameOverScreen : MonoBehaviour
{
    public Text damageCounterText; // Texto que mostrará el contador de daño
    public Button restartButton; // Botón para reiniciar el juego

    public void Setup(int damageCounter)
    {
        gameObject.SetActive(true);
        damageCounterText.text = "Daño acumulado: " + damageCounter.ToString();
        Time.timeScale = 0f; // Detener el tiempo del juego
    }
    public void RestartGame()
    {
        GameConstants.Reset();

        Time.timeScale = GameConstants.minuteToRealTime; // Reanudar el tiempo del juego
        gameObject.SetActive(false);
        GameManager.isGameOver = false;

        // Reiniciar el tiempo en el TimeManager
        TimeManager timeManager = FindObjectOfType<TimeManager>();
        if (timeManager != null)
        {
            timeManager.ResetTime();
        }

        // Reiniciar la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void Start()
    {
        restartButton.onClick.AddListener(RestartGame); // Asignar el método de reinicio al botón
    }
}
