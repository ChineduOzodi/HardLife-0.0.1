using UnityEngine;

public class Alive :GObject 
{
    internal bool isAlive = true;
    internal Date birthTime;
    internal Date age = new Date(0);

    public Alive(string _type, Vector3 _worldPosition, int x, int y)
        :base(_type, _worldPosition, x, y)
    {

    }
    public virtual void UpdateAge(Date _currentTime)
    {
        age = _currentTime - birthTime;
    }
    public override string GetInfo()
    {
        return "Age: " + age.GetDate() + "\n"+ base.GetInfo();
    }
}