using UnityEngine;
using TMPro;

public class WeaponSelector : MonoBehaviour
{
    public bool isLeft;
    public GameObject warning;

    void Start()
    {
        TMP_Dropdown dropdown = GetComponent<TMP_Dropdown>();

        if (isLeft) {
            dropdown.value = VariableHolder.left_weapon;
        } else {
            dropdown.value = VariableHolder.right_weapon;
        }
        
        warning.SetActive((VariableHolder.left_weapon == 0 && VariableHolder.right_weapon == 0));
    }

    public void DropdownValueChanged(int value)
    {
        if (isLeft) {
            VariableHolder.left_weapon = value;
        } else {
            VariableHolder.right_weapon = value;
        }

        warning.SetActive((VariableHolder.left_weapon == 0 && VariableHolder.right_weapon == 0));
    }
}
