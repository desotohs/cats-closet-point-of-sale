function angularCallback($scope) {
    $scope.customers = [
        "Obi-Wan Kenobi",
        "Yoda",
        "Vader",
        "R2D2"
    ];
    $scope.customer = {
        "name": "Obi-Wan Kenobi",
        "balance": 100,
        "picture": "http://vignette3.wikia.nocookie.net/swfanon/images/e/e1/Obiwankenobi_dsws.jpg/revision/latest?cb=20081204152935",
        "code": "012345",
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
            "price": 10,
            "enabled": true
        },
        {
            "name": "Lightsaber",
            "desc": "Purple",
            "picture": "https://s-media-cache-ak0.pinimg.com/564x/fe/85/c2/fe85c2488c48ca28fedb31c1adbe07d9.jpg",
            "price": 15,
            "enabled": true
        }
    ];
    $scope.customProperties = [
        "Gender",
        "Email"
    ];
    $scope.newProduct = {
        "name": "",
        "desc": "",
        "picture": "",
        "price": 0,
        "enabled": true
    };
    $scope.isCustomer = function(name) {
        return name && $scope.customers.indexOf(name) >= 0;
    };
    $scope.selectProduct = function(product) {
        $scope.selectedProduct = product;
    };
    var map = {};
    for ( var i = 0; i < $scope.customers.length; ++i ) {
        map[$scope.customers[i]] = null;
    }
    $("#customer").autocomplete({
        data: map
    });
}
