using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
public static class Utilities
{
    private static Camera m_Camera;

    /// <summary>
    /// The cached camera (Read Only)
    /// </summary>
    public static Camera Camera
    {
        get
        {
            if (m_Camera == null)
            {
                m_Camera = Camera.main;
            }
            return m_Camera;
        }
    }

    /// <summary>
    /// Finds the first child transform named "name" in a parent
    /// </summary>
    /// <param name="parent">The target transform / parent of the children</param>
    /// <param name="name">The name used to find a child</param>
    /// <returns>Child named "name". Null if not found</returns>

    public static Transform FindChildByName(this Transform parent, string name)
    {
        var children = parent.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child.name == name)
                return child;
        }

        return null;
    }

    /// <summary>
    /// loops through an transforms entire hierarchy to find a child named name
    /// </summary>
    /// <param name="parent">The target transform to loop through</param>
    /// <param name="name">The name used to find a child</param>
    /// <returns>Child named "name". Null if not found</returns>
    public static Transform FindChildByNameAll(this Transform parent, string name)
    {
        if (parent.name == name)
        {
            return parent;
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.name == name)
            {
                return child;
            }

            Transform grandchild = FindChildByNameAll(child, name);
            if (grandchild != null)
            {
                return grandchild;
            }
        }

        return null;
    }

    public static T FindChildComponent<T>(this Component parent, string name) where T : Component
    {
        Transform target = parent.transform.FindChildByNameAll(name);

        if (target == null) throw new TransformNotFoundException($"Child with name {name} not found in {parent.transform.   name}!");

        return target.GetComponent<T>();
    }

    /// <summary>
    /// Converts an array to a list.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array</typeparam>
    /// <param name="array">The array to convert</param>
    /// <returns>A list containing the elements of the array</returns>
    public static List<T> ToList<T>(this T[] array)
    {
        return new List<T>(array);
    }

    public static T Random<T>(this IList<T> collection) 
    {
        if ( collection.Count <= 0 )
            return default(T);
        return collection[UnityEngine.Random.Range(0, collection.Count -1)];
    }

    /// <summary>
    /// Converts a list to an array.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list</typeparam>
    /// <param name="list">The list to convert</param>
    /// <returns>An array containing the elements of the list</returns>
    public static T[] ToArray<T>(this IList<T> list)
    {
        var array = new T[list.Count];

        for (int i = 0; i < list.Count; i++)
        {
            array[i] = list[i];
        }

        return array;
    }

    /// <summary>
    /// Finds the component of type T, if its not found, adds a new component of type T
    /// </summary>
    /// <typeparam name="T">The type of component</typeparam>
    /// <param name="target">the target transform</param>
    /// <returns> the found or added component of type T</returns>
    public static T GetOrAddComponent<T>(this Component target) where T : Component
    {
        if (target.TryGetComponent(out T comp))
            return comp;

        return target.gameObject.AddComponent<T>();
    }

    public static T[] GetInterfaces<T>(this Component target)
    {
        Component[] comps = target.GetComponents<Component>();

        List<T> result = new();
        for (int i = 0; i < comps.Length; i++)
        {
            if (comps[i] is T)
            {
                result.Add((T)(object)comps[i]);
            }
        }

        return result.ToArray();
    }

    /// <summary>
    /// Finds all components of type T in all children of root
    /// </summary>
    /// <typeparam name="T">The type of component to get</typeparam>
    /// <param name="root">The top most transform to search through</param>
    /// <returns> An array of type T containing all the components in the roots hierarchy</returns>
    public static T[] GetComponentsInHierarchy<T>(this Transform root) where T : Component
    {
        List<T> result = new List<T>();

        for (int i = 0; i < root.childCount; i++)
        {
            result.AddRange(root.GetChild(i).GetComponents<T>());
            result.AddRange(root.GetChild(i).GetComponentsInHierarchy<T>());
        }

        return result.ToArray();
    }

    public static Transform AddChild(this Transform transform)
    {
        return AddChild(transform, null);
    }

    public static Transform AddChild(this Transform tranform, params Type[] components)
    {
        GameObject child = new GameObject("Empty", components);

        child.transform.parent = tranform;
        return child.transform;
    }

    public static void Reset(this Transform t)
    {
        t.localPosition = Vector3.zero;
        t.localEulerAngles = Vector3.zero;
        t.localScale = Vector3.one;
    }
    public static void ResetRotation(this Transform t)
    {
        t.localEulerAngles = Vector3.up * 90;
    }

    public static UnityEngine.Object Clone(this UnityEngine.Object t)
    {
        return UnityEngine.Object.Instantiate(t);
    }

    /// <summary>
    /// Returns the keycode of the current key that has been pressed when called
    /// </summary>
    /// <param name="key">If a key is pressed, returns the KeyCode of the pressed key</param>
    /// <returns>true if an key has been pressed and found</returns>
    public static bool CurrentKey(out KeyCode key)
    {
        if (Input.anyKeyDown)
        {
            foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(keyCode))
                {
                    key = keyCode;
                    return true;
                }
            }
        }
        key = KeyCode.None;
        return false;
    }

    /// <summary>
    /// Converts the given time in seconds to a string displaying time based on the format.
    /// Formats:
    /// "YYYY:MM:DD:HH:mm:ss" will result in a display like "2023:04:13:22:30:15".
    /// "MM:DD:HH:mm:ss" will result in a display like "04:13:22:30:15".
    /// "DD:HH:mm:ss" will result in a display like "13:22:30:15".
    /// "HH:mm:ss" will result in a display like "22:30:15".
    /// "mm:ss" will result in a display like "30:15".
    /// "ss" will result in a display like "15".
    /// </summary>
    /// <param name="seconds">The amount of seconds</param>
    /// <param name="format">The desired format of the time string. Use the following placeholders:
    /// - "YYYY" for years
    /// - "MM" for months
    /// - "DD" for days
    /// - "HH" for hours
    /// - "mm" for minutes
    /// - "ss" for seconds
    /// EXAMPLE: using format "HH:mm:ss" will result in a display like "12:58:43"
    /// </param>
    /// <returns>The formatted string displaying the time based on the format</returns>

    public static string ToTime(this int seconds, string format)
    {
        bool formatEmpty = format.Length == 0;
        if (seconds <= 0)
            seconds = 0;

        bool includeYears = false;
        bool includeMonths = false;
        bool includeDays = false;
        bool includeHours = false;
        bool includeMinutes = false;

        var years = Mathf.FloorToInt(seconds / 3600 * 24 * 365);
        seconds -= Mathf.FloorToInt(years * 3600 * 24 * 365);

        var months = Mathf.FloorToInt(seconds / 3600 * 24 * 31);
        seconds -= Mathf.FloorToInt(months * 3600 * 24 * 31);

        var days = Mathf.FloorToInt(seconds / 3600 * 24);
        seconds -= Mathf.FloorToInt(days * 3600 * 24);

        var hours = Mathf.FloorToInt(seconds / 3600);
        seconds -= Mathf.FloorToInt(hours * 3600);

        var minutes = Mathf.FloorToInt(seconds / 60);
        seconds -= Mathf.FloorToInt(minutes * 60);

        // Format the time string based on the format parameter

        var StringBuilder = new StringBuilder();

        if (formatEmpty)
        {
            if (format.Contains("YYYY"))
            {
                StringBuilder.AppendLine(years.ToString("D4") + ":");
                includeYears = true;
            }
            if (format.Contains("MM"))
            {
                StringBuilder.AppendLine(months.ToString("D2") + ":");
                includeMonths = true;
            }
            if (format.Contains("DD"))
            {
                StringBuilder.AppendLine(days.ToString("D2") + ":");
                includeDays = true;
            }
            if (format.Contains("HH"))
            {
                StringBuilder.AppendLine(hours.ToString("D2") + ":");
                includeHours = true;
            }
            if (format.Contains("MM"))
            {
                StringBuilder.AppendLine(minutes.ToString("D2") + ":");
                includeMinutes = true;
            }

            StringBuilder.AppendLine(seconds.ToString("D2"));
        }

        // If no format parameter is provided, use the default format
        if (formatEmpty || !includeYears && !includeMonths && !includeDays && !includeHours && !includeMinutes)
        {
            StringBuilder.AppendLine(years.ToString("D4") + ":" + months.ToString("D2") + ":" + days.ToString("D2") + ":" + hours.ToString("D2") + ":" + minutes.ToString("D2") + ":" + seconds.ToString("D2"));
        }

        return StringBuilder.ToString();
    }

    /// <summary>
    /// Is the pointer/input position over an UI Element?
    /// </summary>
    /// <returns>True if the pointer is on UI elements</returns>
    public static bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    /// <summary>
    /// Converts a canvas elements position to 3D-world position
    /// </summary>
    /// <param name="element">The target rect</param>
    /// <returns>The world position of the element</returns>
    public static Vector3 WorldPosition(this RectTransform element)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(element, element.position, Camera, out var result);
        return result;
    }

    public static Vector3 ScreenPosition(this RectTransform element)
    {
        return RectTransformUtility.WorldToScreenPoint(Camera, element.position);
    }

    /// <summary>
    /// Destroys all the children of an transform
    /// </summary>
    /// <param name="t">The target transform</param>
    public static void DeleteChildren(this Transform t)
    {
        foreach (Transform child in t) UnityEngine.Object.Destroy(child.gameObject);
    }

    /// <summary>
    /// Destroys all the children of an transform
    /// </summary>
    /// <param name="t">The target transform</param>
    public static void DeleteChildrenImmediate(this Transform t)
    {
        foreach (Transform child in t) UnityEngine.Object.DestroyImmediate(child.gameObject);
    }

    private static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new Dictionary<float, WaitForSeconds>();

    /// <summary>
    /// Non-Allocated way of using WaitForSeconds(float time).
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static WaitForSeconds Wait(float time)
    {
        if (WaitDictionary.TryGetValue(time, out var value)) return value;

        WaitDictionary[time] = new WaitForSeconds(time);
        return WaitDictionary[time];
    }

    /// <summary>
    /// Directly logs a string message to the unity console
    /// </summary>
    /// <param name="message">The message to log</param>
    public static void Log(this string message)
    {
        Debug.Log(message);
    }

    /// <summary>
    /// Converts a floating- point number to a integer
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int ToInt(this float value)
    {
        return Mathf.RoundToInt(value);
    }
    public static int ToMilliseconds(this float value)
    {
        return Mathf.RoundToInt(value * 1000);
    }

    public static bool IsBetween(this float target, float min, float max)
    {
        return target < max && target >= min;
    }

    public static bool IsBetween(this float target, int min, int max)
    {
        return target < max && target >= min;
    }

    public static bool IsBetween(this int target, int min, int max)
    {
        return target < max && target >= min;
    }

    public static bool IsBetween(this int target, float min, float max)
    {
        return target < max && target >= min;
    }

    public static bool IsEmpty<T>(this ICollection<T> collection)
    {
        return collection.Count <= 0;
    }

    public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
    {
        if (collection == null) return true;
        return collection.IsEmpty();
    }

    public static bool IsEmpty(this ICollection collection)
    {
        return collection.Count <= 0;
    }

    public static bool IsNullOrEmpty(this ICollection collection)
    {
        if (collection == null) return true;
        return collection.IsEmpty();
    }

    public static IList<T> RemoveDuplicates<T>(this IList<T> collection)
    {
        List<T> scannedItems = new();

        for (int i = 0; i < collection.Count; i++)
        {
            if (!scannedItems.Contains(collection[i]))
            {
                scannedItems.Add(collection[i]);
            }
        }

        return scannedItems;
    }

    public static IList<T> Shuffle<T>(this IList<T> collection)
    {
        if (collection.Count <= 1) return collection;

        if (collection.Count == 2)
        {
            collection.Swap(0, 1);
            return collection;
        }

        for (int i = 0; i < collection.Count; i++)
        {
            int index2;

            do
            {
                index2 = UnityEngine.Random.Range(0, collection.Count - 1);
            }
            while (index2 != i);

            collection.Swap(i, index2);
        }

        return collection;
    }

    public static void Swap<T>(this IList<T> collection, int index1, int index2)
    {
        (collection[index2], collection[index1]) = (collection[index1], collection[index2]);
    }

    /// <summary>
    /// Maps a float collection from the range min-max
    /// </summary>
    /// <param name="collection">The collection to iterate through</param>
    /// <param name="min">the minimum value of the mapped collection</param>
    /// <param name="max">the maximum value of the mapped collection</param>
    /// <returns></returns>
    public static IList<float> Map(this IList<float> collection, float min, float max)
    {
        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        for (int i = 0; i < collection.Count; i++)
        {
            float value = collection[i];

            if (value <= minValue) minValue = value;
            if (value >= maxValue) maxValue = value;
        }

        for (int i = 0; i < collection.Count; i++)
        {
            collection[i] = Mathf.InverseLerp(minValue, maxValue, collection[i]) * (max - min) + min;
        }

        return collection;
    }

    public static object LastElement(IList collection)
    {
        if (collection.IsNullOrEmpty()) return default;

        return collection[collection.Count - 1];
    }

    public static float RFloat(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    public static float RInt(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    public static bool IsEmpty(this string message)
    {
        if (message.Length <= 0) return true;

        if (message == string.Empty) return true;

        return false;
    }

    public static float Average(this IList<float> collection)
    {
        if (collection.Count == 0) return 0;

        float avg = 0;

        for (int i = 0; i < collection.Count; i++)
        {
            avg += collection[i];
        }

        if (avg == 0) return 0;

        return avg / collection.Count;
    }

    public static float Average(this IList<int> collection)
    {
        if (collection.Count == 0) return 0;

        float avg = 0;

        for (int i = 0; i < collection.Count; i++)
        {
            avg += collection[i];
        }

        if (avg == 0) return 0;

        return avg / collection.Count;
    }

    public static float Sum(this IList<float> collection)
    {
        float sum = 0;

        for (int i = 0; i < collection.Count; i++)
        {
            sum += collection[i];
        }

        return sum;
    }

    public static float Sum(this IList<int> collection)
    {
        int sum = 0;

        for (int i = 0; i < collection.Count; i++)
        {
            sum += collection[i];
        }

        return sum;
    }

    public static bool GetIndexesOf(this string message, string needle, out int[] indexes)
    {
        if (!message.IsEmpty() && message.Contains(needle))
        {
            indexes = (int[])message.AllIndexesOf(needle);
            return true;
        }

        indexes = null;
        return false;

    }

    
}


[Serializable]
public class TransformNotFoundException : Exception
{
    public TransformNotFoundException() { }
    public TransformNotFoundException(string message) : base(message) { }
    public TransformNotFoundException(string message, Exception inner) : base(message, inner) { }
    protected TransformNotFoundException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
