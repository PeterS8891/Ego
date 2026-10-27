using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemigoGuardia : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador;
    private EgoStealthController egoScript;
    private Animator animator;
    private CharacterController controller;

    [Header("Parámetros de Visión y Detección")]
    public float rangoVision = 12f;
    [Range(10f, 180f)] public float anguloVision = 90f;
    public float alturaOjos = 1.0f;
    public LayerMask mascaraObstaculos; // NUEVO: Para marcar qué capas son paredes/obstáculos

    [Header("Velocidades y Combate")]
    public float velocidadPatrulla = 1.5f;
    public float velocidadPersecucion = 3.5f;
    public float distanciaAtaque = 1.5f; 
    public float tiempoEntreAtaques = 1.5f;
    private float rixTimeAtaque = 0f;

    [Header("Memoria / Pérdida de Rastro")]
    public float tiempoParaPerderRastro = 4.0f; 
    private float temporizadorPerdida = 0f;
    private bool estaViendoAlJugador = false;

    [Header("Físicas")]
    public float gravedad = -9.81f;
    private float velocidadVertical;

    private enum Estado { Patrullando, Persecucion }
    [Header("Estado Actual (Sólo lectura)")]
    [SerializeField] private Estado estadoActual = Estado.Patrullando;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        BuscarJugador();
    }

    void BuscarJugador()
    {
        if (jugador == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
            {
                jugador = p.transform;
            }
        }

        if (jugador != null)
        {
            egoScript = jugador.GetComponent<EgoStealthController>();
        }
    }

    void Update()
    {
        if (jugador == null || egoScript == null)
        {
            BuscarJugador();
            return; 
        }

        if (rixTimeAtaque > 0)
        {
            rixTimeAtaque -= Time.deltaTime;
        }

        // Gravedad
        if (controller.isGrounded && velocidadVertical < 0)
        {
            velocidadVertical = -2f;
        }
        else
        {
            velocidadVertical += gravedad * Time.deltaTime;
        }

        // Si usa el disfraz, pierde el rastro de inmediato
        if (egoScript.estaDisfrazado && estadoActual == Estado.Persecucion)
        {
            PerderRastro();
            return;
        }

        switch (estadoActual)
        {
            case Estado.Patrullando:
                Patrullar();
                VerificarVision();
                VerificarRuido();
                break;

            case Estado.Persecucion:
                Perseguir();
                break;
        }
    }

    void Patrullar()
    {
        transform.Rotate(Vector3.up * 30f * Time.deltaTime);

        Vector3 movimiento = transform.forward * velocidadPatrulla;
        movimiento.y = velocidadVertical;
        controller.Move(movimiento * Time.deltaTime);

        if (animator != null)
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isRunning", false);
        }

        VerificarVision();
    }

    void VerificarVision()
    {
        if (egoScript.estaDisfrazado) 
        {
            estaViendoAlJugador = false;
            return;
        }

        Vector3 posicionOjosZombie = transform.position + Vector3.up * alturaOjos;
        Vector3 posicionOjosJugador = jugador.position + Vector3.up * alturaOjos;

        Vector3 direccionAlJugador = posicionOjosJugador - posicionOjosZombie;
        float distancia = direccionAlJugador.magnitude;

        // 1. Validar distancia y ángulo de visión primero
        if (distancia <= rangoVision)
        {
            float angulo = Vector3.Angle(transform.forward, direccionAlJugador);
            if (angulo <= anguloVision / 2f)
            {
                // 2. VALIDACIÓN DE PAREDES: Lanzamos un rayo desde los ojos del zombie hacia el jugador
                RaycastHit hit;
                // Si el rayo choca contra algo antes de llegar al jugador...
                if (Physics.Raycast(posicionOjosZombie, direccionAlJugador.normalized, out hit, distancia))
                {
                    // Comprobamos si lo que golpeó el rayo ES el jugador
                    if (hit.transform.CompareTag("Player"))
                    {
                        // ¡Linea de visión limpia! Te ve
                        estaViendoAlJugador = true;
                        temporizadorPerdida = 0f;

                        if (estadoActual == Estado.Patrullando)
                        {
                            IniciarPersecucion();
                        }
                        return;
                    }
                    else
                    {
                        // El rayo chocó contra una pared u otro objeto antes de llegar a ti
                        estaViendoAlJugador = false;
                        return;
                    }
                }
            }
        }

        estaViendoAlJugador = false;
    }

    void VerificarRuido()
    {
        if (egoScript.estaDisfrazado) return;

        float distancia = Vector3.Distance(transform.position, jugador.position);

        // Opcional: El ruido podría ignorar paredes si quieres que te escuche a través de ellas, 
        // pero por ahora mantenemos la distancia de ruido estándar cercana.
        if (egoScript.nivelRuido >= 3 && distancia < 5f)
        {
            IniciarPersecucion();
        }
    }

    void IniciarPersecucion()
    {
        estadoActual = Estado.Persecucion;
        temporizadorPerdida = 0f;
        Debug.Log("¡El enemigo te ha visto (línea de visión despejada)!");
    }

    void Perseguir()
    {
        // Volvemos a comprobar si te sigue viendo o hay una pared de por medio
        VerificarVision();

        float distancia = Vector3.Distance(transform.position, jugador.position);

        // Si te ve o está a distancia de ataque, reseteamos el temporizador de pérdida de rastro
        if (estaViendoAlJugador || distancia <= distanciaAtaque)
        {
            temporizadorPerdida = 0f; 
        }
        else
        {
            // Si te metiste detrás de una pared y ya no te ve, empieza la cuenta atrás para rendirse
            temporizadorPerdida += Time.deltaTime;
            if (temporizadorPerdida >= tiempoParaPerderRastro)
            {
                PerderRastro();
                return;
            }
        }

        if (distancia > distanciaAtaque)
        {
            transform.LookAt(new Vector3(jugador.position.x, transform.position.y, jugador.position.z));

            Vector3 movimiento = transform.forward * velocidadPersecucion;
            movimiento.y = velocidadVertical;
            controller.Move(movimiento * Time.deltaTime);

            if (animator != null)
            {
                animator.SetBool("isWalking", false);
                animator.SetBool("isRunning", true);
            }
        }
        else
        {
            Vector3 movimientoDetenido = Vector3.zero;
            movimientoDetenido.y = -2f;
            controller.Move(movimientoDetenido * Time.deltaTime);

            if (animator != null)
            {
                animator.SetBool("isWalking", false);
                animator.SetBool("isRunning", false);

                if (rixTimeAtaque <= 0)
                {
                    animator.SetTrigger("Atacar");
                    rixTimeAtaque = tiempoEntreAtaques;
                }
            }
        }
    }

    void PerderRastro()
    {
        estadoActual = Estado.Patrullando;
        temporizadorPerdida = 0f;
        Debug.Log("Te ocultaste tras la pared. El enemigo perdió tu rastro y vuelve a patrullar.");

        if (animator != null)
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isWalking", true);
        }
    }
}