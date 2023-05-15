using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
/*
  テクスチャの更新を行うスクリプト
*/

namespace OpticalFlow
{
    //クラスのシリアライズ化
    [System.Serializable]
    public class TextureUpdateEvent : UnityEvent<Texture> { }

    public class TextureUpdater : MonoBehaviour {
        //serializefieldにすることでInspector側での設定がAddListnerになる
        [SerializeField] protected TextureUpdateEvent textureUpdateEvent;
		//[SerializeField] protected TextureUpdateEvent resizeTextureUpdateEvent;

    }

}


