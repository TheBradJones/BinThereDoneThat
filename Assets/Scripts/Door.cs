using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    public Transform hinge;

    public float rotateDuration = 1f;

    private bool isOpen = false;
    private bool isAnimating = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = hinge.transform.rotation;
        openRotation = closedRotation * Quaternion.Euler(0f, 90f, 0f);
    }
    public void OpenDoor()
    {
        if (isAnimating) return;

        isOpen = !isOpen;
        StartCoroutine(RotateDoor(isOpen ? openRotation : closedRotation));
    }

    private IEnumerator RotateDoor(Quaternion targetRotation)
    {
        isAnimating = true;
        Quaternion startRotation = hinge.transform.rotation;
        float elapsed = 0f;

        while (elapsed < rotateDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / rotateDuration;
            hinge.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        hinge.transform.rotation = targetRotation;
        isAnimating = false;
    }
}
