using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{

    public float moving_speed = 1;  // カメラの移動速度
    public Rigidbody rigitbody;

    void Start()
    {
        rigitbody = GetComponent<Rigidbody>();
    }

    // 前移動のボタンを押している間実行する
    public void MoveCameraForward()
    {
        Vector3 direction = Vector3.Scale(transform.forward, new Vector3(1, 0, 1));  // 縦方向を0としたカメラの向きを取得する
        rigitbody.velocity = moving_speed * direction;  // カメラの速度ベクトルを変更して移動させる
    }

    // 移動のボタンを離したら実行する
    public void StopCameraMove()
    {
        rigitbody.velocity = new Vector3(0, 0, 0);  // カメラの速度ベクトルを0にして移動を止める
    }
}
