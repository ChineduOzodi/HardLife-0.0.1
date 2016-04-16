using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class NameGen : MonoBehaviour {

    public float timeDelay = 1;
    public float tTime = 0;
    public int order = 2;
    public string namesPick;
    public int maxLen = 20;

    public Dictionary<string, List<char>> table;
    // Use this for initialization
    void Awake() {

        tTime = timeDelay;
        table = new Dictionary<string, List<char>>();
        Load(namesPick, order);


    }

    // Update is called once per frame
    void Update() {

        if (Time.time > tTime)
        {
            
            string name = Generate(maxLen);
            print("Name: " + name);
            tTime += timeDelay;
            
        }

    }

    public void Load(string names, int ord, string[] sep = null)
    {
        if (sep == null)
        {
            sep = new string[] { ", ", ",", "/n", "\n" };
        }
        string[] namesAr = names.Split(sep, StringSplitOptions.RemoveEmptyEntries);
        order = ord;

        foreach (string s in namesAr)
        {
            for (int i = 0; i < s.Length - ord; i++)
            {
                string tableKey = s.Substring(i,ord);
                try
                {
                    table[tableKey].Add(s[i + ord]);
                }
                catch (KeyNotFoundException)
                {
                    table[tableKey] = new List<char>();
                    table[tableKey].Add(s[i + ord]);
                }
                
            }
        }
        
    }
        

    public string Generate(int maxLength = 20, string start = null)
    {
        string s = start;
        if (start == null)
        {
            s = randomStartChoice(table.Keys);
        }

        try
        {
            while (s.Length < maxLength)
            {
                string lastSlice = s.Substring(s.Length - order, order);
                s += randomChoice(table[lastSlice]);
            }
        }
        catch (KeyNotFoundException) { }

        return s;
            
    }

    private string randomStartChoice(Dictionary<string, List<char>>.KeyCollection keys)
    {
        List<string> keyList = new List<string>();
        foreach (string key in keys)
        {
            if (Char.IsUpper(key[0]))
                keyList.Add(key);
        }
        string selectedKey = "" ;
        int randNum = UnityEngine.Random.Range(0, keyList.Count);
        selectedKey = keyList[randNum];

        return selectedKey;
    }
    private char randomChoice(List<char> charList)
    {
        char selectedChar = new Char();
        int randNum = UnityEngine.Random.Range(0, charList.Count);
        selectedChar = charList[randNum];

        return selectedChar;
    }
}

