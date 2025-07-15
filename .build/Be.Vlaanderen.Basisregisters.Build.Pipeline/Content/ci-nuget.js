var fs = require('fs');
var os = require('os');
var FormData = require('form-data');

var userAgent = 'NodeJs/' + process.version + ' (' + os.platform() + ' ' + os.release() + ')'

/**
 * @param {string} filepath fs path to nuget package.
 * @param {string} host host domain name eg. www.nuget.org
 * @param {string} ApiKey nuget api key
 * @param {function} callback params: (error, response) => returned response from http.request
 * @return {ClientRequest}
 */
function push (filepath, host, apiKey, callback) {
    var parts = host.split('//');
    var protocol = "http:";

    if (parts.length > 1){
        protocol = parts[0];
        host = parts[1].split('/')[0]; //make sure we chop of any slashes
    }

    var options = {
        host: host,
        protocol: protocol,
        path: '/api/v2/package/',
        method: 'PUT',
        headers: {
            'X-NuGet-ApiKey': apiKey,
            'X-NuGet-Client-Version': '4.2.0',
            'user-agent': userAgent,
            'Accept-Language': 'en-US'
        }
    };
    var form = new FormData();
    form.append('package', fs.createReadStream(filepath));
    return form.submit(options, callback);
}

push(process.argv[2], process.env.NUGET_HOST, process.env.NUGET_API_KEY, function(error, response){
    if (error)
        throw error;

    if (response.statusCode === 201) {
        //Success
    } else {
        console.warn(response.statusCode + ":" + response.statusMessage);
        //eg: 409: Package already exists;
    }
});
