using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/*
  動きの検知をするスクリプト
*/
namespace OpticalFlow
{
    //クラスのシリアライズ化
    [System.Serializable]
    public class FlowUpdaterEvent : UnityEvent<Vector2[]> { }

    public class FlowUpdater : SingletonMonoBehaviour<FlowUpdater> {

        //SerializeFieldにすることでInspector側での設定がAddlistnerになる
        [SerializeField] public FlowUpdaterEvent flowUpdateEvent;
    }
}
