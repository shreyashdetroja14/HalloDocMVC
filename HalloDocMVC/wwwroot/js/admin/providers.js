const selectList = document.querySelector('.selectlist');

document.addEventListener("DOMContentLoaded", async () => {
    await GetProvidersListPartial(0);
    console.log("DOM is loaded and ready!");
});


async function ValidateCookie() {
    try {
        //cookie validate url
        const validationurl = "/Login/ValidateCookie";


        const validateResponse = await fetch(validationurl);

        //console.log(validateResponse);
        console.log('***************************************');
        console.log('Cookie response: ', validateResponse.statusText, validateResponse.status);
        if (!validateResponse.ok) {
            console.log('invalid cookiee');

            if (validateResponse.status === 401) {
                console.log('Unauthorized access, redirecting to login');
                window.location.reload();

            } else {
                throw new Error(`Something else! `);
            }

            return false;
        }

        return true;
    }
    catch (error) {
        console.error('HTTP VALIDATION error: ', error);
        return false;
    }
}

async function GetProvidersListPartial(regionId = 0) {
    try {

        const isCookieValid = await ValidateCookie();

        if (isCookieValid) {

            let url = `/Providers/FetchProviders?regionId=${regionId}`;

            console.log('url: ', url);

            const response = await fetch(url);

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);

            }

            console.log('response is ok');

            const partialViewHtml = await response.text();
            //console.log(partialViewHtml);
            const partialViewContainer = document.getElementById('partial-container');
            //console.log(partialViewContainer);
            partialViewContainer.innerHTML = partialViewHtml;

            addEventListnersForPartial();


        }

    } catch (error) {
        console.error('Error fetching partial view:', error);
    }
}

async function GetContactProviderModal(providerId) {

    try {

        const isCookieValid = await ValidateCookie();
        if (isCookieValid) {

            let url = `/Providers/ContactProvider?providerId=${providerId}`;

            const response = await fetch(url);

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const contactProviderModalHtml = await response.text();
            const modalContainer = document.getElementById('modal-container');
            modalContainer.innerHTML = contactProviderModalHtml;


            const radioBtns = document.querySelectorAll('input[type=radio]');
            radioBtns.forEach(radio => {
                radio.addEventListener('click', () => {
                    const subjectInput = document.querySelector('#SubjectField');
                    if (radio.value == "email") {
                        subjectInput.classList.remove('d-none');
                    }
                    else {
                        subjectInput.classList.add('d-none');
                    }
                })
            })

        }
    }
    catch (error) {
        console.error('Error fetching partial view:', error);
    }
}


selectList.addEventListener('change', async () => {
    const regionValue = selectList.value;

    await GetProvidersListPartial(regionValue);
})


function addEventListnersForPartial() {
    const contactProviderBtn = document.querySelectorAll('.contact-provider-btn');

    const checkboxes = document.querySelectorAll('.StopNotificationIds');
    const checkboxesMobile = document.querySelectorAll('.StopNotificationIdsMobile');
    const saveBtn = document.querySelector('#submit-form');
    const saveBtnMobile = document.querySelector('#submit-form-mobile');

    if (contactProviderBtn != null) {
        contactProviderBtn.forEach(item => {
            item.addEventListener('click', async (event) => {
                const providerId = event.target.dataset.providerId;
                await GetContactProviderModal(providerId);

                const myModal = new bootstrap.Modal('#contact-provider-modal')
                myModal.show();
            })
        })
    }

    const initialcheckboxstate = [];

    checkboxes.forEach(box => {
        initialcheckboxstate.push(box.checked);
    })
    console.log('initialcheckboxstate', initialcheckboxstate.toString());

    const initialcheckboxstatemobile = [];

    checkboxesMobile.forEach(box => {
        initialcheckboxstatemobile.push(box.checked);
    })
    console.log('initialcheckboxstatemobile', initialcheckboxstatemobile.toString());

    if (checkboxes != null) {
        checkboxes.forEach(checkbox => {
            checkbox.addEventListener('change', () => {
                const currentcheckboxesstate = [];
                checkboxes.forEach(box => {
                    currentcheckboxesstate.push(box.checked);
                });
                console.log('currentcheckboxesstate', currentcheckboxesstate.toString());
                if (currentcheckboxesstate.toString() === initialcheckboxstate.toString()) {
                    saveBtn.classList.remove('d-md-inline');
                }
                else {
                    saveBtn.classList.add('d-md-inline');

                }
            });
        });
    }

    if (checkboxesMobile != null) {
        checkboxesMobile.forEach(checkbox => {
            checkbox.addEventListener('change', () => {
                const currentcheckboxesstatemobile = [];
                checkboxesMobile.forEach(box => {
                    currentcheckboxesstatemobile.push(box.checked);
                });
                console.log('currentcheckboxesstatemobile', currentcheckboxesstatemobile.toString());
                if (currentcheckboxesstatemobile.toString() === initialcheckboxstatemobile.toString()) {
                    saveBtnMobile.classList.add('d-none');
                }
                else {
                    saveBtnMobile.classList.remove('d-none');

                }
            });
        });
    }


}
