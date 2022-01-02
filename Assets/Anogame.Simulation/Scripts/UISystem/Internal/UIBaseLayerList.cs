using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace anogamelib
{
    public class UIBaseLayerList
    {
        private List<UIBaseLayer> m_list = new List<UIBaseLayer>();

		public static int GetNumInGroup(UIGroup _group, List<UIBaseLayer> _list)
		{
			int count = 0;
			for (int i = 0; i < _list.Count; i++)
			{
				if (_group == _list[i].ui.group) { ++count; }
			}
			return count;
		}

		public int GetNumInGroup(UIGroup group)
		{
			return UIBaseLayerList.GetNumInGroup(group, m_list);
		}
		public int Count { get { return m_list.Count; } }

		public void Insert(UIBaseLayer _layer)
		{
			int index = FindInsertPosition(_layer.ui.group);
			//Debug.Log(_layer.ui.group);
			//Debug.Log(index);
			if (index < 0)
			{
				m_list.Add(_layer);
			}
			else
			{
				m_list.Insert(index, _layer);
			}
		}
		public void Eject(UIBaseLayer _layer)
		{
			int index = m_list.FindIndex(l => {
				return (l == _layer);
			});
			if (index >= 0)
			{
				m_list.RemoveAt(index);
			}
		}

		public void ForEachOnlyActive(Action<UIBaseLayer> _action)
		{
			List<UIBaseLayer> list = new List<UIBaseLayer>(m_list);
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].state == UIState.Active)
				{
					_action(list[i]);
				}
			}
		}
		public void ForEachAnything(Action<UIBaseLayer> _action)
		{
			List<UIBaseLayer> list = new List<UIBaseLayer>(m_list);
			for (int i = 0; i < list.Count; i++)
			{
				_action(list[i]);
			}
		}
		
		public UIBaseLayer Find(UIBase _ui)
		{
			return m_list.Find(v => {
				return (v.ui == _ui);
			});
		}

		public List<UIBaseLayer> FindLayers(UIGroup _group)
		{
			List<UIBaseLayer> list = new List<UIBaseLayer>();
			for (int i = 0; i < m_list.Count; i++)
			{
				if (m_list[i].ui.group == _group)
				{
					list.Add(m_list[i]);
				}
			}
			return list;
		}

		public UIBaseLayer FindFrontLayerInGroup(UIGroup _group)
		{
			return m_list.Find(v => {
				return (v.ui.group == _group);
			});
		}
		public bool Has(string _strName)
		{
			return m_list.Exists(v => {
				return (v.ui.name == _strName);
			});
		}

		private int FindInsertPosition(UIGroup _group)
		{
			if (_group == UIGroup.None) { return -1; }

			int index = FindFrontIndexInGroup(_group);
			if (index > -1) { return index; }

			return FindInsertPosition(_group - 1);
		}

		private int FindFrontIndexInGroup(UIGroup _group)
		{
			return m_list.FindIndex(v => {
				return (v.ui.group == _group);
			});
		}
	}
}
