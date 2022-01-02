using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace anogame_strategy
{
    public class StrategyBase : MonoBehaviour
    {
        public event EventHandler LevelLoading;
        public event EventHandler LevelLoadingDone;
        public event EventHandler GameStarted;
        public event EventHandler GameEnded;
        public event EventHandler TurnEnded;

        // GUI Events
        public event EventHandler<bool> UIUnitMenu;
        public event EventHandler<bool> UITurnMenu;

        public List<PlayerBase> Players { get; private set; }
        public List<TileInfo> TileInfos { get; private set; }
        public List<UnitBase> Units { get; private set; }

        public Tilemap m_playgroundTilemap;
        public Transform m_tfTileInfoRoot;
        public GameObject m_prefTileInfo;

        public string m_strLoadUIScene = "GameHUD";

        public event EventHandler<UnitCreatedEventArgs> UnitAdded;
        private GameState _gameState;
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
            if (LevelLoading != null)
            {
                LevelLoading.Invoke(this, new EventArgs());
            }
            yield return null;

            var uiScene = SceneManager.GetSceneByName(m_strLoadUIScene);
            //Debug.Log(uiScene);
            if (uiScene != null && !uiScene.isLoaded)
            {
                SceneManager.LoadSceneAsync(m_strLoadUIScene, LoadSceneMode.Additive);
                var scene = SceneManager.GetSceneByName(m_strLoadUIScene);
                yield return new WaitUntil(() => scene.isLoaded);
            }

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
                foreach (PlayerBase p in PlayerArr)
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
                    info.transform.localPosition = new Vector3(info.transform.localPosition.x, info.transform.localPosition.y, 0f);
                    info.UnMark();
                    TileInfos.Add(info);
                }
            }
            m_playgroundTilemap.GetComponent<TilemapRenderer>().enabled = false;
            foreach (var tileInfo in TileInfos)
            {
                tileInfo.TileInfoClicked += OnTileInfoClicked;
                tileInfo.TileInfoHighlighted += OnTileInfoHighlighted;
                tileInfo.TileInfoDehighlighted += OnTileInfoDehighlighted;

                tileInfo.GetNeighbours(TileInfos);
            }

            Units = new List<UnitBase>();
            UnitBase[] unitBaseArr = FindObjectsOfType<UnitBase>();
            foreach (UnitBase unit in unitBaseArr)
            {
                unit.Initialize();

                var tileinfo = TileInfos.OrderBy(h => Math.Abs((h.transform.position - unit.transform.position).magnitude)).First();
                if (tileinfo != null)
                {
                    // 相互参照
                    unit.CurrentTileInfo = tileinfo;
                    tileinfo.CurrentUnit = unit;

                    unit.transform.position = new Vector3(
                        tileinfo.transform.position.x,
                        tileinfo.transform.position.y,
                        UnitBase.PosZ);
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
            CurrentGameState.OnTileInfoDeselected(sender as TileInfo);
        }
        private void OnTileInfoHighlighted(object sender, EventArgs e)
        {
            CurrentGameState.OnTileInfoSelected(sender as TileInfo);
        }
        private void OnTileInfoClicked(object sender, EventArgs e)
        {
            CurrentGameState.OnTileInfoClicked(sender as TileInfo);
        }

        public void EndTurn()
        {
            if (Units.Select(u => u.PlayerNumber).Distinct().Count() == 1)
            {
                return;
            }
            Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).ForEach(u => { u.OnTurnEnd(); });

            CurrentPlayerNumber = (CurrentPlayerNumber + 1) % NumberOfPlayers;
            while (Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).Count == 0)
            {
                CurrentPlayerNumber = (CurrentPlayerNumber + 1) % NumberOfPlayers;
            }

            if (TurnEnded != null)
            {
                TurnEnded.Invoke(this, new EventArgs());
            }
            Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).ForEach(u => { u.OnTurnStart(); });
            Players.Find(p => p.PlayerNumber.Equals(CurrentPlayerNumber)).Play(this);
        }

        public void TurnMenu(bool _bFlag)
        {
            if (UITurnMenu != null)
            {
                UITurnMenu.Invoke(this, _bFlag);
            }
        }



        private void Update()
        {
            if (CurrentGameState != null)
            {
                CurrentGameState.OnUpdate();
            }
        }
    }
}
