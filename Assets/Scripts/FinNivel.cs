using UnityEngine;
using UnityEngine.SceneManagement;

public class FinNivel : MonoBehaviour
{
    //Iniciar variables de objetivos para superar el nivel
    public int objetivosTotales = 1;
    private int objetivosSuperados = 0;

    //Texto en pantalla indicando que objetivos faltan
    public TMPro.TextMeshProUGUI objetivosText;

    //Indicar a que nivel se pasara
    [Header("Nivel Siguiente")]
    public GameObject siguienteNivel;


    public void NextLevel()
    {
        //si cumple con los objetivos cambia de nivel, sino sale texto indicando que falta y tras 5 segundos desaparece

        if( objetivosSuperados == objetivosTotales)
        {
            SceneManager.LoadScene("Level2");
    }
    else
        {
            objetivosText.text = "Faltan objetivos por superar";
            Invoke("ClearText", 5f);
        }
    }
}