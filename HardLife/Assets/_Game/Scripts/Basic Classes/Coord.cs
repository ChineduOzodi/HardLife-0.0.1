using UnityEngine;
public struct Coord
{
    public int x;
    public int y;
    //public Coord()
    //{
    //    x = 0;
    //    y = 0;
    //}
    public Coord(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static Coord Vector3ToCoord(Vector3 vect)
    {
        Coord coord = new Coord((int) vect.x, (int) vect.y);

        return coord;
    }
}