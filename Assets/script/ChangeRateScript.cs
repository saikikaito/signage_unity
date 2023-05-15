using UnityEngine;

/*
  ���������ɍg�t���������񐶐�����X�N���v�g 
*/
public class ChangeRateScript : MonoBehaviour
{
    private float time_count_ = 0.0f;
    private const float reset_time_ = 0.0f;

    [SerializeField]
    private Camera camera;

    //[SerializeField]
    //private Texture change_material_albedo_;

    //���������ăA�^�b�`���邱�Ƃł��̃X�N���v�g���A�^�b�`����Ă���Particle���g������
    ParticleSystem particleSystem;

    //�ύX�}�e���A��
    [SerializeField]
    Material noon_material;
    [SerializeField]
    Material night_material;

    //Particle��Main module��SimulateSpeed
    private const float normal_speed_ = 0.5f;
    private const float move_speed_ = 1.1f;
    private float now_speed_;
    private readonly float add_speed_ = 0.01f;
    private readonly float minus_speed_ = 0.004f;

    //Particle��Emission��Rate Over Time
    private float normal_emission_val_ = 20.0f;
    private float move_emission_val_ = 100.0f;

    private bool is_create_ = false;
    private bool is_move_ = false;

    private float pos_x_;

    ParticleSystem.EmissionModule emission;

    //ParticleSystemRenderer systemRenderer;

    //�Q�Ɛ�̃I�u�W�F�N�g�ƃX�N���v�g
    /*GameObject s_Change_Obj;
    Sprite_Change_c spriteChange;
    bool is_noon_change_ = false;
    bool is_night_change_ = false;*/
    void Start()
    {
        //�Q�Ƃ���ϐ��������Ă�I�u�W�F�N�g���q�G�����L�[����T��
        //s_Change_Obj = GameObject.Find("c");
        //spriteChange = s_Change_Obj.GetComponent<Sprite_Change_c>();

        pos_x_ = camera.transform.position.x;
        //�d���y���̂��߃L���b�V��
        particleSystem = GetComponent<ParticleSystem>();
        emission = particleSystem.emission;
        emission.rateOverTime = normal_emission_val_;

        now_speed_ = normal_speed_; //normal_speed_ 0.51f

        //�}�e���A���ύX
        //systemRenderer = GetComponent<ParticleSystemRenderer>();
        //systemRenderer.material = noon_material;
        //GetComponent<Renderer>().material.mainTexture = change_material_albedo_;
    }

    void Update()
    {
        //�����������̌v�Z
        var ans = pos_x_ - camera.transform.position.x;

        //�����ĂȂ��Ȃ琶���Ԋu�͂��̂܂�
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
        
        //���������̏���
        if (is_create_)
        {
            time_count_ += Time.deltaTime;
            if (time_count_ < 6.0f)
            {
                //Debug.Log("rate:100");

                //���x�ύX
                var partSystem = GetComponent<ParticleSystem>();
                var main = partSystem.main;
                if (!is_move_ && now_speed_ > move_speed_)
                {
                    //Debug.Log("���x������ɒB���܂���");
                    //Debug.Log($"now_speed_: {now_speed_}");
                    now_speed_ = move_speed_;
                    is_move_ = true;
                }
                //���������̑��x�㏸
                if (!is_move_ && now_speed_ < move_speed_)
                {
                    //Debug.Log("���x�㏸��");
                    //Debug.Log($"now_speed_: {now_speed_}");
                    now_speed_ += add_speed_; //0.01f
                }
                //���x�㏸���I������瑬�x��������茳�ɖ߂��Ă���
                if (is_move_ && now_speed_ > normal_speed_)
                {
                    //Debug.Log("���x������");
                    //Debug.Log($"now_speed_: {now_speed_}");
                    now_speed_ -= minus_speed_; //0.002f
                }
                main.simulationSpeed = now_speed_;

                //�����Ԋu�ύX
                emission = particleSystem.emission;
                emission.rateOverTime = move_emission_val_;
            }
            if (time_count_ > 6.1f)
            {
                //Debug.Log("���Z�b�g");
                //�t���O�Ǝ��Ԍv���ϐ����Z�b�g
                is_create_ = false;
                is_move_ = false;
                time_count_ = reset_time_;
                //���x�����ɖ߂�
                var partSystem = GetComponent<ParticleSystem>();
                var main = partSystem.main;
                now_speed_ = normal_speed_;
                main.simulationSpeed = now_speed_;

                //�����Ԋu�����ɖ߂�
                emission = particleSystem.emission;
                emission.rateOverTime = normal_emission_val_;
            }
        }
        //�O��̍��W����
        pos_x_ = camera.transform.position.x;

        //��
        /*if (!is_night_change_ && spriteChange.is_night)
        {
            systemRenderer.material = night_material;
            //systemRenderer.material.color = new(systemRenderer.material.color.r, systemRenderer.material.color.g, systemRenderer.material.color.b, systemRenderer.material.color.a);
            is_night_change_ = true;
            is_noon_change_ = false;
        }

        //��
        if(!is_noon_change_ && spriteChange.is_AfterNoon)
        {
            systemRenderer.material = noon_material;
            is_noon_change_ = true;
            is_night_change_ = false;
        }*/
    }
}
