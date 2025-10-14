using System;
using UnityEngine;

[Serializable]
public struct SaveGameData
{
    public int health;
   // public int xp;
    public float playerPositionX;
    public float playerPositionY;
    public float playerPositionZ;

    public int potionAmount;
    public int arrowAmount;

}