using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace anogamelib
{
    public class TouchEvent
    {
		private UITouchListener m_listener;
		public UITouchListener listener { get { return m_listener; } }

		private UITouchType m_type;
		public UITouchType type { get { return m_type; } }

		private PointerEventData m_pointer;
		public PointerEventData pointer { get { return m_pointer; } }

		public TouchEvent(UITouchListener _listener, UITouchType _type, PointerEventData _pointer)
		{
			m_listener = _listener;
			m_type = _type;
			m_pointer = _pointer;
		}
	}

	public class DispatchedEvent
	{
		private string m_name;
		public string name { get { return m_name; } }

		private object m_param;
		public object param { get { return m_param; } }

		public DispatchedEvent(string _name, object _param)
		{
			m_name = _name;
			m_param = _param;
		}
	}
}
