using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    // Instancia estática para que sea accesible desde cualquier botón fácilmente
    public static SoundManager Instance;

    [Header("Audio Mixer y Grupo SFX")]
    public AudioMixerGroup grupoSFX; // Arrastra aquí el grupo "SFX" de tu Audio Mixer
    public AudioClip sonidoSalir;   // El sonido estándar para los botones
    public AudioClip sonidoEntrar;  // El sonido estándar para los botones
    public AudioClip sonidoSeleccion; // El sonido estándar para los botones

    void Awake()
    {
        // Patrón Singleton para mantener un único gestor de sonido global
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Opcional si cambias de escena, o déjalo por escena
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Método para los botones de menú e ingresar
    public void ReproducerSonidoEntrada()
    {
        ReproducirClip(sonidoEntrar);
    }

    // Método exclusivo para el botón de salir
    public void ReproducerSonidoSalir()
    {
        ReproducirClip(sonidoSalir);
    }

    // Método para el botón de selección
    public void ReproducerSonidoSeleccion()
    {
        ReproducirClip(sonidoSeleccion);
    }

    // Método universal que llamarán los botones
// Método interno auxiliar para emitir el audio de forma temporal
    private void ReproducirClip(AudioClip clip)
    {
        if (clip != null)
        {
            GameObject tempAudioObj = new GameObject("SFX_Boton_Temp");
            if (Camera.main != null)
            {
                tempAudioObj.transform.position = Camera.main.transform.position;
            }

            AudioSource audioSource = tempAudioObj.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.spatialBlend = 0f; // 2D para interfaz

            if (grupoSFX != null)
            {
                audioSource.outputAudioMixerGroup = grupoSFX;
            }

            audioSource.Play();
            Destroy(tempAudioObj, clip.length);
        }
    }
}