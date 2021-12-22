using System.Linq;

namespace anogame_strategy
{
    public abstract class GameState
    {
        protected StrategyBase m_gameManager;

        protected GameState(StrategyBase _gameManager)
        {
            m_gameManager = _gameManager;
        }

        public virtual void OnUnitClicked(UnitBase _unit)
        {
        }
        public virtual void OnTileInfoDeselected(TileInfo _tileInfo)
        {
            _tileInfo.UnMark();
        }

        public virtual void OnTileInfoSelected(TileInfo _tileInfo)
        {
            _tileInfo.MarkAsHighlighted();
        }

        public virtual void OnTileInfoClicked(TileInfo _tileInfo)
        {
        }

        public virtual void OnStateEnter()
        {
            if (m_gameManager.Units.Select(u => u.PlayerNumber).Distinct().ToList().Count == 1)
            {
                m_gameManager.CurrentGameState = new GameStateGameOver(m_gameManager);
            }
        }

        public virtual void OnStateExit()
        {
        }

        public virtual void OnUpdate() { }
    }
}