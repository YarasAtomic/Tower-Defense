Shader "Custom/TerrainShaderFlat"
{
    Properties
    {
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Control ("Control (RGBA)", 2D) = "black" {}
        _Splat3 ("Layer 3 (A)", 2D) = "white" {}
        _Splat2 ("Layer 2 (B)", 2D) = "white" {}
        _Splat1 ("Layer 1 (G)", 2D) = "white" {}
        _Splat0 ("Layer 0 (R)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { 
            "RenderType"="Opaque" 
            "SplatCount" = "4"
        }
        LOD 200

        Pass
        {
            Tags {"LightMode"="ForwardBase"}

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc" // for _LightColor0

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv_Control : TEXCOORD0;
                float2 uv_Splat0 : TEXCOORD1;
                float2 uv_Splat1 : TEXCOORD2;
                float2 uv_Splat2 : TEXCOORD3;
                float2 uv_Splat3 : TEXCOORD4;
                float3 normal : NORMAL0;
                
            };

            struct v2f
            {
                float2 uv_Control : TEXCOORD0;
                float2 uv_Splat0 : TEXCOORD1;
                float2 uv_Splat1 : TEXCOORD2;
                float2 uv_Splat2 : TEXCOORD3;
                float2 uv_Splat3 : TEXCOORD4;
                fixed4 diff : COLOR0; // diffuse lighting color
                float4 vertex : SV_POSITION;
            };

            sampler2D _Control;
            float4 _Control_ST;
            sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
            float4 _Splat0_ST, _Splat1_ST, _Splat2_ST, _Splat3_ST;
            
            v2f vert (appdata v)
            {
                v2f o;
                // o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertex = v.vertex;
                o.uv_Control = TRANSFORM_TEX(v.uv_Control, _Control);
                o.uv_Splat0 = TRANSFORM_TEX(v.uv_Control, _Splat0);
                o.uv_Splat1 = TRANSFORM_TEX(v.uv_Control, _Splat1);
                o.uv_Splat2 = TRANSFORM_TEX(v.uv_Control, _Splat2);
                o.uv_Splat3 = TRANSFORM_TEX(v.uv_Control, _Splat3);
                
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half nl = max(0,dot(worldNormal,_WorldSpaceLightPos0.xyz));

                o.diff = nl * _LightColor0;

                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 splat_control = tex2D (_Control, i.uv_Control);

                fixed4 col = {0,0,0,1};
                // col = splat_control.a > 0.5 ? tex2D (_Splat3, i.uv_Splat3).rgba : col.rgba;
                // col = splat_control.b > 0.5 ? tex2D (_Splat2, i.uv_Splat2).rgba : col.rgba;
                // col = splat_control.g > 0.5 ? tex2D (_Splat1, i.uv_Splat1).rgba : col.rgba;
                // col = splat_control.r > 0.5 ? tex2D (_Splat0, i.uv_Splat0).rgba : col.rgba;

                // col *= i.diff;
                float3 posddx = ddx(i.vertex);
                float3 posddy = ddy(i.vertex);

                col.rgb += cross(normalize(posddy),normalize(posddx));

                return col;
            }
            ENDCG
        }
    }
}