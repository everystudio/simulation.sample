using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
	public Tilemap m_tmField;
	public Tilemap m_tmPlayerPosition;
	private void Start()
	{
		/*
		for( int y = m_tmField.cellBounds.position.y; y < m_tmField.cellBounds.position.y+m_tmField.cellBounds.size.y; y++)
		{
			for( int x = m_tmField.cellBounds.position.x; x < m_tmField.cellBounds.position.x + m_tmField.cellBounds.size.x; x++)
			{
				Debug.Log(m_tmField.GetTile(new Vector3Int(x, y, 0)));
			}
		}
		*/
		/*
		List<Vector3Int> positionarr = GetPlayerStartPositions();
		foreach(Vector3Int startTilePos in positionarr)
		{
			//Debug.Log(startTilePos.ToString());
		}
		*/
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
				//Debug.Log(tile);
			}
			else
			{
				//Debug.Log("nohit");
			}
		}
	}
	public List<Vector3Int> GetPlayerStartPositions()
	{
		TileBase[] baseArr = m_tmPlayerPosition.GetTilesBlock(m_tmPlayerPosition.cellBounds);
		List<Vector3Int> ret = new List<Vector3Int>();

		foreach( Vector3Int pos in m_tmPlayerPosition.cellBounds.allPositionsWithin)
		{
			UnitStartTile st = m_tmPlayerPosition.GetTile<UnitStartTile>(pos);
			if (st != null)
			{
				ret.Add(pos);
			}
		}
		return ret;
	}
}
