using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageScript : MonoBehaviour
{
	public void EnableAndSet( bool _activate )
	{
		gameObject.SetActive(_activate);
	}
}
