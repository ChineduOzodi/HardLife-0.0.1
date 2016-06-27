public class Alive :GObject 
{
    internal Date birthTime;
    internal Date age = new Date(0);

    public virtual void UpdateAge(Date _currentTime)
    {
        age = _currentTime - birthTime;
    }
    public override string GetInfo()
    {
        return "Age: " + age.GetDate() + "\n"+ base.GetInfo();
    }
}