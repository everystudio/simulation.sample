using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace anogame_strategy
{
	public interface IGraphNode
	{
		int GetDistance(IGraphNode _other);
	}
}
