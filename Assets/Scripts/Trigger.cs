using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class Trigger : MonoBehaviour
{
    [SerializeField] bool stopLine;
    [SerializeField] bool finishLine;
    [SerializeField] int stopLineID;
    [SerializeField] List<Appearance> appearance;
    [SerializeField] Location attachedLocation;

    [SerializeField] ParticleSystem[] RainbowGun;
    [SerializeField] ParticleSystem LocationBasedConfetti;

    GameManager gm;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            Character charScript = other.GetComponent<Character>();

            if(finishLine)
            {
                charScript.CharacterCrossedTheFinishLine();
                CustomGameEventList.CharacterCrossedTheFinishLine.Invoke(charScript.control);
            }

            if(finishLine && charScript.control == Control.Player)
            {
                if(gm == null) gm = GameManager.GetInstance();

                if(gm.IsPlayerFirstToFinish() && RainbowGun.Length != 0)
                    foreach(var gun in RainbowGun)
                        gun.Play();

                if(gm.IsPlayerFirstToFinish() && LocationBasedConfetti != null) 
                    LocationBasedConfetti.Play();

                CustomGameEventList.OnChangeGameState.Invoke(GameState.FinishLine);

                if(attachedLocation == null)
                    attachedLocation = transform.parent.GetComponent<Location>();

                appearance = attachedLocation?.GetLocationDressTypes();

                if(finishLine)
                    appearance = new List<Appearance>() { Appearance.None };

                charScript.OnEnterArea.Invoke(appearance, transform.parent.GetComponent<TutorialScript>()?.ReturnStartEndPositions(), attachedLocation.LocationIndexAfterStart, attachedLocation.GetLocationType());
            }
            else if(stopLine) charScript.OnEnterStopLine.Invoke(stopLineID);
            else
            {
                if(attachedLocation == null)
                    attachedLocation = transform.parent.GetComponent<Location>();
                
                if( attachedLocation != null )
                    appearance = attachedLocation.GetLocationDressTypes();
                
                if(finishLine)
                    appearance = new List<Appearance>() { Appearance.None };

                charScript.OnEnterArea.Invoke(appearance, transform.parent.GetComponent<TutorialScript>()?.ReturnStartEndPositions(), attachedLocation.LocationIndexAfterStart, attachedLocation.GetLocationType());
            }
        }
    }
}
