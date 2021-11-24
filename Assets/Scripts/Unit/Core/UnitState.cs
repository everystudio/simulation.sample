using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitState
{
    protected UnitBase m_unit;

    public UnitState(UnitBase _unit)
    {
        m_unit = _unit;
    }

    public abstract void Apply();
    public abstract void MakeTransition(UnitState _state);
}