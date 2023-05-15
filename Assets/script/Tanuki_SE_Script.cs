using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tanuki_SE_Script : MonoBehaviour
{
    //鳴らすSEとアタッチしたAudio Source
    public AudioClip sound1;
    AudioSource audioSource;

    //参照先のオブジェクトとスクリプト
    GameObject s_Change_Obj;
    SpriteChange spriteChange;

    private int choose_tanuki_se_val_;

    //他クラスの値参照用変数
    [HideInInspector]
    public bool random_choose_ = false;
    [HideInInspector]
    public bool is_ring_ = false;

    void Start()
    {
        //参照する変数を持ってるオブジェクトをヒエラルキーから探す
        s_Change_Obj = GameObject.Find("base_2_Afternoon");
        spriteChange = s_Change_Obj.GetComponent<SpriteChange>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        //たぬき抽選結果を取得して鳴らすか決定
        if (spriteChange.Se_ring_ && !random_choose_)
        {
            choose_tanuki_se_val_ = spriteChange.tanuki_se_ring_;
            //Debug.Log($"choose_tanuki_se_val_:{choose_tanuki_se_val_}");

            //1なら鳴らす 0ならスルー
            if (choose_tanuki_se_val_ == 1)
            {
                random_choose_ = true;
            }
            else
            {
                spriteChange.Se_ring_ = false;
                random_choose_ = false;
            }
        }

        if (random_choose_ && choose_tanuki_se_val_ == 1 && !is_ring_)
        {
            //Debug.Log("なりました。");
            audioSource.PlayOneShot(sound1);
            is_ring_ = true;
        }



        //if (!is_ring_ && choose_tanuki_se_val_ > 4)
        //{
        //    //音(sound1)を鳴らす
        //    audioSource.PlayOneShot(sound1);
        //    if (!audioSource.isPlaying)
        //    {
        //        random_choose_ = false;
        //    }
        //}
        
    }
}
