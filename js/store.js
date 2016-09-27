var store = {
    "initModel": function($scope) {
        $scope.customer = {
            "name": "Obi-Wan Kenobi",
            "balance": 100,
            "picture": "http://vignette3.wikia.nocookie.net/swfanon/images/e/e1/Obiwankenobi_dsws.jpg/revision/latest?cb=20081204152935",
            "properties": [
                {
                    "name": "Gender",
                    "value": "Male"
                },
                {
                    "name": "Email",
                    "value": "kenobobi000@example.com"
                }
            ]
        };
        $scope.products = [
            {
                "name": "Lightsaber",
                "desc": "Green",
                "picture": "http://www.sciencefriday.com/wp-content/uploads/2015/12/lightsaber6.jpg",
                "price": 10
            },
            {
                "name": "Lightsaber",
                "desc": "Purple",
                "picture": "https://s-media-cache-ak0.pinimg.com/564x/fe/85/c2/fe85c2488c48ca28fedb31c1adbe07d9.jpg",
                "price": 15
            }
        ];
        $scope.purchases = [];
        $scope.total = 0;
        $("#barcode").val("");
    },
    "onBarcodeScan": function() {
        location.href = "#step-2";
        return false;
    },
    "verifyAccount": function() {
        location.href = "#step-3";
        return false;
    },
    "purchase": function() {
        location.href = "#step-4";
        setTimeout(function() {
            location.href = "#step-1";
            store.$scope.$apply(function() {
                store.initModel(store.$scope);
            });
        }, 1000);
        return false;
    }
};

function angularCallback($scope) {
    store.initModel($scope);
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
}

window.onkeypress = function(e) {
    if ( e.keyCode == 13 ) {
        store.onBarcodeScan();
    } else if ( e.key.length == 1 ) {
        $("#barcode").val($("#barcode").val() + e.key);
    }
};
