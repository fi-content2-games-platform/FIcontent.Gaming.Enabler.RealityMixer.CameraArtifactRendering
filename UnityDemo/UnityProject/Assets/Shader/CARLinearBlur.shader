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


Shader "CARLinearBlur" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "" {}
	}

	SubShader 
	{
		Pass 
		{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
					
				#include "UnityCG.cginc"
	
				float2 _Direction;
				float4 _MainTex_ST;		
				sampler2D _MainTex;

				struct appdata_t {
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD;
				};
	
				struct v2f {
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD;
				};		
			
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					return o;
				}
				
				float4 frag (v2f i) : COLOR
				{	
					float4 sum = 0;					
					for(int j=-4; j <= 4; j++)
					{
						float scale = j / (float2)(_ScreenParams) / 9;
						sum += tex2D (_MainTex, i.texcoord + _Direction*scale);
					}
					
					sum /= 9;
					return sum;
				}
			ENDCG 	
		}
	}
}