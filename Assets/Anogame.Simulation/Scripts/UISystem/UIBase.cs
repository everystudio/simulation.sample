using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace anogamelib
{
	public enum UIPreset
	{
		None = 0,

		BackVisible = (1 << 0),
		BackTouchable = (1 << 1),
		TouchEventCallable = (1 << 2),
		SystemUntouchable = (1 << 3),
		LoadingWithoutFade = (1 << 4),
		ActiveWituoutFade = (1 << 5),
		View3D = (1 << 6),

		SystemIndicator = (BackVisible | BackTouchable | SystemUntouchable | LoadingWithoutFade | ActiveWituoutFade),
	}

	public class UIBase : UIPart
    {
		private List<UIVisibleController> m_visibleControllerList = new List<UIVisibleController>();
		public List<UIVisibleController> visibleControllerList
		{
			get { return m_visibleControllerList; }
		}

		private bool m_scheduleUpdate = false;
		public bool scheduleUpdate
		{
			get { return m_scheduleUpdate; }
			set { m_scheduleUpdate = value; }
		}

		readonly UIGroup m_group;
		public UIGroup group { get { return m_group; } }

		readonly UIPreset m_preset;
		public UIPreset preset { get { return m_preset; } }

		readonly string m_strBgmName;
		public string BgmName { get { return m_strBgmName; } }


		public UIBase(string _strPrefabPath, UIGroup _group, UIPreset _preset = UIPreset.None, string strBgmName = "") : base(_strPrefabPath)
		{
			m_group = _group;
			m_preset = _preset;
			m_strBgmName = strBgmName;

			if (View3D())
			{
				AddRendererController();
			}
			else
			{
				AddVisibleBehaviourController<Graphic>();
			}
		}
		public string name { get { return GetType().Name; } }

		public bool BackVisible() { return (m_preset & UIPreset.BackVisible) > 0; }
		public bool BackTouchable() { return (m_preset & UIPreset.BackTouchable) > 0; }
		public bool TouchEventCallable() { return (m_preset & UIPreset.TouchEventCallable) > 0; }
		public bool SystemUntouchable() { return (m_preset & UIPreset.SystemUntouchable) > 0; }
		public bool LoadingWithoutFade() { return (m_preset & UIPreset.LoadingWithoutFade) > 0; }
		public bool ActiveWituoutFade() { return (m_preset & UIPreset.ActiveWituoutFade) > 0; }
		public bool View3D() { return (m_preset & UIPreset.View3D) > 0; }

		public override void Destroy()
		{
			base.Destroy();
			for (int i = 0; i < m_visibleControllerList.Count; i++)
			{
				m_visibleControllerList[i].Destroy();
			}
		}

		protected void AddRendererController()
		{
			m_visibleControllerList.Add(new UIRendererController());
		}

		protected void AddVisibleBehaviourController<T>() where T : Behaviour
		{
			m_visibleControllerList.Add(new UIBehaviourController<T>());
		}
		public virtual IEnumerator OnLoaded() { yield break; }

		public virtual void OnUpdate() { }
		public virtual void OnLateUpdate() { }
		public virtual void OnRevisible() { }
		public virtual void OnRetouchable() { }
		public virtual void OnActive() { }
		public virtual void OnDispatchedEvent(string name, object param) { }
		public virtual bool OnBack() { return true; }
		public virtual void OnSwitchFrontUI(string uiName) { }
		public virtual void OnSwitchBackUI(string uiName) { }
	}
}


