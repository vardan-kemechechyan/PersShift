using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITestingScript : MonoBehaviour
{
    [SerializeField] List<GameObject> AllScreens;
    [SerializeField] List<GameObject> AllButons;

    public void DisableAll()
    {
		foreach(var scrn in AllScreens)
		{
			scrn.SetActive(false);
			scrn.GetComponent<Canvas>().enabled = false;
		}
	}

	public void EnableButtons( bool _status)
	{
		foreach(var btn in AllButons)
		{
			btn.SetActive(_status);
		}
	}
}
