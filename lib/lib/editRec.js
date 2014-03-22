/**
 * Created by XadillaX on 14-3-21.
 */
var async = require("async");
var spidex = require("spidex");
var querystring = require("querystring");
var loadToken = require("./loadToken");
var uri = require("./uri");

module.exports = function(argv) {
    var domain = "";
    var name = "";
    var content = "";
    var id = "";
    var type = "A";
    var ttl = "1";

    for(var i = 0; i < argv.length; i++) {
        if(argv[i].indexOf("-D") === 0) {
            domain = argv[i].substr(2);
        } else if(argv[i].indexOf("-N") === 0) {
            name = argv[i].substr(2);
        } else if(argv[i].indexOf("-IP") === 0) {
            content = argv[i].substr(3);
        } else if(argv[i].indexOf("-ID") === 0) {
            id = argv[i].substr(3);
        } else if(argv[i].indexOf("-T") === 0) {
            type = argv[i].substr(2);
        } else if(argv[i].indexOf("-TTL") === 0) {
            ttl = argv[i].substr(4);
        }
    }

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
         *     load records information.
         *
         * @param token
         * @param callback
         */
        function(token, callback) {
            var data = {
                a       : "rec_edit",
                tkn     : token.token,
                email   : token.email,
                z       : domain,

                id      : id,
                name    : name,
                content : content,

                type    : type,
                ttl     : ttl
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
