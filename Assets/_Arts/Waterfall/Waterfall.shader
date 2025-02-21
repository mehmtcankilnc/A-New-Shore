Shader "Custom/WaterfallShader"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.0, 0.5, 1.0, 1.0)
        _GradientTex ("Gradient Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _EmissionStrength ("Emission Strength", Float) = 2.0
        _Speed ("Flow Speed", Float) = 0.5
        _FoamThreshold ("Foam Threshold", Float) = 0.4
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        Pass
        {
            Name "ForwardBase"
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            HLSLINCLUDE
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _GradientTex;
            sampler2D _NoiseTex;
            float4 _BaseColor;
            float _Speed;
            float _EmissionStrength;
            float _FoamThreshold;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Time-based UV scrolling for movement
                float2 uv = i.uv;
                uv.y += _Time.y * _Speed;

                // Base gradient texture
                fixed4 gradientColor = tex2D(_GradientTex, uv);

                // Noise texture for water flow
                fixed4 noise = tex2D(_NoiseTex, uv * 5.0);

                // Combine gradient and noise for base color
                fixed4 baseColor = lerp(_BaseColor, gradientColor, noise.r);

                // Foam effect using threshold
                float foam = step(_FoamThreshold, noise.r);
                fixed4 foamColor = fixed4(1.0, 1.0, 1.0, foam);

                // Add emission for foam
                fixed4 emission = foamColor * _EmissionStrength;

                // Final color output
                fixed4 finalColor = baseColor + emission;

                // Alpha blending for transparency
                finalColor.a = gradientColor.a * noise.r;

                return finalColor;
            }
            ENDHLSL
        }
    }

    FallBack "Transparent/Diffuse"
}
