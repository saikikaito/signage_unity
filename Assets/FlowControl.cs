using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
/*
 　カメラスクロール時にParticle Systemに動きを与えるスクリプト
*/
public class FlowControl : MonoBehaviour
{
	[SerializeField] AnimationCurve _scroll_curve;
	[SerializeField] float _st_pos, _end_pos;
	[SerializeField] float _scroll_val;
	[SerializeField] ParticleSystemForceField _forceField;
	[SerializeField] float _force_pawer = 10f;
	float _vec = 0f, _pos = 0.5f, _force_pos = 0f;

	System.DateTime _se_prev;

	// Start is called before the first frame update
	void Start()
    {
		_se_prev = System.DateTime.Now;
	}

    //Particle Systemのx座標をカメラ移動の大きさに合わせて変える(最大値と最小値はClampで設定済み)
    void Update() { 
		_pos += Mathf.Clamp(_vec * 0.1f, -0.002f, 0.002f);
		_vec *= 0.99f;
		transform.localPosition = new Vector3(
			_scroll_curve.Evaluate(_pos) * _end_pos,
			0f);

		if(_pos < 0f) _pos = 0f;
		if(_pos > 1f)  _pos = 1f;
		//実際にx座標を動かしてるところ
		_forceField.directionX = _force_pos;
		_force_pos += _vec * _force_pawer;
		_force_pos *= 0.98f;
		//最大までスクロールしたらそこで固定
		if(_force_pos < -50) _force_pos = -50;
		if(_force_pos > 50) _force_pos = 50;

		//Debug.Log($"_pos:{_pos}");
		Debug.Log($"transform.localPosition.x:{transform.localPosition.x}");
		Debug.Log($"transform.localPosition.y:{transform.localPosition.y}");
	}
	
	public void Calculate(Vector2[] flow) {
		int w = OpticalFlow.OpticalFlow.Instance.Flow.width;
		int h = OpticalFlow.OpticalFlow.Instance.Flow.height;
		Vector2 pos = new Vector2();
		int av_count = 0;
		Vector2 av_vec = Vector2.zero;
		for(int i = 0; i < h; i++) {
			for(int j = 0; j < w; j++) {
				int index = w * i + j;

				pos.x = (float)(((float)j / (float)w) * (float)Screen.width);
				//pos.x = (float)Screen.width - (((float)j / (float)w) * (float)Screen.width);
				
				pos.y = (float)(float)Screen.height - ((float)i / (float)h * (float)Screen.height);
				//pos.y = ((float)i / (float)h) * (float)Screen.height;
				//Debug.Log($"pos.x:{pos.x}");
				//Debug.Log($"pos.y:{pos.y}");
				if (flow[index].magnitude > 0.1f) {
					//Vector2 vec = new Vector2(-flow[index].x, flow[index].y);
					Vector2 vec = new Vector2(-flow[index].x, flow[index].y);
					// 縦属性の強いベクトルは使わない
					if(Mathf.Abs(vec.x) > Mathf.Abs(vec.y)) {
						av_vec += vec * _scroll_val;
						av_count++;
					}
				}
			}
		}
		//Debug.Log($"pos.x:{pos.x}");
		//Debug.Log($"pos.y:{pos.y}");
		//SEManagerのインスタンス生成とSE再生
		if (av_count > 10) {
			_vec -= av_vec.x;
			if((System.DateTime.Now - _se_prev).TotalSeconds > 3f) {
				SEManager.Instance.Play("se");
				_se_prev = System.DateTime.Now;
			}
		}
	}
}
