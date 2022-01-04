using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace anogame_strategy
{
	public class PlayerHuman : PlayerBase
	{
		public override void Play(StrategyBase _gameManager)
		{
			//Debug.Log($"PlayerHuman:{PlayerNumber}");
			_gameManager.CurrentGameState = new GameStateWaitingForInput(_gameManager);
		}
	}

}
