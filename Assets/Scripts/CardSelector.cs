using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSelector : MonoBehaviour
{
    GameManager gm;

    [SerializeField] Appearance cardType;
    [SerializeField] Image cardImage;
    public Image buttonFill;

    [SerializeField] Color defaultColor;
    [SerializeField] Color pickedColor;

    [SerializeField] List<CardSelector> siblingButtons = new List<CardSelector>();

    [SerializeField] Image disableScreen;
    [SerializeField] GameObject hintText;
    [SerializeField] Animator cardAnimation;

	private void Start()
	{
		for(int i = 0; i < transform.parent.childCount; i++)
            siblingButtons.Add(transform.parent.GetChild(i).GetComponentInChildren<CardSelector>());

        gm = GameManager.GetInstance();
    }

	public void SelectAppearance()
    {
        GameManager.GetInstance().SelectAppearance( cardType );

		foreach(var card in siblingButtons)
		{
            if(card == this)
            {
                card.buttonFill.color = pickedColor;
            }
            else
            {
                card.buttonFill.color = defaultColor;
            }
		}
    }

    public void SetTheButton(Appearance _buttonAppearance)
    {
        cardType = _buttonAppearance;
        cardImage.sprite = Resources.Load<Sprite>("Card Button Dress Images/" + _buttonAppearance.ToString());
        GetComponent<Button>().enabled = true;

    }

    public Appearance GetAppearance() { return cardType; }

    public void EnableHint( bool _status )
    {
        if(gm.CurrentLevelIndex == 0)
        {
            disableScreen.gameObject.SetActive(!_status);
            GetComponent<Button>().enabled = _status;
        }
        
        hintText.gameObject.SetActive(_status);

        if(_status )
        {
            cardAnimation.enabled = true;
            cardAnimation.Play("ScaleUpDown");
		}
        else
        {
            cardAnimation.enabled = false;
            //gameObject.transform.localScale = new Vector3(1.1f, 1.65f, 1f);
            gameObject.transform.localScale = Vector3.one * 0.8f;
        }
    }

    public void DefaultState()
    {
        disableScreen.gameObject.SetActive(false);
        hintText.gameObject.SetActive(false);
        GetComponent<Button>().enabled = true;
        cardAnimation.enabled = false;
        //gameObject.transform.localScale = new Vector3(1.1f, 1.65f, 1f);
        gameObject.transform.localScale = Vector3.one * 0.8f;
        buttonFill.color = defaultColor;
    }
}
