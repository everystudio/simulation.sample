using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace anogame_strategy
{
    public class UnitStateMarkedAsFinished : UnitState
    {
        public UnitStateMarkedAsFinished(UnitBase unit) : base(unit)
        {
        }

        public override void Apply()
        {
            m_unit.MarkAsFinished();
        }

        public override void MakeTransition(UnitState state)
        {
            if (state is UnitStateNormal)
            {
                state.Apply();
                m_unit.UnitState = state;
            }
        }
    }
}