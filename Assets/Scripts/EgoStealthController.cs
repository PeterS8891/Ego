using UnityEngine;
using UnityEngine.InputSystem;

public class EgoStealthController : MonoBehaviour
{
    private playerController movimientoPlayer;

    [Header("Configuración de Velocidades")]
    public float velocidadNormal = 5.0f;
    public float velocidadSigilo = 2.0f;

    [Header("Sensor de Ruido (1 al 3)")]
    [Range(1, 3)] public int nivelRuido = 1; 
    public bool estaQuieto = false;
    private Vector3 ultimaPosicion;

    [Header("Sistema de Disfraz (Invisibilidad Temporal)")]
    public bool estaDisfrazado = false;
    public float duracionDisfraz = 4.0f;
    private float timerDisfraz = 0f;

    private bool enSigilo = false;

    void Start()
    {
        movimientoPlayer = GetComponent<playerController>();
        ultimaPosicion = transform.position;

        if (movimientoPlayer == null)
        {
            Debug.LogError("No se encontró el script playerController en este objeto.");
        }
    }

    void Update()
    {
        if (movimientoPlayer == null) return;

        // Detectar si el jugador está quieto
        float distanciaMovida = Vector3.Distance(transform.position, ultimaPosicion);
        estaQuieto = (distanciaMovida < 0.001f);
        ultimaPosicion = transform.position;

        // Aplicar velocidad y ruido
        if (enSigilo)
        {
            movimientoPlayer.speed = velocidadSigilo;
            nivelRuido = 1;
        }
        else
        {
            movimientoPlayer.speed = velocidadNormal;
            nivelRuido = 2;
        }

        if (estaQuieto)
        {
            nivelRuido = 1;
        }

        if (Keyboard.current != null && Keyboard.current.spaceKey.isPressed && !enSigilo)
        {
            nivelRuido = 3; 
        }

        // Temporizador del disfraz
        if (estaDisfrazado)
        {
            timerDisfraz -= Time.deltaTime;

            if (timerDisfraz <= 0f)
            {
                estaDisfrazado = false;
                Debug.Log("Disfraz desactivado. Vuelves a ser visible.");
            }
        }
    }

    public void OnHabilidad(InputValue value)
    {
        // Se activa únicamente al presionar la tecla (flanco de subida) y si no está activo ya
        if (value.isPressed && !estaDisfrazado)
        {
            ActivarDisfraz();
        }
    }

    public void OnStealth(InputValue value)
    {
        enSigilo = value.isPressed;
    }

    void LateUpdate()
    {
        if (Keyboard.current != null)
        {
            enSigilo = Keyboard.current.leftShiftKey.isPressed;
        }
    }

    void ActivarDisfraz()
    {
        estaDisfrazado = true;
        timerDisfraz = duracionDisfraz;
        Debug.Log("¡Disfraz activado! Eres invisible temporalmente ante los enemigos.");
    }
}