Shader "Custom/DissolveShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _NoiseTex("Noise Texture", 2D) = "white" {}
        _DissolveAmount("Dissolve Amount", Range(0, 1)) = 0.0
        _Color("Color", Color) = (1,1,1,1)
        _RotationSpeed("Rotation Speed", Float) = 1.0
        _Transparency("Transparency", Range(0, 1)) = 0.5
        _VerticalRepeat("Vertical Repeat", Float) = 1.0
        _WaveAmplitude("Wave Amplitude", Float) = 0.1
        _WaveFrequency("Wave Frequency", Float) = 1.0
    }
        SubShader
        {
            Tags { "Queue" = "Transparent+1" "RenderType" = "Transparent" }
            LOD 100

            // Render both sides of the sphere
            Cull Off

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float4 pos : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    float3 worldPos : TEXCOORD1;
                };

                sampler2D _MainTex;
                sampler2D _NoiseTex;
                float _DissolveAmount;
                float4 _Color;
                float _RotationSpeed;
                float _Transparency;
                float _VerticalRepeat;
                float _WaveAmplitude;
                float _WaveFrequency;

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);

                    // Apply wave effect
                    float wave = sin(_Time.y * _WaveFrequency + v.vertex.x * 10.0) * _WaveAmplitude;

                    // Rotate UV coordinates around the Y axis
                    float angle = _Time.y * (_RotationSpeed / 4.0);
                    float2x2 rotationMatrix = float2x2(cos(angle), -sin(angle), sin(angle), cos(angle));
                    float2 uv = v.uv;
                    uv.y *= _VerticalRepeat; // Repeat texture vertically
                    uv.y += wave; // Add wave effect
                    o.uv = mul(rotationMatrix, uv - 0.5) + 0.5;

                    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // Calculate dissolve based on world position's y-coordinate (dissolve from top to bottom)
                    float height = 100.0; // Assuming the height of the sphere is 2 units
                    float dissolve = smoothstep(_DissolveAmount - 0.1, _DissolveAmount, (i.worldPos.y + height / 100.0) / height);

                    // Sample noise texture to create variations in the dissolve effect
                    float noise = tex2D(_NoiseTex, i.uv).r;
                    dissolve *= noise;

                    fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                    col.a *= dissolve * _Transparency; // Apply transparency and dissolve effect
                    if (col.a < 0.1) discard;
                    return col;
                }
                ENDCG
            }
        }
            FallBack "Diffuse"
}
