using TMPro;
using UnityEngine;

public class AvailableItemsDisplayer : MonoBehaviour
{
    public GameObject Slot1;
    public GameObject Slot2;
    public GameObject Slot3;
    public GameObject Slot4;

    public void ChangeName(int slot, string name)
    {
        TMP_Text tmpText;
        switch (slot)
        {
            case 1:
                tmpText = Slot1.transform.GetChild(0).GetComponent<TMP_Text>();
                break;
            case 2:
                tmpText = Slot2.transform.GetChild(0).GetComponent<TMP_Text>();
                break;
            case 3:
                tmpText = Slot3.transform.GetChild(0).GetComponent<TMP_Text>();
                break;
            case 4:
                tmpText = Slot4.transform.GetChild(0).GetComponent<TMP_Text>();
                break;
            default:
                tmpText = null;
                break;

        }
        tmpText.SetText(name);
    }

    public void ChangeDescription(int slot, string name)
    {
        TMP_Text tmpText;
        switch (slot)
        {
            case 1:
                tmpText = Slot1.transform.GetChild(1).GetComponent<TMP_Text>();
                break;
            case 2:
                tmpText = Slot2.transform.GetChild(1).GetComponent<TMP_Text>();
                break;
            case 3:
                tmpText = Slot3.transform.GetChild(1).GetComponent<TMP_Text>();
                break;
            case 4:
                tmpText = Slot4.transform.GetChild(1).GetComponent<TMP_Text>();
                break;
            default:
                tmpText = null;
                break;

        }
        tmpText.SetText(name);
    }
}
