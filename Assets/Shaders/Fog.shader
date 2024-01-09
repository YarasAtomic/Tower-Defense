Shader "Custom/Fog" 
{
    Properties 
    {
        _Color ("Color (RGBA)", Color) = (1, 1, 1, 1) // add _Color property
    }

    SubShader 
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        LOD 100

        Pass 
        {
            CGPROGRAM

            #pragma vertex vert alpha
            #pragma fragment frag alpha

            #include "UnityCG.cginc"

            struct appdata_t 
            {
                float4 vertex   : POSITION;
            };

            struct v2f 
            {
                float4 vertex  : SV_POSITION;
                float4 screenPos : TEXCOORD1;
            };

            float4 _Color;
            sampler _CameraDepthTexture;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex    = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 textureCoordinate = i.screenPos.xy / i.screenPos.w;
                float4 depthValue = tex2D(_CameraDepthTexture,textureCoordinate); // Calculate the depth of the pixels behind
                fixed4 col = _Color;
                // i.vertex.z is the depth of the fragment
                col.a = (i.vertex.z - depthValue)/(i.vertex.z * i.vertex.z * 30); // calculate the alpha
                col.a = col.a < 0 ? 0 : col.a * col.a; // avoid negative alpha and smooth alpha
                col.a = col.a > 1 ? 1 : col.a; // avoid alpha > 1
                return col;
            }

            ENDCG
        }
    }
}