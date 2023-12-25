using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Last Liberation/Options")]
public class OptionsSO : ScriptableObject
{
    [SerializeField] List<KeyCode> _defaultKeycodes;
    [SerializeField] List<KeyCode> _keycodes;

    public void ResetKeys()
    {
        _keycodes = _defaultKeycodes;
    }

    public void SetKeys(List<KeyCode> newkeys)
    {
        _keycodes = newkeys;
    }

    public KeyCode GetKeyCodes(int index)
    {
        return _keycodes[index];
    }
    public List<KeyCode> GetDefaultKeyCodes()
    {
        return _defaultKeycodes;
    }
}
