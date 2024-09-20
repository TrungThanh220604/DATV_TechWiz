// Header
document.querySelectorAll('.go-signup').forEach(function (element) {
    element.addEventListener('click', function () {
        document.getElementById('sign-in').style.display = 'none';
        document.getElementById('sign-up').style.display = 'block';
    });
});

document.querySelectorAll('.go-signin').forEach(function (element) {
    element.addEventListener('click', function () {
        document.getElementById('sign-up').style.display = 'none';
        document.getElementById('sign-in').style.display = 'block';
    });
});

document.querySelectorAll('.open-log').forEach(function (element) {
    element.addEventListener('click', function () {
        document.getElementById('form-sign').style.display = 'block';
    });
});

document.querySelector('.sign-close-icon').addEventListener('click', function () {
    document.getElementById('form-sign').style.display = 'none';
});

document.querySelector('.notification-txt').addEventListener('click', function (event) {
    event.stopPropagation();
    var navuserlist = document.getElementById('nav-user-list');
    var notificationList = document.getElementById('notification-list');
    if (navuserlist.style.display === 'block') {
        navuserlist.style.display = 'none';
    }
    if (notificationList.style.display === 'block') {
        notificationList.style.display = 'none';
    } else {
        notificationList.style.display = 'block';
    }
});

document.querySelector('.nav-user-name').addEventListener('click', function (event) {
    event.stopPropagation();
    var notificationList = document.getElementById('notification-list');
    var navuserlist = document.getElementById('nav-user-list');
    if (notificationList.style.display === 'block') {
        notificationList.style.display = 'none';
    }
    if (navuserlist.style.display === 'block') {
        navuserlist.style.display = 'none';
    } else {
        navuserlist.style.display = 'block';
    }
});
