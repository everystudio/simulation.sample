using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace anogamelib {
    public class UIStateBehaviour : StateMachineBehaviour
    {
        public static readonly string LayerName = "UI.";

        private Action<Animator> m_exitCallbackList;
        public Action<Animator> ExitCallbackList { set { m_exitCallbackList = value; } }

        public string PlayName { private get; set; }
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            AnimatorClipInfo[] infos = animator.GetCurrentAnimatorClipInfo(layerIndex);
            if( stateInfo.IsName(PlayName) && infos.Length == 0 && m_exitCallbackList != null)
            {
                m_exitCallbackList(animator);
                m_exitCallbackList = null;
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            AnimatorClipInfo[] infos = animator.GetCurrentAnimatorClipInfo(layerIndex);
            if(stateInfo.IsName(PlayName) /*&& infos.Length == 0*/ && m_exitCallbackList != null){
                m_exitCallbackList(animator);
                m_exitCallbackList = null;
            }
        }
    }
}

