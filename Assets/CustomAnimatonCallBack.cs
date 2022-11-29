using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomAnimatonCallBack : MonoBehaviour
{
	[SerializeField] Image StartButton;
	[SerializeField] Image LoadingIMage;

	public void EnableStartButtonRaycast()
	{
		StartButton.raycastTarget = true;
		LoadingIMage.gameObject.SetActive(false);
	}
}
