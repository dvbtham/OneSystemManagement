$(function() {
    alertShow();
    setImageUrl();
    stickyPanel();
    tagInputConfig();
    datetimePicker();
    
    $("form").on("submit",
        function() {
            showErrorInput();
        });
    rangeSlider();
});

function rangeSlider() {
    $("#range").ionRangeSlider({
        hide_min_max: true,
        keyboard: true,
        min: 0,
        max: 100000000,
        from: 0,
        to: 1000000,
        type: "double",
        step: 1,
        prefix: "vnđ ",
        grid: true
    });
}

function showErrorInput() {
    //var errorEl = $(".field-validation-error");
    //if (errorEl === undefined) return;
    //if (errorEl.children().length > 0) {
    //    errorEl.parent().closest("div").addClass("has-error");
    //} else {
    //    errorEl.closest("div").removeClass("has-error");
    //}
    
}

function datetimePicker() {

    $(".select2").select2();
    $('.input-daterange-datepicker').daterangepicker({
        buttonClasses: ['btn', 'btn-sm'],
        applyClass: 'btn-primary',
        cancelClass: 'btn-danger',
        locale: {
            applyLabel: "Áp dụng",
            cancelLabel: "Hủy",
            format: 'DD/MM/YYYY'
        }
    });
    $("#datetimepicker").datetimepicker({
        format: "DD/MM/YYYY",
        useCurrent: false,
        icons: {
            time: "fa fa-clock-o",
            date: "fa fa-calendar",
            up: "fa fa-arrow-up",
            down: "fa fa-arrow-down"
        }
    }).on("dp.show", function () {
        if ($(this).data("DateTimePicker").date() === null)
            $(this).data("DateTimePicker").date(moment());
    });
}

function tagInputConfig() {
    $(".bootstrap-tagsinput input").addClass("form-control");
    $(".bootstrap-tagsinput").css("width", "100%");
}

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

function setImageUrl() {
    $(".toggle-image").on("click",
        function() {
            var button = $(this);
            var popup = window.open("/file-manager/Images",
                "_blank",
                "toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=no, copyhistory=no, width= 720px, height= 420px");
            popup.onload = function() {

                const doc = popup.document;

                const input = doc.createElement("input");
                input.id = button.val();
                input.type = "hidden";
                doc.body.appendChild(input);
            }
        });
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