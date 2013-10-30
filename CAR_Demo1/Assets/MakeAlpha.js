#pragma strict
var alpha = 1.0;
private var mat : Material;

function Start ()
{
    mat = new Material(
        "Shader \"Hidden/Clear Alpha\" {" +
        "Properties { _Alpha(\"Alpha\", Float)=1.0 } " +
        "SubShader {" +
        "    Pass {" +
        "        ZTest Always Cull Off ZWrite Off" +
        "        ColorMask A" +
        "        SetTexture [_Dummy] {" +
        "            constantColor(0,0,0,[_Alpha]) combine constant }" +
        "    }" +
        "}" +
        "}"
    );
}

 

function OnPostRender()
{
    GL.PushMatrix();
    GL.LoadOrtho();
    mat.SetFloat( "_Alpha", alpha );
    mat.SetPass(0);
    GL.Begin( GL.QUADS );
    GL.Vertex3( 0, 0, 0.1 );
    GL.Vertex3( 1, 0, 0.1 );
    GL.Vertex3( 1, 1, 0.1 );
    GL.Vertex3( 0, 1, 0.1 );
    GL.End();
    GL.PopMatrix();

}