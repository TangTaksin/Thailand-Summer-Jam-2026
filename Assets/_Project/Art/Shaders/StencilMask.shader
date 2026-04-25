Shader "Custom/StencilMask"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            ColorMask 0 // ไม่แสดงสี

            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace
            }
        }
    }
}