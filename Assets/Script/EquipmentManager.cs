using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 로켓 업그레이드 항목
/// </summary>
public enum RocketUpgradeItem { HP, Power, Fuel, FuelEfficiency }

/// <summary>
/// 로켓 장착 장비 관리 클래스
/// </summary>
public class EquipmentManager : MonoBehaviour
{
    /// <summary>
    /// 업그레이드 관련 UI 모음 오브젝트 배열
    /// </summary>
    public UpgradeGroup[] upgradeGroups;
    /// <summary>
    /// 로켓 업그레이드 데이터 배열 (ScriptableObject)
    /// </summary>
    public RocketUpgradeData[] upgradeDatas;
    /// <summary>
    /// 장착한 장비 이미지
    /// </summary>
    public Image equippedRocket_img;
    /// <summary>
    /// 장착된 로켓의 RectTransform
    /// </summary>
    private RectTransform equippedRocket_RT;
    /// <summary>
    /// 장착한 로켓
    /// </summary>
    public RocketSet rocketSet;
    /// <summary>
    /// 장착한 로켓의 인덱스
    /// </summary>
    public static int equippedRocketIndex { private set; get; }
    /// <summary>
    /// 생성된 로켓 오브젝트
    /// </summary>
    private GameObject loadedRocket;
    /// <summary>
    /// 로켓 프리팹 배열
    /// </summary>
    public GameObject[] rockets_prefab;
    /// <summary>
    /// 로켓 프리팹의 핵심 컴포넌트 배열
    /// </summary>
    public RocketSet[] rocketSets_preset;
    /// <summary>
    /// 로켓 이미지 배열
    /// </summary>
    public Image[] rocketImages;
    /// <summary>
    /// (장비창) 장착된 로켓이라는 표기 오브젝트 배열
    /// </summary>
    public GameObject[] equippedRocketSignImages;

    /// <summary>
    /// 장비 데이터 오브젝트
    /// </summary>
    private EquipmentData equipmentData;
    /// <summary>
    /// 업그레이드 UI 그룹
    /// </summary>
    public GameObject upgrades;
    /// <summary>
    /// 업그레이드 가능한 대상이 없다는 메시지 오브젝트
    /// </summary>
    public GameObject noUpgradeMessage;

    /// <summary>
    /// 장착한 무기 인덱스
    /// </summary>
    public static int equippedWeaponIndex { private set; get; }
    /// <summary>
    /// 무기 프리팹 배열
    /// </summary>
    public Weapon[] weapons_prefab;
    /// <summary>
    /// (장비창) 장착된 무기라는 표기 오브젝트 배열
    /// </summary>
    public GameObject[] equippedWeaponSignImages;
    /// <summary>
    /// 생성된 무기의 게임 오브젝트
    /// </summary>
    private GameObject loadedWeaponObj;
    /// <summary>
    /// 무기 이미지 배열
    /// </summary>
    public Image[] weaponImages;
    /// <summary>
    /// (장비창) 남은 탄환 개수 표기 텍스트
    /// </summary>
    public Text[] bulletAmounts_text;

    /// <summary>
    /// (장비창) 장착된 아이템이라는 표기 오브젝트 배열
    /// </summary>
    public GameObject[] equippedItemSignImages;
    /// <summary>
    /// (장비창) 아이템의 남은 수량 표기 텍스트
    /// </summary>
    public Text[] itemQuantityTexts;

    /// <summary>
    /// (장비창) 보유중인 코인 표기 텍스트
    /// </summary>
    public Text coin_text;

    public DataManager dataManager;
    public ItemManager itemManager;
    public AndroidController androidController;
    public WeaponManager weaponManager;
    public UpdateDisplay equipmentUpdateDisplay;
    public MainSceneMgr mainSceneMgr;
    public LauncherSettings launcherSettings;
    public StatManager statManager;

    private void Awake()
    {
        // 장착된 로켓의 RectTransform 캐싱
        equippedRocket_RT = equippedRocket_img.GetComponent<RectTransform>();
        // 장비 UI 업데이터 등록
        equipmentUpdateDisplay.onEnable = new UpdateDisplay.OnEnableUpdate(UpdateAll);
        // 로켓 프리셋 데이터 캐싱
        rocketSets_preset = new RocketSet[rockets_prefab.Length];
        for (int i = 0; i < rocketSets_preset.Length; i++)
        {
            rocketSets_preset[i] = rockets_prefab[i].GetComponent<RocketSet>();
        }
        // 로켓 생성 및 (장착 시)보조엔진 로드
        LoadRocket(equippedRocketIndex);
        LoadVernierEngineOnStart();
    }

    /// <summary>
    /// 장비창 관련 UI 업데이트
    /// </summary>
    public void UpdateAll()
    {
        ReplaceUpgradeDisplay();
        UpdateBulletAmount();
        UpdateItemQuantity();
        UpdateHeldItem();
        UpdateEquipItemImage();
        if (EconomicMgr.instance != null)
            coin_text.text = PlayManager.SetCommaOnDigit(EconomicMgr.instance.Coin);
    }

    /// <summary>
    /// 로켓 교체
    /// </summary>
    /// <param name="rocket">교체할 로켓 종류</param>
    public void ReplaceRocket(RocketType rocket)
    {
        ReplaceRocket((int)rocket);
    }
    /// <summary>
    /// 로켓 교체
    /// </summary>
    /// <param name="rocketCode">교차할 로켓의 인덱스</param>

    public void ReplaceRocket(int rocketCode)
    {
        // 현재 장착된 로켓이 아닌지 확인
        if (rocketCode != equippedRocketIndex)
        {
            // 해당 로켓을 보유하고 있는지 확인
            if (rocketSets_preset[rocketCode].rocketData.itemData.hasItem)
            {
                // 장착 표기 UI 업데이트 및 교체할 로켓 소환/설정
                equippedRocketSignImages[equippedRocketIndex].SetActive(false);
                equippedRocketIndex = rocketCode;
                equippedRocketSignImages[equippedRocketIndex].SetActive(true);
                LoadRocket(rocketCode);
                OnChangeRocket();
                dataManager.SaveData();
            }
            else
                androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("not_owned_rocket"));
        }
        else
        {
            androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("already_equipped_rocket"));
        }
    }

    /// <summary>
    /// 로켓 생성
    /// </summary>
    /// <param name="index">생성할 로켓의 인덱스</param>
    public void LoadRocket(int index)
    {
        // 로켓 장착 표기 UI 설정
        equippedRocketSignImages[equippedRocketIndex].SetActive(true);
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        // 시작 씬일 때 로켓 샘플 업데이트
        if (buildIndex.Equals(0))
        {
            rocketSet = rocketSets_preset[index];
            mainSceneMgr.UpdateRocketSample(index);
        }
        // 플레이 씬일 때
        else if(buildIndex.Equals(1))
        {
            // 기존 로켓이 있으면 교체로 판단하고 기존 로켓 제거
            bool isRocketReplace = false;
            if (loadedRocket != null)
            {
                Destroy(loadedRocket);
                isRocketReplace = true;
            }
            // 새로운 로켓 소환 및 초기 설정
            loadedRocket = Instantiate(rockets_prefab[index]);
            rocketSet = loadedRocket.GetComponent<RocketSet>();
            RocketSet.instance = rocketSet;
            RocketSet.instance.rocketMain_rigid.bodyType = RigidbodyType2D.Kinematic;
            // 로켓 교체시, 씬 리로드 설정
            if (isRocketReplace)
            {
                TabletManager.needReload = true;
                launcherSettings.CompleteControl(true);
                launcherSettings.CompleteControl(true);
            }
        }
        // 변경된 로켓에 따른 처리 및 무기 리로드
        OnChangeRocket();
        LoadWeapon(equippedWeaponIndex); // TODO: 호환하지 않는 로켓인 경우 장착 해제처리 LoadWeapon(equippedWeaponIndex);
    }
    /// <summary>
    /// 로켓이 변경되었을 때의 부수적인 처리
    /// </summary>
    private void OnChangeRocket()
    {
        // 로켓 이미지 변경 및 RectTransform의 크기 변경
        equippedRocket_img.sprite = rocketSets_preset[equippedRocketIndex].rocket_image;
        equippedRocket_RT.sizeDelta = new Vector2(equippedRocket_img.sprite.rect.width / equippedRocket_img.sprite.rect.height * equippedRocket_RT.sizeDelta.y, equippedRocket_RT.sizeDelta.y);
        // (장비창) 업그레이드 대상 로켓 교체
        ReplaceUpgradeDisplay();
    }
    /// <summary>
    /// (장비창) 업그레이드 대상 로켓 교체
    /// </summary>
    private void ReplaceUpgradeDisplay()
    {
        // 업그레이드가 가능한 로켓인지 확인
        if (upgradeDatas[equippedRocketIndex] != null)
        {
            // 업그레이드 UI 활성화 및 업그레이드 상태 적용
            upgrades.SetActive(true);
            noUpgradeMessage.SetActive(false);
            for(int i = 0; i < upgradeDatas[equippedRocketIndex].upgradeItems.Length; i++)
            {
                UpdateUpgradeDisplay(i);
                upgradeGroups[i].upgradeStatus_RT.sizeDelta = new Vector2(upgradeGroups[i].upgradeStatus_RT.sizeDelta.y * (upgradeDatas[equippedRocketIndex].upgradeItems[i].Effect.Length - 1), upgradeGroups[i].upgradeStatus_RT.sizeDelta.y);
                upgradeGroups[i].upgradeStatus_slider.maxValue = upgradeDatas[equippedRocketIndex].upgradeItems[i].Effect.Length - 1;
            }
        } 
        // 업그레이드 불가 안내 표시
        else
        {
            upgrades.SetActive(false);
            noUpgradeMessage.SetActive(true);
        }
    }

    /// <summary>
    /// 업그레이드 상태 및 효과 안내 UI 업데이트
    /// </summary>
    /// <param name="i">업그레이드 종류</param>
    private void UpdateUpgradeDisplay(int i)
    {
        // 장착중인 로켓의 업그레이드 레벨들
        int[] levels = equipmentData.upgradeLevels[equippedRocketIndex];
        // 레벨이 최대치 이하일 때, 비용 및 업그레이드 효과 표기
        if (levels[i] < upgradeDatas[equippedRocketIndex].upgradeItems[i].Cost.Length)
        {
            upgradeGroups[i].cost_text.text = PlayManager.SetCommaOnDigit(upgradeDatas[equippedRocketIndex].upgradeItems[i].Cost[levels[i]]) + " Coin";
            upgradeGroups[i].effect_text.text = "+" + upgradeDatas[equippedRocketIndex].upgradeItems[i].Effect[levels[i]] * 100 + "% → " + upgradeDatas[equippedRocketIndex].upgradeItems[i].Effect[levels[i] + 1] * 100 + "%";
        }
        // 최대로 업그레이드 했을 때, 완료된 업그레이드로 표기
        else
        {
            upgradeGroups[i].cost_text.text = "-";
            upgradeGroups[i].effect_text.text = "+" + upgradeDatas[equippedRocketIndex].upgradeItems[i].Effect[levels[i]] * 100 + "%";
        }
        // 업그레이드 레벨 슬라이더 적용
        upgradeGroups[i].upgradeStatus_slider.value = levels[i];
    }

    /// <summary>
    /// 로켓 업그레이드 
    /// </summary>
    /// <param name="select">선택된 업그레이드 대상(Inspector)</param>
    public void UpgradeRocket(UpgradeItemSelect select)
    {
        // 업그레이드 대상을 읽어옴
        RocketUpgradeItem item = select.GetUpgradeType();
        // 업그레이드 가능한 레벨인지 확인
        if(equipmentData.upgradeLevels[equippedRocketIndex][(int)item] < upgradeDatas[equippedRocketIndex].upgradeItems[(int)item].Effect.Length - 1)
        {
            // 코인 차감으로 업그레이드 가능 여부 확인
            if(EconomicMgr.instance.PurchaseByCoin(upgradeDatas[equippedRocketIndex].upgradeItems[(int)item].Cost[equipmentData.upgradeLevels[equippedRocketIndex][(int)item]]))
            {
                // 업그레이드 레벨 증가 및 관련 UI 업데이트
                equipmentData.upgradeLevels[equippedRocketIndex][(int)item]++;
                UpdateUpgradeDisplay((int)item);
                coin_text.text = PlayManager.SetCommaOnDigit(EconomicMgr.instance.Coin);
            }
            else
            {
                androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("not_enough_coin"), 1.0f);
            }
        }
        else
        {
            androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("upgrade_limit"), 1.0f);
        }
    }

    /// <summary>
    /// 선택한 무기로 교체
    /// </summary>
    /// <param name="weapon">선택한 무기</param>
    public void ReplaceWeapon(WeaponType weapon)
    {
        ReplaceWeapon((int)weapon);
    }

    /// <summary>
    /// 선택한 무기로 교체
    /// </summary>
    /// <param name="weapon">선택한 무기(Inspector)</param>
    public void ReplaceWeapon(WeaponSelect weapon)
    {
        ReplaceWeapon((int)weapon.weapon);
    }

    /// <summary>
    /// 선택한 무기로 교체
    /// </summary>
    /// <param name="weaponCode">선택한 무기의 인덱스</param>
    private void ReplaceWeapon(int weaponCode)
    {
        // 이미 장착 중인 무기인지 확인
        if (weaponCode != equippedWeaponIndex)
        {
            // 생성할 무기 샘플 로드 및 보유중인지 확인
            Weapon weapon = weapons_prefab[weaponCode].GetComponent<Weapon>();
            if (weapon.GetHasItem())
            {
                // 무기-로켓 간의 호환성 확인
                if (rocketSet.rocketData.weaponCompatibility[weaponCode])
                {
                    // 무기 교체 및 관련 UI 업데이트
                    if (equippedWeaponIndex != -1)
                        equippedWeaponSignImages[equippedWeaponIndex].SetActive(false);
                    equippedWeaponIndex = weaponCode;
                    equippedWeaponSignImages[equippedWeaponIndex].SetActive(true);
                    LoadWeapon(weaponCode);
                    dataManager.SaveData();
                }
                else
                {
                    androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("weapon_compatibility"));
                }
            }
            else
            {
                androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("not_owned_weapon"));
            }
        }
        else
        {
            // 이미 장착된 무기인 경우, 장착 해제
            equippedWeaponSignImages[equippedWeaponIndex].SetActive(false);
            equippedWeaponIndex = -1;
            weaponManager.weapon = null;
            LoadWeapon(equippedWeaponIndex);
        }
    }

    /// <summary>
    /// 무기 생성
    /// </summary>
    /// <param name="index">무기 인덱스</param>
    public void LoadWeapon(int index)
    {
        // 장착해제가 아닌 경우
        if (index >= 0)
        {
            // 무기-로켓 호환성 확인
            if (!rocketSets_preset[equippedRocketIndex].rocketData.weaponCompatibility[index])
            {
                // 호환되지 않을 경우 장착 해제 처리 및 안내 메시지 출력
                ReplaceWeapon(index);
                weaponManager.weapon = null;
                androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("weapon_compatibility"));
                weaponManager.UpdateRemainBullet();
                return;
            }

            // 해당 무기를 장착했다는 UI 업데이트
            equippedWeaponSignImages[equippedWeaponIndex].SetActive(true);
            // 플레이 씬인 경우, 무기 프리팹을 씬에 스폰하여 초기화 및 탄환 장전
            if (SceneManager.GetActiveScene().buildIndex.Equals(1))
            {
                if (loadedWeaponObj != null)
                    Destroy(loadedWeaponObj);
                loadedWeaponObj = Instantiate(weapons_prefab[index].gameObject);
                weaponManager.weapon = loadedWeaponObj.GetComponent<Weapon>();
                weaponManager.weapon.Initialize();
                weaponManager.weaponAudio.clip = weaponManager.weapon.weaponClip;

                if (SpawnManager.instance != null)
                    weaponManager.weapon.ReloadBullet();
                else
                    StartCoroutine(ReloadDelay(0.5f));
            }
        }
        // 플레이 씬인 경우에 장착 해제 처리
        else
        {
            if (SceneManager.GetActiveScene().buildIndex.Equals(1))
                if (loadedWeaponObj != null)
                    Destroy(loadedWeaponObj);
        }

        // 무기의 남은 탄환 수량 업데이트
        weaponManager.UpdateRemainBullet();
    }

    /// <summary>
    /// 초기 로드 오류 관련하여 약간의 딜레이를 두고 장전하는 코루틴
    /// </summary>
    IEnumerator ReloadDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        weaponManager.weapon.ReloadBullet();
    }

    /// <summary>
    /// 각 무기의 남은 탄환 수량 업데이트
    /// </summary>
    private void UpdateBulletAmount()
    {
        for(int i = 0; i < bulletAmounts_text.Length; i++)
        {
            bulletAmounts_text[i].text = weapons_prefab[i].weaponSpecData.weaponData.numOfBulletLeft + "/" + weapons_prefab[i].weaponSpecData.BulletCapacity;
        }
    }

    /// <summary>
    /// 아이템 교체
    /// </summary>
    /// <param name="item">장착할 아이템 종류</param>
    public void ReplaceItem(ItemManager.ItemType item)
    {
        ReplaceItem((int)item);
    }

    /// <summary>
    /// 아이템 교체
    /// </summary>
    /// <param name="itemSelect">장착할 아이템 종류(Inspector)</param>
    public void ReplaceItem(ItemSelect itemSelect)
    {
        ReplaceItem((int)itemSelect.itemType);
    }

    /// <summary>
    /// 장착한 아이템 이미지 업데이트
    /// </summary>
    public void UpdateEquipItemImage()
    {
        // 장착한 아이템이 있을 때
        if(ItemManager.equippedItem != null)
        {
            // (장비창) 장착한 아이템 표기
            equippedItemSignImages[(int)ItemManager.equippedItem].SetActive(true);
            // 기존에 장착한 아이템 표기 비활성화
            for (int i = 0; i < itemManager.items.Length; i++)
            {
                if(!ItemManager.equippedItem.Equals((ItemManager.ItemType)i))
                {
                    if(equippedItemSignImages[i].activeInHierarchy)
                        equippedItemSignImages[i].SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// 아이템 교체
    /// </summary>
    /// <param name="type">아이템 인덱스</param>
    public void ReplaceItem(int type)
    {
        // 이미 장착한 아이템인지 확인
        if(!ItemManager.equippedItem.Equals((ItemManager.ItemType?)type))
        {
            // 아이템을 1개 이상 보유하고 있는지 확인
            if(itemManager.items[type].item.quantity > 0)
            {
                // 보조 엔진인 경우
                if(type.Equals((int)ItemManager.ItemType.VernierEngine))
                {
                    // 보조 엔진 호환성 확인
                    if (rocketSets_preset[equippedRocketIndex].vernierEngine == null)
                    {
                        androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("vernier_compatibility"));
                        return;
                    }
                    // 호환되면 보조 엔진 생성
                    else
                        LoadVernierEngine();
                }
                // 부스터 엔진인 경우
                else if(type.Equals((int)ItemManager.ItemType.BoosterEngine))
                {
                    // 부스터 엔진 스탯의 레벨이 0인지 확인
                    if(statManager.GetLevel(StatType.Booster).Equals(0))
                    {
                        androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("booster_unable"), 2f);
                        return;
                    }
                }

                // 아이템이 신규 장착이 아닌 교체인 경우
                if (ItemManager.equippedItem != null)
                {
                    // 기존 아이템 장착 해제 처리
                    equippedItemSignImages[(int)ItemManager.equippedItem].SetActive(false);
                    if (ItemManager.equippedItem.Equals(ItemManager.ItemType.VernierEngine))
                        RemoveVernierEngine();
                }

                // 아이템 장착 처리 및 장착한 아이템 수량 업데이트
                ItemManager.equippedItem = (ItemManager.ItemType)type;
                equippedItemSignImages[type].SetActive(true);
                if (itemManager.quantity_text != null)
                    itemManager.UpdateItemQuantityText();

                // 플레이 씬에서 보조 엔진 장착을 한 경우, 이에 따른 연료 게이지 업데이트
                if (type.Equals((int)ItemManager.ItemType.VernierEngine) && SceneManager.GetActiveScene().buildIndex.Equals(1))
                    Launch.instance.SetFuelStageText();

                dataManager.SaveData();
            }
        }
        // 이미 장착중인 아이템을 선택한 경우, 장착 해제 처리
        else
        {
            ItemManager.equippedItem = null;
            equippedItemSignImages[type].SetActive(false);
            if (type.Equals((int)ItemManager.ItemType.VernierEngine))
                RemoveVernierEngine();
            dataManager.SaveData();
        }
    }

    /// <summary>
    /// 씬 Start 때 보조 엔진 로드
    /// </summary>
    private void LoadVernierEngineOnStart()
    {
        if (ItemManager.equippedItem.Equals(ItemManager.ItemType.VernierEngine))
            LoadVernierEngine();
    }

    /// <summary>
    /// 보조 엔진 생성
    /// </summary>
    public void LoadVernierEngine()
    {
        // 보조엔진 장착 가능한 플레이 씬인 경우에만 생성
        if (SceneManager.GetActiveScene().buildIndex.Equals(1))
        {
            // 장착된 로켓에 보조 엔진 프리셋 추가
            RocketSet.instance.vernierEngine.gameObject.SetActive(true);
            RocketSet.instance.rocket_rigids.Insert(0, RocketSet.instance.vernierEngine);
            RocketSet.instance.fuelMax.Insert(0, itemManager.items[(int)ItemManager.ItemType.VernierEngine].SubValues[0]);
            RocketSet.instance.positionsOnStage.Insert(0, Vector3.zero);
            RocketSet.instance.powerMax.Insert(0, itemManager.items[(int)ItemManager.ItemType.VernierEngine].MainValue);
            RocketSet.instance.FuelRemain = itemManager.items[(int)ItemManager.ItemType.VernierEngine].SubValues[0];
        }
    }

    /// <summary>
    /// 보조엔진 제거
    /// </summary>
    public void RemoveVernierEngine()
    {
        // 보조엔진 장착 가능한 플레이 씬에서만 제거 작업
        if (SceneManager.GetActiveScene().buildIndex.Equals(1))
        {
            // 보조 엔진 프리셋 제거
            RocketSet.instance.vernierEngine.gameObject.SetActive(false);
            RocketSet.instance.rocket_rigids.RemoveAt(0);
            RocketSet.instance.fuelMax.RemoveAt(0);
            RocketSet.instance.positionsOnStage.RemoveAt(0);
            RocketSet.instance.FuelRemain = RocketSet.instance.fuelMax[0];
            RocketSet.instance.powerMax.RemoveAt(0);
            Launch.instance.SetFuelStageText();
        }
    }

    /// <summary>
    /// 각 아이템의 남은 수량 업데이트
    /// </summary>
    public void UpdateItemQuantity()
    {
        for (int i = 0; i < itemQuantityTexts.Length; i++)
            itemQuantityTexts[i].text = itemManager.items[i].item.quantity.ToString();
    }

    /// <summary>
    /// 보유하고 있는 로켓 및 무기 관련 이미지 색상 업데이트
    /// </summary>
    private void UpdateHeldItem()
    {
        for(int i = 0; i < rocketImages.Length; i++)
        {
            if (rocketSets_preset[i].rocketData.itemData.hasItem)
                rocketImages[i].color = Color.white;
            else
                rocketImages[i].color = Color.black;
        }

        for(int i = 0; i < weaponImages.Length; i++)
        {
            if (weapons_prefab[i].weaponSpecData.weaponData.hasItem)
                weaponImages[i].color = Color.white;
            else
                weaponImages[i].color = Color.black;
        }
    }

    /// <summary>
    /// 장비 데이터 저장
    /// </summary>
    /// <param name="data">저장(write) 대상 데이터</param>
    public void SaveData(ref Data data)
    {
        equipmentData.eqippedRocket = equippedRocketIndex;
        equipmentData.equippedWeapon = equippedWeaponIndex;
        equipmentData.equippedItemType = ItemManager.equippedItem;

        equipmentData.weaponItemData = new List<WeaponData>();
        for(int i = 0; i < weapons_prefab.Length; i++)
        {
            equipmentData.weaponItemData.Add(weapons_prefab[i].weaponSpecData.weaponData);
        }
        data.equipmentData = equipmentData;
    }
    /// <summary>
    /// 장비 데이터 로드
    /// </summary>
    /// <param name="data">불러온(loaded) 데이터</param>
    public void LoadData(Data data)
    {
        // 불러온 데이터 적용
        if (data.equipmentData != null)
        {
            equipmentData = data.equipmentData;
            equippedRocketIndex = data.equipmentData.eqippedRocket;
            equippedWeaponIndex = data.equipmentData.equippedWeapon;
            ItemManager.equippedItem = data.equipmentData.equippedItemType;

            if(data.equipmentData.weaponItemData != null)
            {
                for (int i = 0; i < weapons_prefab.Length; i++)
                {
                    weapons_prefab[i].weaponSpecData.weaponData = data.equipmentData.weaponItemData[i];
                }
            }
        }
        // 장비 데이터 초기화
        else
        {
            equipmentData = new EquipmentData();

            equippedRocketIndex = 0;  // Default:  0 (Bottle Rocket)
            equippedWeaponIndex = -1; // Default: -1 (None)    
        }

        // 업그레이드 종류가 부족한 경우 추가
        for (int i = equipmentData.upgradeLevels.Count; i < upgradeDatas.Length; i++)
        {
            if (upgradeDatas[i] != null)
                equipmentData.upgradeLevels.Add(new int[upgradeDatas[i].upgradeItems.Length]);
            else
                equipmentData.upgradeLevels.Add(new int[0]);
        }
    }
}

/// <summary>
/// 업그레이드 관련 UI 그룹
/// </summary>
[System.Serializable]
public class UpgradeGroup
{
    public Text effect_text;
    public Text cost_text;
    public Slider upgradeStatus_slider;
    public RectTransform upgradeStatus_RT;
}

/// <summary>
/// 장비 데이터
/// </summary>
[System.Serializable]
public class EquipmentData
{
    /// <summary>
    /// 장착 중인 로켓
    /// </summary>
    public int eqippedRocket;
    /// <summary>
    /// 장착 중인 무기
    /// </summary>
    public int equippedWeapon;
    /// <summary>
    /// 장착 중인 아이템
    /// </summary>
    public ItemManager.ItemType? equippedItemType;
    /// <summary>
    /// 무기 아이템 데이터
    /// </summary>
    public List<WeaponData> weaponItemData;

    /// <summary>
    /// 로켓별 업그레이드 레벨
    /// </summary>
    public List<int[]> upgradeLevels;


    public EquipmentData()
    {
        upgradeLevels = new List<int[]>();
    }
}