Shader "Skybox/Gradient" {
Properties {
    _SkyTint ("Sky Tint", Color) = (.5, .5, .5, 1)
    _GroundColor ("Ground", Color) = (.369, .349, .341, 1)
    _SkySize("Sky Size", Float) = 0.01
    _SkyOffset ("Sky Offset", Float) = 10
}

SubShader {
    Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
    Cull Off ZWrite Off

    Pass {

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag

        #include "UnityCG.cginc"

        uniform half3 _SkyTint;
        uniform half3 _GroundColor;
        uniform half _SkySize;
        uniform half _SkyOffset;

        struct appdata_t
        {
            float4 vertex : POSITION;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct v2f
        {
            float4  pos             : SV_POSITION;
            float3  vertex          : TEXCOORD0;

            UNITY_VERTEX_OUTPUT_STEREO
        };

        v2f vert(appdata_t v) {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.vertex = UnityObjectToClipPos(v.vertex);
            return o;
        }
        half4 frag (v2f IN) : SV_Target
        {
            half3 col = lerp(_SkyTint, _GroundColor, clamp((IN.vertex.y + _SkyOffset) * _SkySize, 0, 1));

            return half4(col,1.0);

        }
        ENDCG
    }
}
}
