using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHuman : Player
{
	public override void Play(GameManager _gameManager)
	{
		Debug.Log($"PlayerHuman:{PlayerNumber}");
		_gameManager.CurrentGameState = new GameStateWaitingForInput(_gameManager);
	}
}
