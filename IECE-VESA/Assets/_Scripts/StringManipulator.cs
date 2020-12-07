using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains some neato methods for manipulating strings
/// </summary>
public static class StringManipulator
{
    /// <summary>
    /// A handy method that returns the text in a string after the input character
    /// </summary>
    /// <param name="inputString"></param>
    /// <param name="inputChar"></param>
    /// <returns></returns>
    public static string ExtractStringAfterChar(string inputString, char inputChar)
    {
        int placeToRemoveAt = inputString.IndexOf(inputChar);
        if (inputString[placeToRemoveAt + 1] == ' ') placeToRemoveAt++;

        return inputString.Remove(0, placeToRemoveAt + 1);
    }

    /// <summary>
    /// A handy method that returns the text in a string before the input character
    /// </summary>
    /// <param name="inputString"></param>
    /// <param name="inputChar"></param>
    /// <returns></returns>
    public static string ExtractStringBeforeChar(string inputString, char inputChar)
    {
        int placeToRemoveAt = inputString.IndexOf(inputChar);
        //Debug.Log("CALLED AT: " + inputString + ". inputString.Length = " + inputString.Length + ". placeToRemoveAt = " + placeToRemoveAt);
        if (inputString.Length > placeToRemoveAt + 1 && inputString[placeToRemoveAt + 1] == ' ') placeToRemoveAt++;

        return inputString.Remove(placeToRemoveAt - 1);
    }

    /// <summary>
    /// A handy method that removes the space at the beginning of a string if it's there
    /// </summary>
    /// <param name="inputString"></param>
    /// <returns></returns>
    public static string RemoveFirstSpaceIfThere(string inputString)
    {
        string outputString = inputString;
        if (outputString[0] == ' ') outputString = outputString.Remove(0, 1);
        return outputString;
    }
}
