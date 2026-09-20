using UnityEngine;
using System.Collections;

public class BinDiesel : MonoBehaviour
{

    [Header("Trash")]
    public int total = 0;
    public int valuables = 0;
    public int trash = 0;
    public int upgradeRequirement = 5;

    UI_Updates UIU;

    private void Start()
    {
        UIU = FindAnyObjectByType<UI_Updates>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trash"))
        {
            trash++;
            Debug.Log("ugh");
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Valuable"))
        {
            valuables++;
            Debug.Log("yum");
            Destroy(other.gameObject);
        }

        UpdateCounter();
    }

    void UpdateCounter()
    {
        total = valuables + trash;

        UIU.UpdateTrashCounter(total);

        if (total >= upgradeRequirement)
        {
            UpgradeBinjamin();
        }
    }

    void UpgradeBinjamin()
    {
        Debug.Log("Gurgle");
    }
}
