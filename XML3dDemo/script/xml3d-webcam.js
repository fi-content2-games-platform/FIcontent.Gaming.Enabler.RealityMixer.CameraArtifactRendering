
window.URL = window.URL || window.webkitURL;
navigator.getUserMedia  =
	navigator.getUserMedia || navigator.webkitGetUserMedia ||
	navigator.mozGetUserMedia || navigator.msGetUserMedia;

function activateWebCam(inputId) {
    var video = document.getElementById(inputId);
    if (!video) {
        XML3D.debug.logError("No video element");
        return;
    }

    // audio: true does not work with chrome
    if (navigator.getUserMedia) {
        navigator.getUserMedia({video: true, audio: false},
	        function(stream) {
	            XML3D.debug.logInfo("Accessing WebCam");
	            video.src = window.URL.createObjectURL(stream);
	        },
	        function (e) {
	            XML3D.debug.logError("Cannot access WebCam", e);
	        });
    } else {
        XML3D.debug.logError("No getUserMedia");
    }
}


window.addEventListener('load', function() {
    activateWebCam("inputvideo");
});

