using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondAnimationManager : MonoBehaviour
{
	[SerializeField] Vector2 startJourney;
	[SerializeField] Vector2 endofJourney;

	[SerializeField] float animationTime;

	public void StartAnimation(float time, Vector2 startPosition,  Vector2 endposition)
	{
		transform.position = startPosition;

		animationTime = time;
		endofJourney = endposition;

		gameObject.SetActive(true);

		StartCoroutine(LerpPosition(endofJourney, animationTime));
	}

	IEnumerator LerpPosition(Vector2 targetPosition, float duration)
	{
		float time = 0;

		Vector3 t = transform.position;

		transform.position = new Vector3(t.x + Random.value * 750f - 300f, t.y + Random.value * 750f - 300f, t.z);

		Vector2 startPosition = transform.position;

		while(time < duration)
		{
			transform.position = Vector2.Lerp(startPosition, targetPosition, time / duration);
			time += Time.deltaTime;
			yield return null;
		}

		transform.position = targetPosition;

		gameObject.SetActive(false);

		LevelCompleteScreen.DiamondFinishedAnimation.Invoke();
	}
}
