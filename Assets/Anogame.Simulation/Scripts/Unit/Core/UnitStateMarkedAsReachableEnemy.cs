using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace anogame_strategy
{
    public class UnitStateMarkedAsReachableEnemy : UnitState
    {
        public UnitStateMarkedAsReachableEnemy(UnitBase unit) : base(unit)
        {
        }

        public override void Apply()
        {
            m_unit.MarkAsReachableEnemy();
        }

        public override void MakeTransition(UnitState state)
        {
            state.Apply();
            m_unit.UnitState = state;
        }
    }

}
