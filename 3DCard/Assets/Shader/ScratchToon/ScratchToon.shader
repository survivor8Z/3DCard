Shader "URP/Sketch"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _TileFactor("TileFactor", Float) = 1
        _Sketch0("Sketch0", 2D) = "white" {}
        _Sketch1("Sketch1", 2D) = "white" {}
        _Sketch2("Sketch2", 2D) = "white" {}
        _Sketch3("Sketch3", 2D) = "white" {}
        _Sketch4("Sketch4", 2D) = "white" {}
        _Sketch5("Sketch5", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "SketchPass"
            Tags { "LightMode" = "UniversalForward" }
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
            float4 _Color;
            float _TileFactor;
            CBUFFER_END

            TEXTURE2D(_Sketch0); SAMPLER(sampler_Sketch0);
            TEXTURE2D(_Sketch1); SAMPLER(sampler_Sketch1);
            TEXTURE2D(_Sketch2); SAMPLER(sampler_Sketch2);
            TEXTURE2D(_Sketch3); SAMPLER(sampler_Sketch3);
            TEXTURE2D(_Sketch4); SAMPLER(sampler_Sketch4);
            TEXTURE2D(_Sketch5); SAMPLER(sampler_Sketch5);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS   : TEXCOORD2;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS);
                output.uv = input.uv * _TileFactor;
                output.positionWS = TransformObjectToWorld(input.positionOS);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {

                // 获取主光源信息
                Light mainLight = GetMainLight();

                // 根据光照方向和法线方向的点积，计算漫反射光照强度
                half lightIntensity = max(0, dot(mainLight.direction, input.normalWS));
                half shadowAttenuation = mainLight.shadowAttenuation;

                // 将光照强度映射到 0 到 6 的浮点数，用于混合6个素描纹理
                half sketchIndex = lightIntensity * 6.0;

                // 获取混合权重
                // floor(sketchIndex) 得到当前的整数索引
                // frac(sketchIndex) 得到小数部分，作为混合的权重
                half index = floor(sketchIndex);
                half fracPart = frac(sketchIndex);
                
                half4 finalColor = half4(1, 1, 1, 1);

                if (index == 0) {
                    // 索引 0 和 1 之间的混合
                    finalColor = lerp(SAMPLE_TEXTURE2D(_Sketch0, sampler_Sketch0, input.uv), SAMPLE_TEXTURE2D(_Sketch1, sampler_Sketch1, input.uv), fracPart);
                } else if (index == 1) {
                    finalColor = lerp(SAMPLE_TEXTURE2D(_Sketch1, sampler_Sketch1, input.uv), SAMPLE_TEXTURE2D(_Sketch2, sampler_Sketch2, input.uv), fracPart);
                } else if (index == 2) {
                    finalColor = lerp(SAMPLE_TEXTURE2D(_Sketch2, sampler_Sketch2, input.uv), SAMPLE_TEXTURE2D(_Sketch3, sampler_Sketch3, input.uv), fracPart);
                } else if (index == 3) {
                    finalColor = lerp(SAMPLE_TEXTURE2D(_Sketch3, sampler_Sketch3, input.uv), SAMPLE_TEXTURE2D(_Sketch4, sampler_Sketch4, input.uv), fracPart);
                } else if (index == 4) {
                    finalColor = lerp(SAMPLE_TEXTURE2D(_Sketch4, sampler_Sketch4, input.uv), SAMPLE_TEXTURE2D(_Sketch5, sampler_Sketch5, input.uv), fracPart);
                } else {
                    // 如果 index > 4，则直接使用最暗的纹理
                    finalColor = SAMPLE_TEXTURE2D(_Sketch5, sampler_Sketch5, input.uv);
                }

                return half4(finalColor.rgb * mainLight.color * shadowAttenuation * _Color.rgb, 1);
            }
            ENDHLSL
        }
    }
    Fallback "Hidden/Universal Render Pipeline/Unlit"
}