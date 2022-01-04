using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace anogame_strategy
{
    public class PlayerAI : PlayerBase
    {
        private System.Random m_rnd;
        private StrategyBase m_gameManager;
        public PlayerAI()
        {
            m_rnd = new System.Random();
        }

        public override void play(StrategyBase _gameManager)
        {
            _gameManager.CurrentGameState = new GameStateAITurn(_gameManager);
            m_gameManager = _gameManager;

            StartCoroutine(GamePlay());
        }

        private IEnumerator GamePlay()
        {
            var myUnits = m_gameManager.Units.FindAll(u => u.PlayerNumber.Equals(PlayerNumber)).ToList();
            //Debug.Log(myUnits.Count);
            foreach (var unit in myUnits.OrderByDescending(u => u.CurrentTileInfo.GetNeighbours(m_gameManager.TileInfos).FindAll(u.IsCellTraversable).Count))
            {
                var enemyUnits = m_gameManager.Units.Except(myUnits).ToList();
                var unitsInRange = new List<UnitBase>();
                foreach (var enemyUnit in enemyUnits)
                {
                    if (unit.IsUnitAttackable(enemyUnit, unit.CurrentTileInfo))
                    {
                        unitsInRange.Add(enemyUnit);
                    }
                }//Looking for enemies that are in attack range.
                if (unitsInRange.Count != 0)
                {
                    var index = m_rnd.Next(0, unitsInRange.Count);

                    bool bAttacking = true;
                    unit.AttackHandler(unitsInRange[index], () =>
                    {
                        bAttacking = false;
                    });
                    while (bAttacking)
                    {
                        yield return 0;
                    }
                    continue;
                }

                List<TileInfo> potentialDestinations = new List<TileInfo>();

                foreach (var enemyUnit in enemyUnits)
                {
                    potentialDestinations.AddRange(m_gameManager.TileInfos.FindAll(
                        c => unit.IsTileInfoMovableTo(c) && unit.IsUnitAttackable(enemyUnit, c)));
                }

                var notInRange = potentialDestinations.FindAll(c => c.GetDistance(unit.CurrentTileInfo) > unit.MovementPoints);
                potentialDestinations = potentialDestinations.Except(notInRange).ToList();

                if (potentialDestinations.Count == 0 && notInRange.Count != 0)
                {
                    potentialDestinations.Add(notInRange.ElementAt(m_rnd.Next(0, notInRange.Count - 1)));
                }

                potentialDestinations = potentialDestinations.OrderBy(h => m_rnd.Next()).ToList();
                List<TileInfo> shortestPath = null;
                foreach (var potentialDestination in potentialDestinations)
                {
                    var path = unit.FindPath(m_gameManager.TileInfos, potentialDestination);
                    if ((shortestPath == null && path.Sum(h => h.MovementCost) > 0) || shortestPath != null && (path.Sum(h => h.MovementCost) < shortestPath.Sum(h => h.MovementCost) && path.Sum(h => h.MovementCost) > 0))
                    {
                        shortestPath = path;
                    }

                    var pathCost = path.Sum(h => h.MovementCost);
                    if (pathCost > 0 && pathCost <= unit.MovementPoints)
                    {
                        bool bMoving = true;
                        unit.Move(potentialDestination, path, () => { bMoving = false; });
                        while (bMoving)
                        {
                            yield return 0;
                        }
                        shortestPath = null;
                        break;
                    }
                    yield return 0;
                }

                if (shortestPath != null)
                {
                    foreach (var potentialDestination in shortestPath.Intersect(unit.GetAvailableDestinations(m_gameManager.TileInfos)).OrderByDescending(h => h.GetDistance(unit.CurrentTileInfo)))
                    {
                        var path = unit.FindPath(m_gameManager.TileInfos, potentialDestination);
                        var pathCost = path.Sum(h => h.MovementCost);
                        if (pathCost > 0 && pathCost <= unit.MovementPoints)
                        {
                            bool bMoving = true;
                            unit.Move(potentialDestination, path, () => { bMoving = false; });
                            while (bMoving)
                            {
                                yield return 0;
                            }
                            break;
                        }
                        yield return 0;
                    }
                }

                foreach (var enemyUnit in enemyUnits)
                {
                    var enemyCell = enemyUnit.CurrentTileInfo;
                    if (unit.IsUnitAttackable(enemyUnit, unit.CurrentTileInfo))
                    {
                        //unit.AttackHandler(enemyUnit);

                        bool bAttacking = true;
                        unit.AttackHandler(enemyUnit, () =>
                        {
                            bAttacking = false;
                        });
                        while (bAttacking)
                        {
                            yield return 0;
                        }


                        yield return new WaitForSeconds(0.5f);
                        break;
                    }
                }
            }
            m_gameManager.EndTurn();
        }
    }
}