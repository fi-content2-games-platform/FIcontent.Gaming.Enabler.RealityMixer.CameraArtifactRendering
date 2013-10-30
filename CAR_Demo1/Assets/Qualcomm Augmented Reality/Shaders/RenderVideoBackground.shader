//Copyright (c) 2012-2013 Qualcomm Austria Research Center GmbH.
//All Rights Reserved.
//Confidential and Proprietary - QUALCOMM Austria Research Center GmbH.
Shader "Custom/RenderVideoBackground" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader {
        Tags {"Queue"="overlay+1" "RenderType"="overlay" }
        Pass {
            // Render the teapot
            SetTexture [_MainTex] {
                combine texture 
            }
        }
    } 
    FallBack "Diffuse"
}
