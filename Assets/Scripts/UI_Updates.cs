using UnityEngine;
using TMPro;

public class UI_Updates : MonoBehaviour
{
    public TMP_Text trashCounter;

    public void UpdateTrashCounter(int totalTrash)
    {
        trashCounter.text = $"Trash: {totalTrash}";
    }
}
