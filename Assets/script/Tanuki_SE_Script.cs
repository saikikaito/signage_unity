using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tanuki_SE_Script : MonoBehaviour
{
    //�炷SE�ƃA�^�b�`����Audio Source
    public AudioClip sound1;
    AudioSource audioSource;

    //�Q�Ɛ�̃I�u�W�F�N�g�ƃX�N���v�g
    GameObject s_Change_Obj;
    SpriteChange spriteChange;

    private int choose_tanuki_se_val_;

    //���N���X�̒l�Q�Ɨp�ϐ�
    [HideInInspector]
    public bool random_choose_ = false;
    [HideInInspector]
    public bool is_ring_ = false;

    void Start()
    {
        //�Q�Ƃ���ϐ��������Ă�I�u�W�F�N�g���q�G�����L�[����T��
        s_Change_Obj = GameObject.Find("base_2_Afternoon");
        spriteChange = s_Change_Obj.GetComponent<SpriteChange>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        //���ʂ����I���ʂ��擾���Ė炷������
        if (spriteChange.Se_ring_ && !random_choose_)
        {
            choose_tanuki_se_val_ = spriteChange.tanuki_se_ring_;
            //Debug.Log($"choose_tanuki_se_val_:{choose_tanuki_se_val_}");

            //1�Ȃ�炷 0�Ȃ�X���[
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
            //Debug.Log("�Ȃ�܂����B");
            audioSource.PlayOneShot(sound1);
            is_ring_ = true;
        }



        //if (!is_ring_ && choose_tanuki_se_val_ > 4)
        //{
        //    //��(sound1)��炷
        //    audioSource.PlayOneShot(sound1);
        //    if (!audioSource.isPlaying)
        //    {
        //        random_choose_ = false;
        //    }
        //}
        
    }
}
