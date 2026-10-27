using UnityEngine;

public class camaraFollow : MonoBehaviour
{
    public Transform target; // The target the camera will follow
    public Vector3 offset= new Vector3(0f,40f,-10f);
    public float smoothSpeed = 5f; // The speed at which the camera will follow the target
    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
        transform.LookAt(target); // Make the camera look at the target
    }
}
