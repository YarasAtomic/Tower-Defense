Shader "Custom/TerrainNormalFlat"
{
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
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
  
        struct Input {
            float2 uv_Control : TEXCOORD0;
            float2 uv_Splat0 : TEXCOORD1;
            float2 uv_Splat1 : TEXCOORD2;
            float2 uv_Splat2 : TEXCOORD3;
            float2 uv_Splat3 : TEXCOORD4;
            float2 uv_MainTex;
            
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
        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            o.cameraRelativeWorldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0)) - _WorldSpaceCameraPos.xyz;
        }

        void surf (Input IN, inout SurfaceOutputStandard o) { 
            fixed4 splat_control = tex2D (_Control, IN.uv_Control);

            fixed3 col = {0,0,0};
            col = splat_control.a > 0.5 ? tex2D (_Splat3, IN.uv_Splat3).rgb : col.rgb;
            col = splat_control.b > 0.5 ? tex2D (_Splat2, IN.uv_Splat2).rgb : col.rgb;
            col = splat_control.g > 0.5 ? tex2D (_Splat1, IN.uv_Splat1).rgb : col.rgb;
            col = splat_control.r > 0.5 ? tex2D (_Splat0, IN.uv_Splat0).rgb : col.rgb;
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