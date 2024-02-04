using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputKeys : MonoBehaviour
{
    public List<string> inputKeys;
    public List<KeyCode> inputValues;
    
    void Start()
    {
        UpdateInputKeyValues();
    }

    public void UpdateInputKeyValues()
    {
        InputManager.inputKeyValues = new Dictionary<string, KeyCode>();

        for(int i = 0; i < inputKeys.Count; i++)
        {
            if(!(i < inputValues.Count))
            {
                Debug.Log($"Mismatched number of input values and keys. Aborting key adding at {inputKeys[i]}.");

                break;
            }

            InputManager.inputKeyValues.Add(inputKeys[i], inputValues[i]);
        }
    }
}

public static class InputManager
{
    public static Dictionary<string, KeyCode> inputKeyValues = new Dictionary<string, KeyCode>();

    public static KeyCode GetValue(string keyCode)
    {
        if(inputKeyValues.ContainsKey(keyCode))
            return inputKeyValues[keyCode];

        Debug.Log($"Keycode not found: {keyCode}.");

        return KeyCode.None;
    }
}
