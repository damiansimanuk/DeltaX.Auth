/// <reference path="oidc-client.js" />

function log() {
    document.getElementById('results').innerText = '';

    Array.prototype.forEach.call(arguments, function (msg) {
        if (msg instanceof Error) {
            msg = "Error: " + msg.message;
        }
        else if (typeof msg !== 'string') {
            msg = JSON.stringify(msg, null, 2);
        }
        document.getElementById('results').innerHTML += msg + '\r\n';
    });
}

document.getElementById("login").addEventListener("click", login, false);
document.getElementById("api").addEventListener("click", api, false);
document.getElementById("api_private").addEventListener("click", api_private, false);
document.getElementById("logout").addEventListener("click", logout, false);

var config = {
    authority: "https://localhost:5011",
    client_id: "js",
    client_secret : "js", 
    redirect_uri: "https://localhost:5013/callback.html",
    response_type: "code",
    scope:"openid profile api1",
    post_logout_redirect_uri: "https://localhost:5013/index.html",
};
var mgr = new Oidc.UserManager(config);

mgr.getUser().then(function (user) {
    if (user) {
        log("User logged in", user.profile);
    }
    else {
        log("User not logged in");
    }
});

function login() {
    mgr.signinRedirect();
}

function api() {
    mgr.getUser().then(function (user) {
        var url = "https://localhost:6001/identity";

        var xhr = new XMLHttpRequest();
        xhr.open("GET", url);
        xhr.onload = function () {
            log(xhr.status, "api Result:" , JSON.parse(xhr.responseText));
        }
        xhr.onerror = function () {
            log(xhr.status, "api Error:" ,  xhr.responseText);
        }
        xhr.setRequestHeader("Authorization", "Bearer " + user.access_token);
        xhr.send();
    });
}



function api_private() {
    mgr.getUser().then(function (user) {
        var url = "https://localhost:6001/identity/private";

        var xhr = new XMLHttpRequest();
        xhr.open("GET", url);
        xhr.onload = function () {
            log(xhr.status, "api_private Result:" ,JSON.parse(xhr.responseText));
        }
        xhr.onerror = function () {
            log(xhr.status, "api_private Error:" , xhr.responseText);
        }
        xhr.setRequestHeader("Authorization", "Bearer " + user.access_token);
        xhr.send();
    });
}

function logout() {
    mgr.signoutRedirect();
}