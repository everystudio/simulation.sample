using UnityEngine;
using UnityEngine.EventSystems;

namespace anogamelib
{
	public class UILayerTouchListener : UITouchListener , ICanvasRaycastFilter
	{
		Vector2 m_screenPoint = Vector2.zero;
		bool m_bPressed = false;
		bool m_bRaycasted = false;

		public bool IsRaycastLocationValid(Vector2 _screenPoint, Camera _eventCamera)
		{
			m_screenPoint = _screenPoint;
			m_bRaycasted = true;
			return false;
		}

		public void Update()
		{
			if (!m_bRaycasted || !layer.IsTouchable())
			{
				m_bPressed = false;
				return;
			}

			if (Input.touchCount > 0)
			{
				if (Input.touches[0].phase == TouchPhase.Began)
				{
					m_bPressed = true;
					base.OnPointerDown(CreatePointerEventData());
					return;
				}
				else if (Input.touches[0].phase == TouchPhase.Ended)
				{
					if (!m_bPressed) { return; }
					m_bPressed = false;
					base.OnPointerUp(CreatePointerEventData());
					return;
				}
			}

			if (Input.GetMouseButtonDown(0))
			{
				m_bPressed = true;
				base.OnPointerDown(CreatePointerEventData());
				return;
			}
			else if (Input.GetMouseButtonUp(0))
			{
				if (!m_bPressed) { return; }
				m_bPressed = false;
				base.OnPointerUp(CreatePointerEventData());
				return;
			}

			if (m_bPressed)
			{
				base.OnDrag(CreatePointerEventData());
			}
		}

		public void LateUpdate()
		{
			m_bRaycasted = false;
		}

		public PointerEventData CreatePointerEventData()
		{
			PointerEventData data = new PointerEventData(null);
			data.position = m_screenPoint;
			return data;
		}
	}
}