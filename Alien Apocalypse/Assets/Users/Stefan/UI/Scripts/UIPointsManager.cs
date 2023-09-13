using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class UIPointsManager : MonoBehaviour
{
    [SerializeField]
    PointNotification pointNotification;

    private Queue<NotificationData> notificationQueue = new();

    public void AddNotification(NotificationData data)
    {
        notificationQueue.Enqueue(data);
    }
    public void AddNotification(int points, string message)
    {
        AddNotification(new(points,message));
    }

    private void Update()
    {
        if (!pointNotification.Active && notificationQueue.Count > 0)
        {
            var noti = notificationQueue.Dequeue();
            pointNotification.Notify(noti.points,noti.message);
        }
    }

    
}

[System.Serializable]
public class NotificationData
{
    public int points;
    public string message;

    public NotificationData(int points, string message)
    {
        this.points = points;
        this.message = message;
    }
}
