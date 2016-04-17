using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class NameGen {

    public int order = 2;
    public int maxLen = 20;
    public string maleFirstNames = "Bob, Matthew, Chinedu, Daniel, Emanuel, Aaron, Jonathan";
    public string worldNames = "Mercury, Venus, Earth, Jupiter, Saturn, Neptune, Uranus, Pluto";
    public string regionNames = "America, America, Asia, Europe, Africa, Antartica, Greenland, Iceland";
    

    private Dictionary<string, List<char>> worldNameTable;
    private Dictionary<string, List<char>> regionNameTable;
    private Dictionary<string, List<char>> maleFirstNameTable;
    private string sd = Time.time.ToString();

    // Use this for initialization
    public NameGen() {

        worldNameTable = Load(worldNames, order);
        regionNameTable = Load(regionNames, order);
        maleFirstNameTable = Load(maleFirstNames, order);

    }

    public string GenerateWorldName(string seed = null)
    {
        if (seed == null)
            seed = sd;
        return GenerateName(worldNameTable,seed);
    }
    public string GenerateRegionName(string seed = null)
    {
        if (seed == null)
            seed = sd;
        return GenerateName(regionNameTable,seed);
    }
    public string GenerateMaleFirstName(string seed = null)
    {
        if (seed == null)
            seed = sd;
        return GenerateName(maleFirstNameTable,seed);
    }
    /// <summary>
    /// Loads a string into a Markov chain dictionary
    /// </summary>
    /// <param name="table">the dictionary</param>
    /// <param name="names">the string of names</param>
    /// <param name="ord">The order of the Markov Chain</param>
    /// <param name="sep">Array of seperators for names string</param>
    /// <returns></returns>
    private Dictionary<string, List<char>> Load(string names, int ord, string[] sep = null)
    {
        if (sep == null)
        {
            sep = new string[] { ", ", ",", "/n", "\n" };
        }
        Dictionary<string, List<char>> table = new Dictionary<string, List<char>>();
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

        return table;
        
    }


    private string GenerateName(Dictionary<string, List<char>> table, string seed = null, string start = null)
    {
        string s = start;
        if (start == null)
        {
            s = randomStartChoice(table.Keys, seed);
        }

        try
        {
            while (s.Length < maxLen)
            {
                string lastSlice = s.Substring(s.Length - order, order);
                s += randomChoice(table[lastSlice]);
            }
        }
        catch (KeyNotFoundException) { }

        return s;
            
    }

    private string randomStartChoice(Dictionary<string, List<char>>.KeyCollection keys, string seed = null)
    {
        
        List<string> keyList = new List<string>();
        foreach (string key in keys)
        {
            if (Char.IsUpper(key[0]))
                keyList.Add(key);
        }

        string selectedKey = "" ;

        if (seed == null)
            seed = Time.time.ToString();

        System.Random randNum = new System.Random(seed.GetHashCode());
        selectedKey = keyList[randNum.Next(keys.Count)];

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

