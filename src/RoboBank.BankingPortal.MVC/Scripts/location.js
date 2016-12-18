function showLocation(location) {

    var latLng = new google.maps.LatLng(location.coords.latitude, location.coords.longitude);

    var mapOptions = {
        zoom: 16,
        center: latLng
    }

    var map = new google.maps.Map(document.getElementById('map'), mapOptions);

    var marker = new google.maps.Marker({
        position: latLng,
        map: map,
        title: 'You are here!'
    });
}

function getLocation() {
    if (navigator.geolocation) {
        return navigator.geolocation.getCurrentPosition(showLocation, function () { alert(error); });
    } else {
        alert("Geolocation is not supported by this browser.");
    }
}