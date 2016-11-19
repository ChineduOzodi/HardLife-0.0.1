using UnityEngine;

public class AliveModel :BaseObjectModel 
{
    internal bool isAlive = true;
    
    internal Date birthTime;
    internal Date age = new Date(0);
    internal Date oldAge;
    internal Date youngAge;
    internal Date matureAge;
    internal GrowthStage growthStage = GrowthStage.Child;
}