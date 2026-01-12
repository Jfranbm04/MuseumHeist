using UnityEngine;
using TMPro; 

public class GameManager : MonoBehaviour
{
    public static GameManager instance; 
    public TextMeshProUGUI gemsText;
    public int remainingGems = 10;

    void Awake()
    {
        
        instance = this;
    }

    void Start()
    {
        UpdateUI();
    }

    public void CollectGem()
    {
        remainingGems--;
        UpdateUI();

        if (remainingGems <= 0)
        {
            ShowVictory();
        }
    }

    void UpdateUI()
    {
        gemsText.text = "Gemas restantes: " + remainingGems;
    }

    void ShowVictory()
    {
        Debug.Log("¡Has ganado!");
        
    }
}