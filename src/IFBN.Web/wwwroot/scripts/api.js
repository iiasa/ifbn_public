var APIInstance = function () {
    var self = this;

    var calls = new Array();
	var baseUrl = "https://geo-wiki.org/application/api/";

    var call = function (controller, action, parameter, success, content) {
        var url = baseUrl + encodeURIComponent(controller) + "/" + encodeURIComponent(action);

        var request = $.ajax({
            type: "POST",
            url: url,
            data: { 'parameter': JSON.stringify(parameter) },
            success: success,
            dataType: content
        });

        calls.push(request);

        return self;
    };

    self.done = function (callback) {
        $.when.apply(null, calls).done(function () {

            if (calls.length == 1) {
                callback(arguments[0]);
            }
            else {
                var values = new Array();

                // Call with data values of result for easier usage
                $.each(arguments, function (index, value) {
                    values.push(value[0]);
                });

                callback.apply(null, values);
            }
        });

        return self;
    };

    self.fail = function (callback) {
        $.when.apply(null, calls).fail(callback);

        return self;
    };

    self.callJson = function (controller, action, parameter, success) {
        return call(controller, action, parameter, success, "json");
    };

    self.callKml = function (controller, action, parameter, success) {
        return call(controller, action, parameter, success, "kml");
    };

    self.callKmz = function (controller, action, parameter, success) {
        return call(controller, action, parameter, success, "kmz");
    };

    self.callHtml = function (controller, action, parameter, success) {
        return call(controller, action, parameter, success, "html");
    };

    self.callText = function (controller, action, parameter, success) {
        return call(controller, action, parameter, success, "text");
    };

    self.callXml = function (controller, action, parameter, success) {
        return call(controller, action, parameter, success, "xml");
    };
};

var api = (function () {
    // jQuery setup for custom content types
    $.ajaxSetup({
        accepts: {
            kml: "application/vnd.google-earth.kml+xml",
            kmz: "application/vnd.google-earth.kmz"
        },
        converters: {
            "text kml": function (value) {
                if (ge) return ge.parseKml(value);
                throw "ge object not set.";
            },
            "text kmz": true
        }
    });

    return {
        callJson: function (controller, action, parameter, success) {
            return new APIInstance().callJson(controller, action, parameter, success);
        },
        callKml: function (controller, action, parameter, success) {
            return new APIInstance().callKml(controller, action, parameter, success);
        },
        callKmz: function (controller, action, parameter, success) {
            return new APIInstance().callKmz(controller, action, parameter, success);
        },
        callHtml: function (controller, action, parameter, success) {
            return new APIInstance().callHtml(controller, action, parameter, success);
        },
        callText: function (controller, action, parameter, success) {
            return new APIInstance().callText(controller, action, parameter, success);
        },
        callXml: function (controller, action, parameter, success) {
            return new APIInstance().callXml(controller, action, parameter, success);
        }
    };
})();