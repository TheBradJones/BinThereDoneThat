using UnityEngine;
using System.Collections;

public class BinDiesel : MonoBehaviour
{

    [Header("Trash")]
    public int valuables = 0;
    public int trash = 0;
    public int upgradeRequirement = 5;

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
        int total = valuables + trash;

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
