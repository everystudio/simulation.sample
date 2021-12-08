using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using anogamelib;

public class GameStateTurnMenu : GameState
{
	private StrategyBase m_strategy;
	public GameStateTurnMenu(StrategyBase _gameManager) : base(_gameManager) { m_strategy = _gameManager; }
	public override void OnStateEnter()
	{
		m_strategy.TurnMenu(true);
		UIController.Instance.AddFront(new UITurnMenu(this));
	}

	public void Close(UIBase _uibase)
	{
		UIController.Instance.Remove(_uibase);
		m_gameManager.CurrentGameState = new GameStateWaitingForInput(m_gameManager);
	}
	public void TurnEnd(UIBase _uibase)
	{
		UIController.Instance.Remove(_uibase);
		m_gameManager.EndTurn();
	}
}
