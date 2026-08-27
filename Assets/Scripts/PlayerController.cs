using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 25.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float sensitivity = 5f;

    CharacterController controller;

    float horizontal, vertical;
    float mouseX, mouseY;
    bool jump;

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
        Vector3 moveDirection = Vector3.zero;

        if (controller.isGrounded)
        {
            moveDirection = new Vector3(horizontal, 0, vertical);
            moveDirection = transform.TransformDirection(moveDirection);

            moveDirection *= speed;

            if (jump)
                moveDirection.y = jumpSpeed;
        }

        float turner = mouseX * sensitivity;
        if (turner  != 0)
        {
            transform.eulerAngles += new Vector3(0, turner, 0);
        }

        float looker = -mouseY * sensitivity;
        if (looker != 0)
        {
            transform.eulerAngles += new Vector3(looker, 0, 0);
        }

        moveDirection.y -= gravity * Time.deltaTime;

        controller.Move(moveDirection * Time.deltaTime);
    }
}
