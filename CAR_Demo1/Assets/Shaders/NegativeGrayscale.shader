//Copyright (c) 2012-2013 Qualcomm Austria Research Center GmbH.
//All Rights Reserved.
Shader "Custom/NegativeGrayscale" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _TouchX ("TouchX", Float) = 0.0
        _TouchY ("TouchY", Float) = 0.0
        
    }
    
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface frag Lambert vertex:vert
        #pragma exclude_renderers flash

        sampler2D _MainTex;
        float _TouchX;
        float _TouchY;

        struct Input {
            float2 uv_MainTex;
        };
           
        void vert(inout appdata_full v) {
            float distance;
            float2 direction;
            float2 _Touch;
            float sinDistance;
            
            _Touch.x=_TouchX;
            _Touch.y=_TouchY;
        
            direction =v.vertex.xz-_Touch;
            distance=sqrt(direction.x*direction.x+direction.y*direction.y);
            sinDistance = (sin(distance)+1.0);
            direction=direction/distance;
           
            if ((sinDistance>0.0)&&(_Touch.x != 2.0))
            {
                v.vertex.xz+=(direction*(0.3/sinDistance));
            }

        }            

        void frag(Input IN, inout SurfaceOutput o) {
            half4 incoming = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = 1.0 - ((incoming.r + incoming.g + incoming.b)/3);
            o.Alpha = incoming.a;
        }
        
        ENDCG
    } 
    FallBack "Diffuse"
}