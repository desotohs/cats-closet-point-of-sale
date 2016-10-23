var settings = {
    "initModel": function($scope, $http) {
        pull($http, "/customers", {}, $scope, "customers", function() {
            var map = {};
            for ( var i = 0; i < $scope.customers.length; ++i ) {
                map[$scope.customers[i]] = null;
            }
            $("#customer").autocomplete({
                data: map
            });
        });
        pull($http, "/products", {}, $scope, "products");
        pull($http, "/properties", {}, $scope, "customProperties");
        var fakescope = {};
        pull($http, "/option", {
            "name": "Tax"
        }, fakescope, "result", function() {
            $scope.taxPercent = parseFloat(fakescope.result.value);
        });
        pull($http, "/option", {
            "name": "StoreName"
        }, fakescope, "result2", function() {
            $scope.storeName = fakescope.result2.value;
        });
        $scope.customer = {};
        $scope.newProduct = {
            "name": "",
            "desc": "",
            "picture": "",
            "price": 0,
            "enabled": true,
            "category": ""
        };
    },
    "saveCustomer": function() {
        var fakescope = {};
        pull($http, "/customer/save", $scope.customer, fakescope, "result", function() {
            if ( fakescope.result.success ) {
                Materialize.toast("Customer saved!", 4000);
            } else {
                alert("Customer save failed!");
            }
        });
        return false;
    },
    "addCustomer": function() {
        settings.$scope.customer = {
            "name": settings.$scope.customername,
            "properties": [],
            "isNew": true
        };
        var fakescope = {};
        pull($http, "/properties", {}, fakescope, "result", function() {
            for ( var i = 0; i < fakescope.result.length; ++i ) {
                settings.$scope.customer.properties.push({
                    "name": fakescope.result[i],
                    "value": ""
                });
            }
        });
    },
    "saveProduct": function() {
        var fakescope = {};
        pull($http, "/product/save", settings.$scope.selectedProduct, fakescope, "result", function() {
            if ( fakescope.result.success ) {
                alert("Product saved!");
                // primary key must be calculated into the product $scope
                location.reload();
            } else {
                alert("Unable to save product");
            }
        });
        return false;
    },
    "saveTaxes": function() {
        var fakescope = {};
        pull($http, "/option/save", {
            "name": "Tax",
            "value": settings.$scope.taxPercent
        }, fakescope, "result", function() {
            if ( fakescope.result.success ) {
                Materialize.toast("Taxes saved!", 4000);
            } else {
                alert("Taxes save failed!");
            }
        });
        return false;
    },
    "saveStore": function() {
        var fakescope = {};
        pull($http, "/option/save", {
            "name": "StoreName",
            "value": settings.$scope.storeName
        }, fakescope, "result", function() {
            if ( fakescope.result.success ) {
                Materialize.toast("Store saved!", 4000);
            } else {
                alert("Store save failed!");
            }
        });
        return false;
    },
    "saveProperties": function() {
        var fakescope = {};
        pull($http, "/properties/save", settings.$scope.customProperties, fakescope, "result", function() {
            if ( fakescope.result.success ) {
                Materialize.toast("Properties saved!", 4000);
            } else {
                alert("Properties save failed!");
            }
        });
        return false;
    }
};

function angularCallback($scope, $http) {
    settings.initModel($scope, $http);
    $scope.isCustomer = function(name) {
        return name && $scope.customers.indexOf(name) >= 0 || ($scope.customer.isNew && name == $scope.customer.name);
    };
    $scope.pullCustomer = function(name) {
        if ( $scope.isCustomer(name) ) {
            pull($http, "/customer", {
                "name": name
            }, $scope, "customer");
        } else {
            $scope.customer = {};
        }
    };
    $scope.selectProduct = function(product) {
        $scope.selectedProduct = product;
    };
    $scope.removeProperty = function($index) {
        $scope.customProperties.splice($index, 1);
    };
    $scope.addProperty = function() {
        $scope.customProperties.push(prompt("What should the new property be called?"));
    };
    settings.$scope = $scope;
    settings.$http = $http;
}

function sufficientPermissions(permissions) {
    return permissions.settings;
}
