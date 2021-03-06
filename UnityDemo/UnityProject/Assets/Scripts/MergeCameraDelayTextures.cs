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
using System.Collections;
/// <summary>
/// Calls the merge shader to overlay a foreground and background camera image (with alpha).
/// </summary>
public class MergeCameraDelayTextures : ImageEffectBase {

    /// <summary>
    /// The background camera image.
    /// </summary>
	public TargetTextureDelayBehaviour bgTexture;

    /// <summary>
    /// The foreground camera image.
    /// </summary>
	public TargetTextureDelayBehaviour fgTexture;
	
    /// <summary>
    /// Executes the shader.
    /// </summary>
	void OnRenderImage (RenderTexture source, RenderTexture destination) {		

		material.SetTexture ("_BgTexture", bgTexture.OutTexture());
		material.SetTexture ("_FgTexture", fgTexture.OutTexture());
		
		Graphics.Blit (source, destination, this.material);
	}
}
