using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprite_Change_a_another : MonoBehaviour
{
    //フェードさせたいオブジェクト
    [SerializeField, Header("昼に表示させるSpriteをComponentに持ったゲームオブジェクト"), Tooltip("こっちにアタッチした背景画像が\n最初に表示されます")]
    private SpriteRenderer spriteRenderer;
    [SerializeField, Header("夜に表示させるSpriteをComponentに持ったゲームオブジェクト")]
    private SpriteRenderer spriteRenderer2;

    /// <summary>
    /// アルファ値(スクリプト側から変更する場合は0～1の間,値はfloat)
    /// </summary>
    private float spriterenderer_alpha = 1;
    /// <summary>
    /// アルファ値(スクリプト側から変更する場合は0～1の間,値はfloat)
    /// </summary>
    private float spriterenderer2_alpha = 0;

    [Space, Space, SerializeField, Header("フェード開始時間(秒数指定)")]
    private float Start_fade_time;

    [SerializeField, Header("フェードにかかる時間(秒数指定)")]
    private float fade_change_time;

    /// <summary>
    /// 背景表示用時間計測変数
    /// </summary>
    private float time_count_afternoon, time_count_night;

    /// <summary>
    /// フェードイン用時間計測変数(カウントダウンなのでfade_change_timeで要初期化)
    /// </summary>
    private float fadein_time;

    /// <summary>
    /// フェードアウト用時間計測変数
    /// </summary>
    private float fadeout_time;

    /// <summary>
    /// タイマーリセット定数
    /// </summary>
    private const float timer_reset_ = 0.0f;

    //昼夜判定用bool変数
    private bool is_AfterNoon = true;
    private bool is_night = false;

    /// <summary>
    /// RGB初期化用変数
    /// </summary>
    private float red, red2, green, green2, blue, blue2;

    //Camera camera;
    void Start()
    {
        //colorの初期化とフェードイン用float変数初期化(RGBの変更はしないのでタプルを使用)
        (red, red2) = (spriteRenderer.GetComponent<SpriteRenderer>().color.r, spriteRenderer2.GetComponent<SpriteRenderer>().color.r);
        (green, green2) = (spriteRenderer.GetComponent<SpriteRenderer>().color.g, spriteRenderer2.GetComponent<SpriteRenderer>().color.g);
        (blue, blue2) = (spriteRenderer.GetComponent<SpriteRenderer>().color.b, spriteRenderer2.GetComponent<SpriteRenderer>().color.b);

        spriteRenderer.color = new(red, green, blue, spriterenderer_alpha);
        spriteRenderer2.color = new(red2, green2, blue2, spriterenderer2_alpha);

        fadein_time = fade_change_time;
        //camera = Camera.main;
    }

    void Update()
    {
        //昼→夜のフェードを開始するまでの時間計測
        if (is_AfterNoon)
        {
            time_count_afternoon += Time.deltaTime;
        }
        //フェード開始時間になったらis_AfterNoonをfalseにしてタイマーリセット
        if (is_AfterNoon && time_count_afternoon > Start_fade_time)
        {
            is_AfterNoon = false;
            time_count_afternoon = timer_reset_;
        }

        if (!is_AfterNoon && !is_night)
        {
            //昼→夜への変更
            //フェードインならカウントダウン、フェードアウトならカウントアップのタイマーを使用
            fadeout_time += Time.deltaTime;
            fadein_time -= Time.deltaTime;
            spriterenderer_alpha = 1.0f - fadeout_time / fade_change_time;
            spriterenderer2_alpha = 1.0f - fadein_time / fade_change_time;
            spriteRenderer.color = new(red, green, blue, spriterenderer_alpha);
            spriteRenderer2.color = new(red2, green2, blue2, spriterenderer2_alpha);

            if (spriterenderer2_alpha >= 1)
            {
                //boolを夜にしてフェード用時間計測変数をリセット
                is_night = true;
                fadeout_time = timer_reset_;
                fadein_time = fade_change_time;
            }
        }

        if (is_night)
        {
            //夜の生存フラグが立ったら夜時間計測開始
            time_count_night += Time.deltaTime;
        }

        if (is_night && time_count_night > Start_fade_time)
        {
            //夜→昼への変更
            fadeout_time += Time.deltaTime;
            fadein_time -= Time.deltaTime;
            spriterenderer_alpha = 1.0f - fadein_time / fade_change_time;
            spriterenderer2_alpha = 1.0f - fadeout_time / fade_change_time;
            spriteRenderer.color = new(red, green, blue, spriterenderer_alpha);
            spriteRenderer2.color = new(red2, green2, blue2, spriterenderer2_alpha);

            if (spriterenderer_alpha >= 1)
            {
                //昼と夜のタイマーリセットと昼のタイマー計測開始(昼の生存フラグもtrueにする)
                is_night = false;
                is_AfterNoon = true;
                fadeout_time = timer_reset_;
                fadein_time = fade_change_time;
                time_count_night = timer_reset_;
            }
        }

        //0以下または1以上になったら強制的に0か1に
        if (spriterenderer_alpha < 0)
        {
            spriterenderer_alpha = 0;
        }

        if (spriterenderer_alpha > 1)
        {
            spriterenderer_alpha = 1;
        }

        if (spriterenderer2_alpha < 0)
        {
            spriterenderer2_alpha = 0;
        }

        if (spriterenderer2_alpha > 1)
        {
            spriterenderer2_alpha = 1;
        }
    }
}
