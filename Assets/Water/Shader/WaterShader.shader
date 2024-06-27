Shader "Custom/WaterShader"
{
    Properties
    {
        // Main texture property
        _MainTex ("Main Texture", 2D) = "white" {}

        // Add four color properties for each segment
        _Color1 ("Color 1", Color) = (1, 1, 1, 1)
        _Color2 ("Color 2", Color) = (1, 1, 1, 1)
        _Color3 ("Color 3", Color) = (1, 1, 1, 1)
        _Color4 ("Color 4", Color) = (1, 1, 1, 1)

        // Transparency range
        _TransparencyRange ("Transparency Range", Range(0, 1)) = 0.5 // Default transparency range
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" }
        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha

        Cull Off // Disable back-face culling

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0; // Add UV coordinates
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0; // Pass UV coordinates to fragment shader
                float3 normal : NORMAL;
                float3 worldPos : TEXCOORD1; // Pass world position to fragment shader
                float3 viewDir : TEXCOORD2; // Pass view direction to fragment shader
            };

            sampler2D _MainTex;
            fixed4 _Color1;
            fixed4 _Color2;
            fixed4 _Color3;
            fixed4 _Color4;
            float _TransparencyRange;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; // Calculate world position

                // Calculate view direction in world space
                o.viewDir = normalize(_WorldSpaceCameraPos - o.worldPos);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                uv.y *= 4.0; // Scale UV by 4 vertically to segment the texture into four parts

                // Segment UV coordinates into four parts
                float segment = floor(uv.y);

                // Sample color from the main texture
                fixed4 mainColor = tex2D(_MainTex, uv);

                // Select color based on the segment
                fixed4 selectedColor;
                if (segment == 0.0)
                    selectedColor = _Color1;
                else if (segment == 1.0)
                    selectedColor = _Color2;
                else if (segment == 2.0)
                    selectedColor = _Color3;
                else
                    selectedColor = _Color4;

                // Calculate the dot product of the view direction and the surface normal
                float facingDot = dot(normalize(i.viewDir), normalize(i.normal));
                
                // Adjust the selected color based on whether the fragment is facing towards or away from the camera
                float darkenFactor = -0.2; // Adjust this value to control the darkness of the back face
                selectedColor.rgb -= darkenFactor * (1.0 - facingDot);

                // Clamp the color to ensure it stays within valid range
                selectedColor = clamp(selectedColor, 0.0, 1.0);

                // Calculate final color by multiplying main texture color with adjusted selected color
                fixed4 finalColor = mainColor * selectedColor;

                // Apply transparency based on the transparency range
                float transparencyThreshold = _TransparencyRange * 4.0; // Convert transparency range to UV space
                if (uv.y > transparencyThreshold)
                {
                    finalColor.a = 0; // Set alpha to 0 for fragments above the transparency threshold
                }

                return finalColor;
            }
            ENDCG
        }
    }

    Fallback "Diffuse"
}
