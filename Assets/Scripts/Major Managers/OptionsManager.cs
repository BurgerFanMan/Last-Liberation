using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class OptionsManager : MonoBehaviour
{
    public string fileName = "options.xml";
    public string currentOptionsVersion = "000";

    [Header("Sound")]
    public UnityEvent onSoundValueChanged;
    public List<string> soundNames;
    [Range(0f, 2f)]
    public List<float> soundValues;

    [Header("Keybinds")]
    public List<string> defaultInputKeys;
    public List<KeyCode> defaultInputValues;
    public List<string> defaultInputNames;

    [Header("UI")]
    public Button applyButton;
    public Button cancelButton;
    public GameObject soundPrefab;
    public Transform soundParent;
    public GameObject inputPrefab;
    public Transform inputParent; 

    private bool listeningForInputKeyValueChange = false;
    private string inputKeyToChange;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        Options.manager = this;
        Options.OnStart();

        applyButton.interactable = false;
        cancelButton.interactable = false;
    }

    public void Update()
    {
        if (!listeningForInputKeyValueChange || !Input.anyKeyDown)
            return;

        foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
        {
            if (!Input.GetKey(keyCode))
                continue;

            listeningForInputKeyValueChange = false;
            InputManager.ChangeInputKeyValue(inputKeyToChange, keyCode);

            Options.GenerateOptionsMenu();
        }

    }

    public void SoundValueChanged(int index, float amount)
    {
        applyButton.interactable = true;
        cancelButton.interactable = true;

        Options.soundValues[index] = amount;

        onSoundValueChanged.Invoke();
    }
    public void ListenForInputKeyValueChange(string inputKey)
    {
        applyButton.interactable = true;
        cancelButton.interactable = true;

        listeningForInputKeyValueChange = true;

        inputKeyToChange = inputKey;
    }
    
    public void ApplyOptions()
    {
        Options.WriteOptionsFile();

        applyButton.interactable = false;
        cancelButton.interactable = false;
    }
    public void RevertOptions()
    {
        Options.LoadOptionsFile();

        applyButton.interactable = false;
        cancelButton.interactable = false;
    }
    public void RevertOptionsToDefault()
    {
        Options.WriteOptionsFile(true);
        Options.LoadOptionsFile();

        applyButton.interactable = false;
        cancelButton.interactable = false;
    }
}

public static class Options
{
    public static OptionsManager manager;

    public static List<float> soundValues = new List<float>();

    private static List<GameObject> soundObjects = new List<GameObject>();
    private static List<GameObject> inputObjects = new List<GameObject>();

    public static void OnStart()
    {
        LoadOptionsFile();
    }

    public static void WriteOptionsFile(bool useDefaultSettings = false)
    {
        XDocument xmlDoc = new XDocument(
            new XElement("options", new XAttribute("version", manager.currentOptionsVersion),
            new XElement("sound"),
            new XElement("inputkeyvalues")));

        //deciding whether to use the default settings (in case this is the first time booting the game and we have no options file), or write in the current settings
        List<float> soundValuesToUse = new List<float>();
        Dictionary<string, KeyCode> inputDictionary = new Dictionary<string, KeyCode>();

        if (ReadWriteFiles.FileExists(manager.fileName) && !useDefaultSettings)
        {
            soundValuesToUse = soundValues;
            inputDictionary = InputManager.inputKeyValues;
        }
        else
        {
            soundValuesToUse = manager.soundValues;
            inputDictionary = manager.defaultInputKeys.Zip(manager.defaultInputValues, (k, v) => new { k, v })
              .ToDictionary(x => x.k, x => x.v);
        }

        //writing sound values
        for (int i = 0; i < soundValuesToUse.Count; i++)
        {
            XElement sound = xmlDoc.Descendants().First().Descendants("sound").First();

            sound.Add(new XElement("sound",
                new XAttribute("soundname", manager.soundNames[i]),
                new XAttribute("soundvalue", soundValuesToUse[i])
                ));
        }

        //writing keybinds
        for (int i = 0; i < inputDictionary.Count; i++)
        {
            string inputKey = inputDictionary.Keys.ToList()[i];
            XElement inputKeyValues = xmlDoc.Descendants().First().Descendants("inputkeyvalues").First();

            inputKeyValues.Add(new XElement("inputkeyvalue",
                new XAttribute("inputkey", inputKey),
                new XAttribute("inputvalue", inputDictionary[inputKey])
                ));
        }

        ReadWriteFiles.WriteStringAndClear(manager.fileName, xmlDoc.ToString());

        LoadOptionsFile();
        GenerateOptionsMenu();
    }
    public static void LoadOptionsFile()
    {
        if (!ReadWriteFiles.FileExists(manager.fileName))
        {
            WriteOptionsFile(false);

            return;
        }

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(ReadWriteFiles.DataPath + manager.fileName);

        XmlNode rootNode = xmlDoc.FirstChild;
        if (rootNode.Attributes.GetNamedItem("version").Value != manager.currentOptionsVersion)
        {
            WriteOptionsFile(true);

            return;
        }

        //sound options
        soundValues = new List<float>();
        XmlNodeList sound = xmlDoc.SelectSingleNode("/options/sound").ChildNodes;

        foreach(XmlNode soundNode in sound)
        {
            string soundValueString = soundNode.Attributes.GetNamedItem("soundvalue").Value;
            float soundValueFloat = float.Parse(soundValueString);

            soundValues.Add(soundValueFloat);
        }

        //input rebindings
        Dictionary<string, KeyCode> inputKeyValues = new Dictionary<string, KeyCode>();

        XmlNodeList inputKeyValuesXml = xmlDoc.SelectSingleNode("/options/inputkeyvalues").ChildNodes;

        //parse xml attributes into the string and keycode to put into inputKeyValues
        foreach (XmlNode inputKeyNode in inputKeyValuesXml)
        {
            string inputKey = inputKeyNode.Attributes.GetNamedItem("inputkey").Value;
            string inputValue = inputKeyNode.Attributes.GetNamedItem("inputvalue").Value;

            if (!Enum.TryParse(inputValue, out KeyCode inputValueCode))
            {
                Debug.LogError($"Failed to parse input code value '{inputValue}', key is {inputKey}. Confirm this KeyCode exists.");

                continue;
            }

            inputKeyValues.Add(inputKey, inputValueCode);
        }

        InputManager.inputKeyValues = inputKeyValues;
        InputManager.inputNames = manager.defaultInputNames;

        GenerateOptionsMenu();
    }

    public static void GenerateOptionsMenu()
    {  
        for(int i = 0; i < soundObjects.Count; i++)
        {
            GameObject soundObject = soundObjects[i];
            GameObject.Destroy(soundObject);
        }
        for(int i = 0; i < inputObjects.Count; i++)
        {
            GameObject inputObject = inputObjects[i];
            GameObject.Destroy(inputObject);
        }

        soundObjects = new List<GameObject>();
        inputObjects = new List<GameObject>();

        //generating sound sliders
        for(int i = 0; i < manager.soundValues.Count; i++)
        {
            GenerateSoundObjects(i);
        }

        //generating input rebind buttons
        for (int i = 0; i < manager.defaultInputKeys.Count; i++)
        {
            GenerateInputObjects(i);
        }
    }
    static void GenerateSoundObjects(int index)
    {
        Transform soundSlider = GameObject.Instantiate(manager.soundPrefab, manager.soundParent).transform;

        foreach(Transform childTransform in soundSlider)
        {
            if (!childTransform.TryGetComponent(out TextMeshProUGUI textMesh))
            {
                Slider slider = childTransform.GetComponent<Slider>();

                slider.value = soundValues[index];
                slider.onValueChanged.AddListener((float newValue) =>
                    manager.SoundValueChanged(index, newValue));      

                continue;
            }

            textMesh.text = manager.soundNames[index];
        }

        if (soundObjects.Count <= index)
        {
            soundObjects.Add(soundSlider.gameObject);

            return;
        }

        soundObjects[index] = soundSlider.gameObject;
        soundSlider.SetSiblingIndex(index);
    }
    static void GenerateInputObjects(int index)
    {
        Transform inputObject = GameObject.Instantiate(manager.inputPrefab, manager.inputParent).transform;

        foreach(Transform childTransform in inputObject)
        {
            if(!childTransform.TryGetComponent(out TextMeshProUGUI textMesh))
            {
                childTransform.GetComponent<Button>().onClick.AddListener(() =>
                    manager.ListenForInputKeyValueChange(InputManager.GetKey(index)));

                childTransform.GetComponentInChildren<TextMeshProUGUI>().text = InputManager.GetValueName(index);

                continue;
            }

            textMesh.text = InputManager.GetKeyName(index);
        }

        if (inputObjects.Count <= index)
        {
            inputObjects.Add(inputObject.gameObject);

            return;
        }

        inputObjects[index] = inputObject.gameObject;
        inputObject.SetSiblingIndex(index);
    }
}