using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 상점 시스템 관리 클래스
/// </summary>
public class StoreManager : MonoBehaviour
{
    public PurchaseConfirm purchaseConfirm;

    /// <summary>
    /// 보유 중인 코인 정보 텍스트
    /// </summary>
    public Text coin_text;
    /// <summary>
    /// 보유 중인 캐시 정보 텍스트
    /// </summary>
    public Text cash_text;

    /// <summary>
    /// 카테고리별 그룹 오브젝트
    /// </summary>
    public GameObject[] categories;
    /// <summary>
    /// 현재 활성화 중인 카테고리
    /// </summary>
    private int activatedCategory = 0;

    /// <summary>
    /// 로켓 가격 정보 텍스트 배열
    /// </summary>
    public Text[] rocketPrice_texts;
    /// <summary>
    /// 이미 구매 완료한 로켓 표시 오브젝트 배열
    /// </summary>
    public GameObject[] alreadyPurchasedSigns_rocket;

    /// <summary>
    /// 무기 가격 정보 텍스트 배열
    /// </summary>
    public Text[] weaponPrice_texts;
    /// <summary>
    /// 이미 구매 완료한 무기 표시 오브젝트 배열
    /// </summary>
    public GameObject[] alreadyPurchasedSigns_weapon;
    /// <summary>
    /// 탄환 가격 정보 텍스트 배열
    /// </summary>
    public Text[] bulletPrice_texts;

    /// <summary>
    /// 장착 확인 메뉴 오브젝트
    /// </summary>
    public GameObject equipCheck;
    /// <summary>
    /// 제품 이름 텍스트
    /// </summary>
    public Text productName_text;

    /// <summary>
    /// 상점 정보 자동 업데이트 도우미 오브젝트
    /// </summary>
    public UpdateDisplay storeUpdateDisplay;

    public EquipmentManager equipmentManager;

    private void Start()
    {
        storeUpdateDisplay.onEnable = new UpdateDisplay.OnEnableUpdate(UpdateMoneyData);
        Initialize();
    }

    /// <summary>
    /// 종합 UI 초기화 작업
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
    /// 가격 표시 정보 초기화
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
    /// 이미 구매한 로켓에 대한 UI 업데이트
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
    /// 보유 중인 코인과 캐시 재화 정보 업데이트
    /// </summary>
    private void UpdateMoneyData()
    {
        coin_text.text = PlayManager.SetCommaOnDigit(EconomicMgr.instance.Coin);
        cash_text.text = PlayManager.SetCommaOnDigit(EconomicMgr.instance.Cash);
    }

    /// <summary>
    /// 카테고리 변경 (로켓 / 무기 / 아이템)
    /// </summary>
    /// <param name="cat">변경할 카테고리</param>
    public void SwitchCategory(int cat)
    {
        categories[activatedCategory].SetActive(false);
        activatedCategory = cat;
        categories[activatedCategory].SetActive(true);
    }

    /// <summary>
    /// 상품 선택
    /// </summary>
    /// <param name="data">선택한 아이템의 데이터</param>
    public void SelectProduct(ScriptableObject data)
    {
        purchaseConfirm.OpenConfirm(data);
    }

    /// <summary>
    /// 탄환 상품 선택
    /// </summary>
    /// <param name="data">탄환이 장전될 무기 데이터 정보</param>
    public void SelectBullet(ScriptableObject data)
    {
        purchaseConfirm.OpenConfirm(data, true);
    }

    /// <summary>
    /// 구매 수량 조절 (Slider-OnChangeListener)
    /// </summary>
    /// <param name="index">탄환/아이템 구분 인덱스</param>
    public void ControlQuantity(int index)
    {
        purchaseConfirm.ControlQuantity(index);
    }

    /// <summary>
    /// 구매 확인 버튼 리스너
    /// </summary>
    public void Confirm()
    {
        // 구매 처리가 성공되었으면 관련 정보 업데이트
        if(purchaseConfirm.Confirm())
        {
            UpdateMoneyData();
            // 장착되지 않은 상태면 장착할지 묻는 UI 표시
            if(CheckNotEquipped())
                equipCheck.SetActive(true);
            productName_text.text = purchaseConfirm.GetCurrentItemName();
        }
    }

    /// <summary>
    /// 구매한 아이템이 장착이 되었는지 확인
    /// </summary>
    /// <returns>장착 여부</returns>
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
    /// 구매 취소 처리
    /// </summary>
    public void Cancel()
    {
        purchaseConfirm.Close();
    }

    /// <summary>
    /// 구매한 아이템 장착 처리
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
    /// 구매한 아이템 장착하지 않고 UI 종료 처리
    /// </summary>
    public void CancelEquip()
    {
        equipCheck.SetActive(false);
    }
}