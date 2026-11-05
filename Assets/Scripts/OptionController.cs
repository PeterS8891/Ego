// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.Audio;

// public class OptionsController : MonoBehaviour
// {
//     [Header("Audio Mixer Principal")]
//     public AudioMixer masterMixer;

//     [Header("Sliders de la UI")]
//     public Slider sliderMusica;
//     public Slider sliderSonido;
//     public Slider sliderBrillo;

//     [Header("Panel de Filtro para Brillo")]
//     public Image panelBrilloFiltro; // Asigna aquí el panel negro que oscurece la pantalla

//     // Usamos OnEnable en lugar de Start para que funcione aunque el panel empiece desactivado
//     void OnEnable()
//     {
//         float musicaGuardada = PlayerPrefs.GetFloat("VolumenMusica", 1f);
//         float sonidoGuardado = PlayerPrefs.GetFloat("VolumenSonido", 1f);
//         float brilloGuardado = PlayerPrefs.GetFloat("Brillo", 1f);

//         if (sliderMusica != null)
//         {
//             sliderMusica.value = musicaGuardada;
//             sliderMusica.onValueChanged.RemoveAllListeners();
//             sliderMusica.onValueChanged.AddListener(CambiarVolumenMusica);
//         }

//         if (sliderSonido != null)
//         {
//             sliderSonido.value = sonidoGuardado;
//             sliderSonido.onValueChanged.RemoveAllListeners();
//             sliderSonido.onValueChanged.AddListener(CambiarVolumenSonido);
//         }

//         if (sliderBrillo != null)
//         {
//             sliderBrillo.value = brilloGuardado;
//             sliderBrillo.onValueChanged.RemoveAllListeners();
//             sliderBrillo.onValueChanged.AddListener(CambiarBrillo);
//         }

//         CambiarVolumenMusica(musicaGuardada);
//         CambiarVolumenSonido(sonidoGuardado);
//         CambiarBrillo(brilloGuardado);
//     }

//     public void CambiarVolumenMusica(float valor)
//     {
//         if (masterMixer != null)
//         {
//             float decibeles = valor > 0.001f ? Mathf.Log10(valor) * 20f : -80f;
//             masterMixer.SetFloat("MusicaVolume", decibeles);
//         }
//         PlayerPrefs.SetFloat("VolumenMusica", valor);
//     }

//     public void CambiarVolumenSonido(float valor)
//     {
//         if (masterMixer != null)
//         {
//             float decibeles = valor > 0.001f ? Mathf.Log10(valor) * 20f : -80f;
//             masterMixer.SetFloat("SFXVolume", decibeles);
//         }
//         PlayerPrefs.SetFloat("VolumenSonido", valor);
//     }

//     public void CambiarBrillo(float valor)
//     {
//         Debug.Log("Moviendo slider de brillo a: " + valor); // <-- Mira si esto aparece en la consola

//         if (panelBrilloFiltro != null)
//         {
//             Color colorActual = panelBrilloFiltro.color;
//             colorActual.r = 0f;
//             colorActual.g = 0f;
//             colorActual.b = 0f;
//             colorActual.a = Mathf.Lerp(0.85f, 0f, valor);
//             panelBrilloFiltro.color = colorActual;
//         }

//         PlayerPrefs.SetFloat("Brillo", valor);
//     }
// }
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsController : MonoBehaviour
{
    [Header("Audio Mixer Principal")]
    public AudioMixer masterMixer;
    public AudioMixerGroup grupoSFX; // Arrastra aquí el subgrupo "SFX" de tu Audio Mixer

    [Header("Sliders de la UI")]
    public Slider sliderMusica;
    public Slider sliderSonido;
    public Slider sliderBrillo;

    [Header("Panel de Filtro para Brillo")]
    public Image panelBrilloFiltro; // Asigna aquí el panel negro que oscurece la pantalla

    [Header("Sonido de Prueba al Mover Slider")]
    public AudioClip sonidoDeslizador; // Arrastra un clip corto (clic o beep)
    private float tiempoUltimoSonido = 0f;
    public float intervaloSonido = 0.01f; // Controla cada cuánto tiempo suena al arrastrar

    // Usamos OnEnable en lugar de Start para que funcione aunque el panel empiece desactivado
    void OnEnable()
    {
        float musicaGuardada = PlayerPrefs.GetFloat("VolumenMusica", 1f);
        float sonidoGuardado = PlayerPrefs.GetFloat("VolumenSonido", 1f);
        float brilloGuardado = PlayerPrefs.GetFloat("Brillo", 1f);

        if (sliderMusica != null)
        {
            sliderMusica.value = musicaGuardada;
            sliderMusica.onValueChanged.RemoveAllListeners();
            sliderMusica.onValueChanged.AddListener(CambiarVolumenMusica);
        }

        if (sliderSonido != null)
        {
            sliderSonido.value = sonidoGuardado;
            sliderSonido.onValueChanged.RemoveAllListeners();
            sliderSonido.onValueChanged.AddListener(CambiarVolumenSonido);
        }

        if (sliderBrillo != null)
        {
            sliderBrillo.value = brilloGuardado;
            sliderBrillo.onValueChanged.RemoveAllListeners();
            sliderBrillo.onValueChanged.AddListener(CambiarBrillo);
        }

        CambiarVolumenMusica(musicaGuardada);
        CambiarVolumenSonido(sonidoGuardado);
        CambiarBrillo(brilloGuardado);
    }

    void PlayDeslizador()
    {
        // Reproducir sonido de prueba con control de tiempo al mover el slider
        if (sonidoDeslizador != null && Time.time - tiempoUltimoSonido > intervaloSonido)
        {
            tiempoUltimoSonido = Time.time;
            ReproducirSonidoPrueba();
        }
    }
    public void CambiarVolumenMusica(float valor)
    {
        if (masterMixer != null)
        {
            float decibeles = valor > 0.001f ? Mathf.Log10(valor) * 20f : -80f;
            masterMixer.SetFloat("MusicaVolume", decibeles);
        }
        PlayDeslizador();
        PlayerPrefs.SetFloat("VolumenMusica", valor);
    }

    public void CambiarVolumenSonido(float valor)
    {
        if (masterMixer != null)
        {
            float decibeles = valor > 0.001f ? Mathf.Log10(valor) * 20f : -80f;
            masterMixer.SetFloat("SFXVolume", decibeles);
        }
        PlayDeslizador();
        PlayerPrefs.SetFloat("VolumenSonido", valor);
    }

    void ReproducirSonidoPrueba()
    {
        GameObject tempAudio = new GameObject("TempAudio_Slider");
        if (Camera.main != null)
        {
            tempAudio.transform.position = Camera.main.transform.position;
        }

        AudioSource source = tempAudio.AddComponent<AudioSource>();
        source.clip = sonidoDeslizador;
        source.spatialBlend = 0f; // 2D para UI

        // Conectar al canal SFX del Mixer para que el slider regule su volumen
        if (grupoSFX != null)
        {
            source.outputAudioMixerGroup = grupoSFX;
        }

        source.Play();
        Destroy(tempAudio, sonidoDeslizador.length);
    }

    public void CambiarBrillo(float valor)
    {
        Debug.Log("Moviendo slider de brillo a: " + valor); // Mantiene tu debug de control

        if (panelBrilloFiltro != null)
        {
            Color colorActual = panelBrilloFiltro.color;
            colorActual.r = 0f;
            colorActual.g = 0f;
            colorActual.b = 0f;
            colorActual.a = Mathf.Lerp(0.85f, 0f, valor);
            panelBrilloFiltro.color = colorActual;
        }
        PlayDeslizador();
        PlayerPrefs.SetFloat("Brillo", valor);
    }
}