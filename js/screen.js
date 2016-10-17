var screen = {
    "init": function($scope, $http) {
        $scope.shared = {
            "local": {}
        };
        session.onRead = function(data) {
            if ( data.toDisplay && data.toControl ) {
                session.cancelRecv();
                session.initSend($http, data.toControl);
                session.initRecv($http, data.toDisplay);
                session.onRead = function(data) {
                    $scope.shared.remote = data;
                };
                session.send({
                    "connected": true
                });
                $scope.$watch("shared.local", function(newValue) {
                    session.send($scope.shared.local);
                }, true);
            }
        };
        session.initRecv($http, sessionStorage.displayToken);
    },
    "initWatches": function($scope) {
        $scope.$watch("shared.remote.resetPass", function(newValue, oldValue) {
            if ( newValue != oldValue ) {
                $scope.shared.local.pin = "";
            }
        });
    }
};

function angularCallback($scope, $http) {
    screen.init($scope, $http);
    screen.initWatches($scope);
}
