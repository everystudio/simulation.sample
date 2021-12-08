using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBase : MonoBehaviour
{
    public int PlayerNumber;
    public abstract void Play(StrategyBase _gameManager);
}