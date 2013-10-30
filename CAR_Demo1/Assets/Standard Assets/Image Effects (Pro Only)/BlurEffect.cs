using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Blur/Blur")]
public class BlurEffect : MonoBehaviour
{

    public int waitMS = 33;
    private float waited;
    
	private Vector3 camPosOld;	
	private float camVel;
    private float avgCamVel;
	private float blurSize;
    public float blurMul = 1;
    public float offThreashold = 0.1f;
    
    public bool cameraMovementInputEnabled = false;

    //@Range (0, 1)
    public float blurStabilizer = 0.5f;



	/// Blur iterations - larger number means more blur.
	private int intIterations;
    public int iterations = 3;
	
	/// Blur spread for each iteration. Lower values
	/// give better looking blur, but require more iterations to
	/// get large blurs. Value is usually between 0.5 and 1.0.
    public float blurSpread = 0.5f;
	private float intBlurSpread = 0.6f;
	
	


	// --------------------------------------------------------
	// The blur iteration shader.
	// Basically it just takes 4 texture samples and averages them.
	// By applying it repeatedly and spreading out sample locations
	// we get a Gaussian blur approximation.
	 
	public Shader blurShader = null;	

	//private static string blurMatString =

	static Material m_Material = null;
	protected Material material {
		get {
			if (m_Material == null) {
				m_Material = new Material(blurShader);
				m_Material.hideFlags = HideFlags.DontSave;
			}
			return m_Material;
		} 
	}
	
	protected void OnDisable() {
		if( m_Material ) {
			DestroyImmediate( m_Material );
		}
	}	
	
	// --------------------------------------------------------
	
	protected void Start()
	{
		// Disable if we don't support image effects
		if (!SystemInfo.supportsImageEffects) {
			enabled = false;
			return;
		}
		// Disable if the shader can't run on the users graphics card
		if (!blurShader || !material.shader.isSupported) {
			enabled = false;
			return;
		}

        waited = 0;
        avgCamVel = 0;
	}
	
	// Performs one blur iteration.
	public void FourTapCone (RenderTexture source, RenderTexture dest, int iteration)
	{
		float off = 0.5f + iteration*intBlurSpread;
		Graphics.BlitMultiTap (source, dest, material,
			new Vector2(-off, -off),
			new Vector2(-off,  off),
			new Vector2( off,  off),
			new Vector2( off, -off)
		);
	}
	
	// Downsamples the texture to a quarter resolution.
	private void DownSample4x (RenderTexture source, RenderTexture dest)
	{
		float off = 1.0f;
		Graphics.BlitMultiTap (source, dest, material,
			new Vector2(-off, -off),
			new Vector2(-off,  off),
			new Vector2( off,  off),
			new Vector2( off, -off)
		);
	}

    //void OnGUI()
    //{
    //   GUI.Label(new Rect(0, 0, 300, 100), camVel + "");
    //}


	// Called by the camera to apply the image effect
	void OnRenderImage (RenderTexture source, RenderTexture destination) {

       // waited += Time.deltaTime;
        //if (waited > this.waitMS)
        //{
            waited = 0;
            camVel = (camPosOld - transform.position).magnitude;
            camPosOld = transform.position;

            avgCamVel = camVel * (1 - blurStabilizer) + avgCamVel * blurStabilizer;
            blurSize = blurMul * avgCamVel;
        
            
      //  }

        if (cameraMovementInputEnabled)
            intBlurSpread = blurSize;
        else
            intBlurSpread = blurSpread;

        if (avgCamVel < offThreashold)
        {
            intIterations = 0;
        }
        else
        {
            intIterations = iterations;
        }



		int rtW = source.width;
		int rtH = source.height;
		//RenderTexture buffer = RenderTexture.GetTemporary(rtW, rtH, 0);
		
		// Copy source to the 4x4 smaller texture.
		//DownSample4x (source, buffer);
        
		
		// Blur the small texture
		for(int i = 0; i < intIterations; i++)
		{
			RenderTexture buffer2 = RenderTexture.GetTemporary(rtW, rtH, 0);
            FourTapCone(source, buffer2, i);
            RenderTexture.ReleaseTemporary(source);
            source = buffer2;
		}
        Graphics.Blit(source, destination);

        RenderTexture.ReleaseTemporary(source);
	}	
}
