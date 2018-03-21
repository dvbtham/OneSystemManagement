$(function() {
    alertShow();
    stickyPanel();
});

function alertShow() {
    const alert = $("#alert");
    if (alert.data("val")) {
        $.toast().reset("all");
        $("body").removeAttr("class").removeClass("bottom-center-fullwidth").addClass("top-center-fullwidth");
        $.toast({
            heading: "Thành công",
            text: "Dữ liệu đã được lưu.",
            position: "top-center",
            loaderBg: "#4dd4a8",
            icon: "success",
            hideAfter: 3500
        });
    }
}

function stickyPanel() {
    const panelHeading = $("#panel-heading");
    if (!panelHeading) return;

    panelHeading.sticky({ topSpacing: 65, responsiveWidth: true });
    panelHeading.on("sticky-start", function () {
        $(this).addClass("sticky-heading");
    });
    panelHeading.on("sticky-end", function () {
        $(this).removeClass("sticky-heading");
    });
}