using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 25.0f;

    [Header("Jump")]
    public float jumpSpeed = 8.0f;
    public float jumpHeight = 10.0f;
    public float gravity = 10.0f;

    [Header("Camera")]
    public float sensitivity = 5.0f;

    public Transform cameraTransform;
    CharacterController controller;

    float horizontal, vertical;
    float mouseX, mouseY;
    bool jump;

    float pitch;
    float verticalVelocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        jump = Input.GetButton("Jump");
    }

    void FixedUpdate()
    {
        Vector3 moveDirection = new Vector3(horizontal, 0, vertical);
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= speed;

        if (controller.isGrounded)
        {
            verticalVelocity = -2f;

            if (jump) 
            {
                Debug.Log("Jumping");
                verticalVelocity = Mathf.Sqrt(2f * gravity * jumpHeight);
            }
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        float turner = mouseX * sensitivity;
        if (turner  != 0)
        {
            transform.Rotate(0, turner, 0);
        }

        pitch -= mouseY * sensitivity;
        pitch = Mathf.Clamp(pitch, -89, 89);
        cameraTransform.localEulerAngles = new Vector3(pitch, 0f, 0f);

        moveDirection.y = verticalVelocity;
        controller.Move(moveDirection * Time.deltaTime);
    }
}
