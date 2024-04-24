function GetAddPatientForm() {
    $.ajax({
        url: '/Home/AddPatient',
        type: 'GET',
        success: function (data) {
            $('#modal-container').html(data);

            patientFormEventListeners();


            $('#patient-form-modal').modal('show');


        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error("Error fetching data:", textStatus);
            if (errorThrown) {
                console.error("Specific error:", errorThrown);
            }
            console.error("HTTP status code:", jqXHR.status);
        }
    });
}

function GetEditPatientForm(patientId) {
    $.ajax({
        url: '/Home/EditPatient',
        type: 'GET',
        data: {
            patientId: patientId
        },
        success: function (data) {
            $('#modal-container').html(data);

            editPatientFormEventListeners();

            $('#patient-form-modal').modal('show');


        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error("Error fetching data:", textStatus);
            if (errorThrown) {
                console.error("Specific error:", errorThrown);
            }
            console.error("HTTP status code:", jqXHR.status);
        }
    });
}

function GetPatientsList(pageNumber) {
    $.ajax({
        url: '/Home/GetPatientList',
        type: 'GET',
        data: {
            searchPattern: $('#searchbar').val(),
            pageNumber: pageNumber
        },
        success: function (data) {
            $('#patient-list-container').html(data);

            patientListEventListeners();

        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error("Error fetching data:", textStatus);
            if (errorThrown) {
                console.error("Specific error:", errorThrown);
            }
            console.error("HTTP status code:", jqXHR.status);
        }
    });
}

function CreatePatient() {
    var formData = $('#patient-form').serialize();

    $.ajax({
        url: '/Home/AddPatient',
        data: formData,
        type: 'POST',
        success: function (data) {
            console.log(data);
            $('#patient-form-modal').modal('hide');
            toastr.success("Patient Created Successfully");

            GetPatientsList(1);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error("Error fetching data:", textStatus);
            if (errorThrown) {
                console.error("Specific error:", errorThrown);
            }
            console.error("HTTP status code:", jqXHR.status);

        }
    });
}

function EditPatient() {
    var formData = $('#patient-form').serialize();

    $.ajax({
        url: '/Home/EditPatient',
        data: formData,
        type: 'POST',
        success: function (data) {
            console.log(data);
            $('#patient-form-modal').modal('hide');
            toastr.success("Patient Edited Successfully");

            GetPatientsList(1);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error("Error fetching data:", textStatus);
            if (errorThrown) {
                console.error("Specific error:", errorThrown);
            }
            console.error("HTTP status code:", jqXHR.status);

        }
    });
}

function DeletePatient(patientId) {
    $.ajax({
        url: '/Home/DeletePatient',
        type: 'GET',
        data: {
            patientId: patientId
        },
        success: function (data) {
            console.log(data);
            
            toastr.success("Patient Deleted Successfully");

            $('#searchbar').val();
            GetPatientsList(1);


        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error("Error fetching data:", textStatus);
            if (errorThrown) {
                console.error("Specific error:", errorThrown);
            }
            console.error("HTTP status code:", jqXHR.status);
        }
    });
}

/*function validateForm() {
    var firstName = $('#FirstName').val();
    var lastName = $('#LastName').val();
    var email = $('#Email').val();
    var age = $('#Age').val();
    var phoneNumber = $('#PhoneNumber').val();
    var disease = $('#disease-select').val();
    var specialist = $('#doctor-select').val();

    if (firstName == "") {
        return false;
    }

    if (lastName == "") {
        return false;
    }

    if (email == "") {
        return false;
    }

    if (lastName == "") {
        return false;
    }

}*/

const validateEmail = (email) => {
    return email.match(
        /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
    );
};

function patientFormEventListeners() {
    $('#submit-form-btn').click(function () {

        var form = document.querySelector('.needs-validation')
        if (!form.checkValidity()) {
            form.classList.add('was-validated')


        } else {
            var email = $('#Email').val();
            if (validateEmail(email)) {
                CreatePatient();
            } else {
                $('#emailspan').html("enter valid email");
            }
        }


    });
}

function editPatientFormEventListeners() {
    $('#edit-form-btn').click(function () {

        var form = document.querySelector('.needs-validation')
        if (!form.checkValidity()) {
            form.classList.add('was-validated')

        } else {
            var email = $('#Email').val();
            if (validateEmail(email)) {
                EditPatient();
            } else {
                $('#emailspan').html("enter valid email");
            }
        }

        
    });
}

function patientListEventListeners() {
    $('.pagination-link').each(function () {
        $(this).click(function () {
            const pageNumber = $(this).attr('data-page-number');
            GetPatientsList(pageNumber);
        });
    })

    $('.Edit-btn').each(function () {
        $(this).click(function () {
            const patientId = $(this).attr('data-patient-id');
            GetEditPatientForm(patientId);
        });
    });

    $('.Delete-btn').each(function () {
        $(this).click(function () {
            const patientId = $(this).attr('data-patient-id');
            DeletePatient(patientId);
        });
    });
}

$('document').ready(function () {
    GetPatientsList(1);
})

$('#add-patient-btn').click(function () {
    GetAddPatientForm();
});

$('#searchbar').keyup(function () {
    GetPatientsList(1);
});