using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
namespace anogame_strategy
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class TileInfo : MonoBehaviour, IGraphNode, IEquatable<TileInfo>
    {
        public TileParam m_tileParam;
        public float MovementCost { get { return m_tileParam.MovementCost; } }
        [HideInInspector]
        [SerializeField]
        private Vector2 _offsetCoord;
        public Vector2 OffsetCoord { get { return _offsetCoord; } set { _offsetCoord = value; } }

        //public UnitBase cunit;
        //public UnitBase CurrentUnit { get { return cunit; } set { cunit = value; } }
        public UnitBase CurrentUnit { get; set; }
        public event EventHandler TileInfoClicked;
        public event EventHandler TileInfoHighlighted;
        public event EventHandler TileInfoDehighlighted;

        List<TileInfo> neighbours = null;
        protected static readonly Vector2[] _directions =
        {
            new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1)
    };

        protected virtual void OnMouseEnter()
        {
            if (TileInfoHighlighted != null)
                TileInfoHighlighted.Invoke(this, new EventArgs());
        }
        protected virtual void OnMouseExit()
        {
            if (TileInfoDehighlighted != null)
                TileInfoDehighlighted.Invoke(this, new EventArgs());
        }
        void OnMouseDown()
        {
            if (TileInfoClicked != null)
                TileInfoClicked.Invoke(this, new EventArgs());
        }

        /*
        public abstract List<TileInfo> GetNeighbours(List<TileInfo> TileInfos);
        public abstract Vector3 GetTileInfoDimensions();
        public abstract void MarkAsHighlighted();
        public abstract void UnMark();
        */
        public int GetDistance(TileInfo _other)
        {
            return (int)(Mathf.Abs(OffsetCoord.x - _other.OffsetCoord.x) + Mathf.Abs(OffsetCoord.y - _other.OffsetCoord.y));
        }

        public void MarkAsPath()
        {
            SetColor(new Color(0, 1, 0, 0.5f));
        }
        public void MarkAsReachable()
        {
            //Debug.Log("MarkAsReachable");
            // square
            SetColor(new Color(1, 0.92f, 0.16f, 0.5f));
        }
        public void MarkAsHighlighted()
        {
            SetColor(new Color(0.5f, 0.5f, 0.5f, 0.25f));
        }
        public void UnMark()
        {
            SetColor(new Color(1, 1, 1, 0));
        }
        private void SetColor(Color color)
        {
            var highlighter = transform.Find("Highlighter");
            if (highlighter == null)
            {
                highlighter = transform;
            }
            var spriteRenderer = highlighter.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = color;
            }
            foreach (Transform child in highlighter.transform)
            {
                var childColor = new Color(color.r, color.g, color.b, 1);
                spriteRenderer = child.GetComponent<SpriteRenderer>();
                if (spriteRenderer == null) continue;

                child.GetComponent<SpriteRenderer>().color = childColor;
            }
        }

        public int GetDistance(IGraphNode _other)
        {
            return GetDistance(_other as TileInfo);
        }

        public virtual bool Equals(TileInfo _other)
        {
            //Debug.Log(_other);
            //Debug.Log(OffsetCoord);
            return (OffsetCoord.x == _other.OffsetCoord.x && OffsetCoord.y == _other.OffsetCoord.y);
        }

        public override bool Equals(object _other)
        {
            if (!(_other is TileInfo))
            {
                return false;
            }
            return Equals(_other as TileInfo);
        }

        public override int GetHashCode()
        {
            int hash = 23;

            hash = (hash * 37) + (int)OffsetCoord.x;
            hash = (hash * 37) + (int)OffsetCoord.y;
            return hash;
        }

        public List<TileInfo> GetNeighbours(List<TileInfo> tileInfos)
        {
            if (neighbours == null)
            {
                neighbours = new List<TileInfo>(4);
                foreach (var direction in _directions)
                {
                    //Debug.Log(OffsetCoord + direction);
                    var neighbour = tileInfos.Find(c => c.OffsetCoord == OffsetCoord + direction);

                    if (neighbour == null) continue;

                    neighbours.Add(neighbour);
                }
            }
            //Debug.Log(neighbours.Count);
            return neighbours;
        }


        //public abstract void CopyFields(TileInfo _newTileInfo);
    }
}