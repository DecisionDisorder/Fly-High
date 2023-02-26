using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettleBox : MonoBehaviour
{
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag.Equals("EndPoint"))
        {
            ParkingGameManager.instance.StageSuccess();
        }
    }
}
