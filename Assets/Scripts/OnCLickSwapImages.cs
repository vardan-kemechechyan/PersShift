using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnCLickSwapImages : MonoBehaviour
{
	[SerializeField] Sprite state1;
	[SerializeField] Sprite state2;

	[SerializeField] Image imageToSwap;

	SoundManager sm;

	private void OnEnable()
	{
		if(sm == null) sm = SoundManager.GetInstance();

		imageToSwap.sprite = sm.isSoundOn() ? state1 : state2;
	}

	public void SwapImages()
	{
		imageToSwap.sprite = imageToSwap.sprite == state1 ? state2 : state1;
	}
}
