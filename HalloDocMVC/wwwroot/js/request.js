/* TRIM INPUT VALUES */

//FIRSTNAME
$('#FirstName').change(function () {
    let firstname = $('#FirstName').val();
    let trimmedFirstName = firstname.trim();
    $('#FirstName').val(trimmedFirstName);
});


/* PHONE INPUT INTL SCRIPT */

const phoneInput1 = document.querySelector("#PhoneNumber");
const intlInput1 = window.intlTelInput(phoneInput1, {
    initialCountry: "in",
    preferredCountries: ["in"],
    onlyCountries: ["in", "us", "au", "de"],
    utilsScript:
        "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.8/js/utils.js",
});

const phoneInput2 = document.querySelector("#PhoneNumber2");
if (phoneInput2) {
    const intlInput2 = window.intlTelInput(phoneInput2, {
        initialCountry: "in",
        preferredCountries: ["in"],
        onlyCountries: ["in", "us", "au", "de"],
        utilsScript:
            "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.8/js/utils.js",
    });
}