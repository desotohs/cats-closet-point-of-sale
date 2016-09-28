---
---

var login = {
    "login": function() {
        sessionStorage.server = $("#server").val();
        location.href = "{{ "/welcome/" | prepend: site.baseurl }}";
        return false;
    }
};
