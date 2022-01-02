using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace anogame_strategy
{
    public abstract class PlayerBase : MonoBehaviour
    {
        public int PlayerNumber;
        public abstract void Play(StrategyBase _gameManager);
    }
}