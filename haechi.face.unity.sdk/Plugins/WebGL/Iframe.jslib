var IframePlugin = {
    createIframe: function(url, readyCompleteCallback, readyCallback, showIframeCallback, hideIframeCallback)
    {
        if (document.getElementById('face-iframe')) {
            console.log('Face is already initialized, Face can be initialized only once.');
            return;
        }
        const iframeUrl = new URL(UTF8ToString(url));
    
        const overlayStyles = {
            display: 'none',
            position: 'fixed',
            top: '0',
            right: '0',
            width: '100%',
            height: '100%',
            borderRadius: '0',
            border: 'none',
            zIndex: '2147483647',
        };
        const onload = () => {
            if (!document.getElementById('face-iframe')) {
                let _iframe = document.createElement('iframe');
                _iframe.id = 'face-iframe';
                _iframe.title = 'Secure Modal';
                _iframe.src = iframeUrl.href;
                _iframe.allow = 'clipboard-read; clipboard-write';
                for (const property in overlayStyles) {
                    (_iframe.style)[property] = overlayStyles[property];
                }
                document.body.appendChild(_iframe);
            } else {}
        }
        
        window.addEventListener('message', async (e) => {
            const iframe = document.getElementById('face-iframe');
            if (!e.origin.includes(`${iframeUrl.protocol}//${iframeUrl.host}`)) {
                return;
            }
            switch (e.data.method) {
                case 'face_ready':
                    Module['dynCall_v'](readyCompleteCallback);
                    break;
                case 'face_openIframe':
                    Module['dynCall_v'](readyCallback);
                    Module['dynCall_v'](showIframeCallback);
                    break;
                case 'face_closeIframe':
                    Module['dynCall_v'](readyCallback);
                    Module['dynCall_v'](hideIframeCallback);
                    break;
            }
        });
    
        if (['loaded', 'interactive', 'complete'].includes(document.readyState)) {
            onload();
        } else {
            window.addEventListener('load', onload, false);
        }
    },
    
    sendChildMessage: function(blockchain, serializedMessage, readyCallback)
    {
        const message = JSON.parse(UTF8ToString(serializedMessage));
        Module['dynCall_v'](readyCallback);
        let iframe = document.getElementById('face-iframe');
        iframe.contentWindow.postMessage({id: message.id, method: message.method, params: message.params, blockchain: UTF8ToString(blockchain), from: 'FACE_SDK'}, '*');
    },
    
    waitForResponse: function(id, responseCallback)
    {
        const requestId = UTF8ToString(id);
        return new Promise((resolve, reject) => {
            const listener = (event) => {
                const response = event.data;
                const responseId = response.id ?? null;
                if (requestId && responseId != requestId) {
                    return;
                }
                window.removeEventListener('message', listener);
                if (response.error) {
                    reject(response.error);
                }
               
                resolve(response.result);
            };
            window.addEventListener('message', listener);
        }).then(result => {
            var requestIdBufferSize = lengthBytesUTF8(requestId) + 1;
            var requestIdBuffer = _malloc(requestIdBufferSize);
            stringToUTF8(requestId, requestIdBuffer, requestIdBufferSize);
            
            let stringResponse;
            try {
                stringResponse = JSON.stringify({
                    id: requestId,
                    jsonrpc: '2.0',
                    result: result
                });
            } catch (e) {
                stringResponse = JSON.stringify({
                    id: requestId, 
                    jsonrpc: '2.0',
                    result: null
                });
            }
            var responseBufferSize = lengthBytesUTF8(stringResponse) + 1;
            var responseBuffer = _malloc(responseBufferSize);
            stringToUTF8(stringResponse, responseBuffer, responseBufferSize);

            Module['dynCall_vii'](responseCallback, requestIdBuffer, responseBuffer);
        }).catch(error => {
            if (error.message.includes('user rejected request')) {
                var requestIdBufferSize = lengthBytesUTF8(requestId) + 1;
                var requestIdBuffer = _malloc(requestIdBufferSize);
                stringToUTF8(requestId, requestIdBuffer, requestIdBufferSize);
                
                var stringResponse = 'UserClosedIframe';
                var responseBufferSize = lengthBytesUTF8(stringResponse) + 1;
                var responseBuffer = _malloc(responseBufferSize);
                stringToUTF8(stringResponse, responseBuffer, responseBufferSize);
                
                Module['dynCall_vii'](responseCallback, requestIdBuffer, responseBuffer);
            }
            throw error;
        });
    },
    
    showOverlay: function(readyCallback) 
    {
        Module['dynCall_v'](readyCallback);
        const iframe = document.getElementById('face-iframe');
        iframe.style.display = 'block';
        iframe.focus();
    },
    
    hideOverlay: function(readyCallback) 
    {
        Module['dynCall_v'](readyCallback);
        const iframe = document.getElementById('face-iframe');
        iframe.style.display = 'none';
    },
    
    consoleLog: function(log)
    {
        console.log("LOG: " + UTF8ToString(log));
    }
};

mergeInto(LibraryManager.library, IframePlugin);