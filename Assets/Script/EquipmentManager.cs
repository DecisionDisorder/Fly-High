using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// ���� ���׷��̵� �׸�
/// </summary>
public enum RocketUpgradeItem { HP, Power, Fuel, FuelEfficiency }

/// <summary>
/// ���� ���� ��� ���� Ŭ����
/// </summary>
public class EquipmentManager : MonoBehaviour
{
    /// <summary>
    /// ���׷��̵� ���� UI ���� ������Ʈ �迭
    /// </summary>
    public UpgradeGroup[] upgradeGroups;
    /// <summary>
    /// ���� ���׷��̵� ������ �迭 (ScriptableObject)
    /// </summary>
    public RocketUpgradeData[] upgradeDatas;
    /// <summary>
    /// ������ ��� �̹���
    /// </summary>
    public Image equippedRocket_img;
    /// <summary>
    /// ������ ������ RectTransform
    /// </summary>
    private RectTransform equippedRocket_RT;
    /// <summary>
    /// ������ ����
    /// </summary>
    public RocketSet rocketSet;
    /// <summary>
    /// ������ ������ �ε���
    /// </summary>
    public static int equippedRocketIndex { private set; get; }
    /// <summary>
    /// ������ ���� ������Ʈ
    /// </summary>
    private GameObject loadedRocket;
    /// <summary>
    /// ���� ������ �迭
    /// </summary>
    public GameObject[] rockets_prefab;
    /// <summary>
    /// ���� �������� �ٽ� ������Ʈ �迭
    /// </summary>
    public RocketSet[] rocketSets_preset;
    /// <summary>
    /// ���� �̹��� �迭
    /// </summary>
    public Image[] rocketImages;
    /// <summary>
    /// (���â) ������ �����̶�� ǥ�� ������Ʈ �迭
    /// </summary>
    public GameObject[] equippedRocketSignImages;

    /// <summary>
    /// ��� ������ ������Ʈ
    /// </summary>
    private EquipmentData equipmentData;
    /// <summary>
    /// ���׷��̵� UI �׷�
    /// </summary>
    public GameObject upgrades;
    /// <summary>
    /// ���׷��̵� ������ ����� ���ٴ� �޽��� ������Ʈ
    /// </summary>
    public GameObject noUpgradeMessage;

    /// <summary>
    /// ������ ���� �ε���
    /// </summary>
    public static int equippedWeaponIndex { private set; get; }
    /// <summary>
    /// ���� ������ �迭
    /// </summary>
    public Weapon[] weapons_prefab;
    /// <summary>
    /// (���â) ������ ������ ǥ�� ������Ʈ �迭
    /// </summary>
    public GameObject[] equippedWeaponSignImages;
    /// <summary>
    /// ������ ������ ���� ������Ʈ
    /// </summary>
    private GameObject loadedWeaponObj;
    /// <summary>
    /// ���� �̹��� �迭
    /// </summary>
    public Image[] weaponImages;
    /// <summary>
    /// (���â) ���� źȯ ���� ǥ�� �ؽ�Ʈ
    /// </summary>
    public Text[] bulletAmounts_text;

    /// <summary>
    /// (���â) ������ �������̶�� ǥ�� ������Ʈ �迭
    /// </summary>
    public GameObject[] equippedItemSignImages;
    /// <summary>
    /// (���â) �������� ���� ���� ǥ�� �ؽ�Ʈ
    /// </summary>
    public Text[] itemQuantityTexts;

    /// <summary>
    /// (���â) �������� ���� ǥ�� �ؽ�Ʈ
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
        // ������ ������ RectTransform ĳ��
        equippedRocket_RT = equippedRocket_img.GetComponent<RectTransform>();
        // ��� UI �������� ���
        equipmentUpdateDisplay.onEnable = new UpdateDisplay.OnEnableUpdate(UpdateAll);
        // ���� ������ ������ ĳ��
        rocketSets_preset = new RocketSet[rockets_prefab.Length];
        for (int i = 0; i < rocketSets_preset.Length; i++)
        {
            rocketSets_preset[i] = rockets_prefab[i].GetComponent<RocketSet>();
        }
        // ���� ���� �� (���� ��)�������� �ε�
        LoadRocket(equippedRocketIndex);
        LoadVernierEngineOnStart();
    }

    /// <summary>
    /// ���â ���� UI ������Ʈ
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
    /// ���� ��ü
    /// </summary>
    /// <param name="rocket">��ü�� ���� ����</param>
    public void ReplaceRocket(RocketType rocket)
    {
        ReplaceRocket((int)rocket);
    }
    /// <summary>
    /// ���� ��ü
    /// </summary>
    /// <param name="rocketCode">������ ������ �ε���</param>

    public void ReplaceRocket(int rocketCode)
    {
        // ���� ������ ������ �ƴ��� Ȯ��
        if (rocketCode != equippedRocketIndex)
        {
            // �ش� ������ �����ϰ� �ִ��� Ȯ��
            if (rocketSets_preset[rocketCode].rocketData.itemData.hasItem)
            {
                // ���� ǥ�� UI ������Ʈ �� ��ü�� ���� ��ȯ/����
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
    /// ���� ����
    /// </summary>
    /// <param name="index">������ ������ �ε���</param>
    public void LoadRocket(int index)
    {
        // ���� ���� ǥ�� UI ����
        equippedRocketSignImages[equippedRocketIndex].SetActive(true);
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        // ���� ���� �� ���� ���� ������Ʈ
        if (buildIndex.Equals(0))
        {
            rocketSet = rocketSets_preset[index];
            mainSceneMgr.UpdateRocketSample(index);
        }
        // �÷��� ���� ��
        else if(buildIndex.Equals(1))
        {
            // ���� ������ ������ ��ü�� �Ǵ��ϰ� ���� ���� ����
            bool isRocketReplace = false;
            if (loadedRocket != null)
            {
                Destroy(loadedRocket);
                isRocketReplace = true;
            }
            // ���ο� ���� ��ȯ �� �ʱ� ����
            loadedRocket = Instantiate(rockets_prefab[index]);
            rocketSet = loadedRocket.GetComponent<RocketSet>();
            RocketSet.instance = rocketSet;
            RocketSet.instance.rocketMain_rigid.bodyType = RigidbodyType2D.Kinematic;
            // ���� ��ü��, �� ���ε� ����
            if (isRocketReplace)
            {
                TabletManager.needReload = true;
                launcherSettings.CompleteControl(true);
                launcherSettings.CompleteControl(true);
            }
        }
        // ����� ���Ͽ� ���� ó�� �� ���� ���ε�
        OnChangeRocket();
        LoadWeapon(equippedWeaponIndex); // TODO: ȣȯ���� �ʴ� ������ ��� ���� ����ó�� LoadWeapon(equippedWeaponIndex);
    }
    /// <summary>
    /// ������ ����Ǿ��� ���� �μ����� ó��
    /// </summary>
    private void OnChangeRocket()
    {
        // ���� �̹��� ���� �� RectTransform�� ũ�� ����
        equippedRocket_img.sprite = rocketSets_preset[equippedRocketIndex].rocket_image;
        equippedRocket_RT.sizeDelta = new Vector2(equippedRocket_img.sprite.rect.width / equippedRocket_img.sprite.rect.height * equippedRocket_RT.sizeDelta.y, equippedRocket_RT.sizeDelta.y);
        // (���â) ���׷��̵� ��� ���� ��ü
        ReplaceUpgradeDisplay();
    }
    /// <summary>
    /// (���â) ���׷��̵� ��� ���� ��ü
    /// </summary>
    private void ReplaceUpgradeDisplay()
    {
        // ���׷��̵尡 ������ �������� Ȯ��
        if (upgradeDatas[equippedRocketIndex] != null)
        {
            // ���׷��̵� UI Ȱ��ȭ �� ���׷��̵� ���� ����
            upgrades.SetActive(true);
            noUpgradeMessage.SetActive(false);
            for(int i = 0; i < upgradeDatas[equippedRocketIndex].upgradeItems.Length; i++)
            {
                UpdateUpgradeDisplay(i);
                upgradeGroups[i].upgradeStatus_RT.sizeDelta = new Vector2(upgradeGroups[i].upgradeStatus_RT.sizeDelta.y * (upgradeDatas[equippedRocketIndex].upgradeItems[i].Effect.Length - 1), upgradeGroups[i].upgradeStatus_RT.sizeDelta.y);
                upgradeGroups[i].upgradeStatus_slider.maxValue = upgradeDatas[equippedRocketIndex].upgradeItems[i].Effect.Length - 1;
            }
        } 
        // ���׷��̵� �Ұ� �ȳ� ǥ��
        else
        {
            upgrades.SetActive(false);
            noUpgradeMessage.SetActive(true);
        }
    }

    /// <summary>
    /// ���׷��̵� ���� �� ȿ�� �ȳ� UI ������Ʈ
    /// </summary>
    /// <param name="i">���׷��̵� ����</param>
    private void UpdateUpgradeDisplay(int i)
    {
        // �������� ������ ���׷��̵� ������
        int[] levels = equipmentData.upgradeLevels[equippedRocketIndex];
        // ������ �ִ�ġ ������ ��, ��� �� ���׷��̵� ȿ�� ǥ��
        if (levels[i] < upgradeDatas[equippedRocketIndex].upgradeItems[i].Cost.Length)
        {
            upgradeGroups[i].cost_text.text = PlayManager.SetCommaOnDigit(upgradeDatas[equippedRocketIndex].upgradeItems[i].Cost[levels[i]]) + " Coin";
            upgradeGroups[i].effect_text.text = "+" + upgradeDatas[equippedRocketIndex].upgradeItems[i].Effect[levels[i]] * 100 + "% �� " + upgradeDatas[equippedRocketIndex].upgradeItems[i].Effect[levels[i] + 1] * 100 + "%";
        }
        // �ִ�� ���׷��̵� ���� ��, �Ϸ�� ���׷��̵�� ǥ��
        else
        {
            upgradeGroups[i].cost_text.text = "-";
            upgradeGroups[i].effect_text.text = "+" + upgradeDatas[equippedRocketIndex].upgradeItems[i].Effect[levels[i]] * 100 + "%";
        }
        // ���׷��̵� ���� �����̴� ����
        upgradeGroups[i].upgradeStatus_slider.value = levels[i];
    }

    /// <summary>
    /// ���� ���׷��̵� 
    /// </summary>
    /// <param name="select">���õ� ���׷��̵� ���(Inspector)</param>
    public void UpgradeRocket(UpgradeItemSelect select)
    {
        // ���׷��̵� ����� �о��
        RocketUpgradeItem item = select.GetUpgradeType();
        // ���׷��̵� ������ �������� Ȯ��
        if(equipmentData.upgradeLevels[equippedRocketIndex][(int)item] < upgradeDatas[equippedRocketIndex].upgradeItems[(int)item].Effect.Length - 1)
        {
            // ���� �������� ���׷��̵� ���� ���� Ȯ��
            if(EconomicMgr.instance.PurchaseByCoin(upgradeDatas[equippedRocketIndex].upgradeItems[(int)item].Cost[equipmentData.upgradeLevels[equippedRocketIndex][(int)item]]))
            {
                // ���׷��̵� ���� ���� �� ���� UI ������Ʈ
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
    /// ������ ����� ��ü
    /// </summary>
    /// <param name="weapon">������ ����</param>
    public void ReplaceWeapon(WeaponType weapon)
    {
        ReplaceWeapon((int)weapon);
    }

    /// <summary>
    /// ������ ����� ��ü
    /// </summary>
    /// <param name="weapon">������ ����(Inspector)</param>
    public void ReplaceWeapon(WeaponSelect weapon)
    {
        ReplaceWeapon((int)weapon.weapon);
    }

    /// <summary>
    /// ������ ����� ��ü
    /// </summary>
    /// <param name="weaponCode">������ ������ �ε���</param>
    private void ReplaceWeapon(int weaponCode)
    {
        // �̹� ���� ���� �������� Ȯ��
        if (weaponCode != equippedWeaponIndex)
        {
            // ������ ���� ���� �ε� �� ���������� Ȯ��
            Weapon weapon = weapons_prefab[weaponCode].GetComponent<Weapon>();
            if (weapon.GetHasItem())
            {
                // ����-���� ���� ȣȯ�� Ȯ��
                if (rocketSet.rocketData.weaponCompatibility[weaponCode])
                {
                    // ���� ��ü �� ���� UI ������Ʈ
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
            // �̹� ������ ������ ���, ���� ����
            equippedWeaponSignImages[equippedWeaponIndex].SetActive(false);
            equippedWeaponIndex = -1;
            weaponManager.weapon = null;
            LoadWeapon(equippedWeaponIndex);
        }
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    /// <param name="index">���� �ε���</param>
    public void LoadWeapon(int index)
    {
        // ���������� �ƴ� ���
        if (index >= 0)
        {
            // ����-���� ȣȯ�� Ȯ��
            if (!rocketSets_preset[equippedRocketIndex].rocketData.weaponCompatibility[index])
            {
                // ȣȯ���� ���� ��� ���� ���� ó�� �� �ȳ� �޽��� ���
                ReplaceWeapon(index);
                weaponManager.weapon = null;
                androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("weapon_compatibility"));
                weaponManager.UpdateRemainBullet();
                return;
            }

            // �ش� ���⸦ �����ߴٴ� UI ������Ʈ
            equippedWeaponSignImages[equippedWeaponIndex].SetActive(true);
            // �÷��� ���� ���, ���� �������� ���� �����Ͽ� �ʱ�ȭ �� źȯ ����
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
        // �÷��� ���� ��쿡 ���� ���� ó��
        else
        {
            if (SceneManager.GetActiveScene().buildIndex.Equals(1))
                if (loadedWeaponObj != null)
                    Destroy(loadedWeaponObj);
        }

        // ������ ���� źȯ ���� ������Ʈ
        weaponManager.UpdateRemainBullet();
    }

    /// <summary>
    /// �ʱ� �ε� ���� �����Ͽ� �ణ�� �����̸� �ΰ� �����ϴ� �ڷ�ƾ
    /// </summary>
    IEnumerator ReloadDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        weaponManager.weapon.ReloadBullet();
    }

    /// <summary>
    /// �� ������ ���� źȯ ���� ������Ʈ
    /// </summary>
    private void UpdateBulletAmount()
    {
        for(int i = 0; i < bulletAmounts_text.Length; i++)
        {
            bulletAmounts_text[i].text = weapons_prefab[i].weaponSpecData.weaponData.numOfBulletLeft + "/" + weapons_prefab[i].weaponSpecData.BulletCapacity;
        }
    }

    /// <summary>
    /// ������ ��ü
    /// </summary>
    /// <param name="item">������ ������ ����</param>
    public void ReplaceItem(ItemManager.ItemType item)
    {
        ReplaceItem((int)item);
    }

    /// <summary>
    /// ������ ��ü
    /// </summary>
    /// <param name="itemSelect">������ ������ ����(Inspector)</param>
    public void ReplaceItem(ItemSelect itemSelect)
    {
        ReplaceItem((int)itemSelect.itemType);
    }

    /// <summary>
    /// ������ ������ �̹��� ������Ʈ
    /// </summary>
    public void UpdateEquipItemImage()
    {
        // ������ �������� ���� ��
        if(ItemManager.equippedItem != null)
        {
            // (���â) ������ ������ ǥ��
            equippedItemSignImages[(int)ItemManager.equippedItem].SetActive(true);
            // ������ ������ ������ ǥ�� ��Ȱ��ȭ
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
    /// ������ ��ü
    /// </summary>
    /// <param name="type">������ �ε���</param>
    public void ReplaceItem(int type)
    {
        // �̹� ������ ���������� Ȯ��
        if(!ItemManager.equippedItem.Equals((ItemManager.ItemType?)type))
        {
            // �������� 1�� �̻� �����ϰ� �ִ��� Ȯ��
            if(itemManager.items[type].item.quantity > 0)
            {
                // ���� ������ ���
                if(type.Equals((int)ItemManager.ItemType.VernierEngine))
                {
                    // ���� ���� ȣȯ�� Ȯ��
                    if (rocketSets_preset[equippedRocketIndex].vernierEngine == null)
                    {
                        androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("vernier_compatibility"));
                        return;
                    }
                    // ȣȯ�Ǹ� ���� ���� ����
                    else
                        LoadVernierEngine();
                }
                // �ν��� ������ ���
                else if(type.Equals((int)ItemManager.ItemType.BoosterEngine))
                {
                    // �ν��� ���� ������ ������ 0���� Ȯ��
                    if(statManager.GetLevel(StatType.Booster).Equals(0))
                    {
                        androidController.ShowMessage(LocalizationManager.instance.GetLocalizedValue("booster_unable"), 2f);
                        return;
                    }
                }

                // �������� �ű� ������ �ƴ� ��ü�� ���
                if (ItemManager.equippedItem != null)
                {
                    // ���� ������ ���� ���� ó��
                    equippedItemSignImages[(int)ItemManager.equippedItem].SetActive(false);
                    if (ItemManager.equippedItem.Equals(ItemManager.ItemType.VernierEngine))
                        RemoveVernierEngine();
                }

                // ������ ���� ó�� �� ������ ������ ���� ������Ʈ
                ItemManager.equippedItem = (ItemManager.ItemType)type;
                equippedItemSignImages[type].SetActive(true);
                if (itemManager.quantity_text != null)
                    itemManager.UpdateItemQuantityText();

                // �÷��� ������ ���� ���� ������ �� ���, �̿� ���� ���� ������ ������Ʈ
                if (type.Equals((int)ItemManager.ItemType.VernierEngine) && SceneManager.GetActiveScene().buildIndex.Equals(1))
                    Launch.instance.SetFuelStageText();

                dataManager.SaveData();
            }
        }
        // �̹� �������� �������� ������ ���, ���� ���� ó��
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
    /// �� Start �� ���� ���� �ε�
    /// </summary>
    private void LoadVernierEngineOnStart()
    {
        if (ItemManager.equippedItem.Equals(ItemManager.ItemType.VernierEngine))
            LoadVernierEngine();
    }

    /// <summary>
    /// ���� ���� ����
    /// </summary>
    public void LoadVernierEngine()
    {
        // �������� ���� ������ �÷��� ���� ��쿡�� ����
        if (SceneManager.GetActiveScene().buildIndex.Equals(1))
        {
            // ������ ���Ͽ� ���� ���� ������ �߰�
            RocketSet.instance.vernierEngine.gameObject.SetActive(true);
            RocketSet.instance.rocket_rigids.Insert(0, RocketSet.instance.vernierEngine);
            RocketSet.instance.fuelMax.Insert(0, itemManager.items[(int)ItemManager.ItemType.VernierEngine].SubValues[0]);
            RocketSet.instance.positionsOnStage.Insert(0, Vector3.zero);
            RocketSet.instance.powerMax.Insert(0, itemManager.items[(int)ItemManager.ItemType.VernierEngine].MainValue);
            RocketSet.instance.FuelRemain = itemManager.items[(int)ItemManager.ItemType.VernierEngine].SubValues[0];
        }
    }

    /// <summary>
    /// �������� ����
    /// </summary>
    public void RemoveVernierEngine()
    {
        // �������� ���� ������ �÷��� �������� ���� �۾�
        if (SceneManager.GetActiveScene().buildIndex.Equals(1))
        {
            // ���� ���� ������ ����
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
    /// �� �������� ���� ���� ������Ʈ
    /// </summary>
    public void UpdateItemQuantity()
    {
        for (int i = 0; i < itemQuantityTexts.Length; i++)
            itemQuantityTexts[i].text = itemManager.items[i].item.quantity.ToString();
    }

    /// <summary>
    /// �����ϰ� �ִ� ���� �� ���� ���� �̹��� ���� ������Ʈ
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
    /// ��� ������ ����
    /// </summary>
    /// <param name="data">����(write) ��� ������</param>
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
    /// ��� ������ �ε�
    /// </summary>
    /// <param name="data">�ҷ���(loaded) ������</param>
    public void LoadData(Data data)
    {
        // �ҷ��� ������ ����
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
        // ��� ������ �ʱ�ȭ
        else
        {
            equipmentData = new EquipmentData();

            equippedRocketIndex = 0;  // Default:  0 (Bottle Rocket)
            equippedWeaponIndex = -1; // Default: -1 (None)    
        }

        // ���׷��̵� ������ ������ ��� �߰�
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
/// ���׷��̵� ���� UI �׷�
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
/// ��� ������
/// </summary>
[System.Serializable]
public class EquipmentData
{
    /// <summary>
    /// ���� ���� ����
    /// </summary>
    public int eqippedRocket;
    /// <summary>
    /// ���� ���� ����
    /// </summary>
    public int equippedWeapon;
    /// <summary>
    /// ���� ���� ������
    /// </summary>
    public ItemManager.ItemType? equippedItemType;
    /// <summary>
    /// ���� ������ ������
    /// </summary>
    public List<WeaponData> weaponItemData;

    /// <summary>
    /// ���Ϻ� ���׷��̵� ����
    /// </summary>
    public List<int[]> upgradeLevels;


    public EquipmentData()
    {
        upgradeLevels = new List<int[]>();
    }
}