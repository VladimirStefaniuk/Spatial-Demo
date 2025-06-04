Shader "Custom/BarrierShader"
{
    Properties
    { 
        _ColorA ("Color A", Color) = (1,1,1,1)
        _ColorB ("Color B", Color) = (1,1,1,1)
        _ColorStart ("Color Start", Range(0, 1)) = 1
        _ColorEnd ("Color End", Range(0, 1)) = 0
    }
    SubShader
    {
        // subshader tags
        Tags {
            "RenderType"="Transparent" // tag to inform the render pipeline of what type this is 
            "Queue"="Transparent" // change the render order
        }
    
        Pass
        {
            // shader pass tags
            Cull Off
            ZWrite Off
            Blend One One
            // Blend DstColor Zero // Multiplicative
    
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
  
            #include "UnityCG.cginc"

            #define TAU  6.2831
            
            float4 _ColorA; 
            float4 _ColorB;
            float _ColorStart;
            float _ColorEnd;
             
            // automaticly filled up by Unity  
            struct MeshData
            {
                float4 vertex : POSITION; // local space vertex position
                float2 uv : TEXCOORD0; // <- you don't always need UV to texture an object
                float3 normals : NORMAL; // local space normal direction 
            };

            struct Iterpolators // vertext to fragment
            {
                float4 vertex : SV_POSITION; // clip space position of each vertex 
                float3 normal : TEXCOORD0;
                float2 uv : TEXTCOORD1; 
            };
 
            Iterpolators vert (MeshData v)
            {
                Iterpolators o; // o is for output
                o.vertex = UnityObjectToClipPos(v.vertex); // multiplying by MVC matric. local space to clip space 
                o.normal = UnityObjectToWorldNormal(v.normals);
                o.uv = v.uv; // paththrought
                return o;
            }
 
            float InverseLerp(float a, float b, float v) {
                return (v-a)/(b-a);
            }
            
            // in deferent rendering you can render to multiple targets
            float4 frag (Iterpolators i) : SV_Target
            { 
                float yOffset = cos(i.uv.x * TAU * 8) * 0.01f;
                float t = cos((i.uv.y + yOffset + -1 * _Time.y * 0.1f) * TAU * 5) * 0.5 + 0.5;
                t *= (1 - i.uv.y);
           
                float topRemover = ((i.normal.y) < 0.999);
                float waves = t * topRemover;
                float4 gradient =  lerp(_ColorA, _ColorB, i.uv.y);
                
                return gradient * topRemover * waves; 
            }
            
            ENDCG
        }
    }
} 