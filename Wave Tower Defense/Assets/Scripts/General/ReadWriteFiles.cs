using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ReadWriteFiles : MonoBehaviour
{
    public static void WriteString(string name, string content)
    {
        string path = Application.dataPath + "/" + name;
        //Write some text to the file
        StreamWriter writer = new StreamWriter(path, true);
        writer.Write(content);
        writer.Close();
    }
    public static void WriteStringAndClear(string name, string content)
    {
        string path = Application.dataPath + "/" + name;
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, false);
        writer.Write(content);
        writer.Close();
    }
    public static string ReadString(string name)
    {
        string path = Application.dataPath + "/" + name;
        //Read the text from directly from the file
        StreamReader reader = new StreamReader(path);
        string contents = reader.ReadToEnd();
        reader.Close();
        return contents;
    }
    public static bool FileExists(string name)
    {
        string path = Application.dataPath + "/" + name;
        if (Directory.Exists(path))
        {
            return true;
        }
        return false;
    }
    public static void CreateFile(string name)
    {
        string path = Application.dataPath + "/" + name;
        File.Create(path);
    }
}
