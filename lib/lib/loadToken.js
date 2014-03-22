/**
 * Created by XadillaX on 14-3-21.
 */
var fs = require("fs");
module.exports = function(callback) {
    fs.readFile("token.json", { encoding: "utf8" }, function(err, data) {
        if(err) {
            callback(err);
        } else {
            var json = {};
            try {
                json = JSON.parse(data);
            } catch(e) {
                callback(e);
                return;
            }

            if(undefined === json.email || undefined === json.token) {
                callback(new Error("Broken token information."));
                return;
            }

            callback(null, json);
        }
    })
};
