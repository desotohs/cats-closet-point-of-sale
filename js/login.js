---
---

var login = {
    "login": function() {
        sessionStorage.server = $("#server").val();
        var fakescope = {};
        pull(login.$http, "/authenticate", {
            "username": $("#username").val(),
            "password": $("#password").val()
        }, fakescope, "result", function() {
            if ( fakescope.result.token ) {
                sessionStorage.token = fakescope.result.token;
                location.href = "{{ "/welcome" | prepend: site.baseurl }}";
            } else {
                Materialize.toast("Invalid credentials", 4000);
            }
        });
        return false;
    },
    "connect": function() {
        sessionStorage.server = $("#server2").val();
        sessionStorage.displayToken = $("#token").val();
        location.href = "{{ "/screen" | prepend: site.baseurl }}";
        return false;
    },
    "init": function() {
        sessionStorage.token = "";
    }
};

function angularCallback($scope, $http) {
    login.$http = $http;
}

login.init();
