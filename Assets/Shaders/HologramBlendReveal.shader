Shader "Custom/HologramBlendReveal"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        _HologramColor ("Hologram Color", Color) = (0, 1, 1, 1)
        _GlowPower ("Glow Power", Float) = 2.0
        _Reveal ("Reveal Height", Float) = 0.0
        _BlendZone ("Blend Height Range", Float) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _BaseColor;
            float4 _HologramColor;
            float _GlowPower;
            float _Reveal;
            float _BlendZone;

            Varyings vert(Attributes input)
            {
                Varyings output;
                float3 world = TransformObjectToWorld(input.positionOS.xyz);
                output.worldPos = world;
                output.uv = input.uv;
                output.positionHCS = TransformWorldToHClip(world);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Distance from current pixel to the reveal front
                float heightDiff = _Reveal - input.worldPos.y;

                // Blend factor: 0 (original), 1 (hologram), soft transition
                float blend = saturate(heightDiff / _BlendZone);

                // Base color (from texture or color)
                float4 baseCol = tex2D(_MainTex, input.uv) * _BaseColor;

                // Hologram glow
                float3 holoCol = _HologramColor.rgb * _GlowPower;

                // Final blend
                float3 finalColor = lerp(baseCol.rgb, holoCol, blend);
                float finalAlpha = lerp(baseCol.a, 1.0, blend);

                return float4(finalColor, finalAlpha);
            }
            ENDHLSL
        }
    }
}
