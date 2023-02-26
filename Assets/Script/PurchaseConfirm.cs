using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 상점에서 아이템 구매를 처리하는 관리 클래스
/// </summary>
public class PurchaseConfirm : MonoBehaviour
{
    public StoreManager storeManager;
    public AndroidController androidController;
    public ItemManager itemManager;
    public WeaponManager weaponManager;
    public DataManager dataManager;

    /// <summary>
    /// 구매 확인 메뉴 오브젝트
    /// </summary>
    public GameObject confirm_obj;
    /// <summary>
    /// 아이템 이름 텍스트
    /// </summary>
    public Text name_text;
    /// <summary>
    /// 아이템 가격 텍스트
    /// </summary>
    public Text price_text;
    /// <summary>
    /// 구매 후 남는 코인 텍스트
    /// </summary>
    public Text afterPurchase_text;
    /// <summary>
    /// 제품 이미지
    /// </summary>
    public Image product_img;
    /// <summary>
    /// 구매 대상 아이템
    /// </summary>
    private Item item;
    /// <summary>
    /// 구매 대상 아이템 데이터
    /// </summary>
    public ScriptableObject currentItemData { private set; get; }

    /// <summary>
    /// 종류별 아이템 세부 사항
    /// </summary>
    public ProductDetails[] productDetails; 

    /// <summary>
    /// 구매 수량 슬라이더
    /// </summary>
    public Slider[] purchasingQuantitySliders;
    /// <summary>
    /// 구매 수량 텍스트
    /// </summary>
    public Text[] purchasingQuantityTexts;
    /// <summary>
    /// 아이템 설명 텍스트
    /// </summary>
    public Text itemDescription_text;
    /// <summary>
    /// 아이템 구매 수량
    /// </summary>
    private int quantity = -1;

    /// <summary>
    /// 1회 최대 구매 수량
    /// </summary>
    public int itemMaxQuantity = 50;

    /// <summary>
    /// 구매 효과음
    /// </summary>
    public AudioSource purchaseAudio;
    /// <summary>
    /// 구매 효과음 클립 배열
    /// </summary>
    public AudioClip[] purchaseClips;

    /// <summary>
    /// 구매 대상 아이템 이미지 업데이트
    /// </summary>
    /// <param name="sprite">아이템 이미지</param>
    /// <param name="angle">아이템 회전 각도</param>
    public void UpdateImage(Sprite sprite, float angle = 30f)
    {
        // 아이템 이미지 설정
        product_img.sprite = sprite;
        // 아이템 이미지 높이 계산
        float priorHeight = product_img.rectTransform.sizeDelta.y;
        // 변경된 아이템 이미지 비율 복구 후 높이 고정하여 이미지 크기 및 각도 조정
        product_img.SetNativeSize();
        float ratio = product_img.rectTransform.sizeDelta.x / product_img.rectTransform.sizeDelta.y;
        product_img.rectTransform.sizeDelta = new Vector2(priorHeight * ratio, priorHeight);
        product_img.transform.localRotation = Quaternion.Euler(0, 0, -angle);
    }

    /// <summary>
    /// 아이템 구매 확인 텍스트 정보 업데이트
    /// </summary>
    /// <param name="price">가격</param>
    /// <param name="name">아이템 이름</param>
    public void UpdateText(int price, string name)
    {
        name_text.text = name;

        price_text.text = PlayManager.SetCommaOnDigit(price);

        int afterPrice = EconomicMgr.instance.Coin - price;
        if (afterPrice >= 0)
            afterPurchase_text.text = string.Format("<color=#afffb1>{0:#,##0}</color>", afterPrice);
        else
            afterPurchase_text.text = string.Format("<color=red>{0:#,##0}</color>", afterPrice);
    }

    /// <summary>
    /// 로켓 구매 확인 텍스트 정보 업데이트
    /// </summary>
    /// <param name="price">가격</param>
    /// <param name="rocketData">구매 대상 로켓 데이터</param>
    public void UpdateText(int price, RocketData rocketData)
    {
        string name = LocalizationManager.instance.GetLocalizedValue("rocket_name_" + (int)rocketData.rocketType) + " [" + rocketData.MaxPower.Length + LocalizationManager.instance.GetLocalizedValue("rocket_stage") + "] ";
        UpdateText(price, name);
    }

    /// <summary>
    /// 구매 대상 아이템 이름 찾기
    /// </summary>
    /// <returns>구매 대상 아이템 이름</returns>
    public string GetCurrentItemName()
    {
        string name = "";
        if(currentItemData is RocketData)
        {
            name = LocalizationManager.instance.GetLocalizedValue("rocket_name_" + (int)((RocketData)currentItemData).rocketType);
        }
        else if(currentItemData is WeaponSpecData)
        {
            name = LocalizationManager.instance.GetLocalizedValue("weapon_name_" + (int)((WeaponSpecData)currentItemData).weaponType);
        }
        else if(currentItemData is ItemData)
        {
            name = LocalizationManager.instance.GetLocalizedValue("item_name_" + (int)((ItemData)currentItemData).ItemType);
        }

        return name;
    }

    /// <summary>
    /// 구매 수량 UI 업데이트
    /// </summary>
    /// <param name="data">대상 무기 데이터</param>
    /// <param name="index">무기 인덱스</param>
    public void UpdateQuantityDisplay(WeaponSpecData data, int index)
    {
        UpdateImage(data.bulletSprite);
        UpdateText(data.BulletPrice, LocalizationManager.instance.GetLocalizedValue("bullet_name_" + index));

        // 최대 보유량 이상 탄환을 구매하지 못하도록 설정
        purchasingQuantitySliders[0].maxValue = data.BulletCapacity - data.weaponData.numOfBulletLeft;
        purchasingQuantitySliders[0].value = 1;
        quantity = (int)purchasingQuantitySliders[0].value;
    }

    /// <summary>
    /// 일반 아이템 구매 수량 업데이트
    /// </summary>
    /// <param name="data">아이템 데이터</param>
    /// <param name="index">아이템 인덱스</param>
    public void UpdateQuantityDisplay(ItemData data, int index)
    {
        UpdateImage(data.IconSprite, 0);
        UpdateText(data.item.price, LocalizationManager.instance.GetLocalizedValue("item_name_" + index));

        // 최대 보유량 이상 아이템을 구매하지 못하도록 설정
        purchasingQuantitySliders[1].maxValue = itemMaxQuantity - data.item.quantity;
        purchasingQuantitySliders[1].value = 1;
        quantity = (int)purchasingQuantitySliders[1].value;
    }
    /// <summary>
    /// 아이템 구매 효과음 재생
    /// </summary>
    /// <param name="isQuantity">다회 구매성 아이템 여부</param>
    private void PurchaseSound(bool isQuantity)
    {
        if(isQuantity)
            purchaseAudio.clip = purchaseClips[0];
        else
            purchaseAudio.clip = purchaseClips[1];
        purchaseAudio.Play();
    }

    /// <summary>
    /// 구매 확인 메뉴 비활성화
    /// </summary>
    public void Close()
    {
        for (int i = 0; i < productDetails.Length; i++)
            if (productDetails[i].detail_obj.activeInHierarchy)
                productDetails[i].detail_obj.SetActive(false);
        confirm_obj.SetActive(false);
    }
    /// <summary>
    /// 구매 확인 처리
    /// </summary>
    /// <returns>구매 성공 여부</returns>
    public bool Confirm()
    {
        // 1회 구매성 아이템
        if(quantity.Equals(-1))
        {
            // 이미 아이템을 보유중인지 확인
            if (!item.hasItem)
            {
                // 코인 지불 구매 처리
                if (EconomicMgr.instance.PurchaseByCoin(item.price))
                {
                    // 아이템 구매 처리 후 안내 메시지 출력
                    item.hasItem = true;
                    if (currentItemData is RocketData || currentItemData is WeaponSpecData)
                        storeManager.UpdateAlreadyPurchased();
                    androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("purchase_success"), 0.75f);
                    dataManager.SaveData();
                    PurchaseSound(false);
                    Close();
                    return true;
                }
                // 코인 부족 안내 메시지 출력
                else
                    androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("not_enough_coin"));
                Close();
            }
            // 이미 구매한 아이템 안내 메시지 출력
            else
            {
                androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("already_purchased"));
            }
        }
        else // 다수 구매성 아이템
        {
            // 무기 탄환 아이템인 경우
            if (currentItemData is WeaponSpecData)
            {
                // 해당 무기를 보유했는지 확인
                if (((WeaponSpecData)currentItemData).weaponData.hasItem)
                {
                    // 구매량이 1개 이상인지 확인
                    if (quantity > 0)
                    {
                        // 코인을 소모하여 비용 지불 처리
                        if (EconomicMgr.instance.PurchaseByCoin(((WeaponSpecData)currentItemData).BulletPrice * quantity))
                        {
                            // 무기 탄환 충전 및 장전 후 구매 안내 메시지 출력
                            ((WeaponSpecData)currentItemData).weaponData.numOfBulletLeft += quantity;
                            if (weaponManager.weapon != null && (((WeaponSpecData)currentItemData).weaponData.numOfBulletLeft - quantity).Equals(0))
                                weaponManager.weapon.ReloadBullet();
                            androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("purchase_success"), 0.75f);
                            dataManager.SaveData();
                            PurchaseSound(true);
                            Close();
                            return true;
                        }
                        // 코인 부족 안내 메시지 출력
                        else
                            androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("not_enough_coin"));
                    }
                    // 0개 구매 불가 안내 메시지 출력
                    else
                        androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("zero_quantity"), 1f);
                }
                // 해당 무기 없음 안내 메시지 출력
                else
                    androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("not_have_weapon"), 1f);
            }
            // 일반 아이템인 경우
            else if(currentItemData is ItemData)
            {
                // 수량 1개 이상인지 확인
                if (quantity > 0)
                {
                    // 코인을 소모하여 비용 지불 처리
                    if (EconomicMgr.instance.PurchaseByCoin(item.price * quantity))
                    {
                        // 아이템 수량 추가 및 구매 안내 메시지 출력
                        item.quantity += quantity;
                        androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("purchase_success"), 0.75f);
                        dataManager.SaveData();
                        PurchaseSound(true);
                        Close();
                        return true;
                    }
                    // 코인 부족 안내 메시지 출력
                    else
                        androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("not_enough_coin"));
                }
                // 0개 구매 불가 안내 메시지 출력
                else
                    androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("zero_quantity"), 1f);
            }
            // 구매 가능한 아이템이 아님
            else
                Debug.LogError("Unavailable Item Type.");
            Close();
        }
        return false;
    }

    /// <summary>
    /// 구매 수량 제어
    /// </summary>
    /// <param name="quantityIndex">탄환/아이템 구분 인덱스</param>
    public void ControlQuantity(int quantityIndex)
    {
        // 수량 정보 대입
        quantity = (int)purchasingQuantitySliders[quantityIndex].value;
        // 탄환 구매량 정보 업데이트
        if (currentItemData is WeaponSpecData)
        {
            purchasingQuantityTexts[0].text = (((WeaponSpecData)currentItemData).weaponData.numOfBulletLeft + quantity) + "/" + ((WeaponSpecData)currentItemData).BulletCapacity + " (+" + quantity + ")";
            UpdateText(((WeaponSpecData)currentItemData).BulletPrice * quantity, LocalizationManager.instance.GetLocalizedValue("bullet_name_" + (int)((WeaponSpecData)currentItemData).weaponType));
        }
        // 일반 아이템 구매량 정보 업데이트
        else if(currentItemData is ItemData)
        {
            purchasingQuantityTexts[1].text = quantity + "/" + (int)purchasingQuantitySliders[1].maxValue + " (+" + quantity + ")"; ;
            UpdateText(item.price * quantity, LocalizationManager.instance.GetLocalizedValue("item_name_" + (int)((ItemData)currentItemData).ItemType));
        }
    }

    /// <summary>
    /// 구매 확인 창 활성화
    /// </summary>
    /// <param name="data">구매 대상 아이템 데이터</param>
    /// <param name="isBullet">아이템이 탄환인지 여부</param>
    public void OpenConfirm(ScriptableObject data, bool isBullet = false)
    {
        // 구매 대상 아이템 데이터 설정
        currentItemData = data;
        // 로켓 구매 관련 UI 설정
        if (currentItemData is RocketData)
        {
            item = ((RocketData)data).itemData;
            productDetails[0].detail_obj.SetActive(true);
            productDetails[0].UpdateSepcDisplay((RocketData)currentItemData, (int)((RocketData)currentItemData).rocketType);
            quantity = -1;
        }
        // 무기 구매 관련 UI 설정
        else if (currentItemData is WeaponSpecData)
        {
            // 무기 본품 구매 관련 UI 설정
            item = ((WeaponSpecData)data).weaponData;
            if (!isBullet)
            {
                quantity = -1;
                productDetails[1].detail_obj.SetActive(true);
                productDetails[1].UpdateSpecDisplay((WeaponSpecData)currentItemData, (int)((WeaponSpecData)currentItemData).weaponType);
            }
            // 탄환 구매 관련 UI 설정
            else
            {
                productDetails[2].detail_obj.SetActive(true);
                UpdateQuantityDisplay((WeaponSpecData)data, (int)((WeaponSpecData)data).weaponType);
                ControlQuantity(0);
            }
        }
        // 일반 아이템 구매 관련 UI 설정
        else if (currentItemData is ItemData)
        {
            item = ((ItemData)currentItemData).item;
            itemDescription_text.text = itemManager.GetDescription((ItemData)currentItemData);
            productDetails[3].detail_obj.SetActive(true);
            UpdateQuantityDisplay((ItemData)currentItemData, (int)((ItemData)currentItemData).ItemType);
        }
        confirm_obj.SetActive(true);
    }
}
