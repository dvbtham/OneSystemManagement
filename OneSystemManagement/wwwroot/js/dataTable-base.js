"use strict";

$(document).ready(function () {
    "use strict";
    const options = {
        'lengthMenu': [[10, 20, 30, 50, -1], [10, 20, 30, 50, "Tất cả"]],
        "paging": true,
        "searching": true,
        "ordering": true,
        "info": true,
        "autoWidth": true,
        "language": {
            sProcessing: "Đang xử lý...",
            sLengthMenu: "Xem _MENU_ mục",
            sZeroRecords: "Không tìm thấy dòng nào phù hợp",
            sInfo: "Đang xem _START_ đến _END_ trong tổng số _TOTAL_ mục",
            sInfoEmpty: "Đang xem 0 đến 0 trong tổng số 0 mục",
            sInfoFiltered: "(được lọc từ _MAX_ mục)",
            sInfoPostFix: "",
            sSearch: "Tìm:",
            sUrl: "",
            oPaginate: {
                sFirst: "Đầu",
                sPrevious: "Trước",
                sNext: "Tiếp",
                sLast: "Cuối"
            }
        },
        dom: "Bfrtip",
        buttons: [
            "csv", "excel", "pdf", "print"
        ]
    };
    $("#oneDataTable").DataTable(options);
});