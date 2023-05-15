using UnityEngine;

/*
  c(��)�̃��C���[�ύX�X�N���v�g 
*/

public class Sprite_Change_c : MonoBehaviour
{
    //�t�F�[�h���������I�u�W�F�N�g
    [SerializeField, Header("���ɕ\��������Sprite��Component�Ɏ������Q�[���I�u�W�F�N�g"), Tooltip("�������ɃA�^�b�`�����w�i�摜��\n�ŏ��ɕ\������܂�")]
    private SpriteRenderer spriteRenderer;
    [SerializeField, Header("��ɕ\��������Sprite��Component�Ɏ������Q�[���I�u�W�F�N�g")]
    private SpriteRenderer spriteRenderer2;
    [SerializeField, Header("�܂�ɕ\��������Sprite��Component�Ɏ������Q�[���I�u�W�F�N�g")]
    private SpriteRenderer spriteRenderer3;

    //GameObject s_Change_Obj;
    //SpriteChange spriteChange;

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
    [HideInInspector]
    public bool is_AfterNoon = true;
    [HideInInspector]
    public bool is_night = false;

    /// <summary>
    /// RGB�������p�ϐ�
    /// </summary>
    private float red, red2, green, green2, blue, blue2;
    private float tanu_red, tanu_green, tanu_blue;

    //���ʂ����o��������p�ϐ�
    private bool random_tanuki_seed_ = false;
    [HideInInspector] //�X�N���v�g�̒l��D��
    public int random_val_;
    private bool is_tanuki_ = false;

    //Camera camera;
    void Start()
    {
        //color�̏������ƃt�F�[�h�C���pfloat�ϐ�������(RGB�̕ύX�͂��Ȃ��̂Ń^�v�����g�p)
        (red, red2, tanu_red) = (spriteRenderer.GetComponent<SpriteRenderer>().color.r, spriteRenderer2.GetComponent<SpriteRenderer>().color.r, spriteRenderer3.GetComponent<SpriteRenderer>().color.r);
        (green, green2, tanu_green) = (spriteRenderer.GetComponent<SpriteRenderer>().color.g, spriteRenderer2.GetComponent<SpriteRenderer>().color.g, spriteRenderer3.GetComponent<SpriteRenderer>().color.g);
        (blue, blue2, tanu_blue) = (spriteRenderer.GetComponent<SpriteRenderer>().color.b, spriteRenderer2.GetComponent<SpriteRenderer>().color.b, spriteRenderer3.GetComponent<SpriteRenderer>().color.b);

        spriteRenderer.color = new(red, green, blue, spriterenderer_alpha);
        spriteRenderer2.color = new(red2, green2, blue2, spriterenderer2_alpha);
        spriteRenderer3.color = new(tanu_red, tanu_green, tanu_blue, spriterenderer3_alpha);

        fadein_time = fade_change_time;
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
                random_val_ = Random.Range(1, 11);
                //random_val_ = 6;
                Debug.Log($"SpriteChange_c�̗���:{random_val_}");
                random_tanuki_seed_ = true;
            }

            if (random_val_ < 5) // > 4  < 4
            {
                is_tanuki_ = true;
            }

            //������ւ̕ύX
            //�t�F�[�h�C���Ȃ�J�E���g�_�E���A�t�F�[�h�A�E�g�Ȃ�J�E���g�A�b�v�̃^�C�}�[���g�p
            fadeout_time += Time.deltaTime;
            fadein_time -= Time.deltaTime;
            spriterenderer_alpha = 1.0f - fadeout_time / fade_change_time;
            if (!is_tanuki_) spriterenderer2_alpha = 1.0f - fadein_time / fade_change_time; //�Ƃ̒��̂��ʂ����I���s
            if (is_tanuki_) spriterenderer3_alpha = 1.0f - fadein_time / fade_change_time; //�Ƃ̒��̂��ʂ����I����

            spriteRenderer.color = new(red, green, blue, spriterenderer_alpha);
            if (!is_tanuki_) spriteRenderer2.color = new(red2, green2, blue2, spriterenderer2_alpha); //�Ƃ̒��̂��ʂ����I���s
            if (is_tanuki_) spriteRenderer3.color = new(tanu_red, tanu_green, tanu_blue, spriterenderer3_alpha); //�Ƃ̒��̂��ʂ����I����

            if (spriterenderer2_alpha >= 1 || spriterenderer3_alpha >= 1)
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
        }

        if (is_night && time_count_night > Start_fade_time)
        {
            //�遨���ւ̕ύX
            fadeout_time += Time.deltaTime;
            fadein_time -= Time.deltaTime;
            spriterenderer_alpha = 1.0f - fadein_time / fade_change_time;
            if (!is_tanuki_) spriterenderer2_alpha = 1.0f - fadeout_time / fade_change_time; //�Ƃ̒��̂��ʂ����I���s
            if (is_tanuki_) spriterenderer3_alpha = 1.0f - fadeout_time / fade_change_time; //�Ƃ̒��̂��ʂ����I����

            spriteRenderer.color = new(red, green, blue, spriterenderer_alpha);
            if (!is_tanuki_) spriteRenderer2.color = new(red2, green2, blue2, spriterenderer2_alpha); //�Ƃ̒��̂��ʂ����I���s
            if (is_tanuki_) spriteRenderer3.color = new(tanu_red, tanu_green, tanu_blue, spriterenderer3_alpha); //�Ƃ̒��̂��ʂ����I����

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
            }
        }

        //0�ȉ��܂���1�ȏ�ɂȂ����狭���I��0��1��
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
