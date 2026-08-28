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

    [Header("Pickup")]
    public KeyCode pickupKey = KeyCode.E;
    public KeyCode throwKey = KeyCode.Mouse0;
    public Transform holdPoint;
    public int maxCarry = 1;
    public int pickupUpgrade = 1;

    int carryCount;
    int valuableCount;
    int trashCount;
    public bool isCarrying = false;

    [Header("Throw")]
    public float throwForce = 10f;
    public float maxForce = 20f;
    public float maxDistance = 3f;
    public Transform throwPosition;
    public Vector3 throwDirection = new Vector3(0, 1, 0);

    [Header("Throw Arc")]
    public LineRenderer throwArc;
    public int arcPoints = 30;
    public float arcGravity = 10f;
    
    bool isCharging;
    float chargeTime;

    GameObject trashObject;
    Rigidbody rb;

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (throwArc != null)
            throwArc.enabled = false;
    }

    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        jump = Input.GetButton("Jump");

        if (Input.GetKeyDown(throwKey) && isCarrying)
            StartThrowing();

        if (isCharging)
            ChargeThrow();

        if (Input.GetKeyUp(throwKey) && isCharging)
            ReleaseThrow();


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

    public void PickupTrash(GameObject trashObj)
    {
        if (carryCount >= maxCarry) return;

        
        // Individual trash carry
        if (pickupUpgrade == 1)
        {
            if (isCarrying) return;

            trashObject = trashObj;

            rb = trashObj.GetComponent<Rigidbody>();

            rb.isKinematic = true;
            rb.useGravity = false;
            rb.detectCollisions = true;

            trashObject.transform.SetParent(holdPoint, true);
            trashObject.transform.localPosition = Vector3.zero;

            carryCount++;
            isCarrying = true;

            /*
            if (trashObject.tag == "Trash")
                trashCount++;
            else if (trashObject.tag == "Valuable")
                valuableCount++;
            */
        }
    }

    public void Drop()
    {
        if (!isCarrying || trashObject == null) return;

        isCharging = false;
        chargeTime = 0f;

        if (throwArc != null)
            throwArc.enabled = false;

        trashObject.transform.SetParent(null);

        trashObject.transform.position = holdPoint.position;

        rb.isKinematic = false;
        rb.useGravity = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        carryCount = 0;
        isCarrying = false;
        trashObject = null;
        rb = null;
    }
    private void StartThrowing()
    {
        if (!isCarrying || trashObject == null) return;

        isCharging = true;
        chargeTime = 0f;

        if (throwArc != null)
            throwArc.enabled = true;
    }

    private void ChargeThrow()
    {
        chargeTime += Time.deltaTime;

        float currentForce = Mathf.Min(chargeTime * throwForce, maxForce);
        UpdateThrowArc(currentForce);
    }

    private void ReleaseThrow()
    {
        if (!isCarrying || trashObject == null) return;

        float force = Mathf.Min(chargeTime * throwForce, maxForce);

        Throw(force);

        isCharging = false;
        chargeTime = 0f;

        if (throwArc != null)
            throwArc.enabled = false;
    }

    private void Throw(float force)
    {
        if (trashObject == null || rb == null) return;

        trashObject.transform.SetParent(null);

        trashObject.transform.position = throwPosition.position;

        rb.isKinematic = false;
        rb.useGravity = true;

        Vector3 direction = cameraTransform.forward;

        direction += Vector3.up * throwDirection.y;

        direction.Normalize();

        rb.linearVelocity = direction * force;

        rb.angularVelocity = Vector3.zero;

        carryCount = 0;
        isCarrying = false;

        trashObject = null;
        rb = null;
    }

    private void UpdateThrowArc(float force)
    {
        if (throwArc == null || throwPosition == null) return;

        Vector3 startPosition = throwPosition.position;

        Vector3 direction = cameraTransform.forward;
        direction.Normalize();

        Vector3 velocity = direction * force;

        float distanceMultiplier = Mathf.Clamp01(force / maxForce);

        float totalTime = Mathf.Lerp(0.1f, 1.5f, distanceMultiplier);

        throwArc.positionCount = arcPoints;

        for (int i = 0; i < arcPoints; i++)
        {
            float t = (float)i / (arcPoints - 1);

            float time = t * totalTime;

            Vector3 point = startPosition + velocity * time + 0.5f * Physics.gravity * time * time;

            Vector3 offset = point - startPosition;

            if (offset.magnitude > maxDistance)
            {
                point = startPosition + offset.normalized * maxDistance;
            }

            throwArc.SetPosition(i, point);
        }
    }
}
