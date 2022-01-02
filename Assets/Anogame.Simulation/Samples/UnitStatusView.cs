using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using anogamelib;

namespace anogame_strategy
{
	public class UnitStatusView : UIBase
	{
		private UnitBase m_unitBase;
		public UnitStatusView(UnitBase _unit) : base("UI/UnitStatusView", UIGroup.Dialog,
			UIPreset.BackVisible | UIPreset.BackTouchable)
		{
			m_unitBase = _unit;
		}

		public override void OnActive()
		{
			Canvas canv = GameObject.Find("ANOCanvas").GetComponent<Canvas>();
			Vector2 MousePos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(
				canv.GetComponent<RectTransform>(),
				Input.mousePosition,
				canv.worldCamera,
				out MousePos);

			RectTransform rtTarget = root.Find("window").GetComponent<RectTransform>();
			float fOffset = (rtTarget.sizeDelta.x * 0.8f) * (Input.mousePosition.x < Screen.width * 0.5f ? 1f : -1f);

			MousePos += new Vector2(
				fOffset,
				0f);

			rtTarget.anchoredPosition = new Vector2(
				MousePos.x,
				rtTarget.anchoredPosition.y);

			root.Find("window").GetComponent<WindowStatusView>().Initialize(m_unitBase);
		}

	}
}