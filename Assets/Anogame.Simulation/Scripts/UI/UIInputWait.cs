using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using anogamelib;
using UnityEngine.EventSystems;

namespace anogame_strategy
{
	public class UIInputWait : UIBase
	{
		StrategyBase m_strategyBase;
		public UIInputWait(StrategyBase _strategy) : base("UI/UIInputWait",
			UIGroup.Scene,
			UIPreset.BackVisible | UIPreset.BackTouchable)
		{
			m_strategyBase = _strategy;
		}
		public override bool OnClick(string _strName, GameObject _gameObject, PointerEventData _pointer, SE se)
		{
			if (_strName == "btnMenu")
			{
				m_strategyBase.CurrentGameState = new GameStateTurnMenu(m_strategyBase);
			}
			return base.OnClick(_strName, _gameObject, _pointer, se);
		}
	}
}