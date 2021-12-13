using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{
    public UnitState UnitState { get; set; }
    public void SetState(UnitState _state)
    {
        UnitState.MakeTransition(_state);
    }
    public static readonly float PosZ = -5.0f;
    public event EventHandler UnitClicked;
    public event EventHandler UnitHighlighted;
    public event EventHandler UnitDehighlighted;

    public event EventHandler<AttackEventArgs> UnitAttacked;
    public event EventHandler<AttackEventArgs> UnitDestroyed;
    public event EventHandler<MovementEventArgs> UnitMoved;

    public int PlayerNumber;
    public bool m_bIsMoving;

    public UnitData m_unitData;

    private static DijkstraPathfinding _pathfinder = new DijkstraPathfinding();
    private static Pathfinding _fallbackPathfinder = new AStarPathfinding();

    [SerializeField]
    [HideInInspector]
    private TileInfo currentTileInfo;
    public TileInfo CurrentTileInfo
    {
        get
        {
            return currentTileInfo;
        }
        set
        {
            currentTileInfo = value;
        }
    }

    public int TotalHitPoints { get; private set; }
    public float TotalMovementPoints { get; private set; }
    public float TotalActionPoints { get; private set; }

    private int HitPoints;
    public int CurrentHitPoints { get { return HitPoints; } }
    private int AttackRange;
    public int AttackFactor;
    public int DefenceFactor;

    private float movementPoints;
    public virtual float MovementPoints
    {
        get
        {
            return movementPoints;
        }
        protected set
        {
            movementPoints = value;
        }
    }
    public float MovementAnimationSpeed;

    private float actionPoints = 1;
    public float ActionPoints
    {
        get
        {
            return actionPoints;
        }
        protected set
        {
            actionPoints = value;
        }
    }
    Dictionary<TileInfo, List<TileInfo>> cachedPaths = null;

	private bool m_bInitialized = false;
    public virtual void Initialize()
	{
        if (m_bInitialized == false)
        {
            UnitState = new UnitStateNormal(this);

            HitPoints = m_unitData.HitPoint;
            AttackRange = m_unitData.AttackRange;

            MovementPoints = m_unitData.MovementPoint;
            ActionPoints = m_unitData.ActionPoint;

            //Buffs = new List<Buff>();
            TotalMovementPoints = MovementPoints;
            TotalActionPoints = ActionPoints;
        }
        m_bInitialized = true;
    }

    protected virtual void OnMouseDown()
    {
        //Debug.Log($"OnMouseDown:{gameObject.name}");
        if (UnitClicked != null)
        {
            UnitClicked.Invoke(this, new EventArgs());
        }
    }
    protected virtual void OnMouseEnter()
    {
        if (UnitHighlighted != null)
        {
            UnitHighlighted.Invoke(this, new EventArgs());
        }
    }
    protected virtual void OnMouseExit()
    {
        if (UnitDehighlighted != null)
        {
            UnitDehighlighted.Invoke(this, new EventArgs());
        }
    }
    public virtual void OnTurnStart()
    {
        MovementPoints = TotalMovementPoints;
        ActionPoints = TotalActionPoints;

        SetState(new UnitStateMarkedAsFriendly(this));
    }
    public virtual void OnTurnEnd()
    {
        cachedPaths = null;

        // バフ関係の状態更新処理
        //Buffs.FindAll(b => b.Duration == 0).ForEach(b => { b.Undo(this); });
        //Buffs.RemoveAll(b => b.Duration == 0);
        //Buffs.ForEach(b => { b.Duration--; });

        SetState(new UnitStateNormal(this));
    }
    protected virtual void OnDestroyed()
    {
        MarkAsDestroyed();
        Destroy(gameObject);
    }


    public abstract void MarkAsDefending(UnitBase _aggressor);
    public abstract void MarkAsAttacking(UnitBase _target, Action _onFinished);
    public abstract void MarkAsDestroyed();
    public abstract void MarkAsFriendly();
    public abstract void MarkAsReachableEnemy();
    public abstract void MarkAsSelected();
    public abstract void MarkAsFinished();
    public abstract void UnMark();

    public event EventHandler UnitSelected;
    public event EventHandler UnitDeselected;
    public virtual void OnUnitSelected()
    {
        SetState(new UnitStateMarkedAsSelected(this));
        if (UnitSelected != null)
        {
            UnitSelected.Invoke(this, new EventArgs());
        }
    }
    public virtual void OnUnitDeselected()
    {
        SetState(new UnitStateMarkedAsFriendly(this));
        if (UnitDeselected != null)
        {
            UnitDeselected.Invoke(this, new EventArgs());
        }
    }
    public virtual bool IsUnitAttackable(UnitBase other, TileInfo sourceCell)
    {
        return sourceCell.GetDistance(other.CurrentTileInfo) <= AttackRange
            && other.PlayerNumber != PlayerNumber
            && ActionPoints >= 1;
    }
    public virtual bool IsTileInfoAttackable(TileInfo _target)
	{
        return currentTileInfo.GetDistance(_target) <= AttackRange
            && ActionPoints >= 1;
    }

    public void AttackHandler(UnitBase _targetUnit , Action _onFinished)
    {
        if (!IsUnitAttackable(_targetUnit, CurrentTileInfo))
        {
            _onFinished.Invoke();
            return;
        }
        //Debug.Log("attackhandler");
        AttackAction attackAction = DealDamage(_targetUnit);
        MarkAsAttacking(_targetUnit,()=>
        {
            _targetUnit.DefendHandler(this, attackAction.Damage);
            AttackActionPerformed(attackAction.ActionCost);

            _onFinished.Invoke();
        });
    }
    protected virtual AttackAction DealDamage(UnitBase unitToAttack)
    {
        return new AttackAction(AttackFactor, 1f);
    }
    protected virtual void AttackActionPerformed(float actionCost)
    {
        ActionPoints -= actionCost;
        if (ActionPoints == 0)
        {
            MovementPoints = 0;
            SetState(new UnitStateMarkedAsFinished(this));
        }
    }
    public void DefendHandler(UnitBase aggressor, int damage)
    {
        MarkAsDefending(aggressor);
        int damageTaken = Defend(aggressor, damage);
        HitPoints -= damageTaken;
        DefenceActionPerformed();

        if (UnitAttacked != null)
        {
            UnitAttacked.Invoke(this, new AttackEventArgs(aggressor, this, damage));
        }
        if (HitPoints <= 0)
        {
            if (UnitDestroyed != null)
            {
                UnitDestroyed.Invoke(this, new AttackEventArgs(aggressor, this, damage));
            }
            OnDestroyed();
        }
    }
    protected virtual int Defend(UnitBase aggressor, int damage)
    {
        return Mathf.Clamp(damage - DefenceFactor, 1, damage);
    }
    protected virtual void DefenceActionPerformed() { }

    public List<TileInfo> FindPath(List<TileInfo> cells, TileInfo destination)
    {
        if (cachedPaths != null && cachedPaths.ContainsKey(destination))
        {
            return cachedPaths[destination];
        }
        else
        {
            return _fallbackPathfinder.FindPath(GetGraphEdges(cells), CurrentTileInfo, destination);
        }
    }
    protected virtual Dictionary<TileInfo, Dictionary<TileInfo, float>> GetGraphEdges(List<TileInfo> _tileInfos)
    {
        Dictionary<TileInfo, Dictionary<TileInfo, float>> ret = new Dictionary<TileInfo, Dictionary<TileInfo, float>>();
        foreach (var tileInfo in _tileInfos)
        {
            if (IsCellTraversable(tileInfo) || tileInfo.Equals(CurrentTileInfo))
            {
                ret[tileInfo] = new Dictionary<TileInfo, float>();
                foreach (var neighbour in tileInfo.GetNeighbours(_tileInfos).FindAll(IsCellTraversable))
                {
                    ret[tileInfo][neighbour] = neighbour.m_tileParam.MovementCost;
                }
            }
        }
        return ret;
    }
    public virtual bool IsTileInfoMovableTo(TileInfo _tileInfo)
    {
        if( _tileInfo.m_tileParam.TileType == TILE_TYPE.WATER)
		{
            if (m_unitData.Flying == false)
            {
                return false;
            }
        }
        return _tileInfo.CurrentUnit == null;// !_tileInfo.m_tileParam.IsTaken && _tileInfo.CurrentUnit == null;
    }
    public virtual bool IsCellTraversable(TileInfo _tileInfo)
    {
        return IsTileInfoMovableTo(_tileInfo);

        //return _tileInfo.CurrentUnit == null;// !_tileInfo.m_tileParam.IsTaken && _tileInfo.CurrentUnit == null;
    }
    public HashSet<TileInfo> GetAvailableDestinations(List<TileInfo> _tileInfos)
    {
        cachedPaths = new Dictionary<TileInfo, List<TileInfo>>();

        var paths = CachePaths(_tileInfos);
        //Debug.Log(paths.Count);
        foreach (var key in paths.Keys)
        {
            if (!IsTileInfoMovableTo(key))
            {
                continue;
            }
            var path = paths[key];

            var pathCost = path.Sum(c => c.m_tileParam.MovementCost);
            if (pathCost <= MovementPoints)
            {
                cachedPaths.Add(key, path);
            }
        }
        return new HashSet<TileInfo>(cachedPaths.Keys);
    }
    private Dictionary<TileInfo, List<TileInfo>> CachePaths(List<TileInfo> _tileInfos)
    {
        var edges = GetGraphEdges(_tileInfos);
        //Debug.Log(edges.Count);
        var paths = _pathfinder.findAllPaths(edges, CurrentTileInfo);
        return paths;
    }

    public virtual void Move(TileInfo _destinationCell, List<TileInfo> _path , Action _onFinished)
    {
        var totalMovementCost = _path.Sum(h => h.m_tileParam.MovementCost);
        //MovementPoints -= totalMovementCost;
        // 以前は中途半端に動くと再始動出来たけど、行動は1回だけに変更
        MovementPoints = 0;

        //CurrentTileInfo.m_tileParam.IsTaken = false;
        CurrentTileInfo.CurrentUnit = null;
        CurrentTileInfo = _destinationCell;
        //_destinationCell.m_tileParam.IsTaken = true;
        _destinationCell.CurrentUnit = this;

        if (MovementAnimationSpeed > 0)
        {
            StartCoroutine(MovementAnimation(_path, _onFinished));
        }
        else
        {
            Vector3 targetPos = new Vector3(CurrentTileInfo.transform.localPosition.x, CurrentTileInfo.transform.localPosition.y, transform.localPosition.z);
            transform.position = targetPos;
            _onFinished.Invoke();
        }

        if (UnitMoved != null)
        {
            UnitMoved.Invoke(this, new MovementEventArgs(CurrentTileInfo, _destinationCell, _path));
        }
    }
    protected virtual IEnumerator MovementAnimation(List<TileInfo> path , Action _onFinished)
    {
        m_bIsMoving = true;
        path.Reverse();
        foreach (var cell in path)
        {
            Vector3 destination_pos = new Vector3(cell.transform.localPosition.x, cell.transform.localPosition.y, transform.localPosition.z);
            //Debug.Log(destination_pos);
            while (transform.localPosition != destination_pos)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, destination_pos, Time.deltaTime * MovementAnimationSpeed);
                yield return 0;
            }
        }
        m_bIsMoving = false;
        OnMoveFinished();
        _onFinished.Invoke();
    }
    protected virtual void OnMoveFinished() { }

}

public class AttackEventArgs : EventArgs
{
    public UnitBase Attacker;
    public UnitBase Defender;

    public int Damage;

    public AttackEventArgs(UnitBase _attacker, UnitBase _defender, int _damage)
    {
        Attacker = _attacker;
        Defender = _defender;

        Damage = _damage;
    }
}
public class AttackAction
{
    public readonly int Damage;
    public readonly float ActionCost;

    public AttackAction(int damage, float actionCost)
    {
        Damage = damage;
        ActionCost = actionCost;
    }
}

public class MovementEventArgs : EventArgs
{
    public TileInfo OriginCell;
    public TileInfo DestinationCell;
    public List<TileInfo> Path;

    public MovementEventArgs(TileInfo sourceCell, TileInfo destinationCell, List<TileInfo> path)
    {
        OriginCell = sourceCell;
        DestinationCell = destinationCell;
        Path = path;
    }
}

public class UnitCreatedEventArgs : EventArgs
{
    public Transform m_unit;

    public UnitCreatedEventArgs(Transform _unit)
    {
        this.m_unit = _unit;
    }
}

