
$(document).ready(function () {
    $("#loader-wrapper").show();
    $("#loader-wrapper").hide();
    $('ul li a').click(function () {
        $('li a').removeClass("active");
        $(this).addClass("active");
    });
    $('#namecheck').hide();
    $('#emailcheck').hide();
    $('#helplinecheck').hide();
    $('#companycheck').hide();
    $('#addresscheck').hide();
    $('#fileuploadcheck').hide();
    $("#loader").hide();
    var name_err = true;
    var email_err = true;
    var helpline_err = true;
    var company_err = true;
    var address_err = true;
    var country_err = true;
    var category_err = true;
    var fileupload_err = true;
    var Texterror = true;
    var TextSubcategoryerror = true;
    var TextColumnerror = true;
    $('#name').on('blur', function () {
        eventname_check();
    });
    function eventname_check() {
        var name_val = $('#name').val();
        if (name_val == '') {
            $('#namecheck').show();
            $('#namecheck').html(" **Please Fill the name");
            $('#namecheck').focus();
            $('#namecheck').css("color", "red");
            name_err = false;
            return false;
        }
        else {
            $('#namecheck').hide();
        }
    }
    $('#email').on('blur', function () {
        email_check();
    });
    function email_check() {
        var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
        var email_val = $('#email').val();
        if (email_val.length == '') {
            $('#emailcheck').show();
            $('#emailcheck').html(" **Please Fill the email");
            $('#emailcheck').focus();
            $('#emailcheck').css("color", "red");
            email_err = false;
            return false;
        }
        else {
            $('#emailcheck').hide();
        }
        if (!emailReg.test(email_val)) {
            $('#emailcheck').show();
            $('#emailcheck').html(" **Please enter valid email address.");
            $('#emailcheck').focus();
            $('#emailcheck').css("color", "red");
            email_err = false;
            return false;
        }
        else {
            $('#emailcheck').hide();
        }
    }
    $('#helpline').on('blur', function () {
        helpline_check();
    });
    function helpline_check() {
        var mob = /^[1-9]{1}[0-9]{9}$/;
        var mob_val = $('#helpline').val();
        if (mob_val.length == '') {
            $('#helplinecheck').show();
            $('#helplinecheck').html(" **Please Fill the helpline");
            $('#helplinecheck').focus();
            $('#helplinecheck').css("color", "red");
            helpline_err = false;
            return false;
        }
        else {
            $('#helplinecheck').hide();
        }

        if (mob.test($.trim(mob_val)) == false) {
            $('#helplinecheck').show();
            $('#helplinecheck').html(" **Please valid helpline number");
            $('#helplinecheck').focus();
            $('#helplinecheck').css("color", "red");
            helpline_err = false;
            return false;
        }
        else {
            $('#helplinecheck').hide();
        }
    }
    $('#company').on('blur', function () {
        company_check();
    });
    function company_check() {
        var company_val = $('#company').val();
        if (company_val.length == '') {
            $('#companycheck').show();
            $('#companycheck').html(" **Please Fill the company");
            $('#companycheck').focus();
            $('#companycheck').css("color", "red");
            company_err = false;
            return false;
        }
        else {
            $('#companycheck').hide();
        }
    }
    $('#TextBoxA').on('blur', function () {
        TextBoxA_check();
    });
    function TextBoxA_check() {
        var company_val = $('#TextBoxA').val();
        if (company_val.length == '') {
            $('#CategortTextcheck').show();
            $('#CategortTextcheck').html(" **Please fill this Column");
            $('#CategortTextcheck').focus();
            $('#CategortTextcheck').css("color", "red");
            Texterror = false;
            return false;
            $("#loader-wrapper").hide();
        }
        else {
            $('#CategortTextcheck').hide();
            $("#loader-wrapper").hide();
        }
    }
    $('#txtSubCategoryName').on('blur', function () {
        txtSubCategoryName_check();
    });
    function txtSubCategoryName_check() {
        var company_val = $('#txtSubCategoryName').val();
        if (company_val.length == '') {
            $('#SubCategortTextcheck').show();
            $('#SubCategortTextcheck').html(" **Please fill this Sub category name");
            $('#SubCategortTextcheck').focus();
            $('#SubCategortTextcheck').css("color", "red");
            TextSubcategoryerror = false;
            return false;
            $("#loader-wrapper").hide();
        }
        else {
            $('#SubCategortTextcheck').hide();
            $("#loader-wrapper").hide();
        }
    }

    $('#txtColumns').on('blur', function () {
        txtColumnsName_check();
    });
    function txtColumnsName_check() {
        var company_val = $('#txtColumns').val();
        if (company_val.length == '') {
            $('#ColumnNameTextcheck').show();
            $('#ColumnNameTextcheck').html(" **Please fill this name");
            $('#ColumnNameTextcheck').focus();
            $('#ColumnNameTextcheck').css("color", "red");
            TextSubcategoryerror = false;
            return false;
            $("#loader-wrapper").hide();
        }
        else {
            $('#ColumnNameTextcheck').hide();
            $("#loader-wrapper").hide();
        }
    }

    $('#address').on('blur', function () {
        address_check();
    });
    function address_check() {
        var address_val = $('#address').val();
        if (address_val.length == '') {
            $('#addresscheck').show();
            $('#addresscheck').html(" **Please Fill the address");
            $('#addresscheck').focus();
            $('#addresscheck').css("color", "red");
            address_err = false;
            return false;
        }
        else {
            $('#addresscheck').hide();
        }
    }

    function ddlcategory() {
        if ($("#ddlcategory").val() != null) {
            if ($("#ddlcategory").val() != "Choose option...") {
                $('#categorycheck').hide();
                category_err = true;
                return true;
            }

            else {
                $('#categorycheck').show();
                $('#categorycheck').html(" **Please select category");
                $('#categorycheck').focus();
                $('#categorycheck').css("color", "red");
                category_err = false;
                return false;
            }
        }
    }
    function ddlcontry() {
        if ($("#ddlcountry").val() != null) {
            if ($("#ddlcountry").val() != "Choose option...") {
                $('#countrycheck').hide();
                country_err = true;
                return true;
            }
            else {
                $('#countrycheck').show();
                $('#countrycheck').html(" **Please select country");
                $('#countrycheck').focus();
                $('#countrycheck').css("color", "red");
                country_err = false;
                return false;
            }
        }
    }
    $("#ddlcountry").change(function () {
        var selectedCountry = $(this).children("option:selected").val();
        if (selectedCountry != "Choose option...") {
            $('#countrycheck').hide();
            country_err = true;
            return true;
        }
        else {
            $('#countrycheck').show();
            $('#countrycheck').html(" **Please select country");
            $('#countrycheck').focus();
            $('#countrycheck').css("color", "red");
            category_err = false;
            return false;
        }

    });
    $("#ddlcategory").change(function () {
        var selectedCategory = $(this).children("option:selected").val();
        if (selectedCategory != "Choose option...") {
            $('#categorycheck').hide();
            category_err = true;
            return true;
        }

        else {
            $('#categorycheck').show();
            $('#categorycheck').html(" **Please select category");
            $('#categorycheck').focus();
            $('#categorycheck').css("color", "red");
            category_err = false;
            return false;
        }
    });

    $('[data-toggle="popover"]').popover({
        title: setData,
        html: true,
        placement: 'right'

    });
    function setData(id) {
        var set_data = '';
        var element = $(this);
        var id = element.attr("id");
        $.ajax({
            url: "/Admin/InfoCreateEventPage?id" + id,
            method: "post",
            async: false,
            data: { id: id },
            success: function (data) {
                set_data = data;
            }

        });
        return set_data;
    }

    $('#Catsubmitbtn').click(function (e) {
        Texterror = true;
        TextBoxA_check();
        $("#loader-wrapper").show();
        if ((Texterror == true)) {
            if ($('#BadgeCategoryPage').attr('id') == 'BadgeCategoryPage') {
                var valdata = $("#myformCategory").serialize();
                var respone = $.ajax({
                    url: "/Badges/CreateCategoryBadgeSave",
                    type: "POST",
                    dataType: 'json',
                    contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                    data: valdata,
                });
                respone.done(function (msg) {
                    $("#loader").show();
                    if (msg == "Saved") {
                        $('#msg').html("Record Saved Sucessfully.");

                        window.location.href = 'AddBadgeCategory';
                    }
                    else if (msg == "Not") {
                        $('#msg').html("Record Not Saved Sucessfully.");
                    }
                    else if (msg == "Already") {
                        $('#msg').html("This category already exist.");
                    }
                    $('#msg').focus();
                    $("#loader").focus();
                    $("#loader-wrapper").hide();
                });
                respone.fail(function (msg) {
                    $("#loader").show();
                    $('#msg').html("Record not saved sucessfully.");
                    $('#msg').focus();
                    $("#loader-wrapper").hide();
                });
            }
            return true;
        }
        else {
            $("#loader-wrapper").hide();
            return false;

        }
    });
    $('#SubCatsubmitbtn').click(function (e) {
        TextSubcategoryerror = true;
        txtSubCategoryName_check();
        $("#loader-wrapper").show();
        if ((TextSubcategoryerror == true)) {
            if ($('#BadgeSubCategoryPage').attr('id') == 'BadgeSubCategoryPage') {
                alert("true");
                var valdata = $("#myformSubCategory").serialize();
                var respone = $.ajax({
                    url: "/Badges/CreateSubCategoryBadgeSave",
                    type: "POST",
                    dataType: 'json',
                    contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                    data: valdata,
                });
                respone.done(function (msg) {
                    $("#loader").show();
                    if (msg == "Saved") {
                        $('#msg').html("Record Saved Sucessfully.");
                    }
                    else if (msg == "Not") {
                        $('#msg').html("Record Not Saved Sucessfully.");
                    }
                    else if (msg == "Already") {
                        $('#msg').html("This sub-category already exist.");
                    }
                    $('#msg').focus();
                    $("#loader").focus();
                    $("#loader-wrapper").hide();
                });
                respone.fail(function (msg) {
                    $("#loader").show();
                    $('#msg').html("Record not saved sucessfully.");
                    $('#msg').focus();
                    $("#loader-wrapper").hide();
                });
            }
            return true;
        }
        else {
            return false;
        }
    });
    $('#Colsubmitbtn').click(function (e) {
        TextColumnerror = true;
        txtColumnsName_check();
        $("#loader-wrapper").show();
        if ((TextColumnerror == true)) {
            if ($('#CoulmnsCategoryPage').attr('id') == 'CoulmnsCategoryPage') {
                var valdata = $("#myformColumns").serialize();
                var respone = $.ajax({
                    url: "/Admin/AddCreateColumns",
                    type: "POST",
                    dataType: 'json',
                    contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                    data: valdata,
                });
                respone.done(function (msg) {
                    $("#loader").show();
                    if (msg == "Saved") {
                        $('#msg').html("Record Saved Sucessfully.");
                    }
                    else if (msg == "Not") {
                        $('#msg').html("Record Not Saved Sucessfully.");
                    }
                    else if (msg == "Already") {
                        $('#msg').html("This columns already exist.");
                    }
                    $('#msg').focus();
                    $("#loader").focus();
                    $("#loader-wrapper").hide();
                });
                respone.fail(function (msg) {
                    $("#loader").show();
                    $('#msg').html("Record not saved sucessfully.");
                    $('#msg').focus();
                    $("#loader-wrapper").hide();
                });
            }
            return true;
        }
        else {
            return false;
        }
    });
    
    $('#AllotBadgesubmitbtn').click(function (e) {
        if ((name_err == true) && (email_err == true) && (helpline_err == true)) {
            if ($('#AllotBadgesPage').attr('id') == 'AllotBadgesPage') {
                var valdata = $("#myformAllotBadges").serialize();
                var respone = $.ajax({
                    url: "/Badges/AllotBadgeSave",
                    type: "POST",
                    dataType: 'json',
                    contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                    data: valdata,
                });
                respone.done(function (msg) {
                    $("#loader").show();
                    if (msg == "Saved") {
                        $('#msg').html("Record Saved Sucessfully.");
                    }
                    else if (msg == "Not") {
                        $('#msg').html("Record Not Saved Sucessfully.");
                    }
                    else if (msg == "Already") {
                        $('#msg').html("This columns already exist.");
                    }
                    $('#msg').focus();
                    $("#loader").focus();
                });
                respone.fail(function (msg) {
                    $("#loader").show();
                    $('#msg').html("Record not saved sucessfully.");
                    $('#msg').focus();
                });
            }
            return true;
        }
        else {
            return false;
        }
    });
    
    
});



// Add the following code if you want the name of the file appear on select
$(".custom-file-input").on("change", function () {
    var fileName = $(this).val().split("\\").pop();
    $(this).siblings(".custom-file-label").addClass("selected").html(fileName);
});


//jb validater

<script>
    $(function (){

        let validator = $('form.needs-validation').jbvalidator({
        errorMessage: true,
            successClass: true,
            language: 'dist/lang/en.json'
        });

        //new custom validate methode
        validator.validator.custom = function(el, event){

            if($(el).is('[name=password]') && $(el).val().length < 5){
                return 'Your password is too weak.';
            }

            return '';
        }

        let validatorServerSide = $('form.validatorServerSide').jbvalidator({
        errorMessage: true,
            successClass: false,
        });

        //serverside
        $(document).on('submit', '.validatorServerSide', function(){

        $.ajax({
            method: "get",
            url: "test.json",
            data: $(this).serialize(),
            success: function (data) {
                if (data.status === 'error') {
                    validatorServerSide.errorTrigger($('[name=username]'), data.message);
                }
            }
        })

            return false;
        });
    })
</script>
