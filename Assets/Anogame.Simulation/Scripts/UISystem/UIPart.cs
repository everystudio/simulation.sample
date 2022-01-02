using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace anogamelib
{
	public class UIPart
	{
		public class SE
		{
			public string m_strSoundName = "";
		}

		private Animator[] m_animatorArr;
		public Animator[] animatorArr { set { m_animatorArr = value; } }

		private int m_iPlayCount = 0;
		private Action m_onFinishedCallback = null;

		public Transform root { get; set; }
		private readonly string m_strPrefabPath;
		public string prefabPath { get { return m_strPrefabPath; } }

		private bool m_bExit;

		public UIPart(Transform _tfRoot)
		{
			root = _tfRoot;
		}
		public UIPart(string _strPath)
		{
			m_strPrefabPath = _strPath;
		}
		public virtual void Destroy()
		{
			if( root != null)
			{
				root.SetParent(null);
				GameObject.Destroy(root.gameObject);
				root = null;
			}
			m_animatorArr = null;
			m_onFinishedCallback = null;
		}

		public bool PlayAnimations(string _strName , Action _callback = null , bool _bExit = false)
		{
			if( 0 < m_iPlayCount)
			{
				return false;
			}
			m_bExit = _bExit;

			int iCount = Play(_strName);
			if( iCount <= 0)
			{
				return false;
			}
			if( _callback != null)
			{
				m_iPlayCount = iCount;
				m_onFinishedCallback = _callback;
			}
			return true;
		}

		private int Play(string _strName)
		{
			string strPlayName = UIStateBehaviour.LayerName + _strName;

			int iCount = 0;
			for( int i = 0; i < m_animatorArr.Length; i++)
			{
				UIStateBehaviour[] states = m_animatorArr[i].GetBehaviours<UIStateBehaviour>();
				for( int j = 0; j < states.Length; j++)
				{
					states[j].ExitCallbackList = onExit;
					states[j].PlayName = strPlayName;
				}
				if( 0 < states.Length)
				{
					//Debug.Log(strPlayName);
					m_animatorArr[i].Play(strPlayName);
					iCount += 1;
				}
			}
			return iCount;
		}

		private void onExit(Animator _animator)
		{
			if (m_bExit)
			{
				_animator.enabled = false;
			}
			if (--m_iPlayCount <= 0)
			{
				if (m_onFinishedCallback != null)
				{
					m_onFinishedCallback();
				}
			}
		}
		#region Virtual Methods
		public virtual IEnumerator OnLoaded(UIBase _uiBase) { yield break; }
		public virtual bool OnClick(string _strName, GameObject _gameObject, PointerEventData _pointer, SE se) { return false; }
		public virtual bool OnTouchDown(string _strName, GameObject _gameObject, PointerEventData _pointer) { return false; }
		public virtual bool OnTouchUp(string _strName, GameObject _gameObject, PointerEventData _pointer) { return false; }
		public virtual bool OnDrag(string _strName, GameObject _gameObject, PointerEventData _pointer) { return false; }
		public virtual void OnDestroy() { }
		#endregion

	}
}


