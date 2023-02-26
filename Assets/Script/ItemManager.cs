using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 아이템 관리 클래스
/// </summary>
public class ItemManager : MonoBehaviour
{
    /// <summary>
    /// 로켓별 데이터
    /// </summary>
    public RocketData[] rocketDataSet;
    public EquipmentManager equipmentManager;
    public StatManager statManager;

    /// <summary>
    /// 아이템 종류
    /// </summary>
    public enum ItemType { EmergencyFueling, VernierEngine, BoosterEngine, Shield }
    /// <summary>
    /// 아이템 데이터 (ScriptableObject)
    /// </summary>
    public ItemData[] items;
    /// <summary>
    /// 장착한 아이템 종류
    /// </summary>
    public static ItemType? equippedItem;
    /// <summary>
    /// 아이템 사용 버튼
    /// </summary>
    public Button item_button;
    /// <summary>
    /// 남은 아이템 수량 텍스트
    /// </summary>
    public Text quantity_text;

    /// <summary>
    /// 쉴드 아이템의 남은 HP
    /// </summary>
    public int shieldHp;
    /// <summary>
    /// 쉴드 활성화 여부
    /// </summary>
    public bool shieldOn = false;

    /// <summary>
    /// 부스터 효과 활성화 여부
    /// </summary>
    public bool boosterOn = false;
    /// <summary>
    /// 부스터 남은 시간 표기 타이머 이미지
    /// </summary>
    public Image boosterTimer;

    private void Start()
    {
        UpdateItemQuantityText();
    }

    /// <summary>
    /// 아이템 사용 버튼 리스너
    /// </summary>
    public void UseItem()
    {
        UseItem(equippedItem);
    }

    /// <summary>
    /// 아이템 사용
    /// </summary>
    /// <param name="type">아이템 종류 인덱스</param>
    public void UseItem(int type)
    {
        UseItem((ItemType)type);
    }

    /// <summary>
    /// 아이템 사용
    /// </summary>
    /// <param name="type">아이템 종류</param>
    public void UseItem(ItemType? type)
    {
        // 각 아이템 별로 효과 발동
        int index = (int)type;
        switch (type)
        {
            case ItemType.EmergencyFueling:
                EmergencyFueling(items[index].MainValue);
                break;
            case ItemType.BoosterEngine:
                // 부스터 스탯 레벨 확인 후 발동
                if (statManager.GetLevel(StatType.Booster) != 0)
                {
                    if (!boosterOn && Launch.instance.isAccerlate)
                        BoosterEngine();
                    else
                        return;
                }
                else
                {
                    LocalizationManager.instance.GetLocalizedValue("booster_unable");
                    return;
                }
                break;
            case ItemType.Shield:
                if (!shieldOn)
                    Shield();
                else
                    return;
                break;
        }

        // 아이템 남은 수량 삭감 
        items[index].item.quantity--;
        // 아이템 소진 확인 및 장착 해제 처리
        if (items[index].item.quantity <= 0)
        {
            equippedItem = null;
            item_button.gameObject.SetActive(false);
            equipmentManager.UpdateEquipItemImage();
        }
        else
            UpdateItemQuantityText();
    }

    /// <summary>
    /// 아이템 남은 수량 업데이트
    /// </summary>
    public void UpdateItemQuantityText()
    {
        if(equippedItem != null)
            quantity_text.text = items[(int)equippedItem].item.quantity.ToString();
    }

    /// <summary>
    /// 긴급 급유 아이템 사용
    /// </summary>
    /// <param name="recover">급유량</param>
    private void EmergencyFueling(float recover)
    {
        RocketSet.instance.FuelRemain += RocketSet.instance.fuelMax[RocketSet.instance.currentStage] * recover;
    }

    /// <summary>
    /// 부스터 아이템 효과 발동
    /// </summary>
    private void BoosterEngine()
    {
        // 부스터/무적 활성화 및 효과 재생
        boosterOn = true;
        RocketSet.instance.BoosterFireEffectPlay();
        RocketSet.instance.isInvincible = true;
        boosterTimer.transform.parent.gameObject.SetActive(true);
        StartCoroutine(BoosterRemainTimer(items[(int)equippedItem].MainValue));
        StartCoroutine(BoosterTimer(items[(int)equippedItem].MainValue));
    }

    /// <summary>
    /// 부스터 효과 지속 타이머
    /// </summary>
    /// <param name="fullTime">총 지속시간</param>
    /// <param name="passedTime">지난 시간</param>
    IEnumerator BoosterRemainTimer(float fullTime, float passedTime = 0)
    {
        yield return new WaitForEndOfFrame();

        passedTime += Time.deltaTime;

        // 남은 부스터 시간 표시
        boosterTimer.fillAmount = passedTime / fullTime;


        if (passedTime < fullTime)
        {
            StartCoroutine(BoosterRemainTimer(fullTime, passedTime));
        }
        else
            boosterTimer.transform.parent.gameObject.SetActive(false);
    }

    /// <summary>
    /// 부스터 효과 종료 타이머
    /// </summary>
    /// <param name="time">지속시간</param>
    IEnumerator BoosterTimer(float time)
    {
        yield return new WaitForSeconds(time);
        // 이펙트 중단 및 부스터/무적 처리 해제
        RocketSet.instance.BoosterFireEffectStop(); 
        RocketSet.instance.isInvincible = false;
        boosterOn = false;
    }

    /// <summary>
    /// 보호막 아이템 활성화
    /// </summary>
    private void Shield()
    {
        // 쉴드 효과 활성화 및 초기 값 세팅
        shieldOn = true;
        shieldHp = (int)items[(int)ItemType.Shield].MainValue;
        RocketSet.instance.shield.color = Color.white;
        RocketSet.instance.shield.gameObject.SetActive(true);
        RocketSet.instance.shield_ani.Play();
    }

    /// <summary>
    /// 보호막 충격 흡수 처리
    /// </summary>
    /// <param name="dmg">데미지</param>
    public void HitShield(int dmg)
    {
        // 쉴드량 삭감 후 남은 쉴드량 색상으로 표시
        shieldHp -= dmg;
        float portion = shieldHp / items[(int)ItemType.Shield].MainValue;
        RocketSet.instance.shield.color = new Color(1, portion, portion);
        RocketSet.instance.isInvincible = true;
        // 쉴도 소진 후 무적 효과 제거 및 쉴드 이펙트 제거
        if (shieldHp <= 0)
        {
            RocketSet.instance.isInvincible = false;
            RocketSet.instance.shield_ani["ShieldAppear"].speed = -1;
            RocketSet.instance.shield_ani["ShieldAppear"].time = RocketSet.instance.shield_ani["ShieldAppear"].length;
            RocketSet.instance.shield_ani.Play();
            PlayManager.instance.WaitAnimation(RocketSet.instance.shield_ani, DisableShield);
        }
    }

    /// <summary>
    /// 쉴드 비활성화
    /// </summary>
    private void DisableShield()
    {
        shieldOn = false;
        RocketSet.instance.shield.gameObject.SetActive(false);
    }

    /// <summary>
    /// 아이템 설명 얻어오기
    /// </summary>
    /// <param name="data">대상 아이템 데이터</param>
    /// <returns>현지화된 아이템 설명</returns>
    public string GetDescription(ItemData data)
    {
        ItemType type = data.ItemType;
        switch(type)
        {
            case ItemType.EmergencyFueling:
                return string.Format(LocalizationManager.instance.GetLocalizedValue("item_desc_0"), data.MainValue * 100);
            case ItemType.VernierEngine:
                return string.Format(LocalizationManager.instance.GetLocalizedValue("item_desc_1"), data.MainValue, data.SubValues[0]);
            case ItemType.BoosterEngine:
                return string.Format(LocalizationManager.instance.GetLocalizedValue("item_desc_2"), data.MainValue);
            case ItemType.Shield:
                return string.Format(LocalizationManager.instance.GetLocalizedValue("item_desc_3"), data.MainValue);
        }

        return null;
    }

    /// <summary>
    /// 데이터 저장
    /// </summary>
    /// <param name="data">저장할 데이터</param>
    public void SaveData(ref Data data)
    {
        data.rocketItemData = new List<Item>();
        data.itemsData = new List<Item>();
        for (int i = 0; i < rocketDataSet.Length; i++)
        {
            data.rocketItemData.Add(rocketDataSet[i].itemData);
        }
        for(int i = 0; i < items.Length; i++)
        {
            data.itemsData.Add(items[i].item);
        }
    }
    /// <summary>
    /// 데이터 불러오기
    /// </summary>
    /// <param name="data">불러온 데이터</param>
    public void LoadData(Data data)
    {
        // 불러올 데이터가 있으면 로드 시작
        if (data != null)
        {
            // 로켓 아이템 데이터가 있으면 로켓 데이터 로드
            if(data.rocketItemData != null)
            {
                for (int i = 0; i < data.rocketItemData.Count; i++)
                {
                    rocketDataSet[i].itemData = data.rocketItemData[i];
                }
            }
            // 일반 아이템 데이터가 있으면 데이터 로드
            if(data.itemsData != null)
            {
                for (int i = 0; i < items.Length; i++)
                    items[i].item = data.itemsData[i];
            }
        }
        // 데이터가 없으면 아이템 데이터 초기화
        else
        {
            ResetItems();
        }
    }
    /// <summary>
    /// 아이템 초기화 작업
    /// </summary>
    public void ResetItems()
    {
        for (int i = 0; i < items.Length; i++)
            items[i].item.quantity = 0;

        for (int i = 1; i < rocketDataSet.Length; i++)
            rocketDataSet[i].itemData.hasItem = false;

        for (int i = 0; i < equipmentManager.weapons_prefab.Length; i++)
        {
            equipmentManager.weapons_prefab[i].weaponSpecData.weaponData.hasItem = false;
            equipmentManager.weapons_prefab[i].weaponSpecData.weaponData.numOfBulletLeft = 0;
        }
    }
}

/// <summary>
/// 아이템 데이터
/// </summary>
[System.Serializable]
public class Item
{
    /// <summary>
    /// 아이템 이름
    /// </summary>
    public string itemName;
    /// <summary>
    /// 아이템 가격
    /// </summary>
    public int price;
    /// <summary>
    /// 아이템 보유 여부
    /// </summary>
    public bool hasItem;
    /// <summary>
    /// 아이템 보유량
    /// </summary>
    public int quantity;
}