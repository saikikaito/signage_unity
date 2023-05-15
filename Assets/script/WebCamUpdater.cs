using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
  オプティカルフローの計算結果にMaterialを設定してカメラに表示するスクリプト
   (このスクリプトからマテリアルを設定している)
*/
namespace OpticalFlow.Demo
{

    public class WebCamUpdater : TextureUpdater {

        public int Width { get { return _rot_tex.width;  } }
        public int Height { get { return _rot_tex.height;  } }

		[SerializeField] protected int _dev_index = 0;
        [SerializeField] protected WebCamTexture webCamTexture;
        [SerializeField] protected int width = 512, height = 512;
		[SerializeField] protected Shader _shader;

		RenderTexture _rot_tex;
		Material _material;

        void Start () {
            WebCamDevice userCameraDevice = WebCamTexture.devices[_dev_index];
            webCamTexture = new WebCamTexture(userCameraDevice.name, width, height, 60);
            webCamTexture.Play();

			_rot_tex = new RenderTexture(height, width, 0);
			_rot_tex.Create();

			//マテリアルの作成
		   _material = new Material(_shader);
		   _material.hideFlags = HideFlags.HideAndDontSave;
		   _material.shader.hideFlags = HideFlags.HideAndDontSave;
		   _material.mainTexture = webCamTexture;
		   _material.color = Color.white;
        }

        void Update ()
        {
            //Inspectorからアタッチ(AddListner)したメソッドの実行
			textureUpdateEvent.Invoke(webCamTexture);
		}

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {		
            Graphics.Blit(_rot_tex, destination);
        }

        protected void OnDestroy()
        {
        }

    }

}


