using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using anogamelib;

public class GameStateTurnMenu : GameState
{

	public GameStateTurnMenu(GameManager _gameManager) : base(_gameManager) { }
	public override void OnStateEnter()
	{
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
