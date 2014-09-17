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
using System.Collections.Generic;

public class TargetTextureDelayBehaviour : MonoBehaviour {

    private int MAXDELAY = 36;
    private int delay;

    private RenderTexture[] texs;
    int currentWriteTex;
   
    public int Delay
    {
        get{ return delay; }
        set
        {
            delay = Mathf.Clamp(value, 0, MAXDELAY);
        }
    }

	
    void Start () {
        delay = 0;
        texs = new RenderTexture[MAXDELAY];
        for (int i = 0; i < MAXDELAY; i++)
        {
			texs[i] = new RenderTexture(1024,1024,16);
        }
        currentWriteTex = 0;
    }
	
	
	void OnPreRender() {
        //set this camera's target texture to the next one in the array
        this.camera.targetTexture = texs[currentWriteTex];
        currentWriteTex++;
        if (currentWriteTex >= delay)
        {
            currentWriteTex = 0;
        }
	}

    public RenderTexture OutTexture()
    {
        //copy the "DEALY"-ago written render texture to the out tex
        int readTexIndex = currentWriteTex + 1;
        if (readTexIndex >= delay)
        {
            readTexIndex = 0;
        }
        return texs[readTexIndex];
    }
}
