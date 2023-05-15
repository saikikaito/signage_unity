using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
  背景(月)変更用スクリプト
  (必要ない場合はspriteRendererのInspectorからスクリプトの非アクティブ化をしてください)

  ・夜に変わった後、月のたぬ吉がフェードインする。
→たぬ吉がフェードインの時間を0にしてください。0にするのが難しいのであれば極力0に近づけてください。
　→パッと出るイメージ。(完了)

・月のたぬ吉がフェードインした後、たぬ吉のサウンドを鳴らす。
→「0409_SE_TANU_kyotu_pre.wav」を使ってみてください。(完了)

・（新）たぬ吉のサウンドが終わったら、月のたぬ吉を消してください。
→たぬ吉がいない月夜の背景に切り替えてください。時間尺でOKです。
*/
public class SpriteChange : MonoBehaviour
{
    //フェードさせたいオブジェクト
    [SerializeField, Header("昼に表示させるSpriteをComponentに持ったゲームオブジェクト"), Tooltip("こっちにアタッチした背景画像が\n最初に表示されます")]
    private SpriteRenderer spriteRenderer;
    [SerializeField, Header("夜に表示させるSpriteをComponentに持ったゲームオブジェクト")]
    private SpriteRenderer spriteRenderer2;
    [SerializeField, Header("まれに表示させるSpriteをComponentに持ったゲームオブジェクト")]
    private SpriteRenderer spriteRenderer3;

    //参照先のオブジェクトとスクリプト
    GameObject s_Change_Obj,se_Object;
    Sprite_Change_c spriteChange;
    Tanuki_SE_Script tanuki_Se_sc;

    /// <summary>
    /// アルファ値(スクリプト側から変更する場合は0～1の間,値はfloat)
    /// </summary>
    private float spriterenderer_alpha = 1;
    /// <summary>
    /// アルファ値(スクリプト側から変更する場合は0～1の間,値はfloat)
    /// </summary>
    private float spriterenderer2_alpha = 0;
    /// <summary>
    /// アルファ値(スクリプト側から変更する場合は0～1の間,値はfloat)
    /// </summary>
    private float spriterenderer3_alpha = 0;

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
    private float tanu_red, tanu_green, tanu_blue;

    //たぬきを出すか判定用変数
    private bool random_tanuki_seed_ = false;
    [HideInInspector] //スクリプトの値を優先
    public int random_Appearance_;
    private bool is_tanuki_ = false;
    private bool is_fade_tanuki_ = false;

    /// <summary>
    /// たぬきの月出現用時間計測変数
    /// </summary>
    private float tanukimoon_fadain_time_;
    
    //SEを鳴らすか判定用
    [HideInInspector]
    public int tanuki_se_ring_ = 0;
    [HideInInspector]
    public bool Se_ring_ = false;
    private float tanuki_fadeout_count_ = 0.0f;
    private float delete_count_ = 0.0f;

    [SerializeField, Header("夜になってから何秒後にたぬきをフェードさせるかの時間(秒数指定)\n※ここに指定した時間とフェードにかかる時間が夜の時間を超えないようにね")]
    [Tooltip("まぁ、超えないようにRangeで固定してあるんですけどね!!")]
    [Range(0f,5f)]
    private float tanuki_Appearance_time_;

    [SerializeField, Header("たぬきのフェードにかかる時間(秒数指定)")]
    [Range(1f,3f)]
    private float tanuki_fadeChange_time_;

    void Start()
    {
        //参照する変数を持ってるオブジェクトをヒエラルキーから探す
        s_Change_Obj = GameObject.Find("c");
        spriteChange = s_Change_Obj.GetComponent<Sprite_Change_c>();

        se_Object = GameObject.Find("tanuki_se");
        tanuki_Se_sc = se_Object.GetComponent<Tanuki_SE_Script>();

        //colorの初期化とフェードイン用float変数初期化(RGBの変更はしないのでタプルを使用)
        (red, red2, tanu_red) = (spriteRenderer.GetComponent<SpriteRenderer>().color.r, spriteRenderer2.GetComponent<SpriteRenderer>().color.r, spriteRenderer3.GetComponent<SpriteRenderer>().color.r);
        (green, green2, tanu_green) = (spriteRenderer.GetComponent<SpriteRenderer>().color.g, spriteRenderer2.GetComponent<SpriteRenderer>().color.g, spriteRenderer3.GetComponent<SpriteRenderer>().color.g);
        (blue, blue2, tanu_blue) = (spriteRenderer.GetComponent<SpriteRenderer>().color.b, spriteRenderer2.GetComponent<SpriteRenderer>().color.b, spriteRenderer3.GetComponent<SpriteRenderer>().color.b);

        spriteRenderer.color = new(red, green, blue, spriterenderer_alpha);
        spriteRenderer2.color = new(red2, green2, blue2, spriterenderer2_alpha);
        spriteRenderer3.color = new(tanu_red, tanu_green, tanu_blue, spriterenderer3_alpha);

        fadein_time = fade_change_time;

        tanukimoon_fadain_time_ = tanuki_fadeChange_time_;
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
            //タヌキを出すかの抽選
            if (!random_tanuki_seed_)
            {
                random_Appearance_ = spriteChange.random_val_;
                Debug.Log($"SpriteChangeの乱数:{random_Appearance_}");
                random_tanuki_seed_ = true;
            }

            if (random_Appearance_ > 4)
            {
                is_tanuki_ = true;
                is_fade_tanuki_ = true;
            }

            //昼→夜への変更
            //フェードインならカウントダウン、フェードアウトならカウントアップのタイマーを使用
            fadeout_time += Time.deltaTime;
            fadein_time -= Time.deltaTime;
            spriterenderer_alpha = 1.0f - fadeout_time / fade_change_time;
            /*if(!is_tanuki_) */spriterenderer2_alpha = 1.0f - fadein_time / fade_change_time; //たぬき抽選失敗
            //if(is_tanuki_) spriterenderer3_alpha = 1.0f - fadein_time / fade_change_time; //たぬき抽選成功

            spriteRenderer.color = new(red, green, blue, spriterenderer_alpha);
            /*if (!is_tanuki_) */spriteRenderer2.color = new(red2, green2, blue2, spriterenderer2_alpha); //たぬき抽選失敗
            //if (is_tanuki_) spriteRenderer3.color = new(tanu_red, tanu_green, tanu_blue, spriterenderer3_alpha); //たぬき抽選成功

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

            //指定した秒数経過でフェードインさせる
            if (is_fade_tanuki_ && is_tanuki_ && time_count_night > tanuki_Appearance_time_)
            {
                //たぬきフェードイン
                tanukimoon_fadain_time_ -= Time.deltaTime;

                //if (is_tanuki_) spriterenderer3_alpha = 1.0f - tanukimoon_fadain_time_ / tanuki_fadeChange_time_;
                if (is_tanuki_) spriterenderer3_alpha = 1.0f - tanukimoon_fadain_time_ / 0.0f;
                if (is_tanuki_) spriteRenderer3.color = new(tanu_red, tanu_green, tanu_blue, spriterenderer3_alpha);

                tanuki_se_ring_ = 1;
                
                if (spriterenderer3_alpha >= 1)
                {
                    is_fade_tanuki_ = false;
                    Se_ring_ = true;
                }
            }
            //たぬきを消す
            if (Se_ring_)
            {
                delete_count_ += Time.deltaTime;
                if (delete_count_ > 1.0f)
                {
                    tanuki_fadeout_count_ += Time.deltaTime;
                    if (is_tanuki_) spriterenderer3_alpha = 1.0f - tanuki_fadeout_count_ / tanuki_fadeChange_time_;
                    if (is_tanuki_) spriteRenderer3.color = new(tanu_red, tanu_green, tanu_blue, spriterenderer3_alpha);
                    if (spriterenderer3_alpha <= 0)
                    {
                        Se_ring_ = false;
                        delete_count_ = 0.0f;
                        tanuki_fadeout_count_ = 0.0f;
                    }
                }
            }
        }

        if (is_night && time_count_night > Start_fade_time)
        {
            //夜→昼への変更
            fadeout_time += Time.deltaTime;
            fadein_time -= Time.deltaTime;
            spriterenderer_alpha = 1.0f - fadein_time / fade_change_time;
            /*if (!is_tanuki_) */spriterenderer2_alpha = 1.0f - fadeout_time / fade_change_time; //たぬき抽選失敗
            //if (is_tanuki_) spriterenderer3_alpha = 1.0f - fadeout_time / fade_change_time; //たぬき抽選成功

            spriteRenderer.color = new(red, green, blue, spriterenderer_alpha);
            /*if (!is_tanuki_) */spriteRenderer2.color = new(red2, green2, blue2, spriterenderer2_alpha); //たぬき抽選失敗
            //if (is_tanuki_) spriteRenderer3.color = new(tanu_red, tanu_green, tanu_blue, spriterenderer3_alpha); //たぬき抽選成功

            if (spriterenderer_alpha >= 1)
            {
                //昼と夜のタイマーリセットと昼のタイマー計測開始(昼の生存フラグもtrueにする)
                is_night = false;
                is_AfterNoon = true;
                fadeout_time = timer_reset_;
                fadein_time = fade_change_time;
                time_count_night = timer_reset_;
                random_tanuki_seed_ = false;
                is_tanuki_ = false;
                tanukimoon_fadain_time_ = tanuki_fadeChange_time_;
                tanuki_se_ring_ = 0;
                Se_ring_ = false;
                tanuki_Se_sc.is_ring_ = false;
                tanuki_Se_sc.random_choose_ = false;
            }
        }

        //アルファ値が0以下または1以上になったら強制的に0か1に
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

        if (spriterenderer3_alpha < 0)
        {
            spriterenderer3_alpha = 0;
        }

        if (spriterenderer3_alpha > 1)
        {
            spriterenderer3_alpha = 1;
        }
    }
}