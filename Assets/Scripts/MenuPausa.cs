using UnityEngine;
using UnityEngine.SceneManagement;

public class MainPausa : MonoBehaviour
{
    public GameObject PauseMenu;
    public GameObject OptionMenu;
    public GameObject SaveMenu;

    public void OpenPauseMenuPanel()
    {
        SaveMenu.SetActive(false);
        OptionMenu.SetActive(false);
        PauseMenu.SetActive(true);

    }

    public void OpenOptionPanel()
    {
        PauseMenu.SetActive(false);
        SaveMenu.SetActive(false);
        OptionMenu.SetActive(true);
    }
    public void OpenSavePanel()
    {
        PauseMenu.SetActive(false);
        OptionMenu.SetActive(false);
        SaveMenu.SetActive(true);
    }

    public void ReturnMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
    }

    public void ReturnGame()
    {
        PauseMenu.SetActive(false);
        OptionMenu.SetActive(false);
        SaveMenu.SetActive(false);
        Time.timeScale = 1f;
    }

}
