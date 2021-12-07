using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using anogamelib;
using UnityEngine.EventSystems;

public class UIUnitTop : UIBase
{
	private GameManager m_gameManager;
	private UnitBase m_selectingUnit;
	public UIUnitTop(GameManager _gameManager, UnitBase _unit) : base("UI/UIUnitTop", UIGroup.Dialog,
		UIPreset.BackVisible | UIPreset.LoadingWithoutFade)
	{
		m_gameManager = _gameManager;
		m_selectingUnit = _unit;
	}

	public override void OnActive()
	{
		base.OnActive();
		Canvas canv = GameObject.Find("ANOCanvas").GetComponent<Canvas>();
		Vector2 MousePos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(
			canv.GetComponent<RectTransform>(),
			Input.mousePosition,
			canv.worldCamera,
			out MousePos);

		Debug.Log(MousePos);

		MousePos += new Vector2(-120f, 50f);

		RectTransform rtTarget = root.Find("targetImage").GetComponent<RectTransform>();
		rtTarget.anchoredPosition = new Vector2(
			MousePos.x,
			MousePos.y);


	}

	public override bool OnClick(string _strName, GameObject _gameObject, PointerEventData _pointer, SE se)
	{
		Debug.Log("UIUnitTop.OnClick");
		if( _strName == "imgCommandMove")
		{
			m_gameManager.CurrentGameState = new GameStateUnitMoveArea(m_gameManager, m_selectingUnit);
			UIController.Instance.Remove(this);
		}
		else if( _strName == "imgCommandAttack")
		{
			m_gameManager.CurrentGameState = new GameStateUnitAttack(m_gameManager, m_selectingUnit);
			UIController.Instance.Remove(this);
		}
		else if(_strName == "imgCommandClose")
		{
			m_gameManager.CurrentGameState = new GameStateWaitingForInput(m_gameManager);
			UIController.Instance.Remove(this);
		}

		return base.OnClick(_strName, _gameObject, _pointer, se);
	}
}
