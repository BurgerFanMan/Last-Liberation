using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.CompilerServices;
using System;

public class OptionsManager : MonoBehaviour
{
    public string fileName = "options.xml";
    public string currentOptionsVersion = "000";
    public List<string> defaultInputKeys;
    public List<KeyCode> defaultInputValues;
    public List<string> defaultInputNames;

    [Header("UI")]
    public GameObject buttonPrefab;
    public GameObject textPrefab;
    public Transform buttonsParent;
    public Transform textsParent;

    private bool listeningForInputKeyValueChange = false;
    private string inputKeyToChange;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        Options.manager = this;
        Options.OnStart();
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
        }

    }

    public void ListenForInputKeyValueChange(string inputKey)
    {
        listeningForInputKeyValueChange = true;

        inputKeyToChange = inputKey;
    }
}

public static class Options
{
    public static OptionsManager manager;

    private static List<GameObject> buttons = new List<GameObject>();
    private static List<GameObject> texts = new List<GameObject>();

    public static void OnStart()
    {
        LoadOptionsFile();
    }

    public static void WriteOptionsFile(bool overrideInputKeys)
    {
        //deciding whether to use the default inputs (in case this is the first time booting the game and we have no options file), or write in the current inputs
        Dictionary<string, KeyCode> dictionaryToUse = new Dictionary<string, KeyCode>();

        if (ReadWriteFiles.FileExists(manager.fileName) && !overrideInputKeys)
            dictionaryToUse = InputManager.inputKeyValues;
        else
            dictionaryToUse = manager.defaultInputKeys.Zip(manager.defaultInputValues, (k, v) => new { k, v })
              .ToDictionary(x => x.k, x => x.v);

        XDocument xmlDoc = new XDocument(
            new XElement("options", new XAttribute("version", manager.currentOptionsVersion),
            new XElement("inputkeyvalues")));

        //inputting keycode data
        for (int i = 0; i < dictionaryToUse.Count; i++)
        {
            string inputKey = dictionaryToUse.Keys.ToList()[i];
            XElement inputKeyValues = xmlDoc.Descendants().First().Descendants("inputkeyvalues").First();

            inputKeyValues.Add(new XElement("inputkeyvalue",
                new XAttribute("inputkey", inputKey),
                new XAttribute("inputvalue", dictionaryToUse[inputKey]),
                new XAttribute("inputname", manager.defaultInputNames[i])
                ));
        }

        ReadWriteFiles.WriteStringAndClear(manager.fileName, xmlDoc.ToString());

        LoadOptionsFile();
    }
    public static void LoadOptionsFile()
    {
        if (!ReadWriteFiles.FileExists(manager.fileName))
        {
            WriteOptionsFile(false);

            return;
        }

        Dictionary<string, KeyCode> inputKeyValues = new Dictionary<string, KeyCode>();
        List<string> inputNames = new List<string>();

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(ReadWriteFiles.DataPath + manager.fileName);

        XmlNode rootNode = xmlDoc.FirstChild;
        if (rootNode.Attributes.GetNamedItem("version").Value != manager.currentOptionsVersion)
        {
            WriteOptionsFile(true);

            return;
        }

        XmlNodeList inputKeyValuesXml = xmlDoc.SelectSingleNode("/options/inputkeyvalues").ChildNodes;

        //parse xml attributes into the string and keycode to put into inputKeyValues
        foreach (XmlNode inputKeyNode in inputKeyValuesXml)
        {
            string inputKey = inputKeyNode.Attributes.GetNamedItem("inputkey").Value;
            string inputValue = inputKeyNode.Attributes.GetNamedItem("inputvalue").Value;
            string inputName = inputKeyNode.Attributes.GetNamedItem("inputname").Value;

            if (!Enum.TryParse(inputValue, out KeyCode inputValueCode))
            {
                Debug.LogError($"Failed to parse input code value '{inputValue}', key is {inputKey}. Confirm this KeyCode exists.");

                continue;
            }

            inputKeyValues.Add(inputKey, inputValueCode);
            inputNames.Add(inputName);
        }

        InputManager.inputKeyValues = inputKeyValues;
        InputManager.inputNames = inputNames;

        GenerateButtons();
    }

    public static void GenerateButtons()
    {
        for(int i = 0; i < buttons.Count; i++)
        {
            GameObject button = buttons[i];
            GameObject text = texts[i];

            GameObject.Destroy(button);
            GameObject.Destroy(text);
        }

        buttons = new List<GameObject>();
        texts = new List<GameObject>();

        for (int i = 0; i < manager.defaultInputKeys.Count; i++)
        {
            GenerateButtons(i);
        }
    }
    public static void GenerateButtons(int index)
    {
        GameObject text;
        GameObject button;

        text = GameObject.Instantiate(manager.textPrefab, manager.textsParent);
        button = GameObject.Instantiate(manager.buttonPrefab, manager.buttonsParent);

        text.GetComponent<TextMeshProUGUI>().text = InputManager.GetKeyName(index);
        button.GetComponentInChildren<TextMeshProUGUI>().text = InputManager.GetValueName(index);

        Button buttonComponent = button.GetComponent<Button>();
        buttonComponent.onClick.AddListener(() => 
        manager.ListenForInputKeyValueChange(InputManager.GetKey(index)));

        if (texts.Count <= index)
        {
            texts.Add(text);
            buttons.Add(button);

            return;
        }

        texts[index] = text;
        buttons[index] = button;

        text.transform.SetSiblingIndex(index);
        button.transform.SetSiblingIndex(index);
    }
}
