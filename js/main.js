function initGitVersion($scope) {
    $scope.version = "$Id$".substr(5).substr(0, "$Id$".length - 6);
}

function angularInit() {
    var app = angular.module("app", []);
    app.controller("controller", function($scope) {
        initGitVersion($scope);
        if ( window.angularCallback ) {
            angularCallback($scope);
        }
        window.$scope = $scope;
    });
}
