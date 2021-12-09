using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="ScriptableObject/UnitData")]
public class UnitData : ScriptableObject
{
	public string UnitName;

	public Sprite UnitIcon;

	public int HitPoint;
	public int AttackRange;
	public int MovementPoint;
	public int ActionPoint;

	public bool Flying;
}
