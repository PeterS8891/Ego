using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Importante para usar Keyboard.current

public class PauseMenuController : MonoBehaviour
{
    [Header("Panel de Pausa")]
    public GameObject panelPausa; // Arrastra aquí tu panel de pausa

    private bool juegoPausado = false;

    void Update()
    {
        // Usamos Keyboard.current del nuevo Input System para evitar el error de la clase UnityEngine.Input
        if (Keyboard.current != null && Keyboard.current.mKey.wasPressedThisFrame)
        {
            if (juegoPausado)
            {
                Reanudar();
            }
            else
            {
                Pausar();
            }
        }
    }

    public void Reanudar()
    {
        if (panelPausa != null) panelPausa.SetActive(false);
        Time.timeScale = 1f; // Reanuda el tiempo normal del juego
        juegoPausado = false;
    }

    public void Pausar()
    {
        if (panelPausa != null) panelPausa.SetActive(true);
        Time.timeScale = 0f; // Congela el tiempo del juego
        juegoPausado = true;
    }

    public void VolverAlMenu(string nombreEscenaMenu)
    {
        Time.timeScale = 1f; // Importante restablecer el tiempo antes de cambiar de escena
        SceneManager.LoadScene(nombreEscenaMenu);
    }
}