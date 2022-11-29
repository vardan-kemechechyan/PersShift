using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenTheGateScript : MonoBehaviour
{
	[SerializeField] Animator GateAnimator;

	[SerializeField] ParticleSystem LocationBasedConfetti;

	[SerializeField] int finishRoomIndex;

	SoundManager sm;

	bool GateOpened = false;

	private void OnTriggerExit( Collider other )
	{
		if ( other.CompareTag("Player"))
		{
			if(sm == null) sm = SoundManager.GetInstance();

			if ( !GateOpened && other.GetComponent<Character>().control == Enums.Control.Player)
			{
				GateAnimator.SetTrigger("Open");

				LocationBasedConfetti.Play();

				if(finishRoomIndex == 2)
					sm.PlayerGotToNextRoom();

				GateOpened = true;
			}
		}
	}
}
