using UnityEngine;

public class empujeEsfera : MonoBehaviour
{

    // Fuerza con la que será empujada la esfera
    public float fuerzaDeEmpuje = 5f;

    // Este método se llama automáticamente cuando algo choca con este objeto
    private void OnCollisionEnter(Collision collision)
    {
        // Verificamos si el objeto que chocó tiene la etiqueta "Player"
        // (Asegúrate de que tu jugador tenga asignada esta etiqueta)
        if (collision.gameObject.CompareTag("Player"))
        {
            // Obtenemos la dirección desde el jugador hacia la esfera
            Vector3 direccionEmpuje = transform.position - collision.gameObject.transform.position;
            
            // Normalizamos el vector para que la fuerza sea constante,
            // sin importar qué tan cerca o lejos fue el impacto inicial
            direccionEmpuje = direccionEmpuje.normalized;

            // Obtenemos el Rigidbody de la esfera
            Rigidbody rbEsfera = GetComponent<Rigidbody>();

            // Aplicamos una fuerza de impulso instantáneo en esa dirección
            if (rbEsfera != null)
            {
                rbEsfera.AddForce(direccionEmpuje * fuerzaDeEmpuje, ForceMode.Impulse);
                Debug.Log("¡La esfera fue empujada por el jugador!");
            }
        }
    }
}