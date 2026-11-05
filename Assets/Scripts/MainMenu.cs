using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject optionsMenu;
    public GameObject mainMenu;
    public GameObject loadMenu;

    public void OpenMainMenuPanel()
    {
        loadMenu.SetActive(false);
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);

    }

        public void OpenOptionsPanel()
    {
        mainMenu.SetActive(false);
        loadMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }
        public void OpenLoadPanel()
    {
        mainMenu.SetActive(false); 
        optionsMenu.SetActive(false);
        loadMenu.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Level1");
    }

}
