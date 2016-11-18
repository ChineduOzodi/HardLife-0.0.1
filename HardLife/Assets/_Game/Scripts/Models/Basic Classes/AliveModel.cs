using UnityEngine;

public class AliveModel :BaseObjectModel 
{
    internal bool isAlive = true;
    internal Date birthTime;
    internal Date age = new Date(0);
    internal Date maxAge;
    internal Date youngAge;
    internal Date matureAge;
    internal string ageText = "Child";
}