Shader "Custom/Volumeic"
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
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha  // 반투명 설정
            ZWrite Off   // 깊이 버퍼에 영향 X
            Cull Off     // 모든 방향에서 보이도록 설정

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

            // Fog Properties
            CBUFFER_START(UnityPerMaterial)
            float4 _BaseColor;
            float4 _FogColor;
            float _FogDensity;
            float _FogStart;
            float _FogEnd;
            float _Alpha;
            float _DistanceStart;
            float _DistanceEnd;
            CBUFFER_END

            // Vertex Shader
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.uv = IN.uv;

                // 카메라 방향 벡터
                float3 cameraPos = _WorldSpaceCameraPos;
                OUT.viewDir = normalize(OUT.worldPos - cameraPos);

                return OUT;
            }

            // Fragment Shader (Fog Calculation)
            float4 frag(Varyings IN) : SV_Target
            {
                // 높이에 따른 Fog 강도 (Height Fog)
                float heightFactor = saturate((IN.worldPos.y - _FogStart) / (_FogEnd - _FogStart));

                // 카메라 거리 기반 Fog 강도 (Distance Fog)
                float distance = length(IN.worldPos - _WorldSpaceCameraPos);
                float distanceFactor = smoothstep(_DistanceStart, _DistanceEnd, distance);

                // Volumetric Fog 스타일 (높이 + 거리 혼합)
                float fogFactor = heightFactor * distanceFactor * _FogDensity;

                // 색상 혼합
                float4 finalColor = lerp(_BaseColor, _FogColor, fogFactor);
                finalColor.a = _Alpha * fogFactor;  // 투명도 적용

                return finalColor;
            }

            ENDHLSL
        }
    }
}
