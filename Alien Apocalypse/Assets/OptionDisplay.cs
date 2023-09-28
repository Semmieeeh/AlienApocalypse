using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OptionDisplay : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI nameText,descriptionText;

    public void LoadOption(string name, string desc)
    {
        nameText.text = name;
        descriptionText.text = desc;
    }
}
