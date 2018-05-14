var app = {
    // Application Constructor
    initialize: function () {
        this.bindEvents();
    },
    bindEvents: function () {
        document.addEventListener('deviceready', this.onDeviceReady, false);
    },
    onDeviceReady: function () {
        app.receivedEvent('deviceready');


 
    },
    // Note: This code is taken from the Cordova CLI template.
    receivedEvent: function (id) {
        var parentElement = document.getElementById(id);
        var listeningElement = parentElement.querySelector('.listening');
        var receivedElement = parentElement.querySelector('.received');

        listeningElement.setAttribute('style', 'display:none;');
        receivedElement.setAttribute('style', 'display:block;');

        console.log('Received Event: ' + id);
    }
};

// Here, we redirect to the web site.

var storage = window.localStorage;

function myFunction() {
    document.getElementById('msg').style.display = 'block';
    var targetUrl = document.getElementById("bkpLink").value;
    storage.setItem('url', targetUrl);
    var targeturl = storage.getItem('url');
    window.location.replace(targeturl);
}


app.initialize();