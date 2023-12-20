Shader "Custom/TerrainFlat"
{
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Control ("Control (RGBA)", 2D) = "black" {}        
        _Splat3 ("Layer 3 (A)", 2D) = "white" {}
        _Splat2 ("Layer 2 (B)", 2D) = "white" {}
        _Splat1 ("Layer 1 (G)", 2D) = "white" {}
        _Splat0 ("Layer 0 (R)", 2D) = "white" {}
        [KeywordEnum(Approximate, Exact)] _InverseMatrix ("World To Tangent Matrix", Float) = 0.0
    }
    SubShader {
        Tags { 
            "RenderType"="Opaque"
            "SplatCount" = "4"
        }
 
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert
        #pragma target 4.5
        #pragma shader_feature _ _INVERSEMATRIX_EXACT
  
        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv_Control : TEXCOORD0;
            // We want flat faces, we dont need the texture UVs but it gives a warning
            float2 uv_Splat0 : TEXCOORD1;
            float2 uv_Splat1 : TEXCOORD2;
            float2 uv_Splat2 : TEXCOORD3;
            float2 uv_Splat3 : TEXCOORD4;
            float3 normal : NORMAL0;
            float4 tangent : TANGENT;
        };
    
        struct Input {
            float2 uv_MainTex;
            bool splat0;
            bool splat1;
            bool splat2;
            bool splat3;
            float3 cameraRelativeWorldPos;
            float3 worldNormal;
            INTERNAL_DATA
        };
 
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        sampler2D _Control;
        sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
 
        // pass camera relative world position from vertex to fragment
        void vert(inout appdata v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            o.cameraRelativeWorldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0)) - _WorldSpaceCameraPos.xyz;
            fixed4 splat_control = tex2Dlod (_Control, float4(v.uv_Control.x,v.uv_Control.y,0,0));
            o.splat3 = splat_control.a > 0.5;
            o.splat2 = splat_control.b > 0.5;
            o.splat1 = splat_control.g > 0.5;
            o.splat0 = splat_control.r > 0.5;
        }

        void surf (Input IN, inout SurfaceOutputStandard o) { 
            // Select color from vertex info
            fixed3 col = {0,0,0};
            col = IN.splat0 ? tex2D(_Splat0,fixed2(0,0)) : col;
            col = IN.splat1 ? tex2D(_Splat1,fixed2(0,0)) : col;
            col = IN.splat2 ? tex2D(_Splat2,fixed2(0,0)) : col;
            col = IN.splat3 ? tex2D(_Splat3,fixed2(0,0)) : col;
            o.Albedo = col;
 
    #if !defined(UNITY_PASS_META)
            // flat world normal from position derivatives
            half3 flatWorldNormal = normalize(cross(ddy(IN.cameraRelativeWorldPos.xyz), ddx(IN.cameraRelativeWorldPos.xyz)));
            // construct world to tangent matrix
            half3 worldT =  WorldNormalVector(IN, half3(1,0,0));
            half3 worldB =  WorldNormalVector(IN, half3(0,1,0));
            half3 worldN =  WorldNormalVector(IN, half3(0,0,1));

        #if defined(_INVERSEMATRIX_EXACT)
            // inverse transform matrix
            half3x3 w2tRotation;
            w2tRotation[0] = worldB.yzx * worldN.zxy - worldB.zxy * worldN.yzx;
            w2tRotation[1] = worldT.zxy * worldN.yzx - worldT.yzx * worldN.zxy;
            w2tRotation[2] = worldT.yzx * worldB.zxy - worldT.zxy * worldB.yzx;
 
            half det = dot(worldT.xyz, w2tRotation[0]);
 
            w2tRotation *= rcp(det);
        #else
            half3x3 w2tRotation = half3x3(worldT, worldB, worldN);
        #endif
 
            // apply world to tangent transform to flat world normal
            o.Normal = mul(w2tRotation, flatWorldNormal);
    #endif
        }
        ENDCG
    }
    FallBack "Diffuse"
}