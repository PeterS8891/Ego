// using UnityEngine;
// using UnityEngine.InputSystem;

// public class MoverObjetos : MonoBehaviour
// {
//     public GameObject handPoint;
//     private GameObject pickedObject = null;

//     void Update()
//     {
//         // Opcional: Lógica para soltar el objeto si se presiona otra tecla (por ejemplo, la letra G)
//         if (pickedObject != null && Keyboard.current != null && Keyboard.current.gKey.wasPressedThisFrame)
//         {
//             Rigidbody rb = pickedObject.GetComponent<Rigidbody>();
//             if (rb != null)
//             {
//                 rb.useGravity = true;
//                 rb.isKinematic = false;
//             }
//             pickedObject.transform.SetParent(null);
//             pickedObject = null;
//         }
//     }

//     private void OnTriggerStay(Collider other)
//     {
//         // Verificamos si presionamos la tecla E usando el nuevo Input System
//         bool keyPressedC = Keyboard.current != null && Keyboard.current.cKey.isPressed;

//         if (other.gameObject.CompareTag("movil"))
//         {
//             if (keyPressedC && pickedObject == null)
//             {
//                 Rigidbody rb = other.GetComponent<Rigidbody>();
//                 if (rb != null)
//                 {
//                     rb.useGravity = false;
//                     rb.isKinematic = true;
//                 }
                
//                 other.transform.position = handPoint.transform.position;
//                 other.transform.SetParent(handPoint.transform);
//                 pickedObject = other.gameObject;
//                 Debug.Log("Objeto recogido: " + pickedObject.name);
//             }
//         }
//     }
// }
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro; // Necesario si usas TextMeshPro para la interfaz

public class MoverObjetos : MonoBehaviour
{
    public GameObject handPoint;
    private GameObject pickedObject = null;

    [Header("Configuración de Resistencia")]
    public float resistenciaMaxima = 100f;
    public float resistenciaActual = 100f;
    public float velocidadConsumo = 25f;    // Cuánta resistencia baja por segundo al sostener
    public float velocidadRecuperacion = 20f; // Cuánta resistencia sube por segundo al soltar

    [Header("Indicador en Pantalla (Opcional)")]
    public TextMeshProUGUI textoResistencia; // Arrastra aquí tu texto de UI

    void Start()
    {
        resistenciaActual = resistenciaMaxima;
    }

    void Update()
    {
        // 1. Lógica de Resistencia
        if (pickedObject != null)
        {
            // Consumir resistencia mientras sostiene el objeto
            resistenciaActual -= velocidadConsumo * Time.deltaTime;
            resistenciaActual = Mathf.Clamp(resistenciaActual, 0f, resistenciaMaxima);

            // Si la resistencia llega a 0, soltar automáticamente
            if (resistenciaActual <= 0f)
            {
                SoltarObjeto();
                Debug.Log("¡Te has quedado sin resistencia! Objeto soltado.");
            }
        }
        else
        {
            // Recuperar resistencia gradualmente cuando no sostiene nada
            if (resistenciaActual < resistenciaMaxima)
            {
                resistenciaActual += velocidadRecuperacion * Time.deltaTime;
                resistenciaActual = Mathf.Clamp(resistenciaActual, 0f, resistenciaMaxima);
            }
        }

        // 2. Actualizar indicador en pantalla (porcentaje de 0 a 100)
        ActualizarIndicador();

        // 3. Soltar manualmente con la letra G
        if (pickedObject != null && Keyboard.current != null && Keyboard.current.gKey.wasPressedThisFrame)
        {
            SoltarObjeto();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        bool keyPressedC = Keyboard.current != null && Keyboard.current.cKey.isPressed;

        if (other.gameObject.CompareTag("movil"))
        {
            // Solo puede recoger si presiona C, no tiene objeto y le queda resistencia (> 0)
            if (keyPressedC && pickedObject == null && resistenciaActual > 0f)
            {
                Rigidbody rb = other.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.useGravity = false;
                    rb.isKinematic = true;
                }
                
                other.transform.position = handPoint.transform.position;
                other.transform.SetParent(handPoint.transform);
                pickedObject = other.gameObject;
                Debug.Log("Objeto recogido: " + pickedObject.name);
            }
        }
    }

    void SoltarObjeto()
    {
        if (pickedObject != null)
        {
            Rigidbody rb = pickedObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
                rb.isKinematic = false;
            }
            pickedObject.transform.SetParent(null);
            pickedObject = null;
        }
    }

    void ActualizarIndicador()
    {
        // Calcular el porcentaje exacto de 0 a 100
        float porcentaje = Mathf.Round((resistenciaActual / resistenciaMaxima) * 100f);

        // Mostrar en la consola de Unity (para pruebas rápidas)
        // Debug.Log("Resistencia: " + porcentaje + "%");

        // Si asignaste un texto en el Inspector, se actualiza visualmente en pantalla
        if (textoResistencia != null)
        {
            textoResistencia.text = "Resistencia: " + porcentaje + "%";
        }
    }
}