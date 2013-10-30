
#pragma strict

@script ExecuteInEditMode
@script RequireComponent (Camera)
@script AddComponentMenu ("Image Effects/Other/Screen Overlay")

class ScreenOverlay extends PostEffectsBase {
	
	enum OverlayBlendMode {
		Additive = 0,
		ScreenBlend = 1,
		Multiply = 2,
        Overlay = 3,
        AlphaBlend = 4,	
	}
	
	public var blendMode : OverlayBlendMode = OverlayBlendMode.Overlay;
	public var intensity : float = 1.0f;
	public var alphaMult : float = 1.0f;
	public var textureMat : Material;	
//	public var alphaTextureMat : Material;
			
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
		overlayMaterial.SetFloat ("_Intensity", intensity);
		overlayMaterial.SetFloat ("_AlphaMult", alphaMult);
		overlayMaterial.SetTexture ("_Overlay", textureMat.mainTexture);
		//overlayMaterial.SetTexture ("_Alpha", alphaTextureMat.mainTexture);
		Graphics.Blit (source, destination, overlayMaterial, blendMode);
	}
}