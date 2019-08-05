using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; //追加を忘れないように！

//長押しを発生させるuGUI(ImageやText)にアタッチして使う
public class LongTap : MonoBehaviour
{
    public GameObject BackToTheGame;
    public GameObject RuntimeHierarchy;
    public GameObject RuntimeInspector;
    //EventTriggerをアタッチしておく
    public EventTrigger _EventTrigger;

    //StopCoroutineのためにCoroutineで宣言しておく
    Coroutine PressCorutine;
    bool isPressDown = false;
    float PressTime = 5f;

    void Awake()
    {
        //PointerDownイベントの登録
        EventTrigger.Entry pressdown = new EventTrigger.Entry();
        pressdown.eventID = EventTriggerType.PointerDown;
        pressdown.callback.AddListener((data) => PointerDown());
        _EventTrigger.triggers.Add(pressdown);

        //PointerUpイベントの登録
        EventTrigger.Entry pressup = new EventTrigger.Entry();
        pressup.eventID = EventTriggerType.PointerUp;
        
        _EventTrigger.triggers.Add(pressup);
    }

    //EventTriggerのPointerDownイベントに登録する処理
    void PointerDown()
    {
        Debug.Log("Press Start");
        //連続でタップした時に長押しにならないよう前のCoroutineを止める
        if (PressCorutine != null)
        {
            StopCoroutine(PressCorutine);
        }
        //StopCoroutineで止められるように予め宣言したCoroutineに代入
        PressCorutine = StartCoroutine(TimeForPointerDown());
    }

    //長押しコルーチン
    IEnumerator TimeForPointerDown()
    {
        //プレス開始
        isPressDown = true;

        //待機時間
        yield return new WaitForSeconds(PressTime);

        //押されたままなら長押しの挙動
        if (isPressDown)
        {
            Debug.Log("Long Press Done");

            //お好みの長押し時の挙動をここに書く
            this.BackToTheGame.SetActive(true);
            this.RuntimeInspector.SetActive(true);
            this.RuntimeHierarchy.SetActive(true);


        }
        //プレス処理終了
        isPressDown = false;
    }

   
}
