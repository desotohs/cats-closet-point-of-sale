---
---

function initGitVersion($scope) {
    $scope.version = "$Id$".substr(5).substr(0, "$Id: b3f0aecbaf5905b8095cc455499017fc7a8e27cc $".length - 6);
}

function angularInit() {
    var app = angular.module("app", []);
    app.controller("controller", function($scope, $http) {
        initGitVersion($scope);
        if ( window.angularCallback ) {
            angularCallback($scope, $http);
        }
        window.$scope = $scope;
        window.$http = $http;
    });
}

function pull($http, url, data, $scope, field, callback) {
    $http.post(sessionStorage.server + url, data).then(function(resp) {
        $scope[field] = resp.data;
        if ( callback ) {
            callback();
        }
    });
}

setTimeout(function() {
    if ( !sessionStorage.server && !window.login ) {
        location.href = "{{ "/" | prepend: site.baseurl }}";
    }
}, 0);
