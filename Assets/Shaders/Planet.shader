Shader "Custom/Planet"
{
    Properties {
        _Color3 ("Color snow", Color) = (1,1,1,1)
        _Color2 ("Color mountain", Color) = (1,1,1,1)
        _Color1 ("Color ground", Color) = (1,1,1,1)
        _Color0 ("Color water", Color) = (1,1,1,1)
        _MainTex ("Height map (RGBA)", 2D) = "white" {}    
        _MaxHeight ("Max height", Float) = 0.5
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
            float2 texCoord : TEXCOORD0;
            // We want flat faces, we dont need the texture UVs but it gives a warning
            float3 normal : NORMAL0;
            float4 tangent : TANGENT;
        };
    
        struct Input {
            bool splat0;
            bool splat1;
            bool splat2;
            bool splat3;
            float3 cameraRelativeWorldPos;
            float3 worldNormal;
            float4 vertex  : SV_POSITION;
            INTERNAL_DATA
        };
 
        half _Glossiness;
        half _Metallic;
        fixed4 _Color0,_Color1,_Color2,_Color3;
        float _MaxHeight;
        sampler2D _MainTex;
 
        // pass camera relative world position from vertex to fragment
        void vert(inout appdata v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            o.cameraRelativeWorldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0)) - _WorldSpaceCameraPos.xyz;
            fixed4 heightMap = tex2Dlod (_MainTex,  float4(v.texCoord.x,v.texCoord.y,0,0));
            v.vertex.x += v.normal.x * heightMap.r * _MaxHeight;
            v.vertex.y += v.normal.y * heightMap.g * _MaxHeight;
            v.vertex.z += v.normal.z * heightMap.b * _MaxHeight;



            o.vertex    = UnityObjectToClipPos(v.vertex);
            o.splat3 = heightMap.r > 0.7 || v.vertex.y > 0.95 || v.vertex.y < -0.95;
            o.splat2 = heightMap.r > 0.4;
            o.splat1 = heightMap.r > 0;
            o.splat0 = heightMap.r == 0;
        }

        void surf (Input IN, inout SurfaceOutputStandard o) { 
            // Select color from vertex info
            fixed3 col = {0,0,0};
            col = IN.splat0 ? _Color0 : col;
            col = IN.splat1 ? _Color1 : col;
            col = IN.splat2 ? _Color2 : col;
            col = IN.splat3 ? _Color3 : col;
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