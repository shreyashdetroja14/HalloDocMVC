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
}