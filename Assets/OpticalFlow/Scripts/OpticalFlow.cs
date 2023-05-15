using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/*
  シェーダーからの計算結果とテクスチャをもとに
　MainCameraにオプティカルフローの動きの結果表示(カメラの動き)をするスクリプト
  (フラグメントシェーダーの結果表示)
  Texture関連のものは明示的な破棄をしないと重くなるので注意が必要(GCの対象外)
*/
namespace OpticalFlow
{
    public class OpticalFlow : FlowUpdater {

        protected enum Pass {
            Flow = 0,
            DownSample = 1,
            BlurH = 2,
            BlurV = 3,
            Visualize = 4
        };

        public RenderTexture Flow { get { return resultBuffer; } }
		public Vector2[] FlowVec { get { return _flow; } }

        [SerializeField] protected Material flowMaterial;
        [SerializeField, Range(0, 6)] int blurIterations = 0, blurDownSample = 0;
        [SerializeField] protected bool debug;
		[SerializeField] float _flow_mag;

		Vector2[] _flow;
        protected RenderTexture prevFrame, flowBuffer, resultBuffer;

        #region MonoBehaviour functions

        protected void Start () {
        }
        //最終的な結果を出力用RenderTextureに描画
        protected void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(resultBuffer, destination, flowMaterial, (int)Pass.Visualize);
        }
        //描画に使ったTextureの明示的な開放
        protected void OnDestroy ()
        {
            if(prevFrame != null)
            {
                prevFrame.Release();
                prevFrame = null;

                flowBuffer.Release();
                flowBuffer = null;

                resultBuffer.Release();
                resultBuffer = null;
            }
        }

        //テクスチャにフレーム差分を描画
        protected void OnGUI ()
        {
            if (!debug || prevFrame == null || flowBuffer == null) return;

            const int offset = 10;
            const int width = 256, height = 256;
            GUI.DrawTexture(new Rect(offset, offset, width, height), prevFrame);
            GUI.DrawTexture(new Rect(offset, offset + height, width, height), resultBuffer);
        }

        #endregion

        protected void Setup(int width, int height)
        {
            //前回フレーム記憶用RenderTexture
            prevFrame = new RenderTexture(width, height, 0);
            prevFrame.format = RenderTextureFormat.ARGBFloat;
            prevFrame.wrapMode = TextureWrapMode.Repeat;
            prevFrame.Create();
            //動き記憶用RenderTexture
            flowBuffer = new RenderTexture(width, height, 0);
            flowBuffer.format = RenderTextureFormat.ARGBFloat;
            flowBuffer.wrapMode = TextureWrapMode.Repeat;
            flowBuffer.Create();
            //最終結果表示用RenderTexture
            resultBuffer = new RenderTexture(width >> blurDownSample, height >> blurDownSample, 0);
            resultBuffer.format = RenderTextureFormat.ARGBFloat;
            resultBuffer.wrapMode = TextureWrapMode.Repeat;
            resultBuffer.Create();

			_flow = new Vector2[resultBuffer.width*resultBuffer.height];
        }

		private void Update(){
		}

        /// <summary>
        /// 動きの計算
        /// </summary>
        /// <param name="current">前回座標を保存するTexture変数</param>
		public void Calculate(Texture current)
        {
            //前回フレームの画像を保存
            if(prevFrame == null) {
                Setup(current.width, current.height);
                Graphics.Blit(current, prevFrame);
            }
            //MaterialにTextureの設定(第一引数は変更対象のMaterialを選択してInspectorの名前を右クリック→Edit Shaderから確認)
            //Shader.PropertyToIDを使って取得可能
            flowMaterial.SetTexture("_PrevTex", prevFrame);
            //スクリプトからMaterialに値(float)を渡す
            flowMaterial.SetFloat("_Ratio", 1f * Screen.height / Screen.width);
            //currentのテクスチャをflowBuffer(prevFrame)のテクスチャに描画する
            Graphics.Blit(current, flowBuffer, flowMaterial, (int)Pass.Flow);
            Graphics.Blit(current, prevFrame);

            // Graphics.Blit(flowBuffer, destination, flowMaterial, (int)Pass.Visualize);

            //動きの視覚化
            var downSampled = DownSample(flowBuffer, blurDownSample);
            Blur(downSampled, blurIterations);
            // Graphics.Blit(downSampled, destination, flowMaterial, (int)Pass.Visualize);
            Graphics.Blit(downSampled, resultBuffer);
            //RenderTexture開放
            RenderTexture.ReleaseTemporary(downSampled);

			// バイトデータの取得
			Texture2D tex = new Texture2D(resultBuffer.width, resultBuffer.height, TextureFormat.RGBAFloat, false);
			RenderTexture.active = resultBuffer;
			tex.ReadPixels(new Rect(0, 0, resultBuffer.width, resultBuffer.height), 0, 0);
			tex.Apply();
			Color[] colors = tex.GetPixels();

            //ベクトルデータへ変換
			for(int i = 0; i < _flow.Length; i++ ) {
                _flow[i].x = (colors[i].r == float.NaN) ? 0f : colors[i].r;
                _flow[i].y = (colors[i].g == float.NaN) ? 0f : colors[i].g;
                _flow[i] *= _flow_mag;

                //Debug.Log($"flow.x:{_flow[i].x}");
                //Debug.Log($"flow.y:{_flow[i].y}");
            }
			Object.Destroy(tex);

			//更新イベントの実行(実行対象のオブジェクトと実行するメソッドをこのスクリプトがアタッチされている
            //オブジェクトのInspectorから登録する必要あり)
			if(Time.time > 1f) {
				flowUpdateEvent.Invoke(_flow);
			}
        }
        //カメラのスクロール処理メソッド
        RenderTexture DownSample(RenderTexture source, int lod)
        {
            //レンダリングテクスチャの割り当て
            var dst = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
            //隣接するピクセル間の補完(平均化して滑らかに)
            source.filterMode = FilterMode.Bilinear;
            Graphics.Blit(source, dst);
            //スクロールしてるところ
            for (var i = 0; i < lod; i++)
            {
                var tmp = RenderTexture.GetTemporary(dst.width >> 1, dst.height >> 1, 0, dst.format);
                dst.filterMode = FilterMode.Bilinear;
                Graphics.Blit(dst, tmp, flowMaterial, (int)Pass.DownSample);
                //GetTemporaryで与えられたテクスチャの開放
                RenderTexture.ReleaseTemporary(dst);
                dst = tmp;
            }

            return dst;
        }
        
        /// <summary>
        /// ピクセルの平均化(ぼかし)メソッド
        /// </summary>
        /// <param name="source">ぼかす対象のRenderTexture変数</param>
        /// <param name="iterations"></param>
        void Blur(RenderTexture source, int iterations)
        {
            //レンダリングテクスチャの割り当て
            var tmp0 = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
            var tmp1 = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
            //数値の下限と上限の割り当て
            var iters = Mathf.Clamp(iterations, 0, 10);
            //sourceのテクスチャをtmp0のテクスチャに描画
            Graphics.Blit(source, tmp0);
            for (var i = 0; i < iters; i++)
            {
                for (var pass = 2; pass < 4; pass++)
                {
                    //RenderTextureの内容が使われないことをGPUドライバに伝えFilterModeの変更
                    tmp1.DiscardContents();
                    tmp0.filterMode = FilterMode.Bilinear;
                    //tmp0のテクスチャをtmp1のテクスチャに描画
                    Graphics.Blit(tmp0, tmp1, flowMaterial, pass);
                    var tmpSwap = tmp0;
                    tmp0 = tmp1;
                    tmp1 = tmpSwap;
                    //(tmp1, tmp0) = (tmp0, tmp1);
                }
            }
            Graphics.Blit(tmp0, source);
            //GetTemporaryで与えられたテクスチャの一時的な開放(GCではサポートしていないので明示的に破棄する必要あり)
            RenderTexture.ReleaseTemporary(tmp0);
            RenderTexture.ReleaseTemporary(tmp1);
        }

        /// <summary>
        /// レンダリングテクスチャのバッファ作成メソッド
        /// </summary>
        /// <param name="width">レンダリングテクスチャの幅</param>
        /// <param name="height">レンダリングテクスチャの高さ</param>
        /// <returns></returns>
        protected RenderTexture CreateBuffer(int width, int height)
        {
            //レンダリングテクスチャを新たに作成してフォーマットとwrapModeを設定して作成
            var rt = new RenderTexture(width, height, 0);
            rt.format = RenderTextureFormat.ARGBFloat;
            rt.wrapMode = TextureWrapMode.Repeat;
            rt.Create();
            return rt;
        }
        //自身のインスタンス作成
		private static OpticalFlow instance;
		public static OpticalFlow Instance
		{
			get
			{
				if (instance == null)
				{
					System.Type t = typeof(OpticalFlow);

					instance = (OpticalFlow)FindObjectOfType(t);
					if (instance == null)
					{
                        Debug.LogError($"{t} をアタッチしているGameObjectはありません");
					}
				}

				return instance;
			}
		}

		virtual protected void Awake ()
		{
			// 他のGameObjectにアタッチされているか調べる.
			// アタッチされている場合は破棄する.
			if (this != Instance)
			{
				Destroy(this);
				//Destroy(this.gameObject);
				Debug.LogError(
					typeof(OpticalFlow) +
					" は既に他のGameObjectにアタッチされているため、コンポーネントを破棄しました." +
					" アタッチされているGameObjectは " + Instance.gameObject.name + " です.");
				return;
			}
		}

    }

}


