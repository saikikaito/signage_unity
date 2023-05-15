using UnityEngine;

/*
  動いた時に紅葉をたくさん生成するスクリプト 
*/
public class ChangeRateScript : MonoBehaviour
{
    private float time_count_ = 0.0f;
    private const float reset_time_ = 0.0f;

    [SerializeField]
    private Camera camera;

    //[SerializeField]
    //private Texture change_material_albedo_;

    //こう書いてアタッチすることでこのスクリプトがアタッチされているParticle自身が入る
    ParticleSystem particleSystem;

    //変更マテリアル
    [SerializeField]
    Material noon_material;
    [SerializeField]
    Material night_material;

    //Particle→Main module→SimulateSpeed
    private const float normal_speed_ = 0.5f;
    private const float move_speed_ = 1.1f;
    private float now_speed_;
    private readonly float add_speed_ = 0.01f;
    private readonly float minus_speed_ = 0.004f;

    //Particle→Emission→Rate Over Time
    private float normal_emission_val_ = 20.0f;
    private float move_emission_val_ = 100.0f;

    private bool is_create_ = false;
    private bool is_move_ = false;

    private float pos_x_;

    ParticleSystem.EmissionModule emission;

    //ParticleSystemRenderer systemRenderer;

    //参照先のオブジェクトとスクリプト
    /*GameObject s_Change_Obj;
    Sprite_Change_c spriteChange;
    bool is_noon_change_ = false;
    bool is_night_change_ = false;*/
    void Start()
    {
        //参照する変数を持ってるオブジェクトをヒエラルキーから探す
        //s_Change_Obj = GameObject.Find("c");
        //spriteChange = s_Change_Obj.GetComponent<Sprite_Change_c>();

        pos_x_ = camera.transform.position.x;
        //重さ軽減のためキャッシュ
        particleSystem = GetComponent<ParticleSystem>();
        emission = particleSystem.emission;
        emission.rateOverTime = normal_emission_val_;

        now_speed_ = normal_speed_; //normal_speed_ 0.51f

        //マテリアル変更
        //systemRenderer = GetComponent<ParticleSystemRenderer>();
        //systemRenderer.material = noon_material;
        //GetComponent<Renderer>().material.mainTexture = change_material_albedo_;
    }

    void Update()
    {
        //動いた距離の計算
        var ans = pos_x_ - camera.transform.position.x;

        //動いてないなら生成間隔はそのまま
        if (!is_create_)
        {
            if (ans > -0.2 && ans < 0.2)
            {
                //Debug.Log("rate:20");
            }
            else
            {
                is_create_ = true;
            }
        }
        
        //動いた時の処理
        if (is_create_)
        {
            time_count_ += Time.deltaTime;
            if (time_count_ < 6.0f)
            {
                //Debug.Log("rate:100");

                //速度変更
                var partSystem = GetComponent<ParticleSystem>();
                var main = partSystem.main;
                if (!is_move_ && now_speed_ > move_speed_)
                {
                    //Debug.Log("速度が上限に達しました");
                    //Debug.Log($"now_speed_: {now_speed_}");
                    now_speed_ = move_speed_;
                    is_move_ = true;
                }
                //動いた時の速度上昇
                if (!is_move_ && now_speed_ < move_speed_)
                {
                    //Debug.Log("速度上昇中");
                    //Debug.Log($"now_speed_: {now_speed_}");
                    now_speed_ += add_speed_; //0.01f
                }
                //速度上昇が終わったら速度をゆっくり元に戻していく
                if (is_move_ && now_speed_ > normal_speed_)
                {
                    //Debug.Log("速度減少中");
                    //Debug.Log($"now_speed_: {now_speed_}");
                    now_speed_ -= minus_speed_; //0.002f
                }
                main.simulationSpeed = now_speed_;

                //生成間隔変更
                emission = particleSystem.emission;
                emission.rateOverTime = move_emission_val_;
            }
            if (time_count_ > 6.1f)
            {
                //Debug.Log("リセット");
                //フラグと時間計測変数リセット
                is_create_ = false;
                is_move_ = false;
                time_count_ = reset_time_;
                //速度を元に戻す
                var partSystem = GetComponent<ParticleSystem>();
                var main = partSystem.main;
                now_speed_ = normal_speed_;
                main.simulationSpeed = now_speed_;

                //生成間隔を元に戻す
                emission = particleSystem.emission;
                emission.rateOverTime = normal_emission_val_;
            }
        }
        //前回の座標を代入
        pos_x_ = camera.transform.position.x;

        //夜
        /*if (!is_night_change_ && spriteChange.is_night)
        {
            systemRenderer.material = night_material;
            //systemRenderer.material.color = new(systemRenderer.material.color.r, systemRenderer.material.color.g, systemRenderer.material.color.b, systemRenderer.material.color.a);
            is_night_change_ = true;
            is_noon_change_ = false;
        }

        //昼
        if(!is_noon_change_ && spriteChange.is_AfterNoon)
        {
            systemRenderer.material = noon_material;
            is_noon_change_ = true;
            is_night_change_ = false;
        }*/
    }
}
