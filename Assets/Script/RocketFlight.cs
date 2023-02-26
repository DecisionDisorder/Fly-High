using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RocketFlight : MonoBehaviour
{
    public Launch launch;
    public Rigidbody2D rocket_rigid;
    public GameObject codeObj;
    private RocketController rocketController;

    private void Awake()
    {
        if(SceneManager.GetActiveScene().buildIndex.Equals(1))
        {
            codeObj = GameObject.FindWithTag("Code");
            launch = codeObj.GetComponent<Launch>();
            StartCoroutine(RocketFly());
        }
        else if(SceneManager.GetActiveScene().buildIndex.Equals(2))
        {
            codeObj = GameObject.FindWithTag("Code");
            rocketController = codeObj.GetComponent<RocketController>();
        }
    }

    IEnumerator RocketFly()
    {
        yield return new WaitForEndOfFrame();
        if (SceneManager.GetActiveScene().buildIndex.Equals(1))
        {
            if (!launch.isAccerlate && launch.isStart && PlayManager.isSurvive && !launch.isRotating)
            {
                transform.up = rocket_rigid.velocity;
                if (RocketSet.instance.rocket.transform.rotation.x != 0)
                    transform.rotation = Quaternion.Euler(0, 0, 180);
            }
        }
            

        if (PlayManager.isSurvive)
            StartCoroutine(RocketFly());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag.Equals("Ground"))
        {
            rocket_rigid.angularVelocity = 0f;
            PlayManager.isSurvive = false;
            rocket_rigid.bodyType = RigidbodyType2D.Static;
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
