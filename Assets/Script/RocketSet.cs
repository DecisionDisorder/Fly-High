using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ �ΰ��� �����͸� �Ѱ��ϴ� Ŭ����
/// </summary>
[System.Serializable]
public class RocketSet : MonoBehaviour
{
    /// <summary>
    /// �跲 ������ ������ ��ȯ�ϴ� ���
    /// </summary>
    private const float BarrelToTon = 0.128f;
    /// <summary>
    /// ���� �Ѱ� ������ ������Ʈ�� �̱��� �ν��Ͻ�
    /// </summary>
    public static RocketSet instance;

    /// <summary>
    /// ���� �̸�
    /// </summary>
    public string Name;
    /// <summary>
    /// ������ �ִ� HP
    /// </summary>
    private int maxHp;
    /// <summary>
    /// ������ �ǽð� HP
    /// </summary>
    private int hp;
    /// <summary>
    /// ������ �ǽð� HP (Property)
    /// </summary>
    public int Hp { get { return hp; } 
        set 
        {
            // ����Ǵ� HP�� 0 ������ ��쿡 ���ӿ��� ó��
            if (value <= 0)
            {
                // ���� ����Ʈ�� �ִ� ���, ���� ����Ʈ ���
                if (explosionEffect != null)
                {
                    explosionEffect.Play();
                    for (int i = 0; i < rocket_rends.Length; i++)
                        rocket_rends[i].color = Color.black;
                }
                PlayManager.isSurvive = false;
            }
            // HP�� 1 �̻��� ��쿡 HP�ٷ� ����� HP�� ���� �ð����� Ȱ��ȭ �Ѵ�.
            else
            {
                hp = value;
                hpBar.SetActive(true);
                hpBar_SprtSlider.SetValue(value);
                SetRocketColorByHp();
                StartCoroutine(HpBarClose());
            }
        } 
    }
    /// <summary>
    /// ���� ���ᷮ
    /// </summary>
    private float fuelRemain;
    /// <summary>
    /// ���� ���ᷮ (Property)
    /// </summary>
    public float FuelRemain { get { return fuelRemain; } 
        set
        {
            // ���� ���� ���ᷮ ���� ū ���, ���� �������� ó��
            if (value > fuelRemain)
                if(Launch.instance != null)
                    Launch.instance.SupplyFuel(fuelRemain);
            // �ִ�ġ���� Ŀ���� ���, �ִ�ġ�� ����
            if (value > fuelMax[currentStage])
                fuelRemain = fuelMax[currentStage];
            else
            {
                // ���� ���ᰡ 0 ���Ϸ� ������ ���
                if(value <= 0)
                {
                    // ������ ���� �ƴ� ��쿡 ���� �и�
                    if (currentStage < fuelMax.Count - 1)
                    {
                        fuelRemain = fuelMax[currentStage + 1];
                        DisconnectStage();
                    }
                    // ������ ���̸�, ���� ���� 0���� ó��
                    else
                        fuelRemain = 0;
                }
                else
                    fuelRemain = value;
            }
            // ���� ���ᷮ�� ���� ���� ���� ���
            rocketMain_rigid.mass = rocketData.Weight[weightStage] + GetTotalRemainFuel() * BarrelToTon;
        } 
    }
    /// <summary>
    /// �� ���� �ִ� ���ᷮ ����Ʈ
    /// </summary>
    public List<float> fuelMax;
    /// <summary>
    /// ���⸦ �����ϱ� ���� ���� Transform
    /// </summary>
    public Transform weaponParent;

    /// <summary>
    /// �� ���� �ִ� ��� ����Ʈ
    /// </summary>
    public List<float> powerMax;

    /// <summary>
    /// �߻�� ���� ��
    /// </summary>
    public GameObject launcherAxis;
    /// <summary>
    /// ������ ���� ������Ʈ
    /// </summary>
    public GameObject rocket;
    /// <summary>
    /// ���� ��ü �̹���
    /// </summary>
    public Sprite rocket_image;
    /// <summary>
    /// �� ���� Sprite Renderer
    /// </summary>
    public SpriteRenderer[] rocket_rends;
    /// <summary>
    /// �� ���� RigidBody2D
    /// </summary>
    public List<Rigidbody2D> rocket_rigids;
    /// <summary>
    /// ���� ���� ���� RigidBody2D
    /// </summary>
    public Rigidbody2D rocketMain_rigid;
    /// <summary>
    /// ���� ���� ���� �� (0: n�� / 1: n-1�� [tail���� ���� ����])
    /// </summary>
    public int currentStage = 0;
    /// <summary>
    /// ���� ����� ���� �� Index
    /// </summary>
    public int weightStage = 0;
    /// <summary>
    /// �߻�� RigidBody2D
    /// </summary>
    public Rigidbody2D launcher_rigid;

    /// <summary>
    /// ���� ȭ�� ����Ʈ
    /// </summary>
    public ParticleSystem fireEffect;
    /// <summary>
    /// ���� ȭ�� ����Ʈ (�Ϲ� ����)
    /// </summary>
    public ParticleSystem normalFireEffect;
    /// <summary>
    /// ���� ȭ�� ����Ʈ (�ν��� ����)
    /// </summary>
    public ParticleSystem boosterFireEffect;
    /// <summary>
    /// ���Ͽ��� �� ���� ��� ��ġ
    /// </summary>
    public List<Vector3> positionsOnStage;
    /// <summary>
    /// �߻� ���� ���� ȿ��
    /// </summary>
    public ParticleSystem launchSmokeEffect;
    /// <summary>
    /// ���� ���� ������
    /// </summary>
    public RocketData rocketData;
    /// <summary>
    /// ���� HP�� ������Ʈ
    /// </summary>
    public GameObject hpBar;
    /// <summary>
    /// HP�� ��������Ʈ ��� �����̴� (Ŀ���� �����̴�)
    /// </summary>
    public SpriteSlider hpBar_SprtSlider;
    /// <summary>
    /// ���� ���� ó�� ����
    /// </summary>
    public bool isInvincible = false;
    /// <summary>
    /// HP�� ���� ���� ����
    /// </summary>
    public Color[] rocketColorByHp;
    /// <summary>
    /// ���� ȭ�� ȿ�� ���� ������Ʈ
    /// </summary>
    public RocketFire rocketFire;
    /// <summary>
    /// ���������� RigidBody2D
    /// </summary>
    public Rigidbody2D vernierEngine;
    /// <summary>
    /// �������� ���� ȭ�� ȿ��
    /// </summary>
    public ParticleSystem[] vernierEngineFires;
    /// <summary>
    /// ��ȣ�� �̹��� ��������Ʈ ������
    /// </summary>
    public SpriteRenderer shield;
    /// <summary>
    /// ��ȣ�� �ִϸ��̼� ȿ�� 
    /// </summary>
    public Animation shield_ani;
    /// <summary>
    /// ���� ���� ����Ʈ
    /// </summary>
    public ParticleSystem atmosphereEffect;
    /// <summary>
    /// ���� ���� ����Ʈ
    /// </summary>
    public ParticleSystem explosionEffect;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        InitializeRocket();
    }
    
    /// <summary>
    /// ���� �ʱ�ȭ
    /// </summary>
    public void InitializeRocket()
    {
        // ������ RigidBody ����
        rocketMain_rigid = rocket_rigids[rocket_rigids.Count - 1];
        // ���� �ִ� ������ �ʱ�ȭ (���� ������ ����)
        fuelMax = new List<float>(rocketData.Fuel);
        // ���� �ִ� ��·� �ʱ�ȭ (���� ������ ����)
        powerMax = new List<float>(rocketData.MaxPower);
        // ���� �ܿ����� ���� ����
        FuelRemain = fuelMax[currentStage];
        // �ִ� HP / �ǽð� HP �ʱ�ȭ
        maxHp = hp = rocketData.Hp;
        // ���� �ʱ�ȭ
        rocket_rigids[currentStage].mass = rocketData.Weight[weightStage];
        // �����̴� �ʱ�ȭ
        hpBar_SprtSlider.maxValue = hp;
        hpBar_SprtSlider.SetValue(hp);
    }

    /// <summary>
    /// ���� ������ ���������� ���� Ȯ��
    /// </summary>
    /// <returns>������ ���� ������ ���������� ����</returns>
    public bool IsRocketRight()
    {
        return rocketMain_rigid.velocity.x >= 0 ? true : false;
    }

    /// <summary>
    /// HP�� ��Ȱ��ȭ ��� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    IEnumerator HpBarClose()
    {
        yield return new WaitForSeconds(1.0f);

        hpBar.SetActive(false);
    }

    /// <summary>
    /// HP ������ ���� ���� ���� ����
    /// </summary>
    private void SetRocketColorByHp()
    {
        float per = hp / (float)maxHp;

        for (int i = 0; i < rocket_rends.Length; i++)
        {
            if (per <= 1)
                rocket_rends[i].color = rocketColorByHp[0];
            else if (per <= 0.5)
                rocket_rends[i].color = rocketColorByHp[1];
            else if (per < 0.3)
                rocket_rends[i].color = rocketColorByHp[2];
        }
    }

    /// <summary>
    /// ���� �� �и�
    /// </summary>
    public void DisconnectStage()
    {
        // ���� ��Ȱ��ȭ
        Launch.instance.AcceleratorUp();
        // �÷��̾ �ٽ� �������� ���ϵ��� ��ư ��Ȱ��ȭ
        Launch.instance.acceleratorButton.enabled = false;
        // �߻� ��ư ��Ȱ��ȭ
        Launch.instance.shot_button.SetActive(false);
        // �÷��̾� ȸ�� ���� ��Ȱ��ȭ
        Launch.instance.allowRotate = false;
        // �ӽ÷� �߷� 0���� ó��
        float tempGravity = rocketMain_rigid.gravityScale;
        rocketMain_rigid.gravityScale = 0f;
        StartCoroutine(BeforeDisconnectDelay(1f, tempGravity));
    }

    /// <summary>
    /// ���� �и� ���� 1�� ������ �ڷ�ƾ
    /// </summary>
    /// <param name="delay">���� �ð�</param>
    /// <param name="gravity">���� �߷� ��</param>
    IEnumerator BeforeDisconnectDelay(float delay, float gravity)
    {
        yield return new WaitForSeconds(delay);

        // ���� �и��� �ܼ��� ������ ����
        rocket_rigids[currentStage].bodyType = RigidbodyType2D.Dynamic;
        rocket_rigids[currentStage].velocity = rocketMain_rigid.velocity;
        rocket_rigids[currentStage].GetComponent<Collider2D>().isTrigger = true;
        // ���� �ܼ��� pass
        currentStage++;
        // ���������� ������ ���� ���� index�� pass
        if (!vernierEngine.gameObject.activeInHierarchy)
            weightStage++;
        // ���� �и� 2�� ������ �ڷ�ƾ ����
        StartCoroutine(DisconnectDelay(2f, gravity));
    }

    /// <summary>
    /// ���� �и� 2�� ������ �ڷ�ƾ
    /// </summary>
    /// <param name="delay">���� �ð�</param>
    /// <param name="gravity">���� �߷� ��</param>
    /// <returns></returns>
    IEnumerator DisconnectDelay(float delay, float gravity)
    {
        yield return new WaitForSeconds(delay);
        // �и��� ���� ��Ȱ��ȭ
        rocket_rigids[currentStage - 1].gameObject.SetActive(false);
        // �߷� �� ����
        rocketMain_rigid.gravityScale = gravity;
        // ȭ�� ����Ʈ ��ġ ����
        fireEffect.transform.localPosition = positionsOnStage[currentStage];
        // ���� ��� ����
        Launch.instance.stdPower = rocketData.MaxPower[currentStage];
        // ���� Ȱ��ȭ �� ���� ��Ʈ�� Ȱ��ȭ �۾�
        Launch.instance.AcceleratorDown();
        Launch.instance.acceleratorButton.enabled = true;
        Launch.instance.allowRotate = true;
        // ���ᷮ UI ������Ʈ
        Launch.instance.SetFuelStageText();
    }

    /// <summary>
    /// ������ ��� ���ᷮ �ջ�
    /// </summary>
    /// <returns></returns>
    private float GetTotalRemainFuel()
    {
        float sum = 0;
        for (int i = currentStage + 1; i < fuelMax.Count; i++)
            sum += fuelMax[i];
        sum += fuelRemain;
        return sum;
    }

    /// <summary>
    /// ���� ȭ�� ����Ʈ ��� ����
    /// </summary>
    public void FireEffectPlay()
    {
        if (ItemManager.equippedItem.Equals(ItemManager.ItemType.VernierEngine) && currentStage.Equals(0))
        {
            for (int i = 0; i < vernierEngineFires.Length; i++)
                vernierEngineFires[i].Play();
        }
        else
            fireEffect.Play();
    }
    /// <summary>
    /// ���� ȭ�� ����Ʈ �ߴ�
    /// </summary>
    public void FireEffectStop()
    {
        if (ItemManager.equippedItem.Equals(ItemManager.ItemType.VernierEngine) && currentStage.Equals(0))
        {
            for (int i = 0; i < vernierEngineFires.Length; i++)
                vernierEngineFires[i].Stop();
        }
        else
            fireEffect.Stop();
    }

    /// <summary>
    /// �ν��� ȭ�� ȿ�� �������
    /// </summary>
    public void BoosterFireEffectPlay()
    {
        fireEffect.Stop();
        boosterFireEffect.gameObject.SetActive(true);
        boosterFireEffect.Play();
        fireEffect = boosterFireEffect;
    }

    /// <summary>
    /// �ν��� ȭ�� ȿ�� �ߴ�
    /// </summary>
    public void BoosterFireEffectStop()
    {
        boosterFireEffect.Stop();
        boosterFireEffect.gameObject.SetActive(false);
        fireEffect = normalFireEffect;
        if (Launch.instance.isAccerlate)
            fireEffect.Play();
    }

    /// <summary>
    /// ���� ȿ�� Ȱ��ȭ
    /// </summary>
    /// <param name="time">Ȱ��ȭ �ð�</param>
    public void SetInvincible(float time)
    {
        isInvincible = true;
        StartCoroutine(InvincibleTime(time));
    }
    /// <summary>
    /// ���� ȿ�� ��Ȱ��ȭ ��� �ڷ�ƾ
    /// </summary>
    /// <param name="time">Ȱ��ȭ �ð�</param>
    /// <returns></returns>
    IEnumerator InvincibleTime(float time)
    {
        yield return new WaitForSeconds(time);

        isInvincible = false;
    }
}

