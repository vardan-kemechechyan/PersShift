using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
	[SerializeField] GameObject defaultButton;
	[SerializeField] GameObject extendedButton;

	private void OnEnable()
	{
		defaultButton.SetActive(true);
		extendedButton.SetActive(false);
	}
}
