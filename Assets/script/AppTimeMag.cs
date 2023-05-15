using UnityEngine;
using System;
using System.Collections;
/*
  アプリケーション実行管理スクリプト
*/
public class AppTimeMag : SingletonMonoBehaviour<AppTimeMag> 
{
	/// <summary>
	/// 実行パス
	/// </summary>
	private string _run_path;
	/// <summary>
	/// アプリケーション時間
	/// </summary>
	private int _app_time;
	/// <summary>
	/// 実行時間
	/// </summary>
	private int _run_time;

	/// <summary>
	/// 前回更新時の時間
	/// </summary>
	private DateTime _prev_time;

	/// <summary>
	/// 更新フラグ
	/// </summary>
	private bool _is_update;

	// Use this for initialization
	void Start () 
	{
		OnUpdate();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( _app_time != -1 )
		{
			// 実行時間の加算
			if( _is_update )
			{
				_run_time += (int)(DateTime.Now - _prev_time).TotalMilliseconds;
			}

			_prev_time = DateTime.Now;

			// アプリケーションの終了判定
			if( _run_time > _app_time )
			{
				Application.Quit();
			}
		}
	}

	void Awake()
	{
		//シングルトンのためのコード
		if( this != Instance ) 
		{
			Destroy (this.gameObject);
			return;
		}
		//シーン遷移しても破棄されない設定に
		DontDestroyOnLoad( this.gameObject );

		// コマンドの取得
		string[] cmd = System.Environment.GetCommandLineArgs();
		_run_path = cmd[0];

		// 引数が指定されているかチェック
		if( cmd.Length > 1 )
		{
			// 引数を控える
			_app_time = int.Parse( cmd[1] );
		}
		else
		{
			_app_time = -1;
		}

		// 現在時間の保存
		_prev_time = DateTime.Now;

		// カーソルを非表示
		Cursor.visible = false;
	}

	/// <summary>
	/// 更新処理を実行させます
	/// </summary>
	public void OnUpdate()
	{
		_is_update = true;

		// 実行時間が残り少ない場合
		if( (_app_time - _run_time) < 10000 )
		{
			_run_time = _app_time - 10000;
		}
	}

	/// <summary>
	/// 更新処理を停止させます
	/// </summary>
	public void StopUpdate()
	{
		_is_update = false;
	}
}
