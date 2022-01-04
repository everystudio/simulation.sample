using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace anogame_strategy
{
	public class PlayerHuman : PlayerBase
	{
		public override void play(StrategyBase _gameManager)
		{
			//Debug.Log($"PlayerHuman:{PlayerNumber}");
			_gameManager.CurrentGameState = new GameStateWaitingForInput(_gameManager);
		}
	}

}
