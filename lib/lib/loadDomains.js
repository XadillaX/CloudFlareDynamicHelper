/**
 * Created by XadillaX on 14-3-21.
 */
var async = require("async");
var spidex = require("spidex");
var querystring = require("querystring");
var loadToken = require("./loadToken");
var uri = require("./uri");

module.exports = function() {
    async.waterfall([
        /**
         * step 1.
         *     loading token information
         *
         * @param callback
         */
        function(callback) {
            loadToken(function(err, info) {
                callback(err, info);
            });
        },

        /**
         * step 2.
         *     load domain information.
         *
         * @param token
         * @param callback
         */
        function(token, callback) {
            var data = {
                a       : "zone_load_multi",
                tkn     : token.token,
                email   : token.email
            };

            var qs = querystring.stringify(data);
            spidex.get(uri + "?" + qs, function(html, status, respHeader) {
                if(status !== 200) {
                    callback(new Error("Wrong server status."));
                } else {
                    var json = {};
                    try {
                        json = JSON.parse(html);
                    } catch(e) {
                        callback(e);
                        return;
                    }

                    callback(null, json);
                }
            }, "utf8").on("error", function(e) {
                callback(e);
            })
        }
    ], function(err, data) {
        if(err) {
            console.log({ status: false, msg: err.message });
        } else {
            console.log(JSON.stringify({ status: true, result: data }, null, 2));
        }
    });
};
