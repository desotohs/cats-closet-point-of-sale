---
---

var session = {
    "send": function(data) {
        pull(session.$http, "/session/send", {
            "session": session.token,
            "data": JSON.stringify(data)
        }, {}, "a");
    },
    "hasResponded": function(callback) {
        var fakescope = {};
        pull(session.$http, "/session/responded", {
            "session": session.token
        }, fakescope, "response", function() {
            callback(fakescope.response.success);
        });
    },
    "initRecv": function($http, token) {
        session.$http = $http;
        session.recvLoop = true;
        var recv = function() {
            pull(session.$http, "/session/recv", {
                "session": token
            }, session, "latestPacket", function() {
                if ( session.latestPacket ) {
                    if ( session.onRead ) {
                        session.onRead(session.latestPacket);
                    }
                    session.latestPacket = false;
                }
                if ( session.recvLoop ) {
                    recv();
                }
            });
        };
        recv();
    },
    "initSend": function($http, token) {
        session.$http = $http;
        session.token = token;
    },
    "cancelRecv": function() {
        session.recvLoop = false;
    }
};

function initGitVersion($scope) {
    $scope.version = "$Id$".substr(5).substr(0, "$Id: b3f0aecbaf5905b8095cc455499017fc7a8e27cc $".length - 6);
}

function angularInit() {
    var app = angular.module("app", []);
    app.controller("controller", function($scope, $http) {
        initGitVersion($scope);
        pull($http, "/permissions", {}, $scope, "permissions", function() {
            if ( window.sufficientPermissions ) {
                if ( !sufficientPermissions($scope.permissions) ) {
                    location.href = "{{ "/" | prepend: site.baseurl }}";
                }
            }
        });
        if ( window.angularCallback ) {
            angularCallback($scope, $http);
        }
        window.$scope = $scope;
        window.$http = $http;
        window.onhashchange = function() {
            if ( location.hash.match(/^#step-[0-9]+$/) ) {
                $scope.stepNum = parseInt(location.hash.substr(6));
            }
        };
        window.onhashchange();
        $scope.goBackAStep = function() {
            location.hash = "#step-" + ($scope.stepNum - 1);
        };
    });
}

function transformPictures(obj, server) {
    if ( obj == null || typeof(obj) == "string" ) {
        return;
    }
    if ( obj["picture"] ) {
        obj["picture"] = server + obj["picture"];
    }
    var properties = Object.getOwnPropertyNames(obj);
    for ( var i = 0; i < properties.length; ++i ) {
        transformPictures(obj[properties[i]], server);
    }
}

function pull($http, url, data, $scope, field, callback) {
    $http.post(sessionStorage.server + url, data, {
        "headers": {
            "X-Auth-Token": sessionStorage.token
        }
    }).then(function(resp) {
        transformPictures(resp.data, sessionStorage.server);
        $scope[field] = resp.data;
        if ( callback ) {
            callback();
        }
    }, callback);
}

function randomToken(length) {
    var dict = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    var token = [];
    for ( var i = 0; i < length; ++i ) {
        token.push(dict[Math.floor(dict.length * Math.random())]);
    }
    return token.join("");
}

function bugReport() {
    $("#dialog-bugreport").dialog("open");
}

function initBugReport() {
    $("#dialog-bugreport").dialog({
        "resizable": false,
        "height": "auto",
        "width": 400,
        "modal": true,
        "buttons": {
            "No": function() {
                $(this).dialog("close");
            },
            "Yes": function() {
                $(this).dialog("close");
                window.open("https://github.com/zachdeibert/cats-closet-point-of-sale/issues/new", "_blank");
            }
        },
        "autoOpen": false
    });
}

setTimeout(function() {
    initBugReport();
    if ( !sessionStorage.server && !window.login ) {
        location.href = "{{ "/" | prepend: site.baseurl }}";
    }
}, 0);

window.onload = initBugReport;
