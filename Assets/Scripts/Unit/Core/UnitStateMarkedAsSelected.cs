using System.Collections.Generic;
using UnityEngine;

public class UnitStateMarkedAsSelected : UnitState
{
    public UnitStateMarkedAsSelected(UnitBase _unit) : base(_unit)
    {
    }

    public override void Apply()
    {
        m_unit.MarkAsSelected();
    }

    public override void MakeTransition(UnitState _state)
    {
        _state.Apply();
        m_unit.UnitState = _state;
    }
}
