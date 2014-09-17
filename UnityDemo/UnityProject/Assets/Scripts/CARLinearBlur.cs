﻿/* Copyright (c) 2014 ETH Zurich
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
[AddComponentMenu("Image Effects/CARLinearBlur")]
public class CARLinearBlur : ImageEffectBase
{   
	
	/// <summary>
	/// The camera target. This should be put in the middle of the scene.
	/// </summary>
	public Transform target;
	
	/// <summary>
	/// The camera from which the blur is calculated
	/// </summary>
	public Camera cam;
	
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
	
	
	
	#region PRIVATE_MEMBER_VARIABLES

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
		this.blurDirection = Vector2.zero;
		this.targetLastPos = this.target.position;
	}
	
	
	// Called by camera to apply image effect
	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		// Calculate amount of blur. The blur corresponds to the movement of the target position in screen coordinates and is temporally smoothed.
		Vector3 diff = (this.cam.WorldToScreenPoint(this.target.position) - this.targetLastPos);
		
		this.blurDirection = this.blurDirection * (this.stabilizerStrength) + new Vector2(diff.x, diff.y) * (1 - this.stabilizerStrength);           
		this.targetLastPos = this.cam.WorldToScreenPoint(this.target.position);            

		
		// Execute the shader
		//material.SetVector("_Direction", blurDirection * blurMultiplier);
		material.SetVector("_Direction", Vector2.zero);
		Graphics.Blit(source, destination, material);
		
	}
	
	#endregion UNITY_MONOBEHAVIOUR_METHODS
}