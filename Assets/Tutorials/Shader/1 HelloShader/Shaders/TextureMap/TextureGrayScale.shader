Shader "Custom/Unlit/Texture/GrayScale"
{
    Properties
    {
        [MainColor] _Color ("Color A", Color) = (1,1,1,1)
        [MainTexture] _MainTex("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "PreviewType"="Plane"
            "RenderType"="Opaque"
        }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha // Traditional transparency
            // Blend One OneMinusSrcAlpha // Premultiplied transparency
            // Blend One One // Additive
            // Blend OneMinusDstColor One // Soft additive
            // Blend DstColor Zero // Multiplicative
            // Blend DstColor SrcColor // 2x multiplicative

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _Color;
            sampler2D _MainTex;

            struct VertexData
            {
                float4 vertex : POSITION;   // vertex position
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float4 vertex : SV_POSITION;    // clip space position
                float2 uv : TEXCOORD0;
            };

            Interpolators vert (VertexData v)
            {
                Interpolators output;
                output.vertex = UnityObjectToClipPos(v.vertex); // local space to clip space
                output.uv = v.uv;
                return output;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                float4 sample = tex2D(_MainTex, i.uv);
                float luminance = (sample.r * 0.3 + sample.g * 0.59 + sample.b * 0.11); 
                return float4(luminance, luminance, luminance, sample.a) * _Color;
            }
            ENDCG
        }
    }
}
