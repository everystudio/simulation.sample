using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace anogame_strategy
{
    public class GameStateWaitingForInput : GameState
    {
        public GameStateWaitingForInput(StrategyBase _manager) : base(_manager)
        {

        }
        public override void OnUnitClicked(UnitBase _unit)
        {
            //Debug.Log($"OnUnitClicked:{_unit.name}");
            if (_unit.PlayerNumber.Equals(m_gameManager.CurrentPlayerNumber))
            {
                m_gameManager.CurrentGameState = new GameStateUnitMenuTop(m_gameManager, _unit);
            }
        }

        public override void OnUpdate()
        {
            if (Input.GetMouseButtonDown(1))
            {
                m_gameManager.CurrentGameState = new GameStateTurnMenu(m_gameManager);
            }
        }

    }
}