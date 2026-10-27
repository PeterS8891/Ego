using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ZombieController : MonoBehaviour
{
    private CharacterController controller;
    private Animator animator;

    [Header("Velocidades de Movimiento")]
    public float velocidadCaminar = 2.0f;
    public float velocidadCorrer = 5.0f;
    public float gravedad = -9.81f;
    public float alturaSalto = 1.5f;
    public float velocidadGiro = 15f;

    private float velocidadVertical;
    private bool estaMuerto = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (estaMuerto) return;

        // 1. Manejo de Gravedad y Suelo
        bool enSuelo = controller.isGrounded;
        if (enSuelo && velocidadVertical < 0)
        {
            velocidadVertical = -2f; 
        }

        // 2. Capturar entradas de movimiento (WASD / Joystick)
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 direccion = new Vector3(moveX, 0, moveZ).normalized;

        // Determinar si está corriendo (manteniendo Shift Izquierdo)
        bool estaCorriendo = Input.GetKey(KeyCode.LeftShift);
        float velocidadActual = estaCorriendo ? velocidadCorrer : velocidadCaminar;

        // 3. Mecánica de Giro Suave
        if (direccion.magnitude >= 0.1f)
        {
            float anguloObjetivo = Mathf.Atan2(direccion.x, direccion.z) * Mathf.Rad2Deg;
            Quaternion rotacionObjetivo = Quaternion.Euler(0, anguloObjetivo, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, velocidadGiro * Time.deltaTime);
        }

        // 4. Movimiento horizontal y aplicación de velocidad en el Animator
        Vector3 movimiento = direccion * velocidadActual;
        
        // Calcular magnitud para el Blend Tree (0 = Idle, 0.5 = Caminar, 1 = Correr)
        float magnitudAnimacion = direccion.magnitude * (estaCorriendo ? 1.0f : 0.5f);
        animator.SetFloat("Velocidad", magnitudAnimacion);

        // 5. Mecánica de Salto
        if (Input.GetButtonDown("Jump") && enSuelo)
        {
            velocidadVertical = Mathf.Sqrt(alturaSalto * -2f * gravedad);
            animator.SetTrigger("Saltar");
        }

        // Aplicar gravedad vertical
        velocidadVertical += gravedad * Time.deltaTime;
        movimiento.y = velocidadVertical;

        // Mover el personaje
        controller.Move(movimiento * Time.deltaTime);

        // 6. Mecánicas de Ataque (Golpear con C / Patear con V)
        if (Input.GetKeyDown(KeyCode.C))
        {
            Golpear();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            Patear();
        }

        // 7. Mecánica de Muerte (Tecla M para probar)
        if (Input.GetKeyDown(KeyCode.M))
        {
            Morir();
        }
    }

    void Golpear()
    {
        animator.SetTrigger("Golpear");
        Debug.Log("El zombie ejecuta un golpe.");
    }

    void Patear()
    {
        animator.SetTrigger("Patear");
        Debug.Log("El zombie ejecuta una patada.");
    }

    public void Morir()
    {
        if (estaMuerto) return;
        estaMuerto = true;
        
        animator.SetBool("EstaMuerto", true);
        Debug.Log("El zombie ha muerto.");

        // Desactivar el CharacterController para que no bloquee objetos o físicas al morir
        if (controller != null)
        {
            controller.enabled = false;
        }
    }
}