Shader "Custom/VolumetricJittered"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _FogColor ("Fog Color", Color) = (0.5, 0.5, 0.5, 1)
        _FogDensity ("Fog Density", Range(0, 1)) = 0.5
        _FogStart ("Fog Start Height", Float) = 0
        _FogEnd ("Fog End Height", Float) = 10
        _Alpha ("Fog Transparency", Range(0,1)) = 0.5
        _DistanceStart ("Fog Start Distance", Float) = 5
        _DistanceEnd ("Fog End Distance", Float) = 50
        [NoScaleOffset]_NoiseTex ("Noise Texture", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Float) = 10
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Pass
        {
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
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
            };

            CBUFFER_START(UnityPerMaterial)
            float4 _BaseColor;
            float4 _FogColor;
            float _FogDensity;
            float _FogStart;
            float _FogEnd;
            float _Alpha;
            float _DistanceStart;
            float _DistanceEnd;
            float _NoiseScale;
            CBUFFER_END

            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.uv = IN.uv;

                float3 cameraPos = _WorldSpaceCameraPos;
                OUT.viewDir = normalize(OUT.worldPos - cameraPos);

                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                // Height-based Fog (now using Z-axis instead of Y)
                float heightFactor = saturate((IN.worldPos.z - _FogStart) / (_FogEnd - _FogStart));

                // Distance-based Fog
                float distance = length(IN.worldPos - _WorldSpaceCameraPos);
                float distanceFactor = smoothstep(_DistanceStart, _DistanceEnd, distance);

                // Base fog factor
                float fogFactor = heightFactor * distanceFactor * _FogDensity;

                // Sample noise texture for jitter
                float2 noiseUV = IN.uv * _NoiseScale;
                float noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, noiseUV).r;

                // Apply jitter to fog factor
                fogFactor += (noise - 0.5) * 0.2; // jitter range [-0.1, +0.1]
                fogFactor = saturate(fogFactor);

                // Final color blend
                float4 finalColor = lerp(_BaseColor, _FogColor, fogFactor);
                finalColor.a = _Alpha * fogFactor;

                return finalColor;
            }

            ENDHLSL
        }
    }
}
