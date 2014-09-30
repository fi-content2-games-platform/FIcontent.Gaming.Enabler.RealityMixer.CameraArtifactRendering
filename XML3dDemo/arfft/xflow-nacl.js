
function moduleDidLoad() {
    console.log("Module loaded!");
    nacl.supported = true;
    // module = document.getElementById('xflow-nacl');
}

function handleMessage(message) {
    // TODO: debug
    // console.log(message.data.videoWidth);
    // nacl.lastresult.data = message.data;

    XML3D.math.mat4.copy(nacl.lastresult.transform, new Float32Array(message.data.transform));
    XML3D.math.mat4.copy(nacl.lastresult.perspective, new Float32Array(message.data.perspective));

    nacl.lastresult.visibility = message.data.visibility;
}

var nacl = {
    listener: null,
    module: null,
    supported: false,
    video: {
        width: 0,
        height: 0
    },
    marker: null,
    lastresult: {
        transform: XML3D.math.mat4.create(),
        perspective: XML3D.math.mat4.create(),
        visibility: false
    }
};


Xflow.registerOperator("xflow.nacl", {
    outputs: [
        {type: 'float4x4', name : 'transform', customAlloc: true},
        {type: 'bool', name: 'visibility', customAlloc: true},
        {type: 'float4x4', name : 'perspective', customAlloc: true}
    ],
    params:  [
        {type: 'texture', source : 'arvideo'},
        {type: 'texture', source : 'marker'}
    ],
    alloc: function(sizes, arvideo, marker) {
        sizes['transform'] = 1;
        sizes['visibility'] = 1;
        sizes['perspective'] = 1;
    },
    evaluate: function(transform, visibility, perspective, arvideo, marker) {

        if (nacl.listener == null) {
            console.log("Create listener!")

            var listener = document.createElement('div');
            
            listener.addEventListener('load', moduleDidLoad, true);
            listener.addEventListener('message', handleMessage, true);

            var module = document.createElement('embed');
            module.setAttribute('width', 0);
            module.setAttribute('height', 0);
            module.setAttribute('src', 'arfft/xflow-nacl.nmf');
            module.setAttribute('type', 'application/x-pnacl');
            
            listener.appendChild(module);

            nacl.listener = listener;
            nacl.module = module;
            document.body.appendChild(nacl.listener);
        }

        // no support or module not yet loaded - return default values
        if (!nacl.supported) {
            XML3D.math.mat4.identity(transform);
            XML3D.math.mat4.identity(perspective);
            visibility[0] = false;
            return;
        }

        // detect video stream resolution changes
        if ((nacl.video.width != arvideo.width) || (nacl.video.height != arvideo.height)) {
            console.log("Video resize");
            nacl.module.postMessage({
                message: 'videoresize',
                width: arvideo.width,
                height: arvideo.height
            });
            nacl.video.width = arvideo.width;
            nacl.video.height = arvideo.height;
        }

        // TODO: detect change of the tracking target image
        if (nacl.marker == null) {
            console.log("Add tracking target");
            nacl.module.postMessage({
                message: 'addtarget',
                width: marker.width,
                height: marker.height,
                data: marker.data.buffer
            });
            nacl.marker = marker;
        }

        var s = arvideo.data;
        nacl.module.postMessage(s.buffer);

        // TODO: due to the asynchronious call 'postMessage' last results (
        //       about one frame delayed) will be used here
        XML3D.math.mat4.copy(transform, nacl.lastresult.transform);
        XML3D.math.mat4.copy(perspective, nacl.lastresult.perspective);
        // TODO: visibility is ignored outside as it is not yet supported
        visibility[0] = nacl.lastresult.visibility;

    }
});

