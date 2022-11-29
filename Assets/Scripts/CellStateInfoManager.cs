using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellStateInfoManager : MonoBehaviour
{
    [SerializeField] List<DetailStructure> details;

    public void EnableDetail( LockType _detType, int _value = 0, bool _unLockedByDefault = false ) // true - white, false - grey
    {
		foreach(var det in details)
		{
			det.detailObject.SetActive(_detType == det.detailType);

			if(_detType == LockType.CHECKMARK && det.detailType == LockType.CHECKMARK )
			{
				Image checkImage = det.detailObject.transform.GetChild(0).GetComponent<Image>();

				Color checkColor = checkImage.color;

				checkColor.a = _unLockedByDefault ? .7f : 1f;

				checkImage.color = checkColor;
			}
			
			if(_detType == LockType.WATCH_AD && det.detailType == LockType.WATCH_AD)
			{
				det.detailObject.transform.GetChild(0).GetComponent<Text>().text = "x" + _value;
			}
			else if(_detType == LockType.LEVEL_UNLOCK && det.detailType == LockType.LEVEL_UNLOCK)
			{
				det.detailObject.transform.GetChild(0).GetComponent<Text>().text = _value.ToString();
			}
		}
	}

	public void EnableDetail(LockStatus _ls, HighLight _hl, LockType _lt, int _value = 0)
	{
		foreach(var det in details)
		{
			det.detailObject.SetActive(false);
		}

		if( _lt == LockType.WATCH_AD )
		{
			details[1].detailObject.SetActive( true );

			details[1].detailObject.transform.GetChild(0).GetComponent<Text>().text = "x" + _value;
		}
		else if( _lt == LockType.LEVEL_UNLOCK )
		{
			details[2].detailObject.SetActive(true);

			details[2].detailObject.transform.GetChild(0).GetComponent<Text>().text = _value.ToString();
		}
		else if(_ls == LockStatus.LOCKED)
		{
			if(_hl == HighLight.HIGHLIGHTED)
			{
				details[0].detailObject.SetActive(false);
			}
		}
		else if(_hl != HighLight.NONE)
		{
			details[0].detailObject.SetActive(true);

			Image checkImage = details[0].detailObject.transform.GetChild(0).GetComponent<Image>();
			Color checkColor = checkImage.color;

			if(_hl == HighLight.HIGHLIGHTED)
			{
				checkColor.a = 1f;
			}
			else
			{
				checkColor.a = .7f;
			}

			checkImage.color = checkColor;
		}
	}
}

[System.Serializable]
public struct DetailStructure
{
	public LockType detailType;
	public GameObject detailObject;
}