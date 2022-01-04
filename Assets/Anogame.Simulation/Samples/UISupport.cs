using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using anogamelib;

namespace anogame_strategy
{
	public class UISupport : MonoBehaviour
	{
		[SerializeField]
		private StrategyBase m_strategy;

		private UIBase m_statusView;

		private void Awake()
		{
			m_strategy.UnitAdded += M_strategy_UnitAdded;
			m_strategy.TurnEnded += M_strategy_TurnEnded;
			m_strategy.GameEnded += M_strategy_GameEnded;
		}

		private void M_strategy_UnitAdded(object sender, UnitCreatedEventArgs e)
		{
			RegisterUnit(e.m_unit.GetComponent<UnitBase>());
		}
		private void RegisterUnit(UnitBase _unit)
		{
			_unit.UnitHighlighted += _unit_UnitHighlighted;
			_unit.UnitDehighlighted += _unit_UnitDehighlighted;
			_unit.UnitAttacked += _unit_UnitAttacked;
			_unit.UnitDestroyed += _unit_UnitDestroyed;
		}

		private void _unit_UnitDestroyed(object sender, AttackEventArgs e)
		{
		}

		private void _unit_UnitAttacked(object sender, AttackEventArgs e)
		{
		}

		private void _unit_UnitHighlighted(object sender, System.EventArgs e)
		{
			if (m_statusView == null)
			{
				UnitBase unit = sender as UnitBase;
				m_statusView = new UnitStatusView(unit);
				UIController.Instance.AddFront(m_statusView);
			}
		}

		private void _unit_UnitDehighlighted(object sender, System.EventArgs e)
		{
			if (m_statusView != null)
			{
				UIController.Instance.Remove(m_statusView);
				m_statusView = null;
			}
		}

		private void M_strategy_TurnEnded(object sender, System.EventArgs e)
		{
			// "Player " + ((sender as StrategyBase).CurrentPlayerNumber + 1 ) + " win !!";
		}

		private void M_strategy_GameEnded(object sender, System.EventArgs e)
		{
			GameObject obj = GameObject.Find("SituationText");
			if (obj != null && obj.GetComponent<Text>() != null)
			{
				string msg = "Player " + ((sender as StrategyBase).CurrentPlayerNumber + 1) + " win !!";
				obj.GetComponent<Text>().text = msg;
			}

			if ((sender as StrategyBase).CurrentPlayerNumber == 0)
			{

			}
			else
			{

			}
		}
	}
}