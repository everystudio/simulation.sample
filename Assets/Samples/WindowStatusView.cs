using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace anogame_strategy
{
	public class WindowStatusView : MonoBehaviour
	{
		[SerializeField] private Text m_txtName;
		[SerializeField] private Image m_imgIcon;
		[SerializeField] private Text m_txtHP;
		[SerializeField] private Slider m_slHP;
		[SerializeField] private Text m_txtATK;
		[SerializeField] private Slider m_slATK;
		[SerializeField] private Text m_txtDEF;
		[SerializeField] private Slider m_slDEF;
		[SerializeField] private Text m_txtMovement;
		[SerializeField] private Text m_txtRange;

		public void Initialize(UnitBase _unitBase)
		{
			m_txtName.text = _unitBase.m_unitData.UnitName;
			m_imgIcon.sprite = _unitBase.m_unitData.UnitIcon;
			m_txtHP.text = $"{_unitBase.CurrentHitPoints}/{_unitBase.m_unitData.HitPoint}";
			m_slHP.maxValue = _unitBase.m_unitData.HitPoint;
			m_slHP.value = _unitBase.CurrentHitPoints;

			m_txtATK.text = _unitBase.m_unitData.Attack.ToString();
			m_slATK.maxValue = 50;
			m_slATK.value = _unitBase.m_unitData.Attack;

			m_txtDEF.text = _unitBase.m_unitData.Defence.ToString();
			m_slDEF.maxValue = 50;
			m_slDEF.value = _unitBase.m_unitData.Defence;

			m_txtMovement.text = _unitBase.m_unitData.MovementPoint.ToString();
			m_txtRange.text = _unitBase.m_unitData.AttackRange.ToString();

		}

	}
}