$("#btn_gonder").click(function () {

    var eposta = $("input[name=eposta]").val()
    var adsoyad = $("input[name=adsoyad]").val()
    var mesaj = $("textarea[name=txt_mesaj]").val()

    if (eposta.trim() == "" || adsoyad.trim() == "" || mesaj.trim() == "") {
        alert("Alanları doldurunuz");
        return;
    }
    var formData = new FormData();
    formData.append("adsoyad", adsoyad);
    formData.append("email", eposta);
    formData.append("message", mesaj);


    $.ajax({
        type: "POST",
        url: "/api/User/WebSiteContact",
        contentType: false,
        enctype: 'form-data',
        processData: false,
        dataType: 'json',
        data: formData,
        success: function (result, status, xhr) {

            Swal.fire(
                result.message,
                '',
                result.statusCode != "200" ? "error" : "success"
            );
            if (result.statusCode == "200") {
                $("input[name=eposta]").val("")
                $("input[name=adsoyad]").val("")
                $("textarea[name=txt_mesaj]").val("")
            }
        },
        error: function (xhr, status, error) { }
    });

});