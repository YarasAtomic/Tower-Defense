Shader "Custom/TerrainShader"
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

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 4.5 target, to use many texture interpolators
        #pragma target 4.5


        struct Input
        {
            float2 uv_Control : TEXCOORD0;
            float2 uv_Splat0 : TEXCOORD1;
            float2 uv_Splat1 : TEXCOORD2;
            float2 uv_Splat2 : TEXCOORD3;
            float2 uv_Splat3 : TEXCOORD4;
            float4 vertex : POSITION;
        };

        half _Glossiness;
        half _Metallic;
        sampler2D _Control;
        sampler2D _Splat0,_Splat1,_Splat2,_Splat3;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 splat_control = tex2D (_Control, IN.uv_Control);

            fixed3 col = {0,0,0};
            col = splat_control.a > 0.5 ? tex2D (_Splat3, IN.uv_Splat3).rgb : col.rgb;
            col = splat_control.b > 0.5 ? tex2D (_Splat2, IN.uv_Splat2).rgb : col.rgb;
            col = splat_control.g > 0.5 ? tex2D (_Splat1, IN.uv_Splat1).rgb : col.rgb;
            col = splat_control.r > 0.5 ? tex2D (_Splat0, IN.uv_Splat0).rgb : col.rgb;

            o.Albedo = col;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = 0.0;  
            o.Normal = normalize(cross(ddy(IN.vertex),ddx(IN.vertex)));          
        }
        ENDCG
    }
    FallBack "Diffuse"
}
