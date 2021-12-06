using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using anogamelib;

public class UIGame : MonoBehaviour
{
    void Start()
    {
        UIController.Instance.Implement(new PrefabLoaderResources(), null, new FadeBlackCurtain());
        UIController.Instance.AddFront(new UIInputWait());
    }
}
