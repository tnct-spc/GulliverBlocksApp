using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    public bool Go_or_Stop = false;
    public float moving_speed = 1;  // カメラの移動速度
    public Rigidbody rigitbody;

    void Start()
    {
        rigitbody = GetComponent<Rigidbody>();
        Input.gyro.enabled = true;
    }


    void Update()
    {
        /* ジャイロ機能 */
        transform.rotation = Quaternion.AngleAxis(90.0f, Vector3.right) * Input.gyro.attitude * Quaternion.AngleAxis(180.0f, Vector3.forward);

        /* カメラの移動 */
        Vector3 direction = Vector3.Scale(transform.forward, new Vector3(1, 0, 1));  // 縦方向を0としたカメラの向きを取得する
        if (Go_or_Stop) rigitbody.velocity = moving_speed * direction;  // カメラの速度ベクトルを変更して移動させる
        else rigitbody.velocity = new Vector3(0, 0, 0);  // カメラの速度ベクトルを0にして移動を止める
    }

    // 前移動のボタンを押している間実行する
    public void AllowMoveCamera()
    {
        Go_or_Stop = true;
    }

    // 移動のボタンを離したら実行する
    public void StopCamera()
    {
        Go_or_Stop = false;
    }
}
