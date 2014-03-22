/**
 * Created by XadillaX on 14-3-21.
 */
var sugar = require("sugar/release/sugar-full.development");
var spidex = require("spidex");

// parse cmd
if(process.argv.length === 2) {
    console.log("Please input valid commands.");
    console.log("             -- CloudFlare Command Line Tool");
    return;
}

// each function
var setToken = require("./lib/setToken");
var loadDomains = require("./lib/loadDomains");
var loadRec = require("./lib/loadRec");
var editRec = require("./lib/editRec");

var mainArgv = process.argv[2].toLowerCase();
switch(mainArgv) {
    case "settoken": {
        setToken(process.argv.last(process.argv.length - 3));
        break;
    }

    case "loaddomains": {
        loadDomains();
        break;
    }

    case "loadrecord": {
        loadRec(process.argv.last(process.argv.length - 3));
        break;
    }

    case "editrecord": {
        editRec(process.argv.last(process.argv.length - 3));
        break;
    }

    case "getip": {
        spidex.get("http://ip-api.com/json", function(html, status, respHeader) {
            if(status !== 200) {
                console.log("0.0.0.0");
                return;
            }

            var json = {};
            try {
                json = JSON.parse(html);
            } catch(e) {
                console.log("0.0.0.0");
                return;
            }

            console.log(json.query);
        });

        break;
    }

    default: {
        console.log("Please input right command.");
        break;
    }
}
