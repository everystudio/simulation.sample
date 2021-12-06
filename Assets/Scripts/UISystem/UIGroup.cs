using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace anogamelib
{
	public enum UIGroup
	{
		None = 0,
		View3D,
		MainScene,
		Scene,
		Floater,
		Dialog,
		Debug,
		SystemFade,
		System,
	}
	class UIBackable
	{
		public static readonly List<UIGroup> groupList = new List<UIGroup>(){
			UIGroup.Dialog,
			UIGroup.Scene,
			UIGroup.MainScene,
			UIGroup.View3D,
		};
		public static void Sort()
		{
			groupList.Sort((x, y) => { return y - x; });
		}
	}

	class UIFadeTarget
	{
		public static readonly List<UIGroup> groups = new List<UIGroup>(){
			UIGroup.Floater,
			UIGroup.MainScene,
			UIGroup.View3D,
		};
	}
	class UIFadeThreshold
	{
		public static readonly Dictionary<UIGroup, int> groups = new Dictionary<UIGroup, int>(){
			{ UIGroup.Scene, 1 },
		};
	}

}


