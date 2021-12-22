using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace anogame_strategy
{
    [System.Serializable]
    public enum TILE_TYPE
    {
        GROUND = 0,
        WATER,
        MAX
    }

    [System.Serializable]
    public struct TileParam
    {
        public TILE_TYPE TileType;
        public float MovementCost;


    }

    [CreateAssetMenu(fileName = "TileConfig", menuName = "CustomTile/TileConfig")]
    public class TileConfig : Tile
    {
        public TileParam tileParam;
    }
}