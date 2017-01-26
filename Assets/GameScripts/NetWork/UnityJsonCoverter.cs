using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Globalization;

public class UnityJsonCoverter : IConverter
{
    public Dictionary<string, string> dict = new Dictionary<string, string>();
    public string dictKey = "";
    private const string END_STR = "\n\0";// 結尾的字元 

    //------------------------------------------------------------------------------
    public string serializeObject(object data)
    {
        string jsonStr = JsonUtility.ToJson(data) + END_STR;
        return jsonStr;
    }
    //------------------------------------------------------------------------------
    public T deserializeObject<T>(string value)
    {
        T data = JsonUtility.FromJson<T>(value);

        return data;
    }
    //------------------------------------------------------------------------------
    public object deserializeObject(string value, Type type)
    {
        try
        {
            object data = JsonUtility.FromJson(value, type);
            return data;
        }
        catch (System.Exception ex)
        {
            //UnityDebugger.Debugger.LogError(ex);
            //UnityDebugger.Debugger.LogError("封包反序列化錯誤:" + value);            
            Debug.LogError(ex);
            Debug.LogError("封包反序列化錯誤:" + value);
        }

        return null;
    }
    //------------------------------------------------------------------------------
    public Dictionary<string, string> deserializeObject(string JsonString)
    {
        dictKey = "";
        dict.Clear();

        string sp = ":,";
        string[] strs = JsonString.Split(sp.ToCharArray());
        int count = 0;

        foreach (string str in strs)
        {
            if (str == "")
                continue;

            //去掉不相關的字元
            string newStr = str.Replace("{", "");
            newStr = newStr.Replace("}", "");
            newStr = newStr.Replace("\"", "");

            if (count % 2 == 1)
            {
                if (newStr.Contains("\\u"))
                {
                    string result = UnicodeToString(newStr);
                    dict.Add(dictKey, result);
                    //UnityEngine.Debug.Log("str = " + result);
                }
                else
                {
                    dict.Add(dictKey, newStr);
                    //UnityEngine.Debug.Log("str = " + newStr);
                }
            }
            else
            {
                //UnityEngine.Debug.Log("str = " + newStr);
            }

            dictKey = newStr;
            count++;
        }

        return dict;
    }
    //------------------------------------------------------------------------------
    public string UnicodeToString(string srcText)
    {
        string dst = "";
        string src = srcText;
        int len = srcText.Length / 6;
        for (int i = 0; i <= len - 1; i++)
        {
            string str = "";
            str = src.Substring(0, 6).Substring(2);
            src = src.Substring(6);
            byte[] bytes = new byte[2];
            bytes[1] = byte.Parse(int.Parse(str.Substring(0, 2), NumberStyles.HexNumber).ToString());
            bytes[0] = byte.Parse(int.Parse(str.Substring(2, 2), NumberStyles.HexNumber).ToString());
            dst += Encoding.Unicode.GetString(bytes);
        }
        return dst;
    }
}
