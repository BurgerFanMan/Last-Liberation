using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class Options : MonoBehaviour
{
    [SerializeField] string _fileName;
    [SerializeField] OptionsSO _keyHolder;

    private List<KeyCode> _keycodes = new List<KeyCode>();
    void Start()
    {
        if (ReadWriteFiles.FileExists(_fileName))
        {
            ReadKeycodes();
        }
        else
        {
            _keyHolder.ResetKeys();
            ReadWriteFiles.CreateFile(_fileName);
        }
    }

    void Update()
    {
        
    }

    public void SaveKeycodes()
    {
        string keyString = string.Join(",", _keycodes);
        ReadWriteFiles.WriteString(keyString, _fileName);
    }
    public void ReadKeycodes()
    {
        List<string> keyStrings = new List<string>();
        keyStrings = ReadWriteFiles.ReadString(_fileName).Split(',').ToList();
        List<KeyCode> newKeys = new List<KeyCode>();
        foreach(string key in keyStrings)
        {
            newKeys.Add((KeyCode)System.Enum.Parse(typeof(KeyCode), key));
        }   
        _keyHolder.SetKeys(newKeys);
    }
    public void ResetKeyCodes()
    {
        _keyHolder.ResetKeys();
    }
}
