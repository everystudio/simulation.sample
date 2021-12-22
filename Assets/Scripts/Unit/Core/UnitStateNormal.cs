using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace anogame_strategy
{
    public class UnitStateNormal : UnitState
    {
        public UnitStateNormal(UnitBase _unit) : base(_unit)
        {
        }

        public override void Apply()
        {
            m_unit.UnMark();
        }

        public override void MakeTransition(UnitState _state)
        {
            _state.Apply();
            m_unit.UnitState = _state;
        }
    }
}