using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using anogamelib;

public class UnitStatusView : UIBase
{
	public UnitStatusView(UnitBase _unit):base("UI/UnitStatusView",UIGroup.Dialog,
		UIPreset.BackVisible | UIPreset.BackTouchable)
	{

	}
}
