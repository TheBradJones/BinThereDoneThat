using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

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
    public int toolUpgrade = 1;

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

    [Header("Picker Upper Tool")]
    public GameObject binObj;
    public GameObject toolObj;
    public Transform binPos;
    public Transform toolPos;
    public Transform trashPoint;    

    private bool holding;

    [Header("Emptying")]
    public float tiltSpeed = 1.5f;
    public float returnSpeed = 1.5f;
    public float maxTiltAngle = 110f;
    public float releaseTiltThreshold = 0.5f;

    float tilt;
    Quaternion binStartRotation;

    GameObject trashObject;
    Rigidbody trashRb;

    List<Rigidbody> binTrash = new List<Rigidbody>();
    bool binUnlocked;

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (throwArc != null)
            throwArc.enabled = false;

        binStartRotation = binObj.transform.localRotation;
    }

    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        jump = Input.GetButton("Jump");

        if (toolUpgrade == 1)
        {
            if (Input.GetKeyDown(throwKey) && isCarrying)
                StartThrowing();

            if (isCharging)
                ChargeThrow();

            if (Input.GetKeyUp(throwKey) && isCharging)
                ReleaseThrow();
        }

        if (toolUpgrade == 2)
        {
            bool holdKey = Input.GetKey(throwKey);

            float target = holdKey ? 1f : 0f;
            float speed = holdKey ? tiltSpeed : returnSpeed;

            tilt = Mathf.MoveTowards(tilt, target, speed * Time.deltaTime);
            tilt = Mathf.Clamp01(tilt); // stop at 0 and 1

            //float angle = Mathf.Abs(maxTiltAngle) * tilt * (reverseDirection)

            if (tilt >= releaseTiltThreshold)
                UnlockTrashPos();

            binObj.transform.localRotation = binStartRotation * Quaternion.AngleAxis(maxTiltAngle * tilt, Vector3.forward);

        }
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
        
        if (toolUpgrade == 1)
        {
            // Individual trash carry
            if (isCarrying) return;

            trashObject = trashObj;

            trashRb = trashObj.GetComponent<Rigidbody>();

            trashRb.isKinematic = true;
            trashRb.useGravity = false;
            trashRb.detectCollisions = true;

            trashObject.transform.SetParent(holdPoint, true);
            trashObject.transform.localPosition = Vector3.zero;

            carryCount++;
            isCarrying = true;
        }
        if (toolUpgrade == 2)
        {

            // How much can be carried at once
            maxCarry = 10;

            trashObject = trashObj;

            // Get rigidbody
            trashRb = trashObj.GetComponent<Rigidbody>();

            // Immobilize trashObj
            trashRb.isKinematic = false;
            trashRb.useGravity = true;
            trashRb.detectCollisions = true;

            // Attach trashObj to parent
            trashObject.transform.SetParent(trashPoint, true);
            trashObject.transform.localPosition = Vector3.zero;
            trashObject.transform.localScale = trashObject.transform.parent.localScale;

            // Increment how many being carried
            carryCount++;
            binTrash.Add(trashRb);

            // Lock position in bin after allowing it find its own location. this is so they dont all lock onto the same location in the bin
            StartCoroutine(LockTrashPos(trashRb));
        }
    }

    private IEnumerator LockTrashPos(Rigidbody trashRb)
    {
        yield return new WaitForSeconds(1f);

        trashRb.isKinematic = true;
        trashRb.useGravity = false;
        trashRb.detectCollisions = true;

        // must collide with other trash
    }

    private void UnlockTrashPos()
    {
        for (int i = binTrash.Count - 1; i >= 0; i--)
        {
            Rigidbody rb = binTrash[i];

            if (rb == null)
            {
                binTrash.RemoveAt(i);
                continue;
            }

            rb.transform.SetParent(null, true);
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.detectCollisions = true;

            binTrash.RemoveAt(i);
            carryCount--;
        }
        
    }

    public void Drop()
    {
        if (toolUpgrade == 1)
        {
            if (!isCarrying || trashObject == null) return;

            isCharging = false;
            chargeTime = 0f;

            if (throwArc != null)
                throwArc.enabled = false;

            trashObject.transform.SetParent(null);

            trashObject.transform.position = holdPoint.position;

            trashRb.isKinematic = false;
            trashRb.useGravity = true;

            trashRb.linearVelocity = Vector3.zero;
            trashRb.angularVelocity = Vector3.zero;

            carryCount = 0;
            isCarrying = false;
            trashObject = null;
            trashRb = null;
        }
        if (toolUpgrade == 2)
        {

        }
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
        if (trashObject == null || trashRb == null) return;

        trashObject.transform.SetParent(null);

        trashObject.transform.position = throwPosition.position;

        trashRb.isKinematic = false;
        trashRb.useGravity = true;

        Vector3 direction = cameraTransform.forward;

        direction += Vector3.up * throwDirection.y;

        direction.Normalize();

        trashRb.linearVelocity = direction * force;

        trashRb.angularVelocity = Vector3.zero;

        carryCount = 0;
        isCarrying = false;

        trashObject = null;
        trashRb = null;
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

    public void AttachPickerupper()
    {
        if (toolUpgrade == 2)
        {
            holding = true;

            if (trashObject != null)
                Destroy(trashObject);

            GameObject bin = Instantiate(binObj, binPos.position, binPos.rotation);
            GameObject tool = Instantiate(toolObj, toolPos.position, toolPos.rotation);

            Rigidbody binRb = bin.GetComponent<Rigidbody>();
            Rigidbody toolRb = tool.GetComponent<Rigidbody>();

            binRb.isKinematic = true;
            toolRb.isKinematic = true;

            bin.transform.SetParent(binPos, true);
            tool.transform.SetParent(toolPos, true);

            bin.transform.localPosition = Vector3.zero;
            tool.transform.localPosition = Vector3.zero;

            binObj = bin;
            binStartRotation = bin.transform.localRotation;
            tilt = 0;
        }
    }
}
