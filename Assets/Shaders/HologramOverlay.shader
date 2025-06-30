Shader "Custom/HologramOverlay"
{
    Properties
    {
        _HologramColor ("Hologram Color", Color) = (0, 1, 1, 1)
        _Reveal ("Reveal Height", Float) = 0.0
        _BlendZone ("Blend Zone", Float) = 0.5
        _GlowPower ("Glow Intensity", Float) = 2.0
        _ScanlineIntensity ("Scanline Intensity", Float) = 0.3
        _ScanlineDensity ("Scanline Density", Float) = 50.0
        _FresnelPower ("Fresnel Power", Float) = 3.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent+100" "RenderType"="Transparent" }
        ZWrite Off
        Blend One One // Additive blending for glow
        Cull Back
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Camera position uniform (set by Unity)
          //  float3 _WorldSpaceCameraPos;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
            };

            float4 _HologramColor;
            float _Reveal;
            float _BlendZone;
            float _GlowPower;
            float _ScanlineIntensity;
            float _ScanlineDensity;
            float _FresnelPower;

            Varyings vert(Attributes input)
            {
                Varyings output;
                float3 worldPos = TransformObjectToWorld(input.positionOS.xyz);
                float3 worldNormal = normalize(TransformObjectToWorldNormal(input.normalOS));
                output.worldPos = worldPos;
                output.worldNormal = worldNormal;

                output.positionHCS = TransformWorldToHClip(worldPos);

                float3 camPos = _WorldSpaceCameraPos;

                output.viewDir = normalize(camPos - worldPos);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Reveal blend factor from bottom to top
                float heightDiff = _Reveal - input.worldPos.y;
                float blend = saturate(heightDiff / _BlendZone);

                // Fresnel effect for rim glow
                float fresnel = pow(1.0 - saturate(dot(input.viewDir, input.worldNormal)), _FresnelPower);

                // Scanline effect (horizontal lines)
                float scanline = sin(input.worldPos.y * _ScanlineDensity) * 0.5 + 0.5;

                // Combine scanline with intensity and blend
                float scanlineEffect = lerp(1.0, scanline, _ScanlineIntensity);

                // Final glow color
                float3 glow = _HologramColor.rgb * _GlowPower * blend * fresnel * scanlineEffect;

                // Alpha combines reveal blend and fresnel for transparency
                float alpha = blend * fresnel;

                return float4(glow, alpha);
            }

            ENDHLSL
        }
    }
}
