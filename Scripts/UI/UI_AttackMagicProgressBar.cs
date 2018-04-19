using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UI_AttackMagicProgressBar : MonoBehaviour
{
	private static List<UI_AttackMagicProgressBar>	sm_lProgressBars		= new List<UI_AttackMagicProgressBar>();

	private Player_Base m_pPlayerReference = null;
	//private UISlider	m_pSliderReference = null;
	private int			m_iCooldownTimerID = 0;

	void Start()
	{
		m_pPlayerReference = ImportantObjectsManager.Player.GetComponent<Player_Base>();
		//m_pSliderReference = GetComponent<UISlider>();
		m_iCooldownTimerID = sm_lProgressBars.Count;

		//AddCooldownProgressBar(this);
	}

	// Update is called once per frame
	void Update()
	{
		//m_pSliderReference.value = m_pPlayerReference.GetCooldownPercentage(m_iCooldownTimerID);
	}
}
