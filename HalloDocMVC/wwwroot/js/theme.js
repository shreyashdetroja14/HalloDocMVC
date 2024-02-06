const themeBtn = document.querySelector(".theme-btn");

const isDarkThemeEnable = false;

const toggleCSSclasses = (el, ...cls) => cls.map(cl => el.classList.toggle(cl));

const formControls = document.querySelectorAll('.form-control');
const labels = document.querySelectorAll('label');
const labelIcons = document.querySelectorAll('.label-icon');
const footerLinks = document.querySelectorAll('.footer a');
const navbar = document.querySelector(".navbar");
const main = document.querySelector(".main");
const requestForm = document.querySelector(".request-form");
const uploadLabel = document.querySelector('.upload-label');
const itiList = document.querySelectorAll('.iti__country-list');
/*console.log(navbar);*/


themeBtn.addEventListener('click', () => {
    console.log("event listner entered")

    toggleCSSclasses(document.body, 'dark-bg', 'light-text');


    formControls.forEach(e => {
        toggleCSSclasses(e, 'light-text', 'light-border', 'gray-bg');
    });


    labels.forEach(e => {
        toggleCSSclasses(e, 'light-text');
    });


    labelIcons.forEach(e => {
        toggleCSSclasses(e, 'label-icon-dark');
    });


    footerLinks.forEach(e => {
        toggleCSSclasses(e, 'light-text');
    });

    if (navbar) {
        navbar.classList.toggle('bg-dark');
    }

    if (main) {
        main.classList.toggle('gray-bg');
    }

    if (requestForm) {
        requestForm.classList.toggle('dark-bg');
    }

    if (uploadLabel) {
        uploadLabel.classList.toggle('gray-bg');
    }

    console.log(itiList);
    itiList.forEach(e => {
        toggleCSSclasses(e, 'gray-bg');
    })
    console.log("event listner exited");


})



