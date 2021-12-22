using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using anogamelib;

namespace anogame_strategy
{
    public class UIGame : MonoBehaviour
    {
        void Start()
        {
            StrategyBase strategy = GameObject.FindObjectOfType<StrategyBase>();
            UIController.Instance.Implement(new PrefabLoaderResources(), null, new FadeBlackCurtain());
            UIController.Instance.AddFront(new UIInputWait(strategy));
        }
    }
}