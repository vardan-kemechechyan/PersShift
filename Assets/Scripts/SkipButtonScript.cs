using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipButtonScript : MonoBehaviour
{
	public void SkipCutScene() { CustomGameEventList.CutSceneSkipped.Invoke(); }
}
