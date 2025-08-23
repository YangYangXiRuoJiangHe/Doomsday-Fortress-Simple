Shader "Flooded_Grounds/URP_PBR_Water_Fixed"
{
    Properties
    {
        _Color("Main Color", Color) = (1,1,1,1)
        _Emis("Self-Illumination", Range(0, 1)) = 0.1
        _Smth("Smoothness", Range(0, 1)) = 0.9
        _Parallax("Height", Range(0.005, 0.08)) = 0.02
        _MainTex("Base Color (RGB) Spec (A)", 2D) = "white" {}
        _BumpMap("Normalmap 1", 2D) = "bump" {}
        _BumpMap2("Normalmap 2", 2D) = "bump" {}
        _BumpLerp("Normalmap2 Blend", Range(0, 1)) = 0.5
        _ParallaxMap("Heightmap", 2D) = "black" {}
        _ScrollSpeed("Scroll Speed", Range(0.01, 2.0)) = 0.2
        _FlowSpeed("Flow Speed", Range(0.01, 3.0)) = 1.0  // 控制流动节奏
        _FlowStrength("Flow Disturbance", Range(0.01, 0.5)) = 0.1  // 扰动强度
        _Opacity("Opacity", Range(0, 1)) = 0.8
    }

        SubShader
        {
            Tags
            {
                "RenderType" = "Transparent"
                "Queue" = "Transparent"
                "RenderPipeline" = "UniversalPipeline"
            }
            LOD 300

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            Pass
            {
                Name "ForwardLit"
                Tags { "LightMode" = "UniversalForward" }

                HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            // Parallax 函数
            float2 ParallaxOffset(float height, float heightScale, float3 viewDirTS)
            {
                viewDirTS = normalize(viewDirTS);
                return heightScale * (viewDirTS.xy / viewDirTS.z) * height;
            }

        // 纹理
        TEXTURE2D(_MainTex);     SAMPLER(sampler_MainTex);
        TEXTURE2D(_BumpMap);     SAMPLER(sampler_BumpMap);
        TEXTURE2D(_BumpMap2);    SAMPLER(sampler_BumpMap2);
        TEXTURE2D(_ParallaxMap); SAMPLER(sampler_ParallaxMap);

        // 参数
        CBUFFER_START(UnityPerMaterial)
            float4 _Color;
            float _Emis;
            float _Smth;
            float _Parallax;
            float _ScrollSpeed;
            float _FlowSpeed;
            float _FlowStrength;
            float _BumpLerp;
            float _Opacity;
        CBUFFER_END

        struct Attributes
        {
            float4 positionOS : POSITION;
            float3 normalOS   : NORMAL;
            float4 tangentOS  : TANGENT;
            float2 uv         : TEXCOORD0;
        };

        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float2 uv         : TEXCOORD0;
            float3 positionWS : TEXCOORD1;
            float3 normalWS   : TEXCOORD2;
            float4 tangentWS  : TEXCOORD3;
            float3 viewDirTS  : TEXCOORD4;
        };

        // ✅ 顶点着色器：移除了 sin 波浪！
        Varyings vert(Attributes input)
        {
            Varyings output = (Varyings)0;

            // ✅ 完全移除波浪动画
            // input.positionOS.y += sin(...) ❌ 已删除

            VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
            VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

            output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
            output.positionWS = vertexInput.positionWS;
            output.normalWS = normalInput.normalWS;
            output.tangentWS = float4(normalInput.tangentWS, input.tangentOS.w);
            output.uv = input.uv;

            float3 viewDirOS = _WorldSpaceCameraPos - vertexInput.positionWS;
            float3 binormalOS = cross(input.normalOS, input.tangentOS.xyz) * input.tangentOS.w;
            float3x3 tangentToLocal = float3x3(input.tangentOS.xyz, binormalOS, input.normalOS);
            float3 viewDirTS = mul(tangentToLocal, viewDirOS);
            output.viewDirTS = viewDirTS;

            return output;
        }

        half4 frag(Varyings input) : SV_Target
        {
            // ✅ 1. 单向滚动（纹理流动方向）
            float2 scroll1 = float2(_ScrollSpeed, _ScrollSpeed * 0.5) * _TimeParameters.y;
            float2 scroll2 = float2((1 - _ScrollSpeed), (1 - _ScrollSpeed) * 0.5) * _TimeParameters.y;

            // ✅ 2. 流动扰动 UV（模拟水流波动，但方向一致）
            float2 flow = float2(_FlowSpeed * 0.5, _FlowSpeed * 0.3) * _TimeParameters.y;
            float2 uv_Bump1 = input.uv + scroll1 + flow * _FlowStrength;
            float2 uv_Bump2 = input.uv + scroll2 - flow * _FlowStrength * 0.5;

            // ✅ 3. 视差（基于主滚动方向）
            float2 baseUV = input.uv + scroll1 * 0.2;
            half height = SAMPLE_TEXTURE2D(_ParallaxMap, sampler_ParallaxMap, baseUV).r;
            float2 parallaxOffset = ParallaxOffset(height, _Parallax, input.viewDirTS);

            // 应用视差
            uv_Bump1 += parallaxOffset;
            uv_Bump2 += parallaxOffset;

            // 采样
            half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + parallaxOffset + scroll1);
            half3 normal1 = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, uv_Bump1));
            half3 normal2 = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap2, sampler_BumpMap2, uv_Bump2));

            // TBN 矩阵
            float3 binormalWS = input.tangentWS.w * cross(input.normalWS, input.tangentWS.xyz);
            float3x3 TBN = float3x3(input.tangentWS.xyz, binormalWS, input.normalWS);
            half3 worldNormal1 = normalize(mul(normal1, TBN));
            half3 worldNormal2 = normalize(mul(normal2, TBN));
            half3 blendedNormal = lerp(worldNormal1, worldNormal2, _BumpLerp);

            // PBR 计算
            half3 albedo = texColor.rgb * _Color.rgb;
            half3 emission = albedo * _Emis;
            half smoothness = _Smth;

            Light mainLight = GetMainLight();
            half3 normal = blendedNormal;
            half3 lightDir = mainLight.direction;
            half NdotL = dot(normal, lightDir);
            half3 diffuse = albedo * mainLight.color * max(NdotL, 0.0h);
            half3 ambient = albedo * 0.1h;
            half3 finalColor = diffuse + ambient + emission;

            // 透明度
            half alpha = saturate(texColor.a * _Opacity + 0.3);
            return half4(finalColor, alpha);
        }
        ENDHLSL
    }
        }

            Fallback "Universal Render Pipeline/Lit"
}