XML3D.shaders.register("background-video", {

    vertex : [
        "attribute vec2 position;",

        "varying vec2 fragTexCoord;",

        "void main(void) {",
        "    gl_Position = vec4(position, 0.9999, 1.0);",
        "    fragTexCoord = 0.5 * position + vec2(0.5);",
        "}"
    ].join("\n"),

    fragment : [
        "#ifdef GL_ES",
          "precision highp float;",
        "#endif",

        "uniform sampler2D videoTex;",

        "varying vec2 fragTexCoord;",

        "void main(void) {",
        "  vec3 color = texture2D(videoTex, fragTexCoord).rgb;",
        "  gl_FragColor = vec4(color, 1.0);",
        "}"
    ].join("\n"),

    samplers: { 
        videoTex : null
    }
});

// we have to deactivate frustum culling at all - otherwise the background disappears
// console.log(XML3D.options.getKeys());
XML3D.options.setValue("renderer-frustumCulling", false);
