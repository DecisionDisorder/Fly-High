using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RocketSet : MonoBehaviour
{
    private const float BarrelToTon = 0.128f;
    public static RocketSet instance;

    public string Name;
    private int maxHp;
    private int hp;
    public int Hp { get { return hp; } 
        set 
        {
            if (value <= 0)
            {
                if (explosionEffect != null)
                {
                    explosionEffect.Play();
                    for (int i = 0; i < rocket_rends.Length; i++)
                        rocket_rends[i].color = Color.black;
                }
                PlayManager.isSurvive = false;
            }
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
    private float fuelRemain;
    public float FuelRemain { get { return fuelRemain; } 
        set
        {
            if (value > fuelRemain)
                if(Launch.instance != null)
                    Launch.instance.SupplyFuel(fuelRemain);
            if (value > fuelMax[currentStage])
                fuelRemain = fuelMax[currentStage];
            else
            {
                if(value <= 0)
                {
                    if (currentStage < fuelMax.Count - 1)
                    {
                        fuelRemain = fuelMax[currentStage + 1];
                        DisconnectStage();
                    }
                    else
                        fuelRemain = 0;
                }
                else
                    fuelRemain = value;
            }
            rocketMain_rigid.mass = rocketData.Weight[weightStage] + GetTotalRemainFuel() * BarrelToTon;
        } 
    }
    public List<float> fuelMax;
    public Transform weaponParent;

    public List<float> powerMax;

    public GameObject launcherAxis;
    public GameObject rocket;
    public Sprite rocket_image;
    public SpriteRenderer[] rocket_rends;
    public List<Rigidbody2D> rocket_rigids;
    public Rigidbody2D rocketMain_rigid;
    public int currentStage = 0; //0 : n단, 1 : n - 1단... (맨뒤부터)
    public int weightStage = 0;
    public Rigidbody2D launcher_rigid;
    public TextMesh bonus_textmesh;
    //public GameObject fire;
    public ParticleSystem fireEffect;
    public ParticleSystem normalFireEffect;
    public ParticleSystem boosterFireEffect;
    public List<Vector3> positionsOnStage;
    public ParticleSystem launchSmokeEffect;
    public RocketData rocketData;
    public GameObject hpBar;
    public SpriteSlider hpBar_SprtSlider;
    public bool isInvincible = false;
    public Color[] rocketColorByHp;
    public RocketFire rocketFire;
    public Rigidbody2D vernierEngine;
    public ParticleSystem[] vernierEngineFires;
    public SpriteRenderer shield;
    public Animation shield_ani;
    public ParticleSystem atmosphereEffect;
    public ParticleSystem explosionEffect;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        InitializeRocket();
    }

    public void InitializeRocket()
    {
        rocketMain_rigid = rocket_rigids[rocket_rigids.Count - 1];
        fuelMax = new List<float>(rocketData.Fuel);
        powerMax = new List<float>(rocketData.MaxPower);
        FuelRemain = fuelMax[currentStage];
        maxHp = hp = rocketData.Hp;
        rocket_rigids[currentStage].mass = rocketData.Weight[weightStage];
        hpBar_SprtSlider.maxValue = hp;
        hpBar_SprtSlider.SetValue(hp);
    }

    public bool IsRocketRight()
    {
        return rocketMain_rigid.velocity.x >= 0 ? true : false;
    }

    IEnumerator HpBarClose()
    {
        yield return new WaitForSeconds(1.0f);

        hpBar.SetActive(false);
    }

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

    public void DisconnectStage()
    {
        Launch.instance.AcceleratorUp();
        Launch.instance.acceleratorButton.enabled = false;
        Launch.instance.shot_button.SetActive(false);
        Launch.instance.allowRotate = false;
        float tempGravity = rocketMain_rigid.gravityScale;
        rocketMain_rigid.gravityScale = 0f;
        StartCoroutine(BeforeDisconnectDelay(1f, tempGravity));
    }

    IEnumerator BeforeDisconnectDelay(float delay, float gravity)
    {
        yield return new WaitForSeconds(delay);

        rocket_rigids[currentStage].bodyType = RigidbodyType2D.Dynamic;
        rocket_rigids[currentStage].velocity = rocketMain_rigid.velocity;
        rocket_rigids[currentStage].GetComponent<Collider2D>().isTrigger = true;
        currentStage++;
        if (!vernierEngine.gameObject.activeInHierarchy)
            weightStage++;
        StartCoroutine(DisconnectDelay(2f, gravity));
    }

    IEnumerator DisconnectDelay(float delay, float gravity)
    {
        yield return new WaitForSeconds(delay);
        rocket_rigids[currentStage - 1].gameObject.SetActive(false);
        rocketMain_rigid.gravityScale = gravity;
        fireEffect.transform.localPosition = positionsOnStage[currentStage];
        Launch.instance.stdPower = rocketData.MaxPower[currentStage];
        Launch.instance.AcceleratorDown();
        Launch.instance.acceleratorButton.enabled = true;
        Launch.instance.allowRotate = true;
        Launch.instance.SetFuelStageText();
    }

    private float GetTotalRemainFuel()
    {
        float sum = 0;
        for (int i = currentStage + 1; i < fuelMax.Count; i++)
            sum += fuelMax[i];
        sum += fuelRemain;
        return sum;
    }

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

    public void BoosterFireEffectPlay()
    {
        fireEffect.Stop();
        boosterFireEffect.gameObject.SetActive(true);
        boosterFireEffect.Play();
        fireEffect = boosterFireEffect;
    }

    public void BoosterFireEffectStop()
    {
        boosterFireEffect.Stop();
        boosterFireEffect.gameObject.SetActive(false);
        fireEffect = normalFireEffect;
        if (Launch.instance.isAccerlate)
            fireEffect.Play();
    }

    public void SetInvincible(float time)
    {
        isInvincible = true;
        StartCoroutine(InvincibleTime(time));
    }

    IEnumerator InvincibleTime(float time)
    {
        yield return new WaitForSeconds(time);

        isInvincible = false;
    }
}

