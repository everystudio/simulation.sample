using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace anogamelib
{
    public class UIVisibleController// : MonoBehaviour
    {
		private Dictionary<Component, bool> m_dictComponent = new Dictionary<Component, bool>();

		public void SetVisible(GameObject _target, bool _bEnable)
		{
			if (m_dictComponent == null) { return; }

			if (_bEnable)
			{
				if (IsVisible()) { return; }
				foreach (KeyValuePair<Component, bool> pair in m_dictComponent)
				{
					SetEnable(pair.Key, pair.Value);
				}
				m_dictComponent.Clear();
			}
			else
			{
				if (!IsVisible()) { return; }
				Component[] components = GetComponents(_target);
				for (int i = 0; i < components.Length; i++)
				{
					Component component = components[i];
					m_dictComponent.Add(component, IsEnable(component));
					SetEnable(component, false);
				}
			}
		}
		public void Destroy()
		{
			m_dictComponent = null;
		}
		public bool IsVisible()
		{
			return (m_dictComponent.Count == 0);
		}
		public virtual Component[] GetComponents(GameObject target) { return null; }
		public virtual void SetEnable(Component component, bool enable) { }
		public virtual bool IsEnable(Component component) { return true; }
	}

	public class UIBehaviourController<T> : UIVisibleController where T : Behaviour
	{
		public override Component[] GetComponents(GameObject _target)
		{
			return _target.GetComponentsInChildren<T>();
		}
		public override void SetEnable(Component _component, bool _bEnable)
		{
			(_component as T).enabled = _bEnable;
		}
		public override bool IsEnable(Component _component)
		{
			return (_component as T).enabled;
		}
	}
	public class UIRendererController : UIVisibleController
	{
		public override Component[] GetComponents(GameObject _target)
		{
			return _target.GetComponentsInChildren<Renderer>();
		}
		public override void SetEnable(Component _component, bool _bEnable)
		{
			(_component as Renderer).enabled = _bEnable;
		}
		public override bool IsEnable(Component _component)
		{
			return (_component as Renderer).enabled;
		}
	}
}



