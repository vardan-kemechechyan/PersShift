using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour
{
	[SerializeField] List<GameObject> windEffects;

	public void PlayRandomWind()
	{
		//windEffects[Random.Range(0, windEffects.Count)].SetActive(true);
		if(windEffects[0].activeSelf)
		{
			windEffects[0].GetComponent<ParticleSystem>().Stop();
			windEffects[0].GetComponent<ParticleSystem>().Play();
		}
		else
			windEffects[0].SetActive(true);
	}

	public void DisableAllWinds()
	{
		foreach(var wind in windEffects)
		{
			wind.SetActive(false);
		}
	}
}
