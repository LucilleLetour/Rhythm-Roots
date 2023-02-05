using RhythmReader;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public void StartAsgardLevel()
    {
        SceneManager.LoadScene("Asgard");
    }

    public void StartHelLevel()
    {
        SceneManager.LoadScene("Hel");
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
