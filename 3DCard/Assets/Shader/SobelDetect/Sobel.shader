Shader "SobelEdgeDetection/Animated"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _LineWidth ("Edge Width", Range(0.001, 10)) = 0.005
        _EdgeColor ("Edge Color", Color) = (1,1,1,1)
        _BackgroundColor ("Background Color", Color) = (0,0,0,1)
        _Threshold ("Edge Threshold", Range(0, 1)) = 0.2
        _EdgeIntensity ("Edge Intensity", Range(0, 5)) = 1.0
        _BlurRadius ("Blur Radius", Range(0, 3)) = 0.5
        _JitterAmount ("Jitter Amount", Range(0, 10)) = 0.02
        _JitterSpeed ("Jitter Speed", Range(0, 50)) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        
        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                uint vertexID : SV_VertexID;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_TexelSize;

            float _LineWidth;
            float4 _EdgeColor;
            float4 _BackgroundColor;
            float _Threshold;
            float _EdgeIntensity;
            float _BlurRadius;
            float _JitterAmount;
            float _JitterSpeed;

            // 简单的噪声函数
            float random(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
                output.uv = GetFullScreenTriangleTexCoord(input.vertexID);
                output.worldPos = mul(unity_ObjectToWorld, float4(output.uv, 0, 1)).xyz;
                return output;
            }

            float luminance(float4 color)
            {
                return dot(color.rgb, float3(0.2126, 0.7152, 0.0722));
            }

            float4 sampleBlurred(float2 uv, float2 offset)
            {
                float4 sum = 0;
                float weightSum = 0;
                
                for(int x = -1; x <= 1; x++)
                {
                    for(int y = -1; y <= 1; y++)
                    {
                        float2 sampleUV = uv + float2(x, y) * _MainTex_TexelSize.xy * _BlurRadius;
                        float weight = 1.0 - length(float2(x, y)) / 3.0;
                        sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, sampleUV) * weight;
                        weightSum += weight;
                    }
                }
                return sum / weightSum;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // 添加基于时间和位置的抖动
                float time = _Time.y * _JitterSpeed;
                float2 jitter = float2(
                    random(input.uv + time) * 2.0 - 1.0,
                    random(input.uv + time + 0.5) * 2.0 - 1.0
                ) * _JitterAmount * _MainTex_TexelSize.xy;

                // Sobel operator kernels
                const float Gx[9] = { -1, -2, -1, 
                                      0,  0,  0, 
                                      1,  2,  1 };
                const float Gy[9] = { -1,  0,  1, 
                                     -2,  0,  2, 
                                     -1,  0,  1 };

                // Sample points with jitter
                float2 samplePoints[9];
                float2 offset = _MainTex_TexelSize.xy * _LineWidth;
                
                samplePoints[0] = input.uv + float2(-1,  1) * offset + jitter;
                samplePoints[1] = input.uv + float2( 0,  1) * offset + jitter;
                samplePoints[2] = input.uv + float2( 1,  1) * offset + jitter;
                samplePoints[3] = input.uv + float2(-1,  0) * offset + jitter;
                samplePoints[4] = input.uv + jitter;
                samplePoints[5] = input.uv + float2( 1,  0) * offset + jitter;
                samplePoints[6] = input.uv + float2(-1, -1) * offset + jitter;
                samplePoints[7] = input.uv + float2( 0, -1) * offset + jitter;
                samplePoints[8] = input.uv + float2( 1, -1) * offset + jitter;

                // Calculate gradients
                float edgeX = 0;
                float edgeY = 0;
                
                [unroll]
                for(int i = 0; i < 9; i++)
                {
                    float4 color = sampleBlurred(samplePoints[i], offset);
                    float lum = luminance(color);
                    edgeX += lum * Gx[i];
                    edgeY += lum * Gy[i];
                }

                // Calculate edge strength
                float edgeStrength = length(float2(edgeX, edgeY)) * _EdgeIntensity;
                
                // Apply threshold with smoothstep
                float edge = smoothstep(_Threshold, _Threshold + 0.1, edgeStrength);
                
                // Blend between background and edge color
                float4 result = lerp(_BackgroundColor, _EdgeColor, edge);
                result.a = 1.0;

                return result;
            }
            ENDHLSL
        }
    }
}