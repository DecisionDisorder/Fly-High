using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ���� ���� ������ �����ϴ� Ŭ����
/// </summary>
public class RocketFlight : MonoBehaviour
{
    /// <summary>
    /// ������ RigidBody2D ������Ʈ
    /// </summary>
    public Rigidbody2D rocket_rigid;
    /// <summary>
    /// �ڵ� ���� �� ������Ʈ
    /// </summary>
    public GameObject codeObj;

    public Launch launch;
    private RocketController rocketController;

    private void Awake()
    {
        // ���Ѹ�忡�� �ʿ��� ��ũ��Ʈ �ε� �� �ڼ� ���� �ڷ�ƾ ����
        if(SceneManager.GetActiveScene().buildIndex.Equals(1))
        {
            codeObj = GameObject.FindWithTag("Code");
            launch = codeObj.GetComponent<Launch>();
            StartCoroutine(RocketFly());
        }
        // ������忡�� �ʿ��� ��ũ��Ʈ �ε�
        else if(SceneManager.GetActiveScene().buildIndex.Equals(2))
        {
            codeObj = GameObject.FindWithTag("Code");
            rocketController = codeObj.GetComponent<RocketController>();
        }
    }

    /// <summary>
    /// ���Ѹ�忡�� ������ ���ư��� �������� ȸ���ϵ��� �ڼ� �����ϴ� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    IEnumerator RocketFly()
    {
        yield return new WaitForEndOfFrame();
        if (SceneManager.GetActiveScene().buildIndex.Equals(1))
        {
            // �������� �ƴϰ�, �߻� ���� ���� ���¿��� ����ڰ� ���� ȸ�� �������� �ƴ� ������ ����
            if (!launch.isAccerlate && launch.isStart && PlayManager.isSurvive && !launch.isRotating)
            {
                // ������ ���� ���� ������ �ӵ��� �������� �����Ѵ�.
                transform.up = rocket_rigid.velocity;
                if (RocketSet.instance.rocket.transform.rotation.x != 0)
                    transform.rotation = Quaternion.Euler(0, 0, 180);
            }
        }
            

        if (PlayManager.isSurvive)
            StartCoroutine(RocketFly());
    }

    /// <summary>
    /// ���� �浹�� �� ���� ���� ó��
    /// </summary>
    /// <param name="collision">�浹ü</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag.Equals("Ground"))
        {
            // �浹 ���Ŀ� ȸ������ �ʵ��� ����
            rocket_rigid.angularVelocity = 0f;
            // ���ӿ��� ó��
            PlayManager.isSurvive = false;
            // ���� ������ ����
            rocket_rigid.bodyType = RigidbodyType2D.Static;
            // ���� ���� ����Ʈ�� ���� �� ���� ����Ʈ ���
            if (RocketSet.instance.explosionEffect != null)
            {
                RocketSet.instance.explosionEffect.Play();
                if (launch != null)
                    launch.explosionAudio.Play();
                else if (rocketController != null)
                    rocketController.explosionAudio.Play();
                    
                for (int i = 0; i < RocketSet.instance.rocket_rends.Length; i++)
                    RocketSet.instance.rocket_rends[i].color = Color.black;
            }
        }
    }
}
