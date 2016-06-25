using UnityEngine;
using System.Collections;

public class Tree : Items {

    internal bool isAlive = true;
    internal int maxWood;
    internal int maxFruit;
    internal int maxAge;
    internal int matureAge;
    internal string ageText;
    internal int fruit;
    public Tree(string _type, float _age, int x, int y)
    {
        type = _type;
        age = _age;
        classType = "Tree";

        localMapPositionX = x;
        localMapPositionY = y;

        if (type == "normaltree")
        {
            stackOrder = 2;
            maxFruit = 35;
            maxWood = 75;
            maxAge = 250 * Date.Year;
            matureAge = 25 * Date.Year;

            fruit = Random.Range(0, maxFruit);

            if (age < matureAge)
            {
                ageText = "young";
            }
            else
            {
                ageText = "old";
            }
            
        }
        else if (type == "normalbush")
        {
            maxFruit = 15;
            maxWood = 10;
            maxAge = 25 * Date.Year;
            matureAge = 5 * Date.Year;
            fruit = Random.Range(0, maxFruit);

        }

    }

}
