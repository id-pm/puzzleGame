using UnityEngine;

[System.Serializable]
public struct ColorMaterialPair
{
    public MyColor color;
    public Material material;
}
public enum MyColor
{
    Blue,
    Green,
    White,
    Black,
    Yellow,
    Red,
    None,
    Special
}