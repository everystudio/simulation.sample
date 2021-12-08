using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using anogamelib;

class GameStateUnitMoveArea : GameState
{
    private UnitBase m_unit;
    private HashSet<TileInfo> _pathsInRange;
    private List<UnitBase> _unitsInRange;
    private List<UnitBase> _unitsMarkedInRange;

    private TileInfo _unitCell;

    private List<TileInfo> _currentPath;

    public GameStateUnitMoveArea(StrategyBase _gameManager, UnitBase _unit) : base(_gameManager)
    {
        m_unit = _unit;
        _pathsInRange = new HashSet<TileInfo>();
        _currentPath = new List<TileInfo>();
        _unitsInRange = new List<UnitBase>();
        _unitsMarkedInRange = new List<UnitBase>();

    }

    public override void OnCellClicked(TileInfo cell)
    {
        //Debug.Log("OnCellClicked");
        if (m_unit.m_bIsMoving)
        {
            return;
        }
        if (cell.m_tileParam.IsTaken || !_pathsInRange.Contains(cell))
        {
            m_gameManager.CurrentGameState = new GameStateWaitingForInput(m_gameManager);
            return;
        }
        var path = m_unit.FindPath(m_gameManager.TileInfos, cell);
        m_unit.Move(cell, path , ()=> {
            m_gameManager.CurrentGameState = new GameStateUnitMenuTop(m_gameManager, m_unit);
        });
    }

    public override void OnUnitClicked(UnitBase unit)
    {
        Debug.Log("OnUnitClicked");
        if (unit.Equals(m_unit) || m_unit.m_bIsMoving)
        {
            return;
        }

        /*
        if (_unitsInRange.Contains(unit) && !m_unit.m_bIsMoving)
        {
            m_unit.AttackHandler(unit);
            m_gameManager.CurrentGameState = new GameStateUnitMoveArea(m_gameManager, m_unit);
        }
        */

        if (unit.PlayerNumber.Equals(m_unit.PlayerNumber))
        {
            m_gameManager.CurrentGameState = new GameStateUnitMoveArea(m_gameManager, unit);
        }

    }
    public override void OnCellDeselected(TileInfo cell)
    {
        //Debug.Log("OnCellDeselected");

        base.OnCellDeselected(cell);
        foreach (var _cell in _currentPath)
        {
            if (_pathsInRange.Contains(_cell))
                _cell.MarkAsReachable();
            else
                _cell.UnMark();
        }
        foreach (var unit in _unitsMarkedInRange)
        {
            unit.UnMark();
        }
        _unitsMarkedInRange.Clear();
        foreach (var unit in _unitsInRange)
        {
            unit.MarkAsReachableEnemy();
        }
    }
    public override void OnCellSelected(TileInfo cell)
    {
        //Debug.LogWarning("OnCellSelected");
        base.OnCellSelected(cell);
        if (!_pathsInRange.Contains(cell)) return;

        _currentPath = m_unit.FindPath(m_gameManager.TileInfos, cell);
        foreach (var _cell in _currentPath)
        {
            _cell.MarkAsPath();
        }
        foreach (var unit in _unitsInRange)
        {
            unit.UnMark();
        }
        foreach (var currentUnit in m_gameManager.Units)
        {
            if (m_unit.IsUnitAttackable(currentUnit, cell))
            {
                currentUnit.SetState(new UnitStateMarkedAsReachableEnemy(currentUnit));
                _unitsMarkedInRange.Add(currentUnit);
            }
        }
    }

    public override void OnStateEnter()
    {
        //Debug.Log("OnStateEnter");
        base.OnStateEnter();

        m_unit.OnUnitSelected();
        _unitCell = m_unit.CurrentTileInfo;

        _pathsInRange = m_unit.GetAvailableDestinations(m_gameManager.TileInfos);

        //Debug.Log(_pathsInRange.Count);
        var cellsNotInRange = m_gameManager.TileInfos.Except(_pathsInRange);

        foreach (var cell in cellsNotInRange)
        {
            cell.UnMark();
        }
        foreach (var cell in _pathsInRange)
        {
            cell.MarkAsReachable();
        }

        if (m_unit.ActionPoints <= 0) return;

        foreach (var currentUnit in m_gameManager.Units)
        {
            if (currentUnit.PlayerNumber.Equals(m_unit.PlayerNumber))
                continue;

            if (m_unit.IsUnitAttackable(currentUnit, m_unit.CurrentTileInfo))
            {
                currentUnit.SetState(new UnitStateMarkedAsReachableEnemy(currentUnit));
                _unitsInRange.Add(currentUnit);
            }
        }

        if (_unitCell.GetNeighbours(m_gameManager.TileInfos).FindAll(c => c.m_tileParam.MovementCost <= m_unit.MovementPoints).Count == 0
            && _unitsInRange.Count == 0)
        {
            m_unit.SetState(new UnitStateMarkedAsFinished(m_unit));
        }
    }
    public override void OnStateExit()
    {
        //Debug.Log("OnStateExit");
        m_unit.OnUnitDeselected();
        foreach (var unit in _unitsInRange)
        {
            if (unit == null) continue;
            unit.SetState(new UnitStateNormal(unit));
        }
        foreach (var cell in m_gameManager.TileInfos)
        {
            cell.UnMark();
        }
    }
}