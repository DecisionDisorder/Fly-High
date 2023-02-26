using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    public PurchaseConfirm purchaseConfirm;

    public Text coin_text;
    public Text cash_text;

    public GameObject[] categories;
    private int activatedCategory = 0;

    public Text[] rocketPrice_texts;
    public GameObject[] alreadyPurchasedSigns_rocket;

    public Text[] weaponPrice_texts;
    public GameObject[] alreadyPurchasedSigns_weapon;
    public Text[] bulletPrice_texts;

    public GameObject equipCheck;
    public Text productName_text;

    public UpdateDisplay storeUpdateDisplay;

    public EquipmentManager equipmentManager;

    private void Start()
    {
        storeUpdateDisplay.onEnable = new UpdateDisplay.OnEnableUpdate(UpdateMoneyData);
        Initialize();
    }

    private void Initialize()
    {
        UpdateMoneyData();
        for(int i = 0; i < purchaseConfirm.productDetails.Length; i++)
        {
            purchaseConfirm.productDetails[i].InitializeSliderMax();
        }
        InitializePrice();
        UpdateAlreadyPurchased();
    }

    private void InitializePrice()
    {
        for(int i = 0; i < rocketPrice_texts.Length; i++)
        {
            rocketPrice_texts[i].text =  string.Format("{0:#,###} Coin", equipmentManager.rocketSets_preset[i + 1].rocketData.itemData.price);
        }
        for(int i = 0; i < weaponPrice_texts.Length; i++)
        {
            weaponPrice_texts[i].text = string.Format("{0:#,###} Coin", equipmentManager.weapons_prefab[i].weaponSpecData.weaponData.price);
            bulletPrice_texts[i].text = string.Format("{0:#,###} Coin", equipmentManager.weapons_prefab[i].weaponSpecData.BulletPrice);
        }
    }

    public void UpdateAlreadyPurchased()
    {
        for(int i = 0; i < alreadyPurchasedSigns_rocket.Length; i++)
        {
            if(equipmentManager.rocketSets_preset[i + 1].rocketData.itemData.hasItem)
                alreadyPurchasedSigns_rocket[i].SetActive(true);
        }
        for(int i = 0; i < alreadyPurchasedSigns_weapon.Length; i++)
        {
            if (equipmentManager.weapons_prefab[i].weaponSpecData.weaponData.hasItem)
                alreadyPurchasedSigns_weapon[i].SetActive(true);
        }
    }

    private void UpdateMoneyData()
    {
        coin_text.text = PlayManager.SetCommaOnDigit(EconomicMgr.instance.Coin);
        cash_text.text = PlayManager.SetCommaOnDigit(EconomicMgr.instance.Cash);
    }

    public void SwitchCategory(int cat)
    {
        categories[activatedCategory].SetActive(false);
        activatedCategory = cat;
        categories[activatedCategory].SetActive(true);
    }

    public void SelectProduct(ScriptableObject data)
    {
        purchaseConfirm.OpenConfirm(data);
    }

    public void SelectBullet(ScriptableObject data)
    {
        purchaseConfirm.OpenConfirm(data, true);
    }

    public void ControlQuantity(int index)
    {
        purchaseConfirm.ControlQuantity(index);
    }

    public void Confirm()
    {
        if(purchaseConfirm.Confirm())
        {
            UpdateMoneyData();
            if(CheckNotEquipped())
                equipCheck.SetActive(true);
            productName_text.text = purchaseConfirm.GetCurrentItemName();
        }
    }

    private bool CheckNotEquipped()
    {
        if(purchaseConfirm.currentItemData is WeaponSpecData)
        {
            if (EquipmentManager.equippedWeaponIndex.Equals((int)((WeaponSpecData)purchaseConfirm.currentItemData).weaponType))
                return false;
        }
        if(purchaseConfirm.currentItemData is ItemData)
        {
            if (ItemManager.equippedItem.Equals(((ItemData)purchaseConfirm.currentItemData).ItemType))
                return false;
        }
        return true;
    }

    public void Cancel()
    {
        purchaseConfirm.Close();
    }

    public void Equip()
    {
        equipCheck.SetActive(false);

        if(purchaseConfirm.currentItemData is RocketData)
        {
            equipmentManager.ReplaceRocket(((RocketData)purchaseConfirm.currentItemData).rocketType);
        }
        else if (purchaseConfirm.currentItemData is WeaponSpecData)
        {
            equipmentManager.ReplaceWeapon(((WeaponSpecData)purchaseConfirm.currentItemData).weaponType);
        }
        else if(purchaseConfirm.currentItemData is ItemData)
        {
            equipmentManager.ReplaceItem(((ItemData)purchaseConfirm.currentItemData).ItemType);
        }
    }
    public void CancelEquip()
    {
        equipCheck.SetActive(false);
    }
}