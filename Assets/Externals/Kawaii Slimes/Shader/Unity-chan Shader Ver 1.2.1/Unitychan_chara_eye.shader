Shader "UnityChan/Eye"
{
    Properties
    {
        _Color ("Main Color", Color) = (1, 1, 1, 1)
        _ShadowColor ("Shadow Color", Color) = (0.8, 0.8, 1, 1)
        _EmissionColor ("Emission Color", Color) = (0, 0, 0, 0)

        _MainTex ("Diffuse", 2D) = "white" {}
        _FalloffSampler ("Falloff Control", 2D) = "white" {}
        _RimLightSampler ("RimLight Control", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "Queue"="Geometry"
            "LightMode"="ForwardBase"
        }

        Pass
        {
            Cull Back
            ZTest LEqual

            CGPROGRAM
            #pragma multi_compile_fwdbase
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"

            // ✅ Emission 추가
            fixed4 _EmissionColor;

            // 기존 CharaSkin.cg 파일 포함
            #include "CharaSkin.cg"

            // 최종 출력에 Emission 더함
            float4 frag(v2f i) : COLOR
            {
                float4 baseCol = frag_original(i); // CharaSkin.cg 내 기존 frag 함수 이름이 필요 (예시: frag_original)
                baseCol.rgb += _EmissionColor.rgb;
                return baseCol;
            }

            ENDCG
        }
    }

    FallBack "Transparent/Cutout/Diffuse"
}
