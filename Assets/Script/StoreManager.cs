using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���� �ý��� ���� Ŭ����
/// </summary>
public class StoreManager : MonoBehaviour
{
    public PurchaseConfirm purchaseConfirm;

    /// <summary>
    /// ���� ���� ���� ���� �ؽ�Ʈ
    /// </summary>
    public Text coin_text;
    /// <summary>
    /// ���� ���� ĳ�� ���� �ؽ�Ʈ
    /// </summary>
    public Text cash_text;

    /// <summary>
    /// ī�װ��� �׷� ������Ʈ
    /// </summary>
    public GameObject[] categories;
    /// <summary>
    /// ���� Ȱ��ȭ ���� ī�װ�
    /// </summary>
    private int activatedCategory = 0;

    /// <summary>
    /// ���� ���� ���� �ؽ�Ʈ �迭
    /// </summary>
    public Text[] rocketPrice_texts;
    /// <summary>
    /// �̹� ���� �Ϸ��� ���� ǥ�� ������Ʈ �迭
    /// </summary>
    public GameObject[] alreadyPurchasedSigns_rocket;

    /// <summary>
    /// ���� ���� ���� �ؽ�Ʈ �迭
    /// </summary>
    public Text[] weaponPrice_texts;
    /// <summary>
    /// �̹� ���� �Ϸ��� ���� ǥ�� ������Ʈ �迭
    /// </summary>
    public GameObject[] alreadyPurchasedSigns_weapon;
    /// <summary>
    /// źȯ ���� ���� �ؽ�Ʈ �迭
    /// </summary>
    public Text[] bulletPrice_texts;

    /// <summary>
    /// ���� Ȯ�� �޴� ������Ʈ
    /// </summary>
    public GameObject equipCheck;
    /// <summary>
    /// ��ǰ �̸� �ؽ�Ʈ
    /// </summary>
    public Text productName_text;

    /// <summary>
    /// ���� ���� �ڵ� ������Ʈ ����� ������Ʈ
    /// </summary>
    public UpdateDisplay storeUpdateDisplay;

    public EquipmentManager equipmentManager;

    private void Start()
    {
        storeUpdateDisplay.onEnable = new UpdateDisplay.OnEnableUpdate(UpdateMoneyData);
        Initialize();
    }

    /// <summary>
    /// ���� UI �ʱ�ȭ �۾�
    /// </summary>
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

    /// <summary>
    /// ���� ǥ�� ���� �ʱ�ȭ
    /// </summary>
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

    /// <summary>
    /// �̹� ������ ���Ͽ� ���� UI ������Ʈ
    /// </summary>
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

    /// <summary>
    /// ���� ���� ���ΰ� ĳ�� ��ȭ ���� ������Ʈ
    /// </summary>
    private void UpdateMoneyData()
    {
        coin_text.text = PlayManager.SetCommaOnDigit(EconomicMgr.instance.Coin);
        cash_text.text = PlayManager.SetCommaOnDigit(EconomicMgr.instance.Cash);
    }

    /// <summary>
    /// ī�װ� ���� (���� / ���� / ������)
    /// </summary>
    /// <param name="cat">������ ī�װ�</param>
    public void SwitchCategory(int cat)
    {
        categories[activatedCategory].SetActive(false);
        activatedCategory = cat;
        categories[activatedCategory].SetActive(true);
    }

    /// <summary>
    /// ��ǰ ����
    /// </summary>
    /// <param name="data">������ �������� ������</param>
    public void SelectProduct(ScriptableObject data)
    {
        purchaseConfirm.OpenConfirm(data);
    }

    /// <summary>
    /// źȯ ��ǰ ����
    /// </summary>
    /// <param name="data">źȯ�� ������ ���� ������ ����</param>
    public void SelectBullet(ScriptableObject data)
    {
        purchaseConfirm.OpenConfirm(data, true);
    }

    /// <summary>
    /// ���� ���� ���� (Slider-OnChangeListener)
    /// </summary>
    /// <param name="index">źȯ/������ ���� �ε���</param>
    public void ControlQuantity(int index)
    {
        purchaseConfirm.ControlQuantity(index);
    }

    /// <summary>
    /// ���� Ȯ�� ��ư ������
    /// </summary>
    public void Confirm()
    {
        // ���� ó���� �����Ǿ����� ���� ���� ������Ʈ
        if(purchaseConfirm.Confirm())
        {
            UpdateMoneyData();
            // �������� ���� ���¸� �������� ���� UI ǥ��
            if(CheckNotEquipped())
                equipCheck.SetActive(true);
            productName_text.text = purchaseConfirm.GetCurrentItemName();
        }
    }

    /// <summary>
    /// ������ �������� ������ �Ǿ����� Ȯ��
    /// </summary>
    /// <returns>���� ����</returns>
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

    /// <summary>
    /// ���� ��� ó��
    /// </summary>
    public void Cancel()
    {
        purchaseConfirm.Close();
    }

    /// <summary>
    /// ������ ������ ���� ó��
    /// </summary>
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
    /// <summary>
    /// ������ ������ �������� �ʰ� UI ���� ó��
    /// </summary>
    public void CancelEquip()
    {
        equipCheck.SetActive(false);
    }
}