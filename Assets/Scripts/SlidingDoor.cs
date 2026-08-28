using UnityEngine;
using System.Collections;

public class SlidingDoor : MonoBehaviour
{
    public Transform leftDoor;
    public Transform rightDoor;
    public float duration = 1f;

    private bool isOpen = false;
    private bool isAnimating = false;
    private float lClosed;
    private float lOpen;
    private float rClosed;
    private float rOpen;

    void Start()
    {
        lClosed = leftDoor.transform.localPosition.z;
        lOpen = lClosed - 2.8f;

        rClosed = rightDoor.transform.localPosition.z;
        rOpen = rClosed + 2.8f;
    }
    public void OpenDoor()
    {
        if (isAnimating) return;

        isOpen = !isOpen;
        StartCoroutine(MoveLeft(isOpen ? lOpen : lClosed));
        StartCoroutine(MoveRight(isOpen ? rOpen : rClosed));
    }

    private IEnumerator MoveLeft(float targetPos)
    {
        isAnimating = true;
        float startPos = leftDoor.transform.localPosition.z;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            Vector3 pos = leftDoor.transform.localPosition;
            pos.z = Mathf.Lerp(startPos, targetPos, t);
            leftDoor.transform.localPosition = pos;

            yield return null;
        }

        Vector3 finalPos = leftDoor.transform.localPosition;
        finalPos.z = targetPos;
        leftDoor.transform.localPosition = finalPos;

        isAnimating = false;
    }
    private IEnumerator MoveRight(float targetPos)
    {
        isAnimating = true;
        float startPos = rightDoor.transform.localPosition.z;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            Vector3 pos = rightDoor.transform.localPosition;
            pos.z = Mathf.Lerp(startPos, targetPos, t);
            rightDoor.transform.localPosition = pos;

            yield return null;
        }

        Vector3 finalPos = rightDoor.transform.localPosition;
        finalPos.z = targetPos;
        rightDoor.transform.localPosition = finalPos;

        isAnimating = false;
    }
}
