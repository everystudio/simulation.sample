using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
	public Tilemap m_tmField;
	private void Start()
	{
		//Debug.Log(m_tmField.cellBounds.size);
		//Debug.Log(m_tmField.cellBounds.position);

		for( int y = m_tmField.cellBounds.position.y; y < m_tmField.cellBounds.position.y+m_tmField.cellBounds.size.y; y++)
		{
			for( int x = m_tmField.cellBounds.position.x; x < m_tmField.cellBounds.position.x + m_tmField.cellBounds.size.x; x++)
			{
				Debug.Log(m_tmField.GetTile(new Vector3Int(x, y, 0)));
			}
		}
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 mouse_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector3Int grid = m_tmField.WorldToCell(mouse_position);
			if (m_tmField.HasTile(grid))
			{
				TileBase tile = m_tmField.GetTile(grid);
				Debug.Log(tile);
			}
			else
			{
				Debug.Log("nohit");
			}
		}
	}

}
