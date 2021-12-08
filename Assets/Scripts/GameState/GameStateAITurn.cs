using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateAITurn : GameState
{
    public GameStateAITurn(StrategyBase _gameManager) : base(_gameManager){}

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        foreach (var tileinfo in m_gameManager.TileInfos)
        {
            tileinfo.UnMark();
        }
    }
}
