using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace anogamelib
{
	public class FadeBlackCurtain : IFadeCreator
	{
		public UIFade Create()
		{
			return new UIFade("UI/FadeBlackCurtain");
		}
	}
}
