/* Copyright (c) 2013 ETH Zurich
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */


using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class ARCameraArtifactRenderer : MonoBehaviour
{

    // Connect here the camera that renders the background video image
    //public Camera backgroundCamera;
    

    #region PRIVATE_MEMBER_VARIABLES
   
    // the target render texture for the camera
    //public RenderTexture renderTex;

    // the render material to overlay the background camera
    //public Material renderMat;    

    #endregion PRIVATE_MEMBER_VARIABLES


    #region PRIVATE_METHODS
    #endregion PRIVATE_METHODS


    #region UNITY_MONOBEHAVIOUR_METHODS
    // Use this for initialization
	void Start () {

        ////Screen.SetResolution(300, 480 , true);

        //// Setup textures and materials
        //renderTex = new RenderTexture(1024, 1024, 16);
        //renderMat = new Material(Shader.Find("Transparent/Diffuse"));
       
        //// Setup camera
        //this.camera.clearFlags = CameraClearFlags.SolidColor;
        //this.camera.backgroundColor = new Color(0, 0, 0, 0);
        //this.camera.targetTexture = this.renderTex;
        //this.renderMat.mainTexture = this.renderTex;

        //// Connect render texture to other camera
        //ScreenOverlay bgCamScreenOverlay = this.backgroundCamera.GetComponent<ScreenOverlay>();
        //bgCamScreenOverlay.blendMode = ScreenOverlay.OverlayBlendMode.AlphaBlend;
        //bgCamScreenOverlay.textureMat = this.renderMat;  
       
        
        
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
