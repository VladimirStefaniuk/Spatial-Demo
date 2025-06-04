Shader "Custom/SimpleDissolve"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _DissolveAmount ("Dissolve Amount", Range(0, 1)) = 0
        _EdgeColor ("Edge Color", Color) = (1, 0.5, 0, 1)
        _EdgeWidth ("Edge Width", Range(0.01, 0.2)) = 0.05
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 200
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float _DissolveAmount;
            float _EdgeWidth;
            float4 _EdgeColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float noise = tex2D(_NoiseTex, i.uv).r;
                float4 baseColor = tex2D(_MainTex, i.uv);

                float dissolveEdge = smoothstep(_DissolveAmount - _EdgeWidth, _DissolveAmount, noise);

                // Alpha discard for hard cutoff
                if (noise < _DissolveAmount)
                    discard;

                // Blend edge color
                float edgeFactor = 1.0 - dissolveEdge;
                float4 finalColor = lerp(_EdgeColor, baseColor, dissolveEdge);
                finalColor.a *= edgeFactor;

                return finalColor;
            }
            ENDCG
        }
    }

    FallBack "Transparent/Diffuse"
}
