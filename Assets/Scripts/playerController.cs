using UnityEngine;
using UnityEngine.InputSystem;

public class playerController : MonoBehaviour
{
    public float speed = 5f;
    private CharacterController controller;
    private Vector2 moveInput;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;
    private float verticalVelocity;
    private bool jumpPressed;
    private Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        bool isGrounded = controller.isGrounded;
        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // Small negative value to keep the player grounded
        }

        if (jumpPressed && isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpPressed = false; // Reset jumpPressed after jumping
            
            // --- ANIMATOR: Activar salto (si usas un Trigger) ---
            animator.SetTrigger("Jump");
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        //girar

        if (move.sqrMagnitude > 0.01f){
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(moveInput.x, 0, moveInput.y));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15f * Time.deltaTime);
        }
        //fin girar
        
        move.y = verticalVelocity / speed; // Apply vertical velocity to the move vector

        controller.Move(move * speed * Time.deltaTime);

        // ==========================================
        // --- AQUÍ ACTUALIZAMOS LAS ANIMACIONES ---
        // ==========================================
        
        // 1. Calcular la cantidad de movimiento (0 si está quieto, hasta 1 si camina/corre)
        float moveAmount = moveInput.magnitude;
        
        // Si usas un Blend Tree (Idle/Walk con un parámetro float llamado "Speed"):
        animator.SetFloat("Speed", moveAmount);

        // O si prefieres usar un booleano simple ("isWalking"):
        // animator.SetBool("isWalking", moveAmount > 0.1f);

        // 2. Informar al animator si está en el suelo o en el aire
        animator.SetBool("IsGrounded", isGrounded);
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        Debug.Log("Move Input: " + moveInput);
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            jumpPressed = true;
            Debug.Log("Jump Pressed");
        }
    }
}