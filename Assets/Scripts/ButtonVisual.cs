using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Enums;
using System;
using System.Linq;


public class ButtonVisual : MonoBehaviour
{
    [SerializeField] CellStateInfoManager CellManager;
    [SerializeField] ButtonColorManagement BCManager;

    [SerializeField] Color selected;
    [SerializeField] Color idle;
    [SerializeField] Image outline;

    [SerializeField] Image selectedImage;
    [SerializeField] Image defaultImage;

    [SerializeField] Material GrayscaleShader;
    [SerializeField] Sprite BlockedCase;
    [SerializeField] Sprite OutlineImageForCells;

    [SerializeField] Customizable_SO cd;

    [SerializeField] bool swapImagesInsteadOfColors = false;

    SoundManager sm;

    [SerializeField] int buttonIndex;

    private void Start()
    {
        if(BCManager == null) BCManager = transform.parent.GetComponent<ButtonColorManagement>();
    }

    public void NonCellButtonClicked(bool? _enabledStatus)
    {
        if(_enabledStatus != null)
        {
            outline.color = (bool)_enabledStatus ? selected : idle;

            return;
        }
    }

    public void DrawCell()
    {
        if(cd.lockInfo.lockStatus == LockStatus.LOCKED && (cd.lockInfo.lockType == LockType.WATCH_AD ||
                                                    cd.lockInfo.lockType == LockType.LEVEL_UNLOCK))
        {
            DrawBlockedCell();
        }
        else
        {
            CellManager.EnableDetail(cd.lockInfo.lockStatus, cd.highlight, cd.lockInfo.lockType, cd.lockInfo.lockType == LockType.WATCH_AD ? cd.lockInfo.priceToUnlock : cd.lockInfo.levelToUnlock);

            defaultImage.enabled = true;

            defaultImage.sprite = cd.shopIcon;

            selectedImage.sprite = OutlineImageForCells;

            Color defImageColor = defaultImage.color;

            Color selImageColor = selectedImage.color;

            selImageColor = cd.highlight == HighLight.HIGHLIGHTED ? selected : idle;

            defImageColor.a = 1f;
            selImageColor.a = 1f;
            defaultImage.material = null;

            if(cd.lockInfo.lockStatus == LockStatus.LOCKED && cd.lockInfo.lockType == LockType.BUY_FOR_MONEY)
            {
                defaultImage.material = GrayscaleShader;
            }
            else if(cd.lockInfo.lockStatus == LockStatus.UNLOCKED && (cd.highlight != HighLight.HIGHLIGHTED))
            {
                selImageColor = idle;
            }
            
            if(cd.lockInfo.lockStatus == LockStatus.LOCKED && cd.highlight == HighLight.NONE)
            {
                defImageColor.a = .7f;
                selImageColor.a = .7f;
            }

            defaultImage.color = defImageColor;

            selectedImage.color = selImageColor;

        }
    }

    public void ChangeColorStyle(int _bIndex, Customizable_SO _clickedCell, bool accessoryDontUpdat = false)
    {
        if(cd.lockInfo.lockStatus == LockStatus.LOCKED && ( cd.lockInfo.lockType == LockType.WATCH_AD || 
                                                            cd.lockInfo.lockType == LockType.LEVEL_UNLOCK))
        {
            DrawBlockedCell();
            return;
        }
        else
        {
            if(sm == null) sm = SoundManager.GetInstance();

            if(buttonIndex == _bIndex && !accessoryDontUpdat)
            {
                if(cd.shopSection != ShopSection.Accessory)
                    sm.SelectUnlockedItem(cd.lockInfo.lockStatus != LockStatus.LOCKED);
                else
                    sm.SelectUnlockedItem(cd.lockInfo.lockStatus != LockStatus.LOCKED && cd.highlight == HighLight.HIGHLIGHTED);
            }
        }

        DrawCell();

        if(cd.shopSection == ShopSection.Accessory && buttonIndex == _bIndex )
        {
            CustomizableItems[] ci = Enum.GetValues(typeof(CustomizableItems)).OfType<CustomizableItems>().ToArray();

            ShopPreviewManager.AccessoryCameraChange.Invoke(ci.First((x) => x.ToString() == cd.itemTypeToString), cd.highlight == HighLight.HIGHLIGHTED);
        }
    }

    public void AnimateAdsCell(float animationTime)
    {
        selectedImage.transform.DOScale(Vector3.one * 1.25f, animationTime / 2f).OnComplete(delegate ()
        {
            selectedImage.transform.DOScale(Vector3.one, animationTime / 2f);
        });
    }
    public void LoadIcon(int _buttonIndex, Customizable_SO _item )
    {
        if(BCManager == null) BCManager = transform.parent.GetComponent<ButtonColorManagement>();

        cd = _item;

        buttonIndex = _buttonIndex;
    }

    public void UpdateCell() { DrawCell(); }

    public void DrawBlockedCell()
    {
        if(cd.lockInfo.lockStatus == LockStatus.LOCKED && (cd.lockInfo.lockType == LockType.WATCH_AD ||
                                                    cd.lockInfo.lockType == LockType.LEVEL_UNLOCK))
        {
            selectedImage.sprite = BlockedCase;

            Color selImageColor = selectedImage.color;
            selImageColor.a = 1f;
            selImageColor = Color.white;

            selectedImage.color = selImageColor;

            defaultImage.enabled = false;

            defaultImage.material = null;

            CellManager.EnableDetail(cd.lockInfo.lockStatus, cd.highlight, cd.lockInfo.lockType, cd.lockInfo.lockType == LockType.WATCH_AD ? cd.lockInfo.priceToUnlock : cd.lockInfo.levelToUnlock);
        }
    }
    public bool GetBlockStatus() 
    { 
        return  cd.lockInfo.lockStatus == LockStatus.LOCKED && 
                (   cd.lockInfo.lockType == LockType.WATCH_AD || 
                    cd.lockInfo.lockType == LockType.LEVEL_UNLOCK
                );
    }
    public bool GetUnlockForMoneyStatus() { return cd.lockInfo.lockType == LockType.BUY_FOR_MONEY; }
    public bool GetUnlockForAdsState() { return cd.lockInfo.lockType == LockType.WATCH_AD; }

    public Customizable_SO GetItemAttahcedToButton() { return cd; }
}
