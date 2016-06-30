using UnityEngine;

public class Alive :BaseObjectModel 
{
    internal bool isAlive = true;
    internal Date birthTime;
    internal Date age = new Date(0);
    internal Date maxAge;

    public Alive(string _type, Date _birthTime, Vector3 _worldPosition, int x, int y)
        :base(_type, _worldPosition, x, y)
    {
        birthTime = _birthTime;
    }
    public virtual void UpdateAge(Date _currentTime)
    {
        age = _currentTime - birthTime;
    }
    public override string GetInfo()
    {
        return "Age: " + age.GetDate() + "\nAlive: " + isAlive + "\n" + base.GetInfo();
    }
}