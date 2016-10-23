var store = {
    "initModel": function($scope, $http) {
        pull($http, "/products/enabled", {}, $scope, "products", function() {
            $scope.productsByCategory = [];
            mainloop: for ( var i = 0; i < $scope.products.length; ++i ) {
                for ( var j = 0; j < $scope.productsByCategory.length; ++j ) {
                    if ( $scope.products[i].category == $scope.productsByCategory[j].name ) {
                        $scope.productsByCategory[j].products.push($scope.products[i]);
                        continue mainloop;
                    }
                }
                $scope.productsByCategory.push({
                    "name": $scope.products[i].category,
                    "products": [
                        $scope.products[i]
                    ]
                });
            }
        });
        $scope.shared = {
            "local": {
                "store": true,
                "total": 0,
                "customer": {}
            }
        };
        var fakescope = {};
        pull($http, "/option", {
            "name": "StoreName"
        }, fakescope, "result", function() {
            $scope.shared.local.storeName = fakescope.result.value;
        });
        $("#barcode").val("");
    },
    "onBarcodeScan": function() {
        pull(store.$http, "/customer", {
            "barcode": $("#barcode").val()
        }, $scope.shared.local, "customer", function() {
            if ( $scope.shared.local.customer ) {
                location.href = "#step-2";
                window.onhashchange();
            } else {
                $("#barcode").val("");
            }
        });
        return false;
    },
    "verifyAccount": function() {
        location.href = "#step-3";
        window.onhashchange();
        $scope.shared.local.purchases = [];
        return false;
    },
    "sendPurchase": function() {
        var purchases = [];
        for ( var i = 0; i < store.$scope.shared.local.purchases.length; ++i ) {
            purchases[i] = store.$scope.shared.local.purchases[i].product.id;
        }
        var fakeScope = {};
        pull(store.$http, "/purchase", {
            "barcode": $("#barcode").val(),
            "pin": store.$scope.shared.remote.pin,
            "purchases": purchases
        }, fakeScope, "status", function() {
            if ( fakeScope.status.success ) {
                location.href = "#step-1";
                window.onhashchange();
                store.initModel(store.$scope, store.$http);
            } else {
                alert("Invalid pin");
                store.purchase();
            }
        });
    },
    "purchase": function() {
        location.href = "#step-4";
        window.onhashchange();
        if ( store.$scope.shared.local.customer.pinLength > 0 ) {
            store.$scope.$apply(function() {
                store.$scope.shared.local.resetPass = Math.random();
                store.$scope.shared.local.promptPass = true;
            });
            var unbind = $scope.$watch("shared.remote.pin", function(newValue) {
                if ( newValue.length == $scope.shared.local.customer.pinLength ) {
                    unbind();
                    store.sendPurchase();
                    store.$scope.shared.local.resetPass = Math.random();
                }
            });
        } else {
            store.sendPurchase();
        }
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
    $scope.buy = function(category$index, $index) {
        $scope.shared.local.purchases.push({
            "product": $scope.productsByCategory[category$index].products[$index]
        });
        $scope.shared.local.total += $scope.productsByCategory[category$index].products[$index].price;
    };
    $scope.cancelPurchase = function($index) {
        var purchase = $scope.shared.local.purchases.splice($index, 1)[0];
        $scope.shared.local.total -= purchase.product.price;
    };
    store.$scope = $scope;
    store.$http = $http;
}

function sufficientPermissions(permissions) {
    return permissions && permissions.store;
}

window.onkeypress = function(e) {
    if ( e.keyCode == 13 ) {
        store.onBarcodeScan();
    } else if ( e.key.length == 1 ) {
        $("#barcode").val($("#barcode").val() + e.key);
    }
};
