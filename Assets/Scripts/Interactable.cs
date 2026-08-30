using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float interactionRange = 5f;
    public LayerMask interactableLayer;

    private PlayerController pc;

    void Start()
    {
        pc = FindAnyObjectByType<PlayerController>();
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;

            // origin, direction, out, duration, layer
            if (Physics.Raycast(pc.cameraTransform.position, pc.cameraTransform.forward, out hit, interactionRange, interactableLayer))
            {

                if (hit.collider.CompareTag("Door"))
                {
                    Door door = hit.collider.GetComponent<Door>();
                    if (door != null)
                        door.OpenDoor();
                    else
                        Debug.Log("Door component is null");
                }

                else if (hit.collider.CompareTag("Sliding"))
                {
                    SlidingDoor door = hit.collider.GetComponent<SlidingDoor>();
                    if (door != null)
                        door.OpenDoor();
                    else
                        Debug.Log("Door component is null");
                }

                else if (hit.collider.CompareTag("Trash") || hit.collider.CompareTag("Valuable")) 
                {
                    if (pc.isCarrying)
                    {
                        pc.Drop();
                    }
                    else
                        pc.PickupTrash(hit.collider.gameObject);
                }

                else if (hit.collider.CompareTag("Tool"))
                {
                    pc.AttachPickerupper();
                }
            }
        }
    }
}
