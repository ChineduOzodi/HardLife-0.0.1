using UnityEngine;
using System.Collections;

public enum ObjectType {

    Floor,
    Tree,
    Human,
    Animal,
    Rock,
    Flower,
    Roof,
    Item,
    Structure
}

public enum GrowthStage
{
    Child,
    Young,
    Mature,
    Old
}

public enum State
{
    Healthy,
    Sick,
    Browning,
    NoLeaves,
    Budding
}
