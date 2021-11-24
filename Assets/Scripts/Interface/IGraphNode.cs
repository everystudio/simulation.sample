using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGraphNode 
{
	int GetDistance(IGraphNode _other);
}
