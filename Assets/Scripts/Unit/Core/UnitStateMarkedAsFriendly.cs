using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStateMarkedAsFriendly : UnitState
{
    public UnitStateMarkedAsFriendly(UnitBase _unit) : base(_unit)
    {
    }

    public override void Apply()
    {
        m_unit.MarkAsFriendly();
    }

    public override void MakeTransition(UnitState _state)
    {
        _state.Apply();
        m_unit.UnitState = _state;
    }
}
