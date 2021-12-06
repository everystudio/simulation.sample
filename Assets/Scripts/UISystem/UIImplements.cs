using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace anogamelib
{
	public class PrefabReceiver
	{
		public UnityEngine.Object prefab;
	}
	public interface IPrefabLoader
	{
		IEnumerator Load(string _path, PrefabReceiver _receiver);
		void Release(string _path, UnityEngine.Object _prefab);
	}

	public interface ISounder
	{
		void PlayDefaultClickSE();
		void PlayClickSE(string _name);
		void PlayBGM(string _name);
		void StopBGM();
	}
	public interface IFadeCreator
	{
		UIFade Create();
	}
	public class UIImplements
	{
		private IPrefabLoader m_prefabLoader;
		public IPrefabLoader prefabLoader { get { return m_prefabLoader; } }

		private ISounder m_sounder;
		public ISounder sounder { get { return m_sounder; } }

		private IFadeCreator m_fadeCreator;
		public IFadeCreator fadeCreator { get { return m_fadeCreator; } }

		public UIImplements(IPrefabLoader _prefabLoader, ISounder _sounder, IFadeCreator _fadeCreator)
		{
			m_prefabLoader = _prefabLoader;
			m_sounder = _sounder;
			m_fadeCreator = _fadeCreator;
		}
	}
}