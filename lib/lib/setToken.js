/**
 * Created by XadillaX on 14-3-21.
 */
var fs = require("fs");

module.exports = function(argv) {
    var email = "";
    var token = "";

    for(var i = 0; i < argv.length; i++) {
        if(argv[i].indexOf("-E") === 0) {
            email = argv[i].substr(2);
        } else if(argv[i].indexOf("-T") === 0) {
            token = argv[i].substr(2);
        }
    }

    var json = {
        token   : token,
        email   : email
    };

    fs.writeFile("token.json", JSON.stringify(json), {
        encoding    : "utf8",
        flag        : "w+"
    }, function(err) {
        if(err) {
            console.log(JSON.stringify({ status: false, msg: err.message }));
        } else {
            console.log(JSON.stringify({ status: true }));
        }
    })
};
