
function ValidateText(controlId, description, btnCancel) {
    var value = $.trim($('#' + controlId).val());
    $('#' + controlId).val(value);
    if (value.length < 1) {
        $('#' + controlId).focus();
        var msg = 'Please specify ' + description + '.';
        AddedValidationRequireMsg(controlId, msg);


        bootbox.alert(msg, function () {
            if ((btnCancel != undefined)) {
                $("#" + btnCancel).click();
            }
          //  bootbox.hideAll();
        });

   /*     $('.modal[role="dialog"]').modal('hide')*/
        return false;
    }
    return true;
};

function ValidateTextRange(controlId, description, lenghtText, btnCancel) {
    var value = $.trim($('#' + controlId).val());
    $('#' + controlId).val(value);
    if (value.length > lenghtText) {
        $('#' + controlId).focus();
        var msg = 'The ' + description + ' entered exceeeds the maximum length.';
        AddedValidationRequireMsg(controlId, msg);

        //Swaltype : 1 alert , 2 confirm
        Swal.fire({
            title: '',
            text: 'Error : ' + msg,
            icon: 'error',
            //icontype == 1 ? 'info' : icontype == 2 ? 'success' : icontype == 3 ? 'error' : icontype == 4 ? 'warning' : icontype == 5 ? 'question' : 'info',
            reverseButtons: true,
            showCancelButton: false,
            allowOutsideClick: false,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Cancel'
        }).then(function (result) {
            if ((btnCancel != undefined)) {
                $("#" + btnCancel).click();
            }
        });

        //bootbox.alert(msg, function () {
        //    if ((btnCancel != undefined)) {
        //        $("#" + btnCancel).click();
        //    }
        //    bootbox.hideAll();
        //});

        $('.modal[role="dialog"]').modal('hide')
        return false;
    }
    return true;
};

function ValidateNumeric(controlId, description, lenghtText, btnCancel) {
    var value = $.trim($('#' + controlId).val());
    $('#' + controlId).val(value);
    if (isNaN(value) || value === "") {
        $('#' + controlId).focus();
        var msg = 'Please specify numeric ' + description + '.';
        AddedValidationRequireMsg(controlId, msg);

        //Swaltype : 1 alert , 2 confirm
        Swal.fire({
            title: '',
            text: 'Error : ' + msg,
            icon: 'error',
            //icontype == 1 ? 'info' : icontype == 2 ? 'success' : icontype == 3 ? 'error' : icontype == 4 ? 'warning' : icontype == 5 ? 'question' : 'info',
            reverseButtons: true,
            showCancelButton: false,
            allowOutsideClick: false,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Cancel'
        }).then(function (result) {
        });

        //bootbox.alert(msg, function () { bootbox.hideAll() });

        return false;
    }
    return true;
}

function ValidateDate(controlId, description, btnCancel) {
    var value = $.trim($('#' + controlId).val());
    $('#' + controlId).val(value);

    if (value.length < 1) {
        $('#' + controlId).focus();
        var msg = 'Please specify ' + description + '.';
        AddedValidationRequireMsg(controlId, msg);

        //Swaltype : 1 alert , 2 confirm
        Swal.fire({
            title: '',
            text: 'Error : ' + msg,
            icon: 'error',
            //icontype == 1 ? 'info' : icontype == 2 ? 'success' : icontype == 3 ? 'error' : icontype == 4 ? 'warning' : icontype == 5 ? 'question' : 'info',
            reverseButtons: true,
            showCancelButton: false,
            allowOutsideClick: false,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Cancel'
        }).then(function (result) {
        });

        //bootbox.alert(msg, function () { bootbox.hideAll() });

        $('.modal[role="dialog"]').modal('hide')
        return false;
    }

    var date = Date.parse(value);

    if (date == null || isNaN(date)) {
        $('#' + controlId).focus();
        var msg = 'Invalid ' + description + ".";
        AddedValidationRequireMsg(controlId, msg);


        //Swaltype : 1 alert , 2 confirm
        Swal.fire({
            title: '',
            text: 'Error : ' + msg,
            icon: 'error',
            //icontype == 1 ? 'info' : icontype == 2 ? 'success' : icontype == 3 ? 'error' : icontype == 4 ? 'warning' : icontype == 5 ? 'question' : 'info',
            reverseButtons: true,
            showCancelButton: false,
            allowOutsideClick: false,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Cancel'
        }).then(function (result) {
        });

        //bootbox.alert(msg, function () { bootbox.hideAll() });

        $('.modal[role="dialog"]').modal('hide')
        return false;
    }

    return true;
}

function ValidateDateRange(controlIdStart, controlIdEnd, startDescription, endDescription, btnCancel) {

    var startDateValue = $.trim($('#' + controlIdStart).val());
    $('#' + controlIdStart).val(startDateValue);
    var endDateValue = $.trim($('#' + controlIdEnd).val());
    $('#' + controlIdEnd).val(endDateValue);

    var startDate = Date.parse(startDateValue);
    var endDate = Date.parse(endDateValue);

    if (startDate == null || isNaN(startDate)) {
        $('#' + controlIdStart).focus();
        var msg = 'Invalid ' + startDescription + '.';
        AddedValidationRequireMsg(controlId, msg);


        //Swaltype : 1 alert , 2 confirm
        Swal.fire({
            title: '',
            text: 'Error : ' + msg,
            icon: 'error',
            //icontype == 1 ? 'info' : icontype == 2 ? 'success' : icontype == 3 ? 'error' : icontype == 4 ? 'warning' : icontype == 5 ? 'question' : 'info',
            reverseButtons: true,
            showCancelButton: false,
            allowOutsideClick: false,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Cancel'
        }).then(function (result) {
        });

        //bootbox.alert(msg, function () { bootbox.hideAll() });

        $('.modal[role="dialog"]').modal('hide')
        return false;
    }

    if (endDate == null || isNaN(endDate)) {
        $('#' + controlIdEnd).focus();
        var msg = 'Invalid ' + endDescription + '.';
        AddedValidationRequireMsg(controlId, msg);


        //Swaltype : 1 alert , 2 confirm
        Swal.fire({
            title: '',
            text: 'Error : ' + msg,
            icon: 'error',
            //icontype == 1 ? 'info' : icontype == 2 ? 'success' : icontype == 3 ? 'error' : icontype == 4 ? 'warning' : icontype == 5 ? 'question' : 'info',
            reverseButtons: true,
            showCancelButton: false,
            allowOutsideClick: false,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Cancel'
        }).then(function (result) {
        });

        //bootbox.alert(msg, function () { bootbox.hideAll() });

        $('.modal[role="dialog"]').modal('hide')
        return false;
    }

    if (startDate > endDate) {
        $('#' + controlIdEnd).focus();
        var msg = startDescription + ' cannot be later than ' + endDescription + '.';
        AddedValidationRequireMsg(controlId, msg);


        //Swaltype : 1 alert , 2 confirm
        Swal.fire({
            title: '',
            text: 'Error : ' + msg,
            icon: 'error',
            //icontype == 1 ? 'info' : icontype == 2 ? 'success' : icontype == 3 ? 'error' : icontype == 4 ? 'warning' : icontype == 5 ? 'question' : 'info',
            reverseButtons: true,
            showCancelButton: false,
            allowOutsideClick: false,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Cancel'
        }).then(function (result) {
        });

        //bootbox.alert(msg, function () { bootbox.hideAll() });

        $('.modal[role="dialog"]').modal('hide')
        return false;
    }

    return true;
}

function isNumber(evt, btnCancel) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}

function ValidateInteger(controlId, description, minValue, maxValue, btnCancel) {
    var value = $.trim($('#' + controlId).val());
    var numValue;

    $('#' + controlId).val(value);
    if (value.length < 1) {
        $('#' + controlId).focus();
        var msg = 'Please specify ' + description + '.';
        AddedValidationRequireMsg(controlId, msg);

        //Swaltype : 1 alert , 2 confirm
        Swal.fire({
            title: '',
            text: 'Error : ' + msg,
            icon: 'error',
            //icontype == 1 ? 'info' : icontype == 2 ? 'success' : icontype == 3 ? 'error' : icontype == 4 ? 'warning' : icontype == 5 ? 'question' : 'info',
            reverseButtons: true,
            showCancelButton: false,
            allowOutsideClick: false,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Cancel'
        }).then(function (result) {
        });

        //bootbox.alert(msg, function () { bootbox.hideAll() });

        $('.modal[role="dialog"]').modal('hide')
        return false;
    }

    if (!(!isNaN(parseInt(value, 10)) && (parseFloat(value, 10) == parseInt(value, 10)))) {
        $('#' + controlId).focus();
        var msg = 'Please specify a valid integer value for ' + description + '.';
        AddedValidationRequireMsg(controlId);

        //Swaltype : 1 alert , 2 confirm
        Swal.fire({
            title: '',
            text: 'Error : ' + msg,
            icon: 'error',
            //icontype == 1 ? 'info' : icontype == 2 ? 'success' : icontype == 3 ? 'error' : icontype == 4 ? 'warning' : icontype == 5 ? 'question' : 'info',
            reverseButtons: true,
            showCancelButton: false,
            allowOutsideClick: false,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Cancel'
        }).then(function (result) {
        });

        //bootbox.alert(msg, function () { bootbox.hideAll() });

        $('.modal[role="dialog"]').modal('hide')
        return false;
    }

    numValue = parseInt(value, 10)

    if (minValue != null && numValue <= minValue) {
        $('#' + controlId).focus();
        var msg = 'Please specify a valid integer value for ' + description + ' that is greater than ' + minValue + '.';
        AddedValidationRequireMsg(controlId, msg);

        //Swaltype : 1 alert , 2 confirm
        Swal.fire({
            title: '',
            text: 'Error : ' + msg,
            icon: 'error',
            //icontype == 1 ? 'info' : icontype == 2 ? 'success' : icontype == 3 ? 'error' : icontype == 4 ? 'warning' : icontype == 5 ? 'question' : 'info',
            reverseButtons: true,
            showCancelButton: false,
            allowOutsideClick: false,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Cancel'
        }).then(function (result) {
        });

        //bootbox.alert(msg, function () { bootbox.hideAll() });

        $('.modal[role="dialog"]').modal('hide')
        return false;
    }

    if (maxValue != null && numValue >= minValue) {
        $('#' + controlId).focus();
        var msg = 'Please specify a valid integer value for ' + description + ' that is less than ' + maxValue + '.';
        AddedValidationRequireMsg(controlId, msg);

        //Swaltype : 1 alert , 2 confirm
        Swal.fire({
            title: '',
            text: 'Error : ' + msg,
            icon: 'error',
            //icontype == 1 ? 'info' : icontype == 2 ? 'success' : icontype == 3 ? 'error' : icontype == 4 ? 'warning' : icontype == 5 ? 'question' : 'info',
            reverseButtons: true,
            showCancelButton: false,
            allowOutsideClick: false,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Cancel'
        }).then(function (result) {
        });

        //bootbox.alert(msg, function () { bootbox.hideAll() });

        $('.modal[role="dialog"]').modal('hide')
        return false;
    }

    $('#' + controlId).val(numValue);
    return true;
}

function ValidateNumber(controlId, description, minValue, maxValue, btnCancel) {
    var value = $.trim($('#' + controlId).val().replace(/,/g, ""));
    var numValue;

    $('#' + controlId).val(value);
    if (value.length < 1) {
        $('#' + controlId).focus();
        value = 0;
        var msg = 'Please specify ' + description + '.';
        AddedValidationRequireMsg(controlId, msg);

        //Swaltype : 1 alert , 2 confirm
        Swal.fire({
            title: '',
            text: 'Error : ' + msg,
            icon: 'error',
            //icontype == 1 ? 'info' : icontype == 2 ? 'success' : icontype == 3 ? 'error' : icontype == 4 ? 'warning' : icontype == 5 ? 'question' : 'info',
            reverseButtons: true,
            showCancelButton: false,
            allowOutsideClick: false,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Cancel'
        }).then(function (result) {
        });

        //bootbox.alert(msg, function () { bootbox.hideAll() });

        $('.modal[role="dialog"]').modal('hide')
        return false;
    }
    if (isNaN(value)) {
        $('#' + controlId).focus();
        value = 0;
        var msg = 'Please specify a valid numeric value for ' + description + '.';
        AddedValidationRequireMsg(controlId, msg);

        //Swaltype : 1 alert , 2 confirm
        Swal.fire({
            title: '',
            text: 'Error : ' + msg,
            icon: 'error',
            //icontype == 1 ? 'info' : icontype == 2 ? 'success' : icontype == 3 ? 'error' : icontype == 4 ? 'warning' : icontype == 5 ? 'question' : 'info',
            reverseButtons: true,
            showCancelButton: false,
            allowOutsideClick: false,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Cancel'
        }).then(function (result) {
            //
        });

        //bootbox.alert(msg, function () { bootbox.hideAll() });

        $('.modal[role="dialog"]').modal('hide')
        return false;
    }

    numValue = parseFloat(value);

    if (minValue != null && numValue <= minValue) {
        $('#' + controlId).focus();
        var msg = 'Please specify a valid numeric value for ' + description + ' that is greater than ' + minValue + '.';
        AddedValidationRequireMsg(controlId, msg);

        //Swaltype : 1 alert , 2 confirm
        Swal.fire({
            title: '',
            text: 'Error : ' + msg,
            icon: 'error',
            //icontype == 1 ? 'info' : icontype == 2 ? 'success' : icontype == 3 ? 'error' : icontype == 4 ? 'warning' : icontype == 5 ? 'question' : 'info',
            reverseButtons: true,
            showCancelButton: false,
            allowOutsideClick: false,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Cancel'
        }).then(function (result) {
            //
        });

        //bootbox.alert(msg, function () { bootbox.hideAll() });

        $('.modal[role="dialog"]').modal('hide')
        return false;
    }

    if (maxValue != null && numValue >= minValue) {
        $('#' + controlId).focus();
        var msg = 'Please specify a valid numeric value for ' + description + ' that is less than ' + maxValue + '.';
        AddedValidationRequireMsg(controlId, msg);

        //Swaltype : 1 alert , 2 confirm
        Swal.fire({
            title: '',
            text: 'Error : ' + msg,
            icon: 'error',
            //icontype == 1 ? 'info' : icontype == 2 ? 'success' : icontype == 3 ? 'error' : icontype == 4 ? 'warning' : icontype == 5 ? 'question' : 'info',
            reverseButtons: true,
            showCancelButton: false,
            allowOutsideClick: false,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Cancel'
        }).then(function (result) {
            //
        });

        //bootbox.alert(msg, function () { bootbox.hideAll() });

        $('.modal[role="dialog"]').modal('hide')
        return false;
    }

    $('#' + controlId).val(numValue);

    return true;
}

function ValidateDropdownRequired(controlId, description, btnCancel) {
    var value = $.trim($('#' + controlId)[0].value);
    //$('#' + controlId).val(value);
    if (value == "" || value == 0) {
        $('#' + controlId).focus();
        var msg = 'Please specify ' + description + '.';
        AddedValidationRequireMsg(controlId, msg);

        //Swaltype : 1 alert , 2 confirm
        Swal.fire({
            title: '',
            text: 'Error : ' + msg,
            icon: 'error',
            //icontype == 1 ? 'info' : icontype == 2 ? 'success' : icontype == 3 ? 'error' : icontype == 4 ? 'warning' : icontype == 5 ? 'question' : 'info',
            reverseButtons: true,
            showCancelButton: false,
            allowOutsideClick: false,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Cancel'
        }).then(function (result) {
            if ((btnCancel != undefined)) {
                $("#" + btnCancel).click();
            }
        });


        //bootbox.alert(msg, function () {
        //    if ((btnCancel != undefined)) {
        //        $("#" + btnCancel).click();
        //    }
        //    bootbox.hideAll();
        //});

        $('.modal[role="dialog"]').modal('hide')
        return false;
    }
    return true;
}


function AddedValidationRequireMsg(controlId, msg, btnCancel) {
    $("span[data-valmsg-for$='" + controlId + "']").text(msg);
}


function ValidateNumCopies(controlId, description, btnCancel, maxCopy) {
    var value = $.trim($('#' + controlId).val());
    $('#' + controlId).val(value);
    if (value > maxCopy) {
        $('#' + controlId).focus();
        var msg = 'Maximum value for No of Copies is ' + maxCopy + '.';
        AddedValidationRequireMsg(controlId, msg);

        //Swaltype : 1 alert , 2 confirm
        Swal.fire({
            title: '',
            text: 'Error : ' + msg,
            icon: 'error',
            //icontype == 1 ? 'info' : icontype == 2 ? 'success' : icontype == 3 ? 'error' : icontype == 4 ? 'warning' : icontype == 5 ? 'question' : 'info',
            reverseButtons: true,
            showCancelButton: false,
            allowOutsideClick: false,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Cancel'
        }).then(function (result) {
            if ((btnCancel != undefined)) {
                $("#" + btnCancel).click();
            }
        });

        //bootbox.alert(msg, function () {
        //    if ((btnCancel != undefined)) {
        //        $("#" + btnCancel).click();
        //    }
        //    bootbox.hideAll();
        //});

        $('.modal[role="dialog"]').modal('hide')
        return false;
    }
    return true;
};
function ValidateEmail(email) {
    if (email == "") {

        //Swaltype : 1 alert , 2 confirm
        Swal.fire({
            title: '',
            text: 'Error : ' + "Email Adress is required.",
            icon: 'error',
            //icontype == 1 ? 'info' : icontype == 2 ? 'success' : icontype == 3 ? 'error' : icontype == 4 ? 'warning' : icontype == 5 ? 'question' : 'info',
            reverseButtons: true,
            showCancelButton: false,
            allowOutsideClick: false,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Cancel'
        }).then(function (result) {

        });

        //bootbox.alert("Email Adress is required.");

        $('.modal[role="dialog"]').modal('hide')
        return false;
    }
    //check if email is valid
    if (!IsEmailValid(email)) {

        //Swaltype : 1 alert , 2 confirm
        Swal.fire({
            title: '',
            text: 'Error : ' + "Email Adress is not valid.",
            icon: 'error',
            //icontype == 1 ? 'info' : icontype == 2 ? 'success' : icontype == 3 ? 'error' : icontype == 4 ? 'warning' : icontype == 5 ? 'question' : 'info',
            reverseButtons: true,
            showCancelButton: false,
            allowOutsideClick: false,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Cancel'
        }).then(function (result) {

        });

        //bootbox.alert("Email Adress is not valid.");

        $('.modal[role="dialog"]').modal('hide')
        return false;
    }
    return true;
};

function IsEmailValid(email) {
    const re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}

function cascadeDropdown(ddlid, selectedValue, path, param, addDefaultOption, defaultOptionText, initialize, callback) {
    var select = $('#' + ddlid);
    //selectedValue = "";

    $.ajax({
        type: "POST",
        url: path,
        data: param,
        success: function (returndata) {
            select.empty();

            if (returndata.data == "" || returndata.data == undefined) {
                select.append($('<option></option>').val("").html("No Data Found"));
            }
            if (addDefaultOption) {

                defaultOptionText = (returndata.data != "" || returndata.data != undefined) ? defaultOptionText : "No Data Found";
                select.empty();
                select.append($('<option></option>').val("").html(defaultOptionText));
            }

            $.each(returndata.data, function (index, itemData) {
                if (itemData.IsSelected) {

                    selectedValue = itemData.Id
                }
                //select.append($('<option></option>').val(itemData.Id).html(itemData.Name));


                var nValue = shortenText(itemData.Name);

                select.append($('<option data-typeid="' + itemData.RequestTypeId + '" data-subtypeid="' + itemData.RequestSubTypeId + '"></option>').val(itemData.Id).html(nValue));

            });

            if (selectedValue != "") {
                select.val(selectedValue);
            }

            if (initialize) {
                select.multiselect('destroy');
                initializeDDL(ddlid, defaultOptionText);
            }

            if ($.isFunction(callback)) {
                setTimeout(function () {
                    callback();
                }, 500);
            }
        },
        error: function (request, textStatus, errorThrown) {

            //Swaltype : 1 alert , 2 confirm
            Swal.fire({
                title: '',
                text: 'Error : ' + "System Notification: No Internet Connection.",
                icon: 'error',
                //icontype == 1 ? 'info' : icontype == 2 ? 'success' : icontype == 3 ? 'error' : icontype == 4 ? 'warning' : icontype == 5 ? 'question' : 'info',
                reverseButtons: true,
                showCancelButton: false,
                allowOutsideClick: false,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#3085d6',
                confirmButtonText: 'Cancel'
            }).then(function (result) {

            });

            //alert("System Notification: No Internet Connection");
            //hideLoading(ddlId);
        }
    });
}

function RedirectTo(targetURL, status) {
    if (status == 'Failed to Save') {
        $("div.modal").toggleClass("in");
    } else {
        window.location.href = targetURL;
    }
}

function shortenText(data) {
    var maxLength = 50;

    var trimmedString = data;

    if (data.length > maxLength) {
        //trim the string to the maximum length
        trimmedString = data.substr(0, maxLength);

        //re-trim if we are in the middle of a word and 
        trimmedString += "....";
    }

    return trimmedString;
};

function ValidateTextMinimumRange(controlId, description, minLenght) {
    var value = $.trim($('#' + controlId).val());
    $('#' + controlId).val(value);
    if (value.length < minLenght) {
        $('#' + controlId).focus();
        var msg = 'The ' + description + ' must be at least ' + minLenght + ' characters long.';

        //Swaltype : 1 alert , 2 confirm
        Swal.fire({
            title: '',
            text: 'Error : ' + msg,
            icon: 'error',
            //icontype == 1 ? 'info' : icontype == 2 ? 'success' : icontype == 3 ? 'error' : icontype == 4 ? 'warning' : icontype == 5 ? 'question' : 'info',
            reverseButtons: true,
            showCancelButton: false,
            allowOutsideClick: false,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Cancel'
        }).then(function (result) {
            //
        });


        //bootbox.alert(msg, function () {
        //    bootbox.hideAll();
        //});

        return false;
    }
    return true;
};

function CompareText(controlId, description, controlId2, description2) {
    var value1 = $.trim($('#' + controlId).val());
    var value2 = $.trim($('#' + controlId2).val());

    if (value1 !== value2) {
        $('#' + controlId2).focus();
        var msg = 'The ' + description + ' and ' + description2 + ' do not match.';

        //Swaltype : 1 alert , 2 confirm
        Swal.fire({
            title: '',
            text: 'Error : ' + msg,
            icon: 'error',
            //icontype == 1 ? 'info' : icontype == 2 ? 'success' : icontype == 3 ? 'error' : icontype == 4 ? 'warning' : icontype == 5 ? 'question' : 'info',
            reverseButtons: true,
            showCancelButton: false,
            allowOutsideClick: false,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Cancel'
        }).then(function (result) {
            //
        });


        //bootbox.alert(msg, function () {
        //    bootbox.hideAll();
        //});

        return false;
    }
    return true;
};


function ValidateSpecialCharacter() {
    var elements = $('[validateSpecialChar]');
    var validationCtr = 0;
    var liTags = "";
    var retVal = false;

    $('input[validateSpecialChar="disallow"]').each(function () {
        var control = $(this);
        var controlid = control.attr('id');
        var controlValue = control.val();
        var validationType = control.attr("validateSpecialChar");

        if (controlValue !== "") {
            if (!Validate(controlValue)) {
                liTags += "<li>" + titleCase(controlid) + " must not contain special Characters." + "</li>";
                validationCtr++;
            }
        }
    });

    if (validationCtr > 0) {

        //Swaltype : 1 alert , 2 confirm
        Swal.fire({
            title: '',
            text: "<ul>" + liTags + "</ul>",
            icon: 'error',
            //icontype == 1 ? 'info' : icontype == 2 ? 'success' : icontype == 3 ? 'error' : icontype == 4 ? 'warning' : icontype == 5 ? 'question' : 'info',
            reverseButtons: true,
            showCancelButton: false,
            allowOutsideClick: false,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Cancel'
        }).then(function (result) {
            if ((btnCancel != undefined)) {
                $("#" + btnCancel).click();
            }
        });

        return false;
    }
    else {
        return true;
    }
}

function Validate(item) {
    if (item == "") return;

    var regex = /^[a-zA-Z0-9,./&()-/' ]+$/
    var regex2 = /[\:*?#!$%^;*+_{}~=<>|]/

    var isValid = regex.test(item);
    var isValid2 = regex2.test(item);
    if (!isValid || isValid2) {

        isValid = false;
    }

    return isValid;
}

function titleCase(txt) {
    if (txt != "") {
        var title = txt.replace(/([A-Z])/g, " $1");
        return title.charAt(0).toUpperCase() + title.slice(1);
    }
    return txt;
}


function alphaNumericOnly(e) {
    var k;
    document.all ? k = e.keyCode : k = e.which; 
    return ((k > 64 && k < 91) || (k > 96 && k < 123) || k == 8 || k == 32 || (k >= 48 && k <= 57));
}


