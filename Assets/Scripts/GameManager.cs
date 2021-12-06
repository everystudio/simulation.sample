using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public event EventHandler LevelLoading;
    public event EventHandler LevelLoadingDone;
    public event EventHandler GameStarted;
    public event EventHandler GameEnded;
    public event EventHandler TurnEnded;

    public List<PlayerBase> Players { get; private set; }
    public List<TileInfo> TileInfos { get; private set; }
    public List<UnitBase> Units { get; private set; }

    public Tilemap m_playgroundTilemap;
    public Transform m_tfTileInfoRoot;
    public GameObject m_prefTileInfo;

    public event EventHandler<UnitCreatedEventArgs> UnitAdded;
    private GameState _gameState; //The grid delegates some of its behaviours to cellGridState object.
    public GameState CurrentGameState
    {
        get
        {
            return _gameState;
        }
        set
        {
            if (_gameState != null)
            {
                _gameState.OnStateExit();
            }
            _gameState = value;
            _gameState.OnStateEnter();
        }
    }
    public int NumberOfPlayers { get; private set; }

    public PlayerBase CurrentPlayer
    {
        get { return Players.Find(p => p.PlayerNumber.Equals(CurrentPlayerNumber)); }
    }
    public int CurrentPlayerNumber { get; private set; }

	private IEnumerator Start()
	{
        //Debug.Log("start");
        if(LevelLoading != null)
		{
            LevelLoading.Invoke(this, new EventArgs());
		}
        yield return null;

        Initialize();

        if (LevelLoadingDone != null)
        {
            LevelLoadingDone.Invoke(this, new EventArgs());
        }

        StartGame();
    }

    private void Initialize()
	{
        if (Players == null || Players.Count == 0)
        {
            Players = new List<PlayerBase>();
            // プレイヤーの設定
            PlayerBase[] PlayerArr = FindObjectsOfType<PlayerBase>();
            foreach(PlayerBase p in PlayerArr)
			{
                Players.Add(p);
			}
        }
        NumberOfPlayers = Players.Count;
        if (0 < Players.Count)
        {
            CurrentPlayerNumber = Players.Min(p => p.PlayerNumber);
        }
        else
		{
            CurrentPlayerNumber = 0;
		}

        TileInfos = new List<TileInfo>();
        foreach (Vector3Int pos in m_playgroundTilemap.cellBounds.allPositionsWithin)
        {
            TileConfig config = m_playgroundTilemap.GetTile<TileConfig>(pos);
            if (config != null)
            {
                TileInfo info = Instantiate(m_prefTileInfo, m_tfTileInfoRoot).GetComponent<TileInfo>();
                info.m_tileParam = config.tileParam;
                info.OffsetCoord = new Vector2(pos.x, pos.y);
                info.transform.position = pos + m_playgroundTilemap.transform.position + (1f * m_playgroundTilemap.tileAnchor);
                info.UnMark();
                TileInfos.Add(info);
            }
        }
        foreach (var cell in TileInfos)
        {
            cell.TileInfoClicked += OnTileInfoClicked;
            cell.TileInfoHighlighted += OnTileInfoHighlighted;
            cell.TileInfoDehighlighted += OnTileInfoDehighlighted;

            cell.GetComponent<TileInfo>().GetNeighbours(TileInfos);
        }

        Units = new List<UnitBase>();
        UnitBase[] unitBaseArr = FindObjectsOfType<UnitBase>();
        foreach( UnitBase unit in unitBaseArr)
		{
            unit.Initialize();

            var tileinfo = TileInfos.OrderBy(h => Math.Abs((h.transform.position - unit.transform.position).magnitude)).First();
            if(tileinfo != null)
			{
                // 相互参照
                unit.CurrentTileInfo = tileinfo;
                tileinfo.CurrentUnit = unit;

                unit.transform.position = new Vector3(
                    tileinfo.transform.position.x, tileinfo.transform.position.y, UnitBase.PosZ);

            }
            AddUnit(unit.transform);
		}
    }
    public void StartGame()
    {
        if (GameStarted != null)
        {
            GameStarted.Invoke(this, new EventArgs());
        }
        Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).ForEach(u => { u.OnTurnStart(); });
        Players.Find(p => p.PlayerNumber.Equals(CurrentPlayerNumber)).Play(this);
    }

    private void OnUnitClicked(object sender, EventArgs e)
    {
        //Debug.Log("OnUnitClicked");
        //Debug.Log(CurrentGameState);
        CurrentGameState.OnUnitClicked(sender as UnitBase);
    }
    private void OnUnitDestroyed(object sender, AttackEventArgs e)
    {
        Units.Remove(sender as UnitBase);
        var totalPlayersAlive = Units.Select(u => u.PlayerNumber).Distinct().ToList(); //Checking if the game is over
        if (totalPlayersAlive.Count == 1)
        {
            if (GameEnded != null)
            {
                GameEnded.Invoke(this, new EventArgs());
            }
        }
    }

    public void AddUnit(Transform _unit)
    {
        Units.Add(_unit.GetComponent<UnitBase>());
        _unit.GetComponent<UnitBase>().UnitClicked += OnUnitClicked;
        _unit.GetComponent<UnitBase>().UnitDestroyed += OnUnitDestroyed;

        if (UnitAdded != null)
        {
            UnitAdded.Invoke(this, new UnitCreatedEventArgs(_unit));
        }
    }

    private void OnTileInfoDehighlighted(object sender, EventArgs e)
    {
        CurrentGameState.OnCellDeselected(sender as TileInfo);
    }
    private void OnTileInfoHighlighted(object sender, EventArgs e)
    {
        CurrentGameState.OnCellSelected(sender as TileInfo);
    }
    private void OnTileInfoClicked(object sender, EventArgs e)
    {
        CurrentGameState.OnCellClicked(sender as TileInfo);
    }

    public void EndTurn()
	{
        if(Units.Select(u=>u.PlayerNumber).Distinct().Count() == 1)
		{
            return;
		}
        Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).ForEach(u => { u.OnTurnEnd(); });

        CurrentPlayerNumber = (CurrentPlayerNumber + 1) % NumberOfPlayers;
        while (Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).Count == 0)
        {
            CurrentPlayerNumber = (CurrentPlayerNumber + 1) % NumberOfPlayers;
        }//Skipping players that are defeated.

        if (TurnEnded != null)
        {
            TurnEnded.Invoke(this, new EventArgs());
        }
        Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).ForEach(u => { u.OnTurnStart(); });
        Players.Find(p => p.PlayerNumber.Equals(CurrentPlayerNumber)).Play(this);
    }

	private void Update()
	{
		if( CurrentGameState != null)
		{
            CurrentGameState.OnUpdate();
        }
	}
}
