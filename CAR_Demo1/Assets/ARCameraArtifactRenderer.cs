using UnityEngine;
using System.Collections;


/// <summary>
/// Nice description 
/// </summary>
[RequireComponent(typeof(Camera))]
public class ARCameraArtifactRenderer : MonoBehaviour
{

    // Connect here the camera that renders the background video image
    public Camera backgroundCamera;
    

    #region PRIVATE_MEMBER_VARIABLES
   
    // the target render texture for the camera
    //public RenderTexture renderTex;

    // the render material to overlay the background camera
   // public Material renderMat;
 //   private Texture2D renderMatTex;

    #endregion PRIVATE_MEMBER_VARIABLES


    #region PRIVATE_METHODS
    #endregion PRIVATE_METHODS


    #region UNITY_MONOBEHAVIOUR_METHODS
    // Use this for initialization
	void Start () {

        //Screen.SetResolution(300, 480 , true);

        // Setup textures and materials
        //renderTex = new RenderTexture(1024, 1024, 16);
        //renderMat = new Material(Shader.Find("Transparent/Diffuse"));
        //this.renderMat.mainTexture = new Texture2D(512, 512, TextureFormat.ARGB32, false);

        //this.renderMatTex = new Texture2D(512, 512, TextureFormat.ARGB32, false);
     //   this.renderMat.mainTexture = this.renderMatTex;


        // Setup camera
      //  this.camera.clearFlags = CameraClearFlags.SolidColor;
       // this.camera.backgroundColor = new Color(0, 0, 0, 0);
       // this.camera.targetTexture = this.renderTex;

        // Connect render texture to other camera
      //  ScreenOverlay bgCamScreenOverlay = this.backgroundCamera.GetComponent<ScreenOverlay>();
      //  bgCamScreenOverlay.blendMode = ScreenOverlay.OverlayBlendMode.AlphaBlend;
      //  bgCamScreenOverlay.textureMat = this.renderMat;
        
       
        
        
	}


    void OnPostRender()
    {
     //   RenderTexture.active = renderTex;
      //  renderMatTex.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
      //  renderMatTex.Apply();
        

        //debug
        //byte[] bytes = tex.EncodeToPNG();
        //File.WriteAllBytes("E:\\test.png", bytes);
    }
	
	
    #endregion UNITY_MONOBEHAVIOUR_METHODS




}
