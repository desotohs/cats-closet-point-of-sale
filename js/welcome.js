---
---

function sufficientPermissions(permissions) {
    if ( permissions.store ) {
        location.href = "{{ "/store" | prepend: site.baseurl }}";
    } else if ( permissions.office ) {
        location.href = "{{ "/office" | prepend: site.baseurl }}";
    } else if ( permissions.settings ) {
        location.href = "{{ "/settings" | prepend: site.baseurl }}";
    } else {
        return false;
    }
    return true;
}
