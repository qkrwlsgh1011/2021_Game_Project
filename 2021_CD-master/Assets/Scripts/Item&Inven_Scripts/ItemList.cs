using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemList : MonoBehaviour
{
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon, House }
    public Type type;
    public int value;
}
