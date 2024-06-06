const Alexa = require('ask-sdk-core');
const https = require('https');

const BackendProxyHandler = {
    canHandle(handlerInput) {
        return Alexa.getRequestType(handlerInput.requestEnvelope) === 'IntentRequest';
    },
    handle(handlerInput) {
        const backendUrl = 'https://iot.xaverb.dev/api/api/Alexa'; // Replace with your backend URL
        
        return new Promise((resolve, reject) => {
            const options = {
                hostname: backendUrl.split('/')[2],
                path: '/' + backendUrl.split('/').slice(3).join('/'),
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                rejectUnauthorized: false, // Disable certificate verification
            };

            const req = https.request(options, (res) => {
                let data = '';

                res.on('data', (chunk) => {
                    data += chunk;
                });

                res.on('end', () => {
                    if (res.statusCode === 200) {
                        const responseBody = JSON.parse(data);
                        resolve(handlerInput.responseBuilder
                            .speak(responseBody.response.outputSpeech.text)
                            .getResponse());
                    } else {
                        reject(new Error(`Backend request failed with status code: ${res.statusCode}`));
                    }
                });
            });

            req.on('error', (error) => {
                reject(error);
            });

            req.write(JSON.stringify(handlerInput.requestEnvelope));
            req.end();
        });
    },
};

const ErrorHandler = {
    canHandle() {
        return true;
    },
    handle(handlerInput, error) {
        console.error(`Error handled: ${error.message}`);

        return handlerInput.responseBuilder
            .speak('Sorry, an error occurred while processing your request.')
            .getResponse();
    },
};

exports.handler = Alexa.SkillBuilders.custom()
    .addRequestHandlers(BackendProxyHandler)
    .addErrorHandlers(ErrorHandler)
    .lambda();