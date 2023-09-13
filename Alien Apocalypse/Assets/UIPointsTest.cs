using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPointsTest : MonoBehaviour
{
    public NotificationData[] notis;

    public bool start;

    public UIPointsManager manager;

    private void Update()
    {
        if (start)
        {
            start = false;

            foreach (var item in notis)
            {
                manager.AddNotification(item);
            }
        }
    }
}
