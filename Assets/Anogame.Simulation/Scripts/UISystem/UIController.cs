using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace anogamelib
{
    public class UIController : MonoBehaviour
    {
        public readonly static string LayerTouchAreaName = "LayoutTouchArea";

        public GameObject[] m_raycasterArr;
        public Transform m_uiLayer;
        public Transform m_view3D;

        private List<BaseRaycaster> m_raycasterComponentList = new List<BaseRaycaster>();

        private List<UIBaseLayer> m_addingList = new List<UIBaseLayer>();
        private List<UIBaseLayer> m_removingList = new List<UIBaseLayer>();
        private UIBaseLayerList m_uiList = new UIBaseLayerList();

		private Queue<TouchEvent> m_touchEvents = new Queue<TouchEvent>();
		private Queue<DispatchedEvent> m_dispatchedEvents = new Queue<DispatchedEvent>();
		private int m_touchOffCount;

		private UIBaseLayer m_uiFade;

		private UIImplements m_implements;
		public static UIImplements implements { get { return m_instance.m_implements; } }
		public void Implement(IPrefabLoader _prefabLoader, ISounder _sounder, IFadeCreator _fadeCreator)
		{
			m_instance.m_implements = new UIImplements(_prefabLoader, _sounder, _fadeCreator);
		}

		private static UIController m_instance;
		public static UIController Instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = GameObject.Find("ANOCanvas").GetComponent<UIController>();

					for (int i = 0; i < m_instance.m_raycasterArr.Length; i++)
					{
						BaseRaycaster raycaster = m_instance.m_raycasterArr[i].GetComponent<BaseRaycaster>();
						m_instance.m_raycasterComponentList.Add(raycaster);
					}

					UIBackable.Sort();
				}
				return m_instance;
			}
		}

		public void AddFront(UIBase _ui)
		{
			if (_ui == null) { return; }
			UIBaseLayer layer = new UIBaseLayer(_ui, m_uiLayer);
			//Debug.Log(layer.ui.group);
			//Debug.Log(_ui.name);
			//Debug.Log(layer.ui.LoadingWithoutFade());
			if (layer.ui.LoadingWithoutFade())
			{
				StartCoroutine(layer.Load());
			}
			if (ShouldFadeByAdding(_ui))
			{
				//Debug.Log("fadein");
				FadeIn();
			}
			m_addingList.Add(layer);
			m_uiList.Insert(layer);
		}

		public void Remove(UIBase _ui)
		{
			if (_ui == null) { return; }

			UIBaseLayer layer = m_uiList.Find(_ui);
			if (layer != null && layer.Inactive())
			{
				m_removingList.Add(layer);
			}

			if (ShouldFadeByRemoving(_ui))
			{
				FadeIn();
			}
		}

		public void Replace(UIBase[] _uiArr, UIGroup[] _removeGroups = null)
		{
			HashSet<UIGroup> removes = (_removeGroups == null) ? new HashSet<UIGroup>() : new HashSet<UIGroup>(_removeGroups);
			for (int i = 0; i < _uiArr.Length; i++)
			{
				removes.Add(_uiArr[i].group);
			}
			foreach (UIGroup group in removes)
			{
				List<UIBaseLayer> layers = m_uiList.FindLayers(group);
				for (int i = 0; i < layers.Count; i++)
				{
					Remove(layers[i].ui);
				}
			}
			for (int i = 0; i < _uiArr.Length; i++)
			{
				AddFront(_uiArr[i]);
			}
		}

		public void ListenTouch(UITouchListener _listener, UITouchType _type, PointerEventData _pointer)
		{
			if (_listener.layer == null) { return; }
			m_touchEvents.Enqueue(new TouchEvent(_listener, _type, _pointer));
		}

		public void Dispatch(string name, object param)
		{
			m_dispatchedEvents.Enqueue(new DispatchedEvent(name, param));
		}

		public void Back()
		{
			UIBaseLayer layer = null;
			for (int i = 0; i < UIBackable.groupList.Count; i++)
			{
				layer = m_uiList.FindFrontLayerInGroup(UIBackable.groupList[i]);
				if (layer != null) { break; }
			}
			if (layer == null) { return; }

			bool ret = layer.ui.OnBack();
			if (ret)
			{
				Remove(layer.ui);
			}
		}
		public IEnumerator YieldAttachParts(UIBase _uiBase, List<UIPart> _parts)
		{
			UIBaseLayer layer = m_uiList.Find(_uiBase);
			if (layer == null) { yield break; }
			yield return layer.AttachParts(_parts);
		}

		public void AttachParts(UIBase _uiBase, List<UIPart> _parts)
		{
			UIBaseLayer layer = m_uiList.Find(_uiBase);
			if (layer == null) { return; }
			StartCoroutine(layer.AttachParts(_parts));
		}

		public void DetachParts(UIBase _uiBase, List<UIPart> _parts)
		{
			UIBaseLayer layer = m_uiList.Find(_uiBase);
			if (layer == null) { return; }
			layer.DetachParts(_parts);
		}

		public void SetScreenTouchable(UIBase _uiBase, bool _bEnable)
		{
			UIBaseLayer layer = m_uiList.Find(_uiBase);
			if (layer == null) { return; }
			SetScreenTouchableByLayer(layer, _bEnable);
		}

		public void SetScreenTouchableByLayer(UIBaseLayer _layer, bool _bEnable)
		{
			if (_layer == null) { return; }

			if (_bEnable)
			{
				if (m_touchOffCount <= 0) { return; }
				--m_touchOffCount;
				--_layer.screenTouchOffCount;
				if (m_touchOffCount == 0)
				{
					for (int i = 0; i < m_raycasterComponentList.Count; i++)
					{
						m_raycasterComponentList[i].enabled = true;
					}
				}
			}
			else
			{
				if (m_touchOffCount == 0)
				{
					for (int i = 0; i < m_raycasterComponentList.Count; i++)
					{
						m_raycasterComponentList[i].enabled = false;
					}
				}
				++m_touchOffCount;
				++_layer.screenTouchOffCount;
			}
		}

		public bool HasUI(string _name)
		{
			return m_uiList.Has(_name);
		}

		public string GetFrontUINameInGroup(UIGroup _group)
		{
			UIBaseLayer layer = m_uiList.FindFrontLayerInGroup(_group);
			if (layer == null)
			{
				return "";
			}
			else
			{
				return layer.ui.name;
			}
		}

		public int GetUINumInGroup(UIGroup _group)
		{
			return m_uiList.GetNumInGroup(_group);
		}

		private void Update()
		{
			m_uiList.ForEachOnlyActive(layer => {
				if (layer.ui.scheduleUpdate)
				{
					layer.ui.OnUpdate();
				}
			});

			RunTouchEvents();
			RunDispatchedEvents();

			bool isInsert = Insert();
			bool isEject = Eject();
			if (isEject || isInsert)
			{
				RefreshLayer();

				if (isEject && IsHidden())
				{
					Unload();
				}
				if (m_addingList.Count == 0 && m_removingList.Count == 0)
				{
					PlayBGM();
					FadeOut();
				}
			}
		}

		private void LateUpdate()
		{
			m_uiList.ForEachOnlyActive(layer => {
				if (layer.ui.scheduleUpdate)
				{
					layer.ui.OnLateUpdate();
				}
			});
		}

		private void OnDestroy()
		{
			UIController.m_instance = null;
		}


		private bool Insert()
		{
			bool isInsert = false;
			if (m_addingList.Count <= 0) { return isInsert; }
			List<UIBaseLayer> list = m_addingList;
			m_addingList = new List<UIBaseLayer>();
			bool isFadeIn = IsFadeIn();
			//Debug.Log(isFadeIn);
			for (int i = 0; i < list.Count; i++)
			{
				UIBaseLayer layer = list[i];
				if (!isFadeIn && layer.state == UIState.InFading)
				{
					StartCoroutine(layer.Load());
				}
				if (layer.IsNotYetLoaded() || (isFadeIn && !layer.ui.ActiveWituoutFade()))
				{
					m_addingList.Add(layer);
					continue;
				}
				if (layer.Activate())
				{
					isInsert = true;
				}
			}
			return isInsert;
		}


		private bool Eject()
		{
			bool isEject = false;

			if (m_removingList.Count <= 0) { return isEject; }

			bool isLoading = m_addingList.Exists(layer => {
				return (layer.IsNotYetLoaded());
			});

			List<UIBaseLayer> list = m_removingList;
			m_removingList = new List<UIBaseLayer>();
			bool isFadeIn = IsFadeIn();
			for (int i = 0; i < list.Count; i++)
			{
				UIBaseLayer layer = list[i];
				if (!isFadeIn && layer.state == UIState.OutFading)
				{
					layer.Remove();
				}

				if (layer.state != UIState.Removing || isLoading)
				{
					m_removingList.Add(layer);
					continue;
				}

				m_uiList.Eject(layer);
				layer.Destroy();
				isEject = true;
			}
			return isEject;
		}


		private void RefreshLayer()
		{
			bool visible = true;
			bool touchable = true;
			UIBaseLayer frontLayer = null;
			int index = m_uiLayer.childCount - 1;
			m_uiList.ForEachAnything(layer => {
				if (layer.IsNotYetLoaded()) { return; }

				bool preVisible = layer.IsVisible();
				bool preTouchable = layer.IsTouchable();
				layer.SetVisible(visible);
				layer.SetTouchable(touchable);
				if (!preVisible && visible) { layer.ui.OnRevisible(); }
				if (!preTouchable && touchable) { layer.ui.OnRetouchable(); }

				visible = visible && layer.ui.BackVisible();
				touchable = touchable && layer.ui.BackTouchable();

				layer.siblingIndex = index--;

				if (frontLayer != null)
				{
					frontLayer.back = layer;
					frontLayer.CallSwitchBack();
				}
				layer.front = frontLayer;
				layer.CallSwitchFront();

				layer.back = null;
				frontLayer = layer;
			});
		}

		private void RunTouchEvents()
		{
			if (m_touchEvents.Count == 0) { return; }

			bool ret = false;
			int untouchableIndex = FindUntouchableIndex();

			Queue<TouchEvent> queue = new Queue<TouchEvent>(m_touchEvents);
			m_touchEvents.Clear();

			while (queue.Count > 0)
			{
				TouchEvent touch = queue.Dequeue();

				if (ret) { continue; }
				if (touch.listener.layer == null) { continue; }

				bool touchable = true;
				touchable = touchable && IsScreentouchable();
				touchable = touchable && touch.listener.layer.IsTouchable();
				touchable = touchable && untouchableIndex < touch.listener.layer.siblingIndex;
				if (!touchable) { continue; }

				UIPart ui = touch.listener.ui;
				switch (touch.type)
				{
					case UITouchType.Click:
						UIPart.SE se = new UIPart.SE();
						ret = ui.OnClick(touch.listener.gameObject.name, touch.listener.gameObject, touch.pointer, se);
						if (ret && m_implements.sounder != null)
						{
							if (!string.IsNullOrEmpty(se.m_strSoundName))
							{
								m_implements.sounder.PlayClickSE(se.m_strSoundName);
							}
							else
							{
								m_implements.sounder.PlayDefaultClickSE();
							}
						}
						break;

					case UITouchType.Down:
						ret = ui.OnTouchDown(touch.listener.gameObject.name, touch.listener.gameObject, touch.pointer);
						break;

					case UITouchType.Up:
						ret = ui.OnTouchUp(touch.listener.gameObject.name, touch.listener.gameObject, touch.pointer);
						break;

					case UITouchType.Drag:
						ret = ui.OnDrag(touch.listener.gameObject.name, touch.listener.gameObject, touch.pointer);
						break;

					default: break;
				}
			}
		}

		private void RunDispatchedEvents()
		{
			if (m_dispatchedEvents.Count == 0) { return; }

			Queue<DispatchedEvent> queue = new Queue<DispatchedEvent>(m_dispatchedEvents);
			m_dispatchedEvents.Clear();

			while (queue.Count > 0)
			{
				DispatchedEvent e = queue.Dequeue();
				m_uiList.ForEachOnlyActive(layer => {
					layer.ui.OnDispatchedEvent(e.name, e.param);
				});
			}
		}

		private bool ShouldFadeByAdding(UIBase _ui)
		{
			if (m_uiFade != null) { return false; }
			if (UIFadeTarget.groups.Contains(_ui.group)) { return true; }

			bool has = UIFadeThreshold.groups.ContainsKey(_ui.group);
			if (has && m_uiList.GetNumInGroup(_ui.group) <= UIFadeThreshold.groups[_ui.group])
			{
				return true;
			}
			return false;
		}
		private bool ShouldFadeByRemoving(UIBase _ui)
		{
			if (m_uiFade != null) { return false; }
			if (UIFadeTarget.groups.Contains(_ui.group)) { return true; }

			bool has = UIFadeThreshold.groups.ContainsKey(_ui.group);
			if (has)
			{
				int sceneNum = UIBaseLayerList.GetNumInGroup(_ui.group, m_removingList);
				if (m_uiList.GetNumInGroup(_ui.group) - sceneNum <= UIFadeThreshold.groups[_ui.group])
				{
					return true;
				}
			}
			return false;
		}

		private void FadeIn()
		{
			if (m_uiFade != null) { return; }

			if (m_implements.fadeCreator == null) { return; }
			//Debug.Log("fadein2");
			UIFade fade = m_implements.fadeCreator.Create();
			AddFront(fade);
			m_uiFade = m_addingList.Find(v => { return v.ui == fade; });
		}

		private void FadeOut()
		{
			//Debug.Log("fadeout1");
			if (m_uiFade == null) { return; }
			//Debug.Log("fadeout2");
			Remove(m_uiFade.ui);
			m_uiFade = null;
		}
		private bool IsFadeIn()
		{
			return (m_uiFade != null && m_uiFade.state <= UIState.InAnimation);
		}

		private bool IsHidden()
		{
			return (m_uiFade != null && m_uiFade.state == UIState.Active);
		}

		private bool IsScreentouchable()
		{
			if (m_raycasterComponentList.Count == 0) { return false; }

			return m_raycasterComponentList[0].enabled;
		}

		private int FindUntouchableIndex()
		{
			int index = -1;
			m_uiList.ForEachOnlyActive(layer => {
				if (index >= 0) { return; }
				if (layer.ui.SystemUntouchable())
				{
					index = layer.siblingIndex - 1;
				}
			});
			return index;
		}

		private void Unload()
		{
			System.GC.Collect();
			Resources.UnloadUnusedAssets();
		}

		private void PlayBGM()
		{
			if (m_implements.sounder == null) { return; }

			string bgm = "";
			m_uiList.ForEachAnything(v => {
				if (!StateFlags.map[v.state].visible) { return; }
				if (!string.IsNullOrEmpty(bgm)) { return; }
				if (!string.IsNullOrEmpty(v.ui.BgmName))
				{
					bgm = v.ui.BgmName;
				}
			});

			if (string.IsNullOrEmpty(bgm))
			{
				m_implements.sounder.StopBGM();
			}
			else
			{
				m_implements.sounder.PlayBGM(bgm);
			}
		}

	}
}
