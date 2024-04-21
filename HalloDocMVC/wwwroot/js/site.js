// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(window).on('ready', function () {
    $('#loader').fadeIn();
});

$(window).on('load', function () {
    $('#loader').fadeOut();
});

$(document).ready(function () {
    // Show loader on Ajax start
    $(document).ajaxStart(function () {
        console.log('ajax start called')
        $('#loader').fadeIn();
    });

    // Hide loader on Ajax stop (completion)
    $(document).ajaxStop(function () {
        console.log('ajax stop called')
        $('#loader').fadeOut();
    });
});