using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using anogamelib;
using UnityEngine.EventSystems;

public class UIUnitTop : UIBase
{
	public UIUnitTop() : base("UI/UIUnitTop", UIGroup.Dialog,
		UIPreset.BackVisible|UIPreset.LoadingWithoutFade)
	{ }

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

		return base.OnClick(_strName, _gameObject, _pointer, se);
	}
}
