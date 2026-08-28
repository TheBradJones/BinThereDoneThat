using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float interactionRange = 5f;
    public LayerMask interactableLayer;

    void FixedUpdate()
    {

        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            // origin, direction, out, duration, layer
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, interactionRange, interactableLayer))
            {

                if (hit.collider.CompareTag("Door"))
                {
                    Door door = hit.collider.GetComponent<Door>();
                    if (door != null)
                        door.OpenDoor();
                    else
                        Debug.Log("Door component is null");
                }

                if (hit.collider.CompareTag("Sliding"))
                {
                    SlidingDoor door = hit.collider.GetComponent<SlidingDoor>();
                    if (door != null)
                        door.OpenDoor();
                    else
                        Debug.Log("Door component is null");
                }
            }
        }
    }
}
