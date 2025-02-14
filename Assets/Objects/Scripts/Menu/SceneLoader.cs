using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private string selectedLevel;
    private string selectedDifficulty;

    public void SelectLevel(string level)
    {
        selectedLevel = level;
        PlayerPrefs.SetString("SelectedLevel", selectedLevel);
        Debug.Log("Livello selezionato: " + selectedLevel);
    }

    public void SelectDifficulty(string difficulty)
    {
        selectedDifficulty = difficulty;
        PlayerPrefs.SetString("SelectedDifficulty", selectedDifficulty);
        Debug.Log("Difficoltà selezionata: " + selectedDifficulty);
    }

    public void LoadGameScene()
    {
        string level = PlayerPrefs.GetString("SelectedLevel", "1"); // Default Livello 1
        string difficulty = PlayerPrefs.GetString("SelectedDifficulty", "A"); // Default A
        string sceneName = "Livello" + level + "_" + difficulty; // Es: "Livello1_A"

        Debug.Log("Caricamento scena: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
    public void LoadMainMenu()
{
    SceneManager.LoadScene(0);
}

}
