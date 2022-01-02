using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace anogamelib
{
    public enum UITouchType
    {
        None = 0,
        Click,
        Down,
        Up,
        Drag,
    }


    public class UITouchListener : MonoBehaviour
    , IPointerClickHandler
    , IPointerDownHandler
    , IPointerUpHandler
    , IDragHandler
    {
        private UIBaseLayer m_layer;
        public UIBaseLayer layer { get { return m_layer; } }

        private UIPart m_ui = null;
        public UIPart ui { get { return m_ui; } }

        private int m_generation = int.MaxValue;

        public void SetUI(UIBaseLayer _layer, UIPart _ui)
        {
            int generation = GetGeneration(transform, _ui.root);
            if (m_generation < generation)
            {
                return;
            }

            m_layer = _layer;
            m_ui = _ui;
            m_generation = generation;
        }
        public void ResetUI()
        {
            m_layer = null;
            m_ui = null;
            m_generation = int.MaxValue;
        }


        public void OnPointerClick(PointerEventData pointer)
        {
            UIController.Instance.ListenTouch(this, UITouchType.Click, pointer);
        }

        public void OnPointerDown(PointerEventData pointer)
        {
            UIController.Instance.ListenTouch(this, UITouchType.Down, pointer);
        }

        public void OnPointerUp(PointerEventData pointer)
        {
            UIController.Instance.ListenTouch(this, UITouchType.Up, pointer);
        }

        public void OnDrag(PointerEventData pointer)
        {
            UIController.Instance.ListenTouch(this, UITouchType.Drag, pointer);
        }

        private int GetGeneration(Transform target, Transform dest, int generation = 0)
        {
            if (target == null || dest == null)
            {
                return -1;
            }
            else if (target == dest)
            {
                return generation;
            }
            else
            {
                return GetGeneration(target.parent, dest, generation + 1);
            }
        }
    }
}
