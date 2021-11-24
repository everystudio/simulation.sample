using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public struct TileParam
{
    public bool IsTaken;
    public bool IsSkyTaken;
    public float MovementCost;

    public GroundType groundType;
    public enum GroundType
    {
        Land,
        Water
    };
}

[CreateAssetMenu(fileName = "TileConfig", menuName = "CustomTile/TileConfig")]
public class TileConfig : Tile
{
    public TileParam tileParam;
}





