

/*window.addEventListener("pageshow", function (event) {
    var historyTraversal = event.persisted ||
        (typeof window.performance != "undefined" &&
            window.performance.navigation.type === 2);
    if (historyTraversal) {
        // Handle page restore.
        window.location.reload();
    }
});*/


//status buttons

const newBtn = document.querySelector('.new-btn');
const pendingBtn = document.querySelector('.pending-btn');
const activeBtn = document.querySelector('.active-btn');
const concludeBtn = document.querySelector('.conclude-btn');
/*const toCloseBtn = document.querySelector('.to-close-btn');
const unpaidBtn = document.querySelector('.unpaid-btn');
*/
const triangles = document.querySelectorAll('.triangle');

const requestStatusNameSpan = document.querySelector('#request-status-name');


//url parameters for filters

const urlparams = {
    requestStatus: localStorage.physicianReqStatus,
    requestType: null,
    searchPattern: null,
    searchRegion: null,
    pageNumber: 1
}


//on document ready, call partial view

document.addEventListener("DOMContentLoaded", async () => {
    let requestStatus = 1;
    if (localStorage.physicianReqStatus) {
        urlparams.requestStatus = localStorage.physicianReqStatus;
    }
    else {
        localStorage.physicianReqStatus = requestStatus;
        urlparams.requestStatus = requestStatus;
    }
    console.log('localstorage value: ', urlparams.requestStatus);
    console.log('reqstatus value: ', requestStatus);

    addActiveTabClass(localStorage.physicianReqStatus);
    await GetPartialViewData(urlparams);
    console.log("DOM is loaded and ready!");
});

// validate cookie
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



// requests partial view data ajax call

async function GetPartialViewData(urlparams) {
    try {

        const isCookieValid = await ValidateCookie();
        console.log('is cookie valid? ', isCookieValid);
        console.log('urlparams: ', urlparams);

        if (isCookieValid) {

            let url = `/FetchRequests?requestStatus=${urlparams.requestStatus}`;

            if (urlparams.requestType) {
                url += `&requestType=${urlparams.requestType}`;
            }

            if (urlparams.searchPattern) {
                url += `&searchPattern=${encodeURIComponent(urlparams.searchPattern)}`;
            }

            if (urlparams.searchRegion) {
                url += `&searchRegion=${encodeURIComponent(urlparams.searchRegion)}`;
            }

            url += `&pageNumber=${encodeURIComponent(urlparams.pageNumber)}`;

            console.log('url: ', url);

            const response = await fetch(url);

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);

            }

            //console.log(response);
            console.log('response is ok');

            const partialViewHtml = await response.text();
            //console.log(partialViewHtml);

            const partialViewContainer = document.getElementById('requests');
            partialViewContainer.innerHTML = partialViewHtml;

            addEventListnersForPartial();
        }

    } catch (error) {
        console.error('Error fetching partial view:', error);
    }
}



//send agreement modal data ajax call

async function GetSendAgreementModalData(requestId) {

    try {

        const isCookieValid = await ValidateCookie();
        if (isCookieValid) {

            let url = `/DashBoard/SendAgreement?requestId=${requestId}`;

            const response = await fetch(url);

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const sendAgreementModalHtml = await response.text();
            const modalContainer = document.getElementById('modal-container');
            modalContainer.innerHTML = sendAgreementModalHtml;

            $.validator.unobtrusive.parse($('#send-agreement-form'));


            const myModal = new bootstrap.Modal('#send-agreement-modal');
            myModal.show();

            
        }
    }
    catch (error) {
        console.error('Error fetching partial view:', error);
    }
}

async function GetTransferRequestModalData(requestId) {

    try {

        const isCookieValid = await ValidateCookie();
        if (isCookieValid) {

            let url = `/Physician/TransferToAdmin?requestId=${requestId}`;

            const response = await fetch(url);

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const transferRequestModalHtml = await response.text();
            const modalContainer = document.getElementById('modal-container');
            modalContainer.innerHTML = transferRequestModalHtml;

        }
    }
    catch (error) {
        console.error('Error fetching partial view:', error);
    }
}

async function GetCareTypeModalData(requestId) {

    try {

        const isCookieValid = await ValidateCookie();
        if (isCookieValid) {

            let url = `/Physician/CareType?requestId=${requestId}`;

            const response = await fetch(url);

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const careTypeModalHtml = await response.text();
            const modalContainer = document.getElementById('modal-container');
            modalContainer.innerHTML = careTypeModalHtml;

        }
    }
    catch (error) {
        console.error('Error fetching partial view:', error);
    }
}

async function GetDownloadModalData(requestId) {

    try {

        const isCookieValid = await ValidateCookie();
        if (isCookieValid) {

            let url = `/Physician/DownloadEncounterForm?requestId=${requestId}`;

            const response = await fetch(url);

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const downloadModalHtml = await response.text();
            const modalContainer = document.getElementById('modal-container');
            modalContainer.innerHTML = downloadModalHtml;

        }
    }
    catch (error) {
        console.error('Error fetching partial view:', error);
    }
}


//active button functions

function removeActiveTabClass() {
    const statusBtns = document.querySelectorAll('.status-btn');
    statusBtns.forEach(btn => {
        btn.classList.remove('active-tab');
    });

    triangles.forEach(triangle => {
        triangle.classList.remove('visible');
    })
}

function addActiveTabClass() {
    let button = null;
    let triangle = null;
    if (urlparams.requestStatus == 1) {
        button = newBtn;
        triangle = triangles[0];
        requestStatusNameSpan.textContent = "(New)";
    }
    else if (urlparams.requestStatus == 2) {
        button = pendingBtn;
        triangle = triangles[1];
        requestStatusNameSpan.textContent = "(Pending)";
    }
    else if (urlparams.requestStatus == 3) {
        button = activeBtn;
        triangle = triangles[2];
        requestStatusNameSpan.textContent = "(Active)";
    }
    else if (urlparams.requestStatus == 4) {
        button = concludeBtn;
        triangle = triangles[3];
        requestStatusNameSpan.textContent = "(Conclude)";
    }
    

    button.classList.add('active-tab');
    triangle.classList.add('visible');
}


//request status buttons event listeners

newBtn.addEventListener('click', async () => {
    localStorage.physicianReqStatus = 1;
    urlparams.requestStatus = 1;
    urlparams.pageNumber = 1;

    removeActiveTabClass();
    addActiveTabClass();
    await GetPartialViewData(urlparams);

});


pendingBtn.addEventListener('click', async () => {
    localStorage.physicianReqStatus = 2;
    urlparams.requestStatus = 2;
    urlparams.pageNumber = 1;

    removeActiveTabClass();
    addActiveTabClass();
    await GetPartialViewData(urlparams);
});


activeBtn.addEventListener('click', async () => {
    localStorage.physicianReqStatus = 3;
    urlparams.requestStatus = 3;
    urlparams.pageNumber = 1;

    removeActiveTabClass();
    addActiveTabClass();
    await GetPartialViewData(urlparams);
});


concludeBtn.addEventListener('click', async () => {
    localStorage.physicianReqStatus = 4;
    urlparams.requestStatus = 4;
    urlparams.pageNumber = 1;

    removeActiveTabClass();
    addActiveTabClass();
    await GetPartialViewData(urlparams);
});


// request type buttons

const allBtn = document.querySelector('.all-btn');
const patientBtn = document.querySelector('.patient-btn');
const familyBtn = document.querySelector('.family-btn');
const businessBtn = document.querySelector('.business-btn');
const conciergeBtn = document.querySelector('.concierge-btn');

const typeBtns = document.querySelectorAll('.type-btn');

function removeTypeBtnBorders() {
    typeBtns.forEach(btn => {
        btn.classList.remove('border');
    })
}

allBtn.addEventListener('click', async () => {
    urlparams.requestType = null;
    urlparams.pageNumber = 1;

    removeTypeBtnBorders()
    allBtn.classList.add('border');
    await GetPartialViewData(urlparams);
});

patientBtn.addEventListener('click', async () => {
    urlparams.requestType = 2;
    urlparams.pageNumber = 1;

    removeTypeBtnBorders()
    patientBtn.classList.add('border');
    await GetPartialViewData(urlparams);
});

familyBtn.addEventListener('click', async () => {
    urlparams.requestType = 3;
    urlparams.pageNumber = 1;

    removeTypeBtnBorders()
    familyBtn.classList.add('border');
    await GetPartialViewData(urlparams);
});

businessBtn.addEventListener('click', async () => {
    urlparams.requestType = 1;
    urlparams.pageNumber = 1;

    removeTypeBtnBorders()
    businessBtn.classList.add('border');
    await GetPartialViewData(urlparams);
});

conciergeBtn.addEventListener('click', async () => {
    urlparams.requestType = 4;
    urlparams.pageNumber = 1;

    removeTypeBtnBorders()
    conciergeBtn.classList.add('border');
    await GetPartialViewData(urlparams);
});


//searchbar

const searchbar = document.querySelector('.searchbar');
searchbar.addEventListener('keyup', async () => {
    const searchPattern = searchbar.value;

    if (searchPattern !== "") {
        urlparams.searchPattern = searchPattern;
    } else {
        urlparams.searchPattern = null;

    }
    urlparams.pageNumber = 1;

    await GetPartialViewData(urlparams);
})


//selectlist

const selectList = document.querySelector('.selectlist');

selectList.addEventListener('change', async () => {
    const regionValue = selectList.value;

    if (regionValue == 0) {
        urlparams.searchRegion = null;
    } else {
        urlparams.searchRegion = regionValue;
    }
    urlparams.pageNumber = 1;

    await GetPartialViewData(urlparams);
})


// event listeners for dropdown action modals

function addEventListnersForPartial() {

    const sendAgreementBtn = document.querySelectorAll('.send-agreement-btn');
    const transferRequestBtn = document.querySelectorAll('.transfer-case-btn');
    const careTypeBtn = document.querySelectorAll('.care-type-btn');
    const downloadBtn = document.querySelectorAll('.download-btn');
    const pageNumberLinks = document.querySelectorAll('.page-number');
    //const nextPageLink = document.querySelector('page-next');


    pageNumberLinks.forEach(link => {
        link.addEventListener('click', async (event) => {
            const pageNumber = event.target.dataset.pageNumber;
            console.log('page number: ', pageNumber);
            urlparams.pageNumber = pageNumber;

            await GetPartialViewData(urlparams);
        });
    });


    if (sendAgreementBtn !== null) {
        sendAgreementBtn.forEach(item => {
            item.addEventListener('click', async (event) => {
                const requestId = event.target.dataset.requestId;
                console.log(requestId);

                await GetSendAgreementModalData(requestId);

                
            });
        });
    }

    if (transferRequestBtn !== null) {
        transferRequestBtn.forEach(item => {
            item.addEventListener('click', async (event) => {
                const requestId = event.target.dataset.requestId;
                console.log(requestId);

                await GetTransferRequestModalData(requestId);

                const myModal = new bootstrap.Modal('#transfer-request-modal');
                myModal.show();
            });
        });
    }

    if (careTypeBtn !== null) {
        careTypeBtn.forEach(item => {
            item.addEventListener('click', async (event) => {
                const requestId = event.target.dataset.requestId;
                console.log(requestId);

                await GetCareTypeModalData(requestId);

                const myModal = new bootstrap.Modal('#care-type-modal');
                myModal.show();
            });
        });
    }

    if (downloadBtn !== null) {
        downloadBtn.forEach(item => {
            item.addEventListener('click', async (event) => {
                const requestId = event.target.dataset.requestId;
                console.log(requestId);

                await GetDownloadModalData(requestId);

                const myModal = new bootstrap.Modal('#download-form-modal');
                myModal.show();
            });
        });
    }
}


//SEND LINK LOADER ON SUBMIT

$('#send-link-btn').click(function () {

    if ($('#sendlinkform').valid()) {
        $('#loader').fadeIn();
    }

    $('#sendlinkform').submit();
    console.log('form submitted');
});


