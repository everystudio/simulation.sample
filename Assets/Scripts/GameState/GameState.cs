using System.Linq;

public abstract class GameState
{
    protected GameManager m_gameManager;

    protected GameState(GameManager _gameManager)
    {
        m_gameManager = _gameManager;
    }

    public virtual void OnUnitClicked(UnitBase _unit)
    {
    }
    public virtual void OnCellDeselected(TileInfo _tileInfo)
    {
        _tileInfo.UnMark();
    }

    public virtual void OnCellSelected(TileInfo _tileInfo)
    {
        _tileInfo.MarkAsHighlighted();
    }

    public virtual void OnCellClicked(TileInfo _tileInfo)
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