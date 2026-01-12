using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Esta función se llamará desde los botones
    public void SetDifficultyAndStart(int numGuards)
    {
        // Guardamos el número de guardias en la "memoria" del juego
        PlayerPrefs.SetInt("GuardsCount", numGuards);

        // Cargamos la escena del juego 
        SceneManager.LoadScene("SampleScene");
    }
}