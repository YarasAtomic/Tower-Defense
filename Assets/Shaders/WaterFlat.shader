  Shader "Custom/WaterFlat" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Amount ("Extrusion Amount", Range(-1,1)) = 0.5
        _Color ("Color", Color) = (1,1,1,1)
        [KeywordEnum(Approximate, Exact)] _InverseMatrix ("World To Tangent Matrix", Float) = 0.0
    }
    SubShader {
      Tags { "RenderType" = "Opaque" }
      CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert
        #pragma shader_feature _ _INVERSEMATRIX_EXACT

        struct appdata{
            float4 vertex : POSITION;
            float2 uv_MainTex : TEXCOORD0;
            float3 normal : NORMAL0;
            float4 tangent : TANGENT;
        };

        struct Input {
            float2 uv_MainTex;
            float3 cameraRelativeWorldPos;
            float3 worldNormal;
            INTERNAL_DATA
        };

        float _Amount;
        sampler2D _MainTex;
        float4 _Color;
        half _Glossiness;
        half _Metallic;

        void vert (inout appdata v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            float texPosX = v.uv_MainTex.x + _Time;
            float texPosY = v.uv_MainTex.y + _Time;
            fixed4 heightMap = tex2Dlod (_MainTex, float4(texPosX,texPosY,0,0));
            v.vertex.y = (_SinTime.a * 0.01 + (0.25 - heightMap.r * 0.5)) * _Amount;

            o.cameraRelativeWorldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0)) - _WorldSpaceCameraPos.xyz;
        }

        void surf (Input IN, inout SurfaceOutputStandard o) {
            //   o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
            o.Albedo = _Color;
            o.Smoothness = _Glossiness;
            o.Metallic = _Metallic;
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
    Fallback "Diffuse"
  }