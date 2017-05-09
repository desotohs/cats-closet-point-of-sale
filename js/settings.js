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
        pull($http, "/products", {}, $scope, "products", function() {
            var map = {};
            var catMap = {};
            for ( var i = 0; i < $scope.products.length; ++i ) {
                map[$scope.products[i].name] = null;
                catMap[$scope.products[i].category] = null;
            }
            $("#product").autocomplete({
                data: map
            });
            $(".product-category-autocomplete").autocomplete({
                data: catMap
            });
        });
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
        pull($http, "/users", {}, $scope, "users");
        $scope.customer = {};
        $scope.newProduct = {
            "name": "",
            "desc": "",
            "picture": "",
            "price": 0,
            "enabled": true,
            "category": "",
            "inventory": 0
        };
        $scope.newUser = {
            "username": "",
            "password": "",
            "storeAccess": false,
            "officeAccess": false,
            "settingsAccess": false,
            "invalidateToken": false,
            "inventory": 0
        };
        $scope.links = [];
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
        pull($http, "/product/save", settings.$scope.selectedProduct || settings.$scope.newProduct, fakescope, "result", function() {
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
    "saveUser": function() {
        var fakescope = {};
        pull($http, "/user/save", settings.$scope.selectedUser || settings.$scope.newUser, fakescope, "result", function() {
            if ( fakescope.result.success ) {
                alert("User saved!");
                // primary key must be calculated into the user $scope
                location.reload();
            } else {
                alert("Unable to save user");
            }
        });
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
    },
    "saveImages": function() {
        var file = document.getElementById("image").files[0];
        if ( file.size > 0 ) {
            var reader = new FileReader();
            reader.addEventListener("load", function() {
                var req = {
                    "data": reader.result.split(',')[1],
                    "customerNames": [],
                    "productIds": []
                };
                for ( var i = 0; i < settings.$scope.links.length; ++i ) {
                    req[settings.$scope.links[i].type].push(settings.$scope.links[i].id);
                }
                var fakescope = {};
                pull(settings.$http, "/image/save", req, fakescope, "result", function() {
                    if ( fakescope.result.success ) {
                        Materialize.toast("Image saved!", 4000);
                    } else {
                        alert("Image save failed!");
                    }
                });
            }, false);
            reader.readAsDataURL(file);
        }
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
    $scope.selectUser = function(user) {
        $scope.selectedUser = user;
    };
    $scope.removeProperty = function($index) {
        $scope.customProperties.splice($index, 1);
    };
    $scope.addProperty = function() {
        $scope.customProperties.push(prompt("What should the new property be called?"));
    };
    $scope.removeLink = function($index) {
        $scope.links.splice($index, 1);
    };
    $scope.linkProduct = function() {
        var product = null;
        for ( var i = 0; i < $scope.products.length; ++i ) {
            if ( $scope.products[i].name == $scope.productname ) {
                product = $scope.products[i];
                break;
            }
        }
        if ( product == null ) {
            return;
        }
        $scope.links.push({
            "name": $scope.productname,
            "type": "productIds",
            "id": product.id
        });
    };
    $scope.linkCustomer = function() {
        if ( $scope.isCustomer($scope.customername) ) {
            $scope.links.push({
                "name": $scope.customername,
                "type": "customerNames",
                "id": $scope.customername
            });
        }
    };
    $scope.import = function() {
        var rdr = new FileReader();
        rdr.addEventListener("load", function() {
            var callback = function() {
                if ($scope.importData.error) {
                    Materialize.toast($scope.importData.error, 4000);
                    $scope.importData = null;
                }
                if ($scope.importData.completeSteps != $scope.importData.totalSteps) {
                    setTimeout(function() {
                        pull($http, "/import/status", "'" + $scope.importData.importId + "'", $scope, "importData", callback);
                    }, 100);
                }
            };
            pull($http, "/import/start", {
                "type": $scope.importType,
                "data": rdr.result.split(",")[1]
            }, $scope, "importData", callback);
        });
        rdr.readAsDataURL($("#importFile")[0].files[0]);
    };
    $scope.getImportProgress = function() {
        return {
            "width": "" + ($scope.importData ? $scope.importData.completeSteps * 100 / $scope.importData.totalSteps : 0) + "%"
        };
    };
    settings.$scope = $scope;
    settings.$http = $http;
}

function sufficientPermissions(permissions) {
    return permissions && permissions.settings;
}
