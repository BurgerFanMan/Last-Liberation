using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public static class InputManager
{
    public static Dictionary<string, KeyCode> inputKeyValues = new Dictionary<string, KeyCode>();
    public static List<string> inputNames = new List<string>();

    public static string GetKey(int index)
    {
        return inputKeyValues.Keys.ToList()[index];
    }

    public static KeyCode GetValue(string inputKey)
    {
        if(inputKeyValues.ContainsKey(inputKey))
            return inputKeyValues[inputKey];

        Debug.Log($"Keycode not found: {inputKey}.");

        return KeyCode.None;
    }
    public static KeyCode GetValue(int index)
    {
        return inputKeyValues.Values.ToList()[index];
    }

    public static string GetKeyName(int index)
    {
        return inputNames[index];
    }
    public static string GetValueName(int index)
    {
        string valueName = inputKeyValues.Values.ToList()[index].ToString();

        return valueName;
    }

    public static void ChangeInputKeyValue(string inputKey, KeyCode inputValue)
    {
        if (!inputKeyValues.ContainsKey(inputKey))
        {
            Debug.LogWarning($"InputManager does not have a key matching {inputKey}, yet it is attempting to change its value to {inputValue}.");

            return;
        }

        inputKeyValues[inputKey] = inputValue;

        Options.WriteOptionsFile(false);
    }
}
