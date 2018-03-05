
$("#oneDataTable").on("click", ".delete-func",
    function (e) {
        const link = $(e.target);
        swal({
            title: "Bạn có chắc chắn xóa?",
            text: "Bạn sẽ không thể phục hồi lại khi xóa!",
            type: "warning",
            cancelButtonText: "Hủy",
            showCancelButton: true,
            confirmButtonColor: "#d9534f",
            confirmButtonText: "Có, Tôi muốn xóa!",
            closeOnConfirm: false
        }, function () {

            $.ajax({
                url: `/systemadmin/Function/Delete/${link.attr("data-id")}`,
                method: "delete",
                success: function () {
                    link.parents("tr").fadeOut(function () {
                        $(this).remove();
                    });
                    swal("Đã xóa!", `${link.attr("data-name")} đã bị xóa khỏi hệ thống.`, "success");
                },
                error: function (error) {
                    console.log(error);
                    swal("Opps!", "Đã xảy ra lỗi trong quá trình thực thi yêu cầu.", "error");
                }
            });


        });
        return false;
    });