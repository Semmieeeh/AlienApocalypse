using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField]
    Transform indicator;

    [SerializeField]
    Transform[] places;

    [SerializeField]
    Vector3 offset;

    int currentIndex;

    private void Start ( )
    {
        Indicate (0);
    }
    private void OnValidate ( )
    {
        Indicate (currentIndex);
    }

    public void Indicate ( int index )
    {
        index = Mathf.Clamp (index, 0, places.Length);
        currentIndex = index;

        indicator.transform.position = places[index].position + offset;
    }
}
