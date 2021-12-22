using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraTargetController : MonoBehaviour
{
	public CinemachineVirtualCamera m_virtualCamera;
	public Vector3 mousePos;

	private Vector2 m_screenSize;
	private void Start()
	{
		m_screenSize = new Vector2(Screen.width, Screen.height);
	}
	private void Update()
	{
		mousePos = Input.mousePosition;

		bool bMove = false;
		if( mousePos.x / m_screenSize.x < 0.1f || 0.9f < mousePos.x / m_screenSize.x)
		{
			bMove = true;
		}
		else if( mousePos.y / m_screenSize.y < 0.1f || 0.9f < mousePos.y / m_screenSize.y )
		{
			bMove = true;
		}
		Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
		transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);

		//m_virtualCamera.Follow = bMove?transform:null;
	}
}
