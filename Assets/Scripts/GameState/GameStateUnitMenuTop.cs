using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using anogamelib;

public class GameStateUnitMenuTop : GameState
{
	private UnitBase m_unit;

	public GameStateUnitMenuTop(GameManager _gameManager, UnitBase _unit) : base(_gameManager)
	{
		m_unit = _unit;
		UIController.Instance.Replace(new UIBase[] { new UIUnitTop(_gameManager,_unit) });

		//Canvas canv = GameObject.Find("HUDCanvas").GetComponent<Canvas>();
		/*
		Vector2 MousePos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(
			canv.GetComponent<RectTransform>(),
			Input.mousePosition,
			canv.worldCamera,
			out MousePos);

		Debug.Log(MousePos);

		MousePos += new Vector2(-120f, 50f);

		RectTransform rtTarget = GameObject.Find("targetImage").GetComponent<RectTransform>();
		rtTarget.anchoredPosition = new Vector2(
			MousePos.x,
			MousePos.y);
				*/

	}
}
