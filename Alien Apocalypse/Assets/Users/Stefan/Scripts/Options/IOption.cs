using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Interface for handling the saving and loading of ui options element
/// </summary>
/// <typeparam name="T">The type of data the options element holds</typeparam>

public interface IOption<T>
{
    public int OptionIndex
    {
        get;
    }


    /// <summary>
    /// Reads the saveable value of the options element
    /// </summary>
    /// <returns></returns>
    public T GetValue ( );

    /// <summary>
    /// Writes the saveable value of the options element
    /// </summary>
    /// <param name="value">the value of the options element</param>
    public void SetValue ( T value );
}
