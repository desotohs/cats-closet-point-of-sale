var office = {
    "initModel": function($scope) {
        $scope.customer = {};
        $scope.deposit = 0;
        $("#barcode").val("");
        office.allowScan = true;
    },
    "onBarcodeScan": function() {
        office.allowScan = false;
        pull(office.$http, "/customer", {
            "barcode": $("#barcode").val()
        }, $scope, "customer", function() {
            location.href = "#step-2";
        });
        return false;
    },
    "deposit": function() {
        location.href = "#step-3";
        var fakeScope = {};
        pull(office.$http, "/deposit", {
            "barcode": $("#barcode").val(),
            "amount": office.$scope.deposit
        }, fakeScope, "status", function() {
            if ( fakeScope.status.success ) {
                location.href = "#step-1";
                office.initModel(office.$scope, office.$http);
            } else {
                alert("Unable to deposit money");
                location.href = "#step-2";
            }
        });
        return false;
    }
};

function angularCallback($scope, $http) {
    office.initModel($scope);
    office.$scope = $scope;
    office.$http = $http;
}

window.onkeypress = function(e) {
    if ( office.allowScan ) {
        if ( e.keyCode == 13 ) {
            office.onBarcodeScan();
        } else if ( e.key.length == 1 ) {
            $("#barcode").val($("#barcode").val() + e.key);
        }
    }
};
