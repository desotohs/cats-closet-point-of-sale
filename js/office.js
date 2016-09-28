var office = {
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
        $scope.deposit = 0;
        $("#barcode").val("");
    },
    "onBarcodeScan": function() {
        location.href = "#step-2";
        return false;
    },
    "deposit": function() {
        location.href = "#step-3";
        setTimeout(function() {
            location.href = "#step-1";
            office.$scope.$apply(function() {
                office.initModel(office.$scope);
            });
        }, 1000);
        return false;
    }
};

function angularCallback($scope) {
    office.initModel($scope);
    office.$scope = $scope;
}

window.onkeypress = function(e) {
    if ( e.keyCode == 13 ) {
        office.onBarcodeScan();
    } else if ( e.key.length == 1 ) {
        $("#barcode").val($("#barcode").val() + e.key);
    }
};
