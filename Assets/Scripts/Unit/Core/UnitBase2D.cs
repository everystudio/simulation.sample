using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase2D : UnitBase
{
	private Coroutine PulseCoroutine;

	private void SetColor(Color _color)
	{
		var _renderer = GetComponent<SpriteRenderer>();
		if (_renderer != null)
		{
			_renderer.color = _color;
		}
	}
	public override void OnUnitDeselected()
	{
		base.OnUnitDeselected();
		StopCoroutine(PulseCoroutine);
		transform.localScale = new Vector3(1, 1, 1);
	}
	public override void MarkAsAttacking(UnitBase _target)
	{
		StartCoroutine(Jerk(_target));
	}
	public override void MarkAsDefending(UnitBase _aggressor)
	{
		StartCoroutine(Glow(new Color(1, 0, 0, 0.5f), 1));
	}
	public override void MarkAsDestroyed()
	{
	}

	public override void MarkAsFinished()
	{
		SetColor(Color.gray);
	}

	public override void MarkAsFriendly()
	{
		SetColor(new Color(0.8f, 1, 0.8f));
	}

	public override void MarkAsReachableEnemy()
	{
		SetColor(new Color(1, 0.8f, 0.8f));
	}

	public override void MarkAsSelected()
	{
		PulseCoroutine = StartCoroutine(Pulse(1.0f, 0.5f, 1.25f));
		SetColor(new Color(0.8f, 0.8f, 1));
	}

	public override void UnMark()
	{
		SetColor(Color.white);
	}

	private IEnumerator Jerk(UnitBase other)
	{
		GetComponent<SpriteRenderer>().sortingOrder = 6;

		float fDuration = 0.25f;

		Vector3 startPos = transform.position;
		Vector3 dir = new Vector3(
			other.transform.position.x - transform.position.x,
			other.transform.position.y - transform.position.y, 0f);
		Vector3 targetPos = startPos + dir.normalized * 0.5f;

		float startTime = Time.time;
		while (Time.time < startTime + fDuration )
		{
			transform.position = Vector3.Lerp(startPos, targetPos, (Time.time - startTime ) / fDuration);
			yield return 0;
		}
		startTime = Time.time;
		while (Time.time < startTime + fDuration)
		{
			transform.position = Vector3.Lerp(targetPos, startPos, (Time.time - startTime ) / fDuration);
			yield return 0;
		}
		transform.position = new Vector3(
			CurrentTileInfo.transform.position.x,
			CurrentTileInfo.transform.position.y,
			transform.position.z);

		GetComponent<SpriteRenderer>().sortingOrder = 4;
	}

	private IEnumerator Glow(Color color, float cooloutTime)
	{
		Transform marker = transform.Find("Marker");

		if(marker == null)
		{
			yield break;
		}
		SpriteRenderer _renderer = marker.GetComponent<SpriteRenderer>();
		float startTime = Time.time;

		while (startTime + cooloutTime > Time.time)
		{
			if (_renderer != null)
			{
				_renderer.color = Color.Lerp(new Color(1, 1, 1, 0), color, (startTime + cooloutTime) - Time.time);
			}
			yield return 0;
		}

		_renderer.color = Color.clear;
	}
	private IEnumerator Pulse(float breakTime, float delay, float scaleFactor)
	{
		var baseScale = transform.localScale;
		while (true)
		{
			float growingTime = Time.time;
			while (growingTime + delay > Time.time)
			{
				transform.localScale = Vector3.Lerp(baseScale * scaleFactor, baseScale, (growingTime + delay) - Time.time);
				yield return 0;
			}

			float shrinkingTime = Time.time;
			while (shrinkingTime + delay > Time.time)
			{
				transform.localScale = Vector3.Lerp(baseScale, baseScale * scaleFactor, (shrinkingTime + delay) - Time.time);
				yield return 0;
			}

			yield return new WaitForSeconds(breakTime);
		}
	}


}
