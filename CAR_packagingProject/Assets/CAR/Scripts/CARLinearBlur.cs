/* Copyright (c) 2014 ETH Zurich
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

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("CameraArtifactRenderer/CARLinearBlur")]
public class CARLinearBlur: MonoBehaviour
{   

    /// <summary>
    /// The camera target. This should be put in the middle of the scene.
    /// </summary>
    public Transform target;

    /// <summary>
    /// The temporal smoothing term
    /// </summary>
    [Range(0,1)]
    public float stabilizerStrength = 0.5f;

    /// <summary>
    /// For different exposures, the blur strength needs to be adjusted
    /// </summary>
    [Range(0,20)]
    public float blurMultiplier = 0.9f;

    /// <summary>
    /// Enable GUI Label
    /// </summary>
    public bool guiEnabled = true;

    

    #region PRIVATE_MEMBER_VARIABLES


    /// <summary>
    /// Enables this effect
    /// </summary>
    private bool enabled = true;    

    /// <summary>
    /// The last screen position of the camera target
    /// </summary>
    private Vector3 targetLastPos;

    /// <summary>
    /// The blur vector to be applied
    /// </summary>
    private Vector2 blurDirection;

    #endregion PRIVATE_MEMBER_VARIABLES

    #region UNITY_MONOBEHAVIOUR_METHODS

    void Start()
    {
		InternalStart();
        this.blurDirection = Vector2.zero;
        this.targetLastPos = this.target.position;
    }

    void OnGUI()
    {
        if (guiEnabled)
            this.enabled = GUI.Toggle(new Rect(20, 20, 100, 20), this.enabled, "Enable Camera Artifact Renderer Camera Blur");
    }
    
    // Called by camera to apply image effect
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (this.enabled)
        {
            // Calculate amount of blur. The blur corresponds to the movement of the target position in screen coordinates and is temporally smoothed.
            Vector3 diff = (this.camera.WorldToScreenPoint(this.target.position) - this.targetLastPos);
            this.blurDirection = this.blurDirection * (this.stabilizerStrength) + new Vector2(diff.x, diff.y) * (1 - this.stabilizerStrength);
            this.targetLastPos = this.camera.WorldToScreenPoint(this.target.position);            
        }
        else
        {
            blurDirection = Vector2.zero;
        }

        // Execute the shader
        material.SetVector("_Direction", blurDirection * blurMultiplier);
        Graphics.Blit(source, destination, material);
        
    }

	/// Imported from ImageEffectsBase


	/// Provides a shader property that is set in the inspector
	/// and a material instantiated from the shader
	public Shader   shader;
	private Material m_Material;
	
	protected void InternalStart ()
	{
		// Disable if we don't support image effects
		if (!SystemInfo.supportsImageEffects) {
			enabled = false;
			return;
		}
		
		// Disable the image effect if the shader can't
		// run on the users graphics card
		if (!shader || !shader.isSupported)
			enabled = false;
	}
	
	protected Material material {
		get {
			if (m_Material == null) {
				m_Material = new Material (shader);
				m_Material.hideFlags = HideFlags.HideAndDontSave;
			}
			return m_Material;
		} 
	}
	
	protected void OnDisable() {
		if( m_Material ) {
			DestroyImmediate( m_Material );
		}
	}

    #endregion UNITY_MONOBEHAVIOUR_METHODS
}