using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using anogamelib;
using UnityEngine.EventSystems;

namespace anogame_strategy
{
	public class UITurnMenu : UIBase
	{
		private GameStateTurnMenu m_gameState;
		public UITurnMenu(GameStateTurnMenu _gameState) : base("UI/UITurnMenu", UIGroup.Dialog,
			UIPreset.BackVisible)
		{ m_gameState = _gameState; }
		public override bool OnClick(string _strName, GameObject _gameObject, PointerEventData _pointer, SE se)
		{
			Debug.Log(_strName);
			if (_strName == "btnClose")
			{
				m_gameState.Close(this);
			}
			else if (_strName == "btnTurnEnd")
			{
				m_gameState.TurnEnd(this);
			}
			return base.OnClick(_strName, _gameObject, _pointer, se);
		}
	}
}