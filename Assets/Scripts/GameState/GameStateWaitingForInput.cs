using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateWaitingForInput : GameState
{
	public GameStateWaitingForInput(GameManager _manager) : base(_manager)
	{

	}
    public override void OnUnitClicked(UnitBase _unit)
    {
        //Debug.Log($"OnUnitClicked:{_unit.name}");
        if (_unit.PlayerNumber.Equals(m_gameManager.CurrentPlayerNumber))
        {
            m_gameManager.CurrentGameState = new GameStateUnitSelected(m_gameManager, _unit);
        }
    }

}
