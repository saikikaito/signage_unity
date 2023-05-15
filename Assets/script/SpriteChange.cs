using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
  �w�i(��)�ύX�p�X�N���v�g
  (�K�v�Ȃ��ꍇ��spriteRenderer��Inspector����X�N���v�g�̔�A�N�e�B�u�������Ă�������)

  �E��ɕς������A���̂��ʋg���t�F�[�h�C������B
�����ʋg���t�F�[�h�C���̎��Ԃ�0�ɂ��Ă��������B0�ɂ���̂�����̂ł���΋ɗ�0�ɋ߂Â��Ă��������B
�@���p�b�Əo��C���[�W�B(����)

�E���̂��ʋg���t�F�[�h�C��������A���ʋg�̃T�E���h��炷�B
���u0409_SE_TANU_kyotu_pre.wav�v���g���Ă݂Ă��������B(����)

�E�i�V�j���ʋg�̃T�E���h���I�������A���̂��ʋg�������Ă��������B
�����ʋg�����Ȃ�����̔w�i�ɐ؂�ւ��Ă��������B���Ԏڂ�OK�ł��B
*/
public class SpriteChange : MonoBehaviour
{
    //�t�F�[�h���������I�u�W�F�N�g
    [SerializeField, Header("���ɕ\��������Sprite��Component�Ɏ������Q�[���I�u�W�F�N�g"), Tooltip("�������ɃA�^�b�`�����w�i�摜��\n�ŏ��ɕ\������܂�")]
    private SpriteRenderer spriteRenderer;
    [SerializeField, Header("��ɕ\��������Sprite��Component�Ɏ������Q�[���I�u�W�F�N�g")]
    private SpriteRenderer spriteRenderer2;
    [SerializeField, Header("�܂�ɕ\��������Sprite��Component�Ɏ������Q�[���I�u�W�F�N�g")]
    private SpriteRenderer spriteRenderer3;

    //�Q�Ɛ�̃I�u�W�F�N�g�ƃX�N���v�g
    GameObject s_Change_Obj,se_Object;
    Sprite_Change_c spriteChange;
    Tanuki_SE_Script tanuki_Se_sc;

    /// <summary>
    /// �A���t�@�l(�X�N���v�g������ύX����ꍇ��0�`1�̊�,�l��float)
    /// </summary>
    private float spriterenderer_alpha = 1;
    /// <summary>
    /// �A���t�@�l(�X�N���v�g������ύX����ꍇ��0�`1�̊�,�l��float)
    /// </summary>
    private float spriterenderer2_alpha = 0;
    /// <summary>
    /// �A���t�@�l(�X�N���v�g������ύX����ꍇ��0�`1�̊�,�l��float)
    /// </summary>
    private float spriterenderer3_alpha = 0;

    [Space, Space, SerializeField, Header("�t�F�[�h�J�n����(�b���w��)")]
    private float Start_fade_time;

    [SerializeField, Header("�t�F�[�h�ɂ����鎞��(�b���w��)")]
    private float fade_change_time;

    /// <summary>
    /// �w�i�\���p���Ԍv���ϐ�
    /// </summary>
    private float time_count_afternoon, time_count_night;

    /// <summary>
    /// �t�F�[�h�C���p���Ԍv���ϐ�(�J�E���g�_�E���Ȃ̂�fade_change_time�ŗv������)
    /// </summary>
    private float fadein_time;

    /// <summary>
    /// �t�F�[�h�A�E�g�p���Ԍv���ϐ�
    /// </summary>
    private float fadeout_time;

    /// <summary>
    /// �^�C�}�[���Z�b�g�萔
    /// </summary>
    private const float timer_reset_ = 0.0f;

    //���锻��pbool�ϐ�
    private bool is_AfterNoon = true;
    private bool is_night = false;

    /// <summary>
    /// RGB�������p�ϐ�
    /// </summary>
    private float red, red2, green, green2, blue, blue2;
    private float tanu_red, tanu_green, tanu_blue;

    //���ʂ����o��������p�ϐ�
    private bool random_tanuki_seed_ = false;
    [HideInInspector] //�X�N���v�g�̒l��D��
    public int random_Appearance_;
    private bool is_tanuki_ = false;
    private bool is_fade_tanuki_ = false;

    /// <summary>
    /// ���ʂ��̌��o���p���Ԍv���ϐ�
    /// </summary>
    private float tanukimoon_fadain_time_;
    
    //SE��炷������p
    [HideInInspector]
    public int tanuki_se_ring_ = 0;
    [HideInInspector]
    public bool Se_ring_ = false;
    private float tanuki_fadeout_count_ = 0.0f;
    private float delete_count_ = 0.0f;

    [SerializeField, Header("��ɂȂ��Ă��牽�b��ɂ��ʂ����t�F�[�h�����邩�̎���(�b���w��)\n�������Ɏw�肵�����Ԃƃt�F�[�h�ɂ����鎞�Ԃ���̎��Ԃ𒴂��Ȃ��悤�ɂ�")]
    [Tooltip("�܂��A�����Ȃ��悤��Range�ŌŒ肵�Ă����ł����ǂ�!!")]
    [Range(0f,5f)]
    private float tanuki_Appearance_time_;

    [SerializeField, Header("���ʂ��̃t�F�[�h�ɂ����鎞��(�b���w��)")]
    [Range(1f,3f)]
    private float tanuki_fadeChange_time_;

    void Start()
    {
        //�Q�Ƃ���ϐ��������Ă�I�u�W�F�N�g���q�G�����L�[����T��
        s_Change_Obj = GameObject.Find("c");
        spriteChange = s_Change_Obj.GetComponent<Sprite_Change_c>();

        se_Object = GameObject.Find("tanuki_se");
        tanuki_Se_sc = se_Object.GetComponent<Tanuki_SE_Script>();

        //color�̏������ƃt�F�[�h�C���pfloat�ϐ�������(RGB�̕ύX�͂��Ȃ��̂Ń^�v�����g�p)
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
        //������̃t�F�[�h���J�n����܂ł̎��Ԍv��
        if (is_AfterNoon)
        {
            time_count_afternoon += Time.deltaTime;
        }
        //�t�F�[�h�J�n���ԂɂȂ�����is_AfterNoon��false�ɂ��ă^�C�}�[���Z�b�g
        if (is_AfterNoon && time_count_afternoon > Start_fade_time)
        {
            is_AfterNoon = false;
            time_count_afternoon = timer_reset_;
        }

        if (!is_AfterNoon && !is_night)
        {
            //�^�k�L���o�����̒��I
            if (!random_tanuki_seed_)
            {
                random_Appearance_ = spriteChange.random_val_;
                Debug.Log($"SpriteChange�̗���:{random_Appearance_}");
                random_tanuki_seed_ = true;
            }

            if (random_Appearance_ > 4)
            {
                is_tanuki_ = true;
                is_fade_tanuki_ = true;
            }

            //������ւ̕ύX
            //�t�F�[�h�C���Ȃ�J�E���g�_�E���A�t�F�[�h�A�E�g�Ȃ�J�E���g�A�b�v�̃^�C�}�[���g�p
            fadeout_time += Time.deltaTime;
            fadein_time -= Time.deltaTime;
            spriterenderer_alpha = 1.0f - fadeout_time / fade_change_time;
            /*if(!is_tanuki_) */spriterenderer2_alpha = 1.0f - fadein_time / fade_change_time; //���ʂ����I���s
            //if(is_tanuki_) spriterenderer3_alpha = 1.0f - fadein_time / fade_change_time; //���ʂ����I����

            spriteRenderer.color = new(red, green, blue, spriterenderer_alpha);
            /*if (!is_tanuki_) */spriteRenderer2.color = new(red2, green2, blue2, spriterenderer2_alpha); //���ʂ����I���s
            //if (is_tanuki_) spriteRenderer3.color = new(tanu_red, tanu_green, tanu_blue, spriterenderer3_alpha); //���ʂ����I����

            if (spriterenderer2_alpha >= 1)
            {
                //bool���ɂ��ăt�F�[�h�p���Ԍv���ϐ������Z�b�g
                is_night = true;
                fadeout_time = timer_reset_;
                fadein_time = fade_change_time;
            }
        }

        if (is_night)
        {
            //��̐����t���O����������鎞�Ԍv���J�n
            time_count_night += Time.deltaTime;

            //�w�肵���b���o�߂Ńt�F�[�h�C��������
            if (is_fade_tanuki_ && is_tanuki_ && time_count_night > tanuki_Appearance_time_)
            {
                //���ʂ��t�F�[�h�C��
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
            //���ʂ�������
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
            //�遨���ւ̕ύX
            fadeout_time += Time.deltaTime;
            fadein_time -= Time.deltaTime;
            spriterenderer_alpha = 1.0f - fadein_time / fade_change_time;
            /*if (!is_tanuki_) */spriterenderer2_alpha = 1.0f - fadeout_time / fade_change_time; //���ʂ����I���s
            //if (is_tanuki_) spriterenderer3_alpha = 1.0f - fadeout_time / fade_change_time; //���ʂ����I����

            spriteRenderer.color = new(red, green, blue, spriterenderer_alpha);
            /*if (!is_tanuki_) */spriteRenderer2.color = new(red2, green2, blue2, spriterenderer2_alpha); //���ʂ����I���s
            //if (is_tanuki_) spriteRenderer3.color = new(tanu_red, tanu_green, tanu_blue, spriterenderer3_alpha); //���ʂ����I����

            if (spriterenderer_alpha >= 1)
            {
                //���Ɩ�̃^�C�}�[���Z�b�g�ƒ��̃^�C�}�[�v���J�n(���̐����t���O��true�ɂ���)
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

        //�A���t�@�l��0�ȉ��܂���1�ȏ�ɂȂ����狭���I��0��1��
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