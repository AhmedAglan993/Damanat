Shader "Custom/HologramReveal"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0, 1, 1, 1)
        _Reveal ("Reveal Height", Float) = 0.0
        _GlowPower ("Glow Power", Float) = 2.0
        _Cutoff ("Alpha Cutoff", Float) = 0.1
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            float4 _BaseColor;
            float _Reveal;
            float _GlowPower;
            float _Cutoff;

            Varyings vert(Attributes input)
            {
                Varyings output;
                float3 world = TransformObjectToWorld(input.positionOS.xyz);
                output.worldPos = world;
                output.positionHCS = TransformWorldToHClip(world);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Reveal factor: 1 if below reveal line, 0 if not yet revealed
                float revealMask = saturate((_Reveal - input.worldPos.y) * 20.0);

                // Skip fragment if not yet revealed
                if (revealMask < _Cutoff)
                    discard;

                // Hologram effect stays after reveal
                float3 baseColor = _BaseColor.rgb;
                float alpha = 1.0; // Fully visible once revealed
                float3 emission = baseColor * _GlowPower;

                return float4(baseColor + emission, alpha);
            }

            ENDHLSL
        }
    }
}
