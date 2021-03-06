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
        $scope.$watch("shared.remote.pinDigit", function(newValue, oldValue) {
            if ( newValue && newValue != oldValue ) {
                $scope.shared.local.pin += "" + $scope.shared.remote.pinDigit;
            }
        });
        $scope.$watch("shared.remote.alertHash", function(newValue, oldValue) {
            if (newValue && newValue != oldValue) {
                Materialize.toast($scope.shared.remote.alert, 4000);
            }
        });
    }
};

function angularCallback($scope, $http) {
    screen.init($scope, $http);
    $scope.pinNeedsSet = function(pin) {
        return pin.match(/^-*$/) != null;
    };
    screen.initWatches($scope);
}
