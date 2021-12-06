using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : UnitBase
{
	public override void MarkAsAttacking(UnitBase _target)
	{
		throw new System.NotImplementedException();
	}

	public override void MarkAsDefending(UnitBase _aggressor)
	{
		throw new System.NotImplementedException();
	}

	public override void MarkAsDestroyed()
	{
		throw new System.NotImplementedException();
	}

	public override void MarkAsFinished()
	{
		//SetColor(Color.gray);
		//SetHighlighterColor(new Color(0.75f, 0.75f, 0.75f, 0.5f));
		//Debug.Log($"MarkAsFinished:{gameObject.name}");
	}

	public override void MarkAsFriendly()
	{
		//Debug.Log("unit.markasfriendly");
	}

	public override void MarkAsReachable()
	{
		Debug.Log("unit.MarkAsReachable");
	}

	public override void MarkAsReachableEnemy()
	{
		Debug.Log($"MarkAsReachableEnemy:{gameObject.name}");
	}

	public override void MarkAsSelected()
	{
		//Debug.Log($"MarkAsSelected:{gameObject.name}");
	}

	public override void UnMark()
	{
		//Debug.Log("unit.unmark");
	}


}
