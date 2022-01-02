using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace anogamelib
{
	public class UIPartContainer
	{
		protected UnityEngine.Object m_prefab;
		public UnityEngine.Object prefab { get { return m_prefab; } }

		private UIPart m_ui;
		public UIPart ui { get { return m_ui; } }

		private UITouchListener[] m_listeners = null;
		public UIPartContainer(UIPart _ui)
		{
			m_ui = _ui;
		}

		public IEnumerator LoadAndSetup(UIBaseLayer _layer)
		{
			if (m_ui.root == null && !string.IsNullOrEmpty(m_ui.prefabPath))
			{
				PrefabReceiver receiver = new PrefabReceiver();

				yield return UIController.implements.prefabLoader.Load(m_ui.prefabPath, receiver);

				m_prefab = receiver.prefab;
				if (m_prefab != null)
				{
					GameObject g = GameObject.Instantiate(m_prefab) as GameObject;
					m_ui.root = g.transform;
				}
			}
			if (m_ui.root == null)
			{
				m_ui.root = new GameObject("root").transform;
			}
			m_ui.root.gameObject.SetActive(false);
			CollectComponents(m_ui.root.gameObject, _layer);

			yield return m_ui.OnLoaded((UIBase)_layer.m_ui);

			m_ui.root.gameObject.SetActive(true);
		}
		public virtual void Destroy()
		{
			UIController.implements.prefabLoader.Release(m_ui.prefabPath, m_prefab);
			m_prefab = null;
			m_ui.Destroy();
			m_ui = null;
			for (int i = 0; i < m_listeners.Length; i++)
			{
				m_listeners[i].ResetUI();
			}
			m_listeners = null;
		}

		protected void CollectComponents(GameObject _target, UIBaseLayer _layer)
		{
			m_listeners = _target.GetComponentsInChildren<UITouchListener>();
			for (int i = 0; i < m_listeners.Length; i++)
			{
				m_listeners[i].SetUI(_layer, m_ui);
			}

			Animator[] animators = _target.GetComponentsInChildren<Animator>();
			m_ui.animatorArr = animators;
		}
	}
}