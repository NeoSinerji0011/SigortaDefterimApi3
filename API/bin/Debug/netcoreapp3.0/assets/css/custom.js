var params = {};
window.location.search
    .replace(/[?&]+([^=&]+)=([^&]*)/gi, function (str, key, value) {
        params[key] = value;
    }
    );
var messageList;


$(".myButton").click(function () {
    var containerState = $(".main_container").css("display");
    var acenteMesaj = "";
    if (containerState == "none") {
        acenteMesaj = $("textarea[name=message]").eq(0).val();
    }
    else
        acenteMesaj = $("textarea[name=message]").eq(1).val();
    console.log(acenteMesaj);
    if (acenteMesaj.length > 0) {
       
        var files = $(".input_customFile")[0].files;
        var formData = new FormData();
        for (var i = 0; i < files.length; ++i) {
            formData.append('file[]', files[i]);
        }
        formData.append("Message", acenteMesaj.trim());
        formData.append("Session", $("input[name=session]").val());
        formData.append("Sendtype", $("input[name=sendtype]").val());

        $(".loader").css("display", "block")
        $.ajax({
            type: "POST",
            url: "/api/MobileApp/companyMessage",
            contentType: false,
            enctype: 'multipart/form-data',
            processData: false,
            dataType: 'json',
            data: formData,
            headers: {
                Authorization: 'Bearer ' + params.token
            },

            success: function (result, status, xhr) {

                if (result.statusCode == 200) {
                    $(".input_customFile").val(null)
                    $("textarea[name=message]").val("")
                    $(".file_context").css("display", "none");
                    $(".lbl_reselect").css("display", "none");
                    $(".fileimagecontent").html('');
                }
                Swal.fire(
                    result.message,
                    '',
                    result.statusCode == 200 ? 'success' : 'error'
                )
                $(".loader").css("display", "none")
            },
            error: function (xhr, status, error) {
                $(".loader").css("display", "none")
                console.log(error);
            }
        });
    }
    else
        messageBox("mesajwarning")
    // $(".messageinput").addClass("alert-validate")

});
$(document).ready(function () {
    pageLoad();

});
var temptest;
function pageLoad() {
    $(".loader").css("display", "block")
    $.ajax({
        type: "POST",
        url: "/api/MobileApp/GetCompanyMessage",
        contentType: 'application/json',
        headers: {
            Authorization: 'Bearer ' + params.token
        },

        success: function (result, status, xhr) {

            if (xhr.status != 200) {
                $(".bg-contact2").css("display", "none")
                messageBox("companywarning");
            }
            else {
                $(".bg-contact2").css("display", "block")

                messageList = result.messageList;
                fnk_messageList(result.brasAdi);

                $(".username").html(result.adSoyad)
                
                $(".policenumarasi").html(result.policeNumarasi)
                
                $(".talepno").html(result.talepNo)
                $(".bransAdi").html(result.brasAdi)
                $(".urun_aciklama").html(result.urun_Plaka_Sehir)

                $("input[name=session]").val(result.policeId)
                $("input[name=sendtype]").val(1)

                if (result.adSoyad == null || result.adSoyad == "") {
                    $(".username").html("-")
                }
                if (result.policeNumarasi == null || result.policeNumarasi == "") {
                    $(".policenumarasi").html("-")
                }
                if (result.talepNo == null || result.talepNo == "") {
                    $(".talepno").html("-")
                }
                if (result.brasAdi == null || result.brasAdi == "") {
                    $(".bransAdi").html("-")
                }
                if (result.urun_Plaka_Sehir == null || result.urun_Plaka_Sehir == "") {
                    $(".urun_aciklama").html("-")
                }
            }
            $(".loader").css("display", "none")
        },
        error: function (xhr, status, error) {
            messageBox("companywarning");
            $(".loader").css("display", "none")
            console.log(error);
        }
    });

    $(".input_customFile").change(function () {

        $(".file_context").css("display", "none");
        $(".lbl_reselect").css("display", "none");
        $(".fileimagecontent").html('');
        var currentInput = $(this);
        var val = "",
            regex = new RegExp("(.*?)\.(docx|doc|pdf|xml|bmp|ppt|pptx|xls|xlsx|png|jpg|jpeg)$");
        currentInput = currentInput[0].files;

        if (currentInput.length > 5) {
            messageBox("selectfilewarning");
            $(this).val(null);
            return;
        }
        for (var i = 0; i < currentInput.length; i++) {
            val = currentInput[i].name;
            if (!(regex.test(val))) {
                $(this).val(null);
                messageBox("filewarning");
                return;
            }
        }

        if (currentInput.length > 0) {

            $(".file_context").css("display", "block");
            $(".lbl_reselect").css("display", "block");
            var imageName = "", item = "", template = "";
            var filesList = $('.input_customFile')[0].files
            for (var i = 0; i < filesList.length; i++) {
                item = filesList[i].name;
                item = item.substr(item.lastIndexOf('.') + 1);
                imageName = (item == "pdf" ? "pdf" : (item == "png" || item == "jpg" || item == "jpeg" ? "picture" : (item.includes('xls') ? "excel" : (item.includes('doc') ? "word" : (item.includes('ppt') ? "powerpoint" : "doc")))));

                template += '<img src="/assets/images/' + imageName + '.png" style="width: 28px;margin: 3px;">'
            }
            $(".fileimagecontent").html(template);
            $(".file_count").text(currentInput.length);
        }

    });
    $(".fileUploadTrigger1").click(function () {
        $("#customFile").trigger("click");
    });
    $(".fileUploadTrigger2").click(function () {
        $("#customFile").trigger("click");
    });
    $(".fileUploadTrigger3").click(function () {
        $("#customFile").trigger("click");
    });

}
