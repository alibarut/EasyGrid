function Reload() {
    window.location.reload();
}
function RedirectTo(url) {
    parent.window.location = url; 
}
function Reload2() {
    parent.window.location.reload(true);
}
function LoadListView(pageUrl) {
    $("#loading").show();
    $.ajax({
        url: pageUrl,
        success: function (msg) {
            $("#loading").hide();
            $('#divList').html(msg);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $("#loading").hide();
            $('#divList').html("İŞLEM SIRASINDA HATA OLUŞTU!");
        }
    });
}
function LoadListView2(pageUrl, div) {
    $("#loading").show();
    $.ajax({
        url: pageUrl,
        success: function (msg) {
            $("#loading").hide();
            $(div).html(msg);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $("#loading").hide();
            $(div).html("İŞLEM SIRASINDA HATA OLUŞTU!");
        }
    });
}