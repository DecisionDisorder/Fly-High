using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 비행 중인 로켓을 제어하는 클래스
/// </summary>
public class RocketFlight : MonoBehaviour
{
    /// <summary>
    /// 로켓의 RigidBody2D 컴포넌트
    /// </summary>
    public Rigidbody2D rocket_rigid;
    /// <summary>
    /// 코드 모음 빈 오브젝트
    /// </summary>
    public GameObject codeObj;

    public Launch launch;
    private RocketController rocketController;

    private void Awake()
    {
        // 무한모드에서 필요한 스크립트 로드 후 자세 제어 코루틴 시작
        if(SceneManager.GetActiveScene().buildIndex.Equals(1))
        {
            codeObj = GameObject.FindWithTag("Code");
            launch = codeObj.GetComponent<Launch>();
            StartCoroutine(RocketFly());
        }
        // 주차모드에서 필요한 스크립트 로드
        else if(SceneManager.GetActiveScene().buildIndex.Equals(2))
        {
            codeObj = GameObject.FindWithTag("Code");
            rocketController = codeObj.GetComponent<RocketController>();
        }
    }

    /// <summary>
    /// 무한모드에서 로켓이 나아가는 방향으로 회전하도록 자세 제어하는 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator RocketFly()
    {
        yield return new WaitForEndOfFrame();
        if (SceneManager.GetActiveScene().buildIndex.Equals(1))
        {
            // 가속중이 아니고, 발사 이후 생존 상태에서 사용자가 직접 회전 제어중이 아닐 때에만 제어
            if (!launch.isAccerlate && launch.isStart && PlayManager.isSurvive && !launch.isRotating)
            {
                // 로켓이 위를 보는 방향을 속도의 방향으로 제어한다.
                transform.up = rocket_rigid.velocity;
                if (RocketSet.instance.rocket.transform.rotation.x != 0)
                    transform.rotation = Quaternion.Euler(0, 0, 180);
            }
        }
            

        if (PlayManager.isSurvive)
            StartCoroutine(RocketFly());
    }

    /// <summary>
    /// 땅과 충돌할 때 게임 오버 처리
    /// </summary>
    /// <param name="collision">충돌체</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag.Equals("Ground"))
        {
            // 충돌 이후에 회전하지 않도록 제어
            rocket_rigid.angularVelocity = 0f;
            // 게임오버 처리
            PlayManager.isSurvive = false;
            // 로켓 물리적 고정
            rocket_rigid.bodyType = RigidbodyType2D.Static;
            // 로켓 폭발 이펙트가 있을 때 관련 이펙트 재생
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
