﻿

/*window.addEventListener("pageshow", function (event) {
//    var historyTraversal = event.persisted ||
//        (typeof window.performance != "undefined" &&
//            window.performance.navigation.type === 2);
//    if (historyTraversal) {
//        // Handle page restore.
//        window.location.reload();
//    }
//});*/


//status buttons

const newBtn = document.querySelector('.new-btn');
const pendingBtn = document.querySelector('.pending-btn');
const activeBtn = document.querySelector('.active-btn');
const concludeBtn = document.querySelector('.conclude-btn');
const toCloseBtn = document.querySelector('.to-close-btn');
const unpaidBtn = document.querySelector('.unpaid-btn');

const triangles = document.querySelectorAll('.triangle');

const requestStatusNameSpan = document.querySelector('#request-status-name');


//url parameters for filters

const urlparams = {
    requestStatus: localStorage.status,
    requestType: null,
    searchPattern: null,
    searchRegion: null,
}


//on document ready, call partial view

document.addEventListener("DOMContentLoaded", async () => {
    //let requestStatus = 1;
    if (localStorage.status) {
        urlparams.requestStatus = localStorage.status;
    }
    else {
        localStorage.status = requestStatus;
    }
    console.log('localstorage value: ', urlparams.requestStatus);

    addActiveTabClass(localStorage.status);
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

            let url = `/AdminDashBoard/FetchRequests?requestStatus=${urlparams.requestStatus}`;

            if (urlparams.requestType) {
                url += `&requestType=${urlparams.requestType}`;
            }

            if (urlparams.searchPattern) {
                url += `&searchPattern=${encodeURIComponent(urlparams.searchPattern)}`;
            }

            if (urlparams.searchRegion) {
                url += `&searchRegion=${encodeURIComponent(urlparams.searchRegion)}`;
            }

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


// cancel case modal data ajax call

async function GetCancelCaseModalData(requestId) {

    try {

        const isCookieValid = await ValidateCookie();
        if (isCookieValid) {

            let url = `/AdminDashBoard/CancelCase?requestId=${requestId}`;
            const response = await fetch(url);

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const cancelCaseModalHtml = await response.text();
            const modalContainer = document.getElementById('modal-container');
            modalContainer.innerHTML = cancelCaseModalHtml;
        }
    }
    catch (error) {
        console.error('Error fetching partial view:', error);
    }
}


//assign case modal data ajax call

async function GetAssignCaseModalData(requestId, isTransferRequest = null, regionId = 0) {

    try {

        const isCookieValid = await ValidateCookie();
        if (isCookieValid) {

            let url = `/AdminDashBoard/AssignCase?requestId=${requestId}&isTransferRequest=${isTransferRequest}&regionId=${regionId}`;
            const response = await fetch(url);

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const assignCaseModalHtml = await response.text();
            const modalContainer = document.getElementById('modal-container');
            modalContainer.innerHTML = assignCaseModalHtml;

            const myModal = new bootstrap.Modal('#assign-case-modal')
            myModal.show();

            const regionSelectList = document.querySelector('.region-select');
            regionSelectList.addEventListener('change', async () => {
                const regionId = regionSelectList.value;
                console.log('region value:: ', regionId);
                console.log('request id:: ', requestId);
                console.log('transfer req:: ', isTransferRequest);

                myModal.hide();
                await GetAssignCaseModalData(requestId, isTransferRequest, regionId);
            });
        }


    }
    catch (error) {
        console.error('Error fetching partial view:', error);
    }
}


// block request modal data ajax call

async function GetBlockRequestModalData(requestId) {

    try {

        const isCookieValid = await ValidateCookie();
        if (isCookieValid) {

            let url = `/AdminDashBoard/BlockRequest?requestId=${requestId}`;
            const response = await fetch(url);

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const blockRequestModalHtml = await response.text();
            const modalContainer = document.getElementById('modal-container');
            modalContainer.innerHTML = blockRequestModalHtml;
        }

    }
    catch (error) {
        console.error('Error fetching partial view:', error);
    }
}


// block case modal data ajax call

async function GetClearCaseModalData(requestId) {

    try {

        const isCookieValid = await ValidateCookie();
        if (isCookieValid) {
            let url = `/AdminDashBoard/ClearCase?requestId=${requestId}`;

            const response = await fetch(url);

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const clearCaseModalHtml = await response.text();
            const modalContainer = document.getElementById('modal-container');
            modalContainer.innerHTML = clearCaseModalHtml;
        }
    }
    catch (error) {
        console.error('Error fetching partial view:', error);
    }
}


//send agreement modal data ajax call

async function GetSendAgreementModalData(requestId) {

    try {

        const isCookieValid = await ValidateCookie();
        if (isCookieValid) {

            let url = `/AdminDashBoard/SendAgreement?requestId=${requestId}`;

            const response = await fetch(url);

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const sendAgreementModalHtml = await response.text();
            const modalContainer = document.getElementById('modal-container');
            modalContainer.innerHTML = sendAgreementModalHtml;

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
    else if (urlparams.requestStatus == 5) {
        button = toCloseBtn;
        triangle = triangles[4];
        requestStatusNameSpan.textContent = "(To Close)";
    }
    else if (urlparams.requestStatus == 6) {
        button = unpaidBtn;
        triangle = triangles[5];
        requestStatusNameSpan.textContent = "(Unpaid)";
    }

    button.classList.add('active-tab');
    triangle.classList.add('visible');
}


//request status buttons event listeners

newBtn.addEventListener('click', async () => {
    localStorage.status = 1;
    urlparams.requestStatus = 1;

    removeActiveTabClass();
    addActiveTabClass();
    await GetPartialViewData(urlparams);

});


pendingBtn.addEventListener('click', async () => {
    localStorage.status = 2;
    urlparams.requestStatus = 2;

    removeActiveTabClass();
    addActiveTabClass();
    await GetPartialViewData(urlparams);
});


activeBtn.addEventListener('click', async () => {
    localStorage.status = 3;
    urlparams.requestStatus = 3;

    removeActiveTabClass();
    addActiveTabClass();
    await GetPartialViewData(urlparams);
});


concludeBtn.addEventListener('click', async () => {
    localStorage.status = 4;
    urlparams.requestStatus = 4;

    removeActiveTabClass();
    addActiveTabClass();
    await GetPartialViewData(urlparams);
});


toCloseBtn.addEventListener('click', async () => {
    localStorage.status = 5;
    urlparams.requestStatus = 5;

    removeActiveTabClass();
    addActiveTabClass();
    await GetPartialViewData(urlparams);
});


unpaidBtn.addEventListener('click', async () => {
    localStorage.status = 6;
    urlparams.requestStatus = 6;

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

    removeTypeBtnBorders()
    allBtn.classList.add('border');
    await GetPartialViewData(urlparams);
});

patientBtn.addEventListener('click', async () => {
    urlparams.requestType = 2;

    removeTypeBtnBorders()
    patientBtn.classList.add('border');
    await GetPartialViewData(urlparams);
});

familyBtn.addEventListener('click', async () => {
    urlparams.requestType = 3;

    removeTypeBtnBorders()
    familyBtn.classList.add('border');
    await GetPartialViewData(urlparams);
});

businessBtn.addEventListener('click', async () => {
    urlparams.requestType = 1;

    removeTypeBtnBorders()
    businessBtn.classList.add('border');
    await GetPartialViewData(urlparams);
});

conciergeBtn.addEventListener('click', async () => {
    urlparams.requestType = 4;

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

    await GetPartialViewData(urlparams);
})


// event listeners for dropdown action modals

function addEventListnersForPartial() {

    const cancelCaseBtn = document.querySelectorAll('.cancel-case-btn');
    const assignCaseBtn = document.querySelectorAll('.assign-case-btn');
    const blockRequestBtn = document.querySelectorAll('.block-request-btn');
    const clearCaseBtn = document.querySelectorAll('.clear-case-btn');
    const sendAgreementBtn = document.querySelectorAll('.send-agreement-btn');

    if (cancelCaseBtn !== null) {
        cancelCaseBtn.forEach(item => {
            item.addEventListener('click', async (event) => {
                const requestId = event.target.dataset.requestId;
                console.log(requestId);

                await GetCancelCaseModalData(requestId);

                const myModal = new bootstrap.Modal('#cancel-case-modal')
                myModal.show();
            });
        });
    }

    if (assignCaseBtn !== null) {
        assignCaseBtn.forEach(item => {
            item.addEventListener('click', async (event) => {
                const requestId = event.target.dataset.requestId;
                const isTransferRequest = event.target.dataset.isTransferRequest;
                console.log(requestId);
                console.log(isTransferRequest);

                await GetAssignCaseModalData(requestId, isTransferRequest);
            })
        })
    }

    if (blockRequestBtn !== null) {
        blockRequestBtn.forEach(item => {
            item.addEventListener('click', async (event) => {
                const requestId = event.target.dataset.requestId;
                console.log(requestId);

                await GetBlockRequestModalData(requestId);

                const myModal = new bootstrap.Modal('#block-request-modal');
                myModal.show();
            });
        });
    }

    if (clearCaseBtn !== null) {
        clearCaseBtn.forEach(item => {
            item.addEventListener('click', async (event) => {
                const requestId = event.target.dataset.requestId;
                console.log(requestId);

                await GetClearCaseModalData(requestId);

                const myModal = new bootstrap.Modal('#clear-case-modal');
                myModal.show();
            });
        });
    }

    if (sendAgreementBtn !== null) {
        sendAgreementBtn.forEach(item => {
            item.addEventListener('click', async (event) => {
                const requestId = event.target.dataset.requestId;
                console.log(requestId);

                await GetSendAgreementModalData(requestId);

                const myModal = new bootstrap.Modal('#send-agreement-modal');
                myModal.show();
            });
        });
    }
}

