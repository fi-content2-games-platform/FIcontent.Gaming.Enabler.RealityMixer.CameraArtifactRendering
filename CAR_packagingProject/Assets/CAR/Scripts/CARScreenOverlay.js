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
 *
 * This class is based on the ScreenOverlay in the Standard Assets Library
 */
 
#pragma strict

@script ExecuteInEditMode
@script RequireComponent (Camera)
@script AddComponentMenu ("CameraArtifactRenderer/CARScreenOverlay")

class CARScreenOverlay extends MonoBehaviour {
	
	public var texture : RenderTexture;	
			
	public var overlayShader : Shader;
	private var overlayMaterial : Material = null;
	
	function CheckResources () : boolean {
		CheckSupport (false);
		
		overlayMaterial = CheckShaderAndCreateMaterial (overlayShader, overlayMaterial);
		
		if 	(!isSupported)
			ReportAutoDisable ();
		return isSupported;
	}
	
	function OnRenderImage (source : RenderTexture, destination : RenderTexture) {		

		if (CheckResources() == false) {
			Graphics.Blit (source, destination);
			return;
		}	
		var UV_Transform : Vector4 = Vector4(1, 0, 0, 1);
		
		#if UNITY_WP8
		// WP8 has no OS support for rotating screen with device orientation,
		// so we do those transformations ourselves.
		if (Screen.orientation == ScreenOrientation.LandscapeLeft) {
			UV_Transform = Vector4(0, -1, 1, 0);
		}
		if (Screen.orientation == ScreenOrientation.LandscapeRight) {
			UV_Transform = Vector4(0, 1, -1, 0);
		}
		if (Screen.orientation == ScreenOrientation.PortraitUpsideDown) {
			UV_Transform = Vector4(-1, 0, 0, -1);
		}	
		#endif
		
		overlayMaterial.SetVector("_UV_Transform", UV_Transform);		
		overlayMaterial.SetTexture ("_Overlay", texture);
		Graphics.Blit (source, destination, overlayMaterial);
	}
	
	
	
	
	/* Imported from PostEffectBase
	 * 
	 */
	
	
	protected var supportHDRTextures : boolean = true;
	protected var supportDX11 : boolean = false;
	protected var isSupported : boolean = true;
	
	function CheckShaderAndCreateMaterial (s : Shader, m2Create : Material) : Material {
		if (!s) { 
			Debug.Log("Missing shader in " + this.ToString ());
			enabled = false;
			return null;
		}
			
		if (s.isSupported && m2Create && m2Create.shader == s) 
			return m2Create;
		
		if (!s.isSupported) {
			NotSupported ();
			Debug.Log("The shader " + s.ToString() + " on effect "+this.ToString()+" is not supported on this platform!");
			return null;
		}
		else {
			m2Create = new Material (s);	
			m2Create.hideFlags = HideFlags.DontSave;		
			if (m2Create) 
				return m2Create;
			else return null;
		}
	}

	function CreateMaterial (s : Shader, m2Create : Material) : Material {
		if (!s) { 
			Debug.Log ("Missing shader in " + this.ToString ());
			return null;
		}
			
		if (m2Create && (m2Create.shader == s) && (s.isSupported)) 
			return m2Create;
		
		if (!s.isSupported) {
			return null;
		}
		else {
			m2Create = new Material (s);	
			m2Create.hideFlags = HideFlags.DontSave;		
			if (m2Create) 
				return m2Create;
			else return null;
		}
	}
	
	function OnEnable() {
		isSupported = true;
	}	

	function CheckSupport () : boolean {
		return CheckSupport (false);
	}
	
	
	function Start () {
		 CheckResources ();
	}	
		
	function CheckSupport (needDepth : boolean) : boolean {
		isSupported = true;
		supportHDRTextures = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf);
		supportDX11 = SystemInfo.graphicsShaderLevel >= 50 && SystemInfo.supportsComputeShaders;
		
		if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures) {
			NotSupported ();
			return false;
		}		
		
		if(needDepth && !SystemInfo.SupportsRenderTextureFormat (RenderTextureFormat.Depth)) {
			NotSupported ();
			return false;
		}
		
		if(needDepth)
			camera.depthTextureMode |= DepthTextureMode.Depth;	
		
		return true;
	}

	function CheckSupport (needDepth : boolean, needHdr : boolean) : boolean {
		if(!CheckSupport(needDepth))
			return false;
		
		if(needHdr && !supportHDRTextures) {
			NotSupported ();
			return false;		
		}
		
		return true;
	}	
	
	function Dx11Support() : boolean {
		return supportDX11;
	}

	function ReportAutoDisable () {
		Debug.LogWarning ("The image effect " + this.ToString() + " has been disabled as it's not supported on the current platform.");
	}
			
	// deprecated but needed for old effects to survive upgrading
	function CheckShader (s : Shader) : boolean {
		Debug.Log("The shader " + s.ToString () + " on effect "+ this.ToString () + " is not part of the Unity 3.2+ effects suite anymore. For best performance and quality, please ensure you are using the latest Standard Assets Image Effects (Pro only) package.");		
		if (!s.isSupported) {
			NotSupported ();
			return false;
		} 
		else {
			return false;
		}
	}
	
	function NotSupported () {
		enabled = false;
		isSupported = false;
		return;
	}
	
	function DrawBorder (dest : RenderTexture, material : Material ) {
		var x1 : float;	
		var x2 : float;
		var y1 : float;
		var y2 : float;		
		
		RenderTexture.active = dest;
        var invertY : boolean = true; // source.texelSize.y < 0.0f;
        // Set up the simple Matrix
        GL.PushMatrix();
        GL.LoadOrtho();		
        
        for (var i : int = 0; i < material.passCount; i++)
        {
            material.SetPass(i);
	        
	        var y1_ : float; var y2_ : float;
	        if (invertY)
	        {
	            y1_ = 1.0; y2_ = 0.0;
	        }
	        else
	        {
	            y1_ = 0.0; y2_ = 1.0;
	        }
	        	        
	        // left	        
	        x1 = 0.0;
	        x2 = 0.0 + 1.0/(dest.width*1.0);
	        y1 = 0.0;
	        y2 = 1.0;
	        GL.Begin(GL.QUADS);
	        
	        GL.TexCoord2(0.0, y1_); GL.Vertex3(x1, y1, 0.1);
	        GL.TexCoord2(1.0, y1_); GL.Vertex3(x2, y1, 0.1);
	        GL.TexCoord2(1.0, y2_); GL.Vertex3(x2, y2, 0.1);
	        GL.TexCoord2(0.0, y2_); GL.Vertex3(x1, y2, 0.1);
	
	        // right
	        x1 = 1.0 - 1.0/(dest.width*1.0);
	        x2 = 1.0;
	        y1 = 0.0;
	        y2 = 1.0;

	        GL.TexCoord2(0.0, y1_); GL.Vertex3(x1, y1, 0.1);
	        GL.TexCoord2(1.0, y1_); GL.Vertex3(x2, y1, 0.1);
	        GL.TexCoord2(1.0, y2_); GL.Vertex3(x2, y2, 0.1);
	        GL.TexCoord2(0.0, y2_); GL.Vertex3(x1, y2, 0.1);	        
	
	        // top
	        x1 = 0.0;
	        x2 = 1.0;
	        y1 = 0.0;
	        y2 = 0.0 + 1.0/(dest.height*1.0);

	        GL.TexCoord2(0.0, y1_); GL.Vertex3(x1, y1, 0.1);
	        GL.TexCoord2(1.0, y1_); GL.Vertex3(x2, y1, 0.1);
	        GL.TexCoord2(1.0, y2_); GL.Vertex3(x2, y2, 0.1);
	        GL.TexCoord2(0.0, y2_); GL.Vertex3(x1, y2, 0.1);
	        
	        // bottom
	        x1 = 0.0;
	        x2 = 1.0;
	        y1 = 1.0 - 1.0/(dest.height*1.0);
	        y2 = 1.0;

	        GL.TexCoord2(0.0, y1_); GL.Vertex3(x1, y1, 0.1);
	        GL.TexCoord2(1.0, y1_); GL.Vertex3(x2, y1, 0.1);
	        GL.TexCoord2(1.0, y2_); GL.Vertex3(x2, y2, 0.1);
	        GL.TexCoord2(0.0, y2_); GL.Vertex3(x1, y2, 0.1);	
	                	              
	        GL.End();	
        }	
        
        GL.PopMatrix();
	}
	
	/* END Imported from PostEffectBase
	 *
	 */
}