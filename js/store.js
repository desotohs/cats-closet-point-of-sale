var store = {
    "initModel": function($scope, $http) {
        $scope.customer = {};
        pull($http, "/products/enabled", {}, $scope, "products");
        $scope.purchases = [];
        $scope.total = 0;
        $("#barcode").val("");
    },
    "onBarcodeScan": function() {
        pull(store.$http, "/customer", {
            "barcode": $("#barcode").val()
        }, $scope, "customer", function() {
            location.href = "#step-2";
        });
        return false;
    },
    "verifyAccount": function() {
        location.href = "#step-3";
        return false;
    },
    "purchase": function() {
        location.href = "#step-4";
        var purchases = [];
        for ( var i = 0; i < store.$scope.purchases.length; ++i ) {
            purchases[i] = store.$scope.purchases[i].product.id;
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
    }
};

function angularCallback($scope, $http) {
    store.initModel($scope, $http);
    $scope.buy = function($index) {
        $scope.purchases.push({
            "product": $scope.products[$index]
        });
        $scope.total += $scope.products[$index].price;
    };
    $scope.cancelPurchase = function($index) {
        var purchase = $scope.purchases.splice($index, 1)[0];
        $scope.total -= purchase.product.price;
    };
    store.$scope = $scope;
    store.$http = $http;
}

window.onkeypress = function(e) {
    if ( e.keyCode == 13 ) {
        store.onBarcodeScan();
    } else if ( e.key.length == 1 ) {
        $("#barcode").val($("#barcode").val() + e.key);
    }
};
