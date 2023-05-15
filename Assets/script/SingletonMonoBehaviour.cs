using System;
using UnityEngine;

/*
 MonoBehaviorをシングルトン化したクラス 
*/

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    //自動的にシングルトンが作られるようにする処理

    //共有したいクラスの宣言
    private static T instance;

    //インスタンスを取得できる唯一のプロパティ
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                Type t = typeof(T);

                instance = (T)FindObjectOfType(t);
                if (instance == null)
                {
                    Debug.LogError($"{t} をアタッチしているGameObjectはありません");
                }
            }

            return instance;
        }
    }

    //シーン間でもインスタンスが1つになるように
    virtual protected void Awake ()
    {
        // 他のGameObjectにアタッチされているか調べる.
        // アタッチされている場合は破棄する.
        if (this != Instance)
        {
            Destroy(this);
            //Destroy(this.gameObject);
            Debug.LogError(
                typeof(T) +
                " は既に他のGameObjectにアタッチされているため、コンポーネントを破棄しました." +
                " アタッチされているGameObjectは " + Instance.gameObject.name + " です.");
            return;
        }
    }

}
