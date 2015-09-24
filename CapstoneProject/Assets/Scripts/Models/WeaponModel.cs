using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class WeaponModel
{
    public Sprite Sprite;
    public string Name;
    public int Cost;
    public WeaponType Type;
    public int Damage;
    public float FireRate;
    public int Ammo;
}

public enum WeaponType { None, Pistol, SubMachine, Shotgun, Rifle, RPG };