var store = {
    "initModel": function($scope, $http) {
        pull($http, "/products/enabled", {}, $scope, "products");
        $scope.shared = {
            "local": {
                "store": true,
                "total": 0,
                "customer": {}
            }
        };
        $("#barcode").val("");
    },
    "onBarcodeScan": function() {
        pull(store.$http, "/customer", {
            "barcode": $("#barcode").val()
        }, $scope.shared.local, "customer", function() {
            if ( $scope.shared.local.customer ) {
                location.href = "#step-2";
            } else {
                $("#barcode").val("");
            }
        });
        return false;
    },
    "verifyAccount": function() {
        location.href = "#step-3";
        $scope.shared.local.purchases = [];
        return false;
    },
    "purchase": function() {
        location.href = "#step-4";
        var purchases = [];
        for ( var i = 0; i < store.$scope.shared.local.purchases.length; ++i ) {
            purchases[i] = store.$scope.shared.local.purchases[i].product.id;
        }
        var fakeScope = {};
        pull(store.$http, "/purchase", {
            "barcode": $("#barcode").val(),
            "purchases": purchases
        }, fakeScope, "status", function() {
            if ( fakeScope.status.success ) {
                location.href = "#step-1";
                store.initModel(store.$scope, store.$http);
            } else {
                alert("Unable to purchase items");
                location.href = "#step-3";
            }
        });
        return false;
    },
    "initDisplay": function($scope, $http) {
        $scope.displayToken = randomToken(6);
        var tokens = {
            "toDisplay": randomToken(64),
            "toControl": randomToken(64)
        };
        session.initSend($http, $scope.displayToken);
        session.send(tokens);
        session.initSend($http, tokens.toDisplay);
        session.initRecv($http, tokens.toControl);
        session.onRead = function(data) {
            if ( data.connected ) {
                $scope.displayToken = false;
                session.onRead = function(data) {
                    $scope.shared.remote = data;
                };
                $scope.$watch("shared.local", function(newValue) {
                    session.send($scope.shared.local);
                }, true);
            }
        };
    }
};

function angularCallback($scope, $http) {
    store.initModel($scope, $http);
    store.initDisplay($scope, $http);
    $scope.buy = function($index) {
        $scope.shared.local.purchases.push({
            "product": $scope.products[$index]
        });
        $scope.shared.local.total += $scope.products[$index].price;
    };
    $scope.cancelPurchase = function($index) {
        var purchase = $scope.shared.local.purchases.splice($index, 1)[0];
        $scope.shared.local.total -= purchase.product.price;
    };
    store.$scope = $scope;
    store.$http = $http;
}

function sufficientPermissions(permissions) {
    return permissions.store;
}

window.onkeypress = function(e) {
    if ( e.keyCode == 13 ) {
        store.onBarcodeScan();
    } else if ( e.key.length == 1 ) {
        $("#barcode").val($("#barcode").val() + e.key);
    }
};
