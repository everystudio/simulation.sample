using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowMapGrid : MonoBehaviour
{
    public Transform m_gridTransform;
    public LineRenderer m_lineRenderer;
    void Start()
    {
        m_lineRenderer.positionCount = 10 * 2;
        for ( int i = 0; i < 10; i++)
		{
            bool bUp = i % 2 == 0;
            float fHeightBase = 10f;
            float fHeight1 = bUp ? fHeightBase : -1f * fHeightBase;
            float fHeight2 =!bUp ? fHeightBase : -1f * fHeightBase;

            m_lineRenderer.SetPosition(i * 2, new Vector3(i, fHeight1,-1f));
            m_lineRenderer.SetPosition(i * 2+1, new Vector3(i, fHeight2, -1f));
        }
    }

}
