using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Necesario para reiniciar la escena

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public TextMeshProUGUI gemsText;
    public GameObject panelGanar;
    public GameObject panelPerder;
    public int remainingGems = 10;

    private bool gameEnded = false;

    void Awake() { instance = this; }

    void Start()
    {
        panelGanar.SetActive(false);
        panelPerder.SetActive(false);
        UpdateUI();
    }

    public void CollectGem()
    {
        if (gameEnded) return;
        remainingGems--;
        UpdateUI();
        if (remainingGems <= 0) ShowVictory();
    }

    public void PlayerCaught()
    {
        if (gameEnded) return;
        gameEnded = true;
        panelPerder.SetActive(true);
        Time.timeScale = 0f; // Pausa el juego
    }

    void ShowVictory()
    {
        gameEnded = true;
        panelGanar.SetActive(true);
        Time.timeScale = 0f; // Pausa el juego
    }

    void UpdateUI() { gemsText.text = "Gemas restantes: " + remainingGems; }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}