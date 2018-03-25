$(function () {
    $(".select2").select2();
    const areaFunctionController = function () {
        var functionName = [];
        var roleFunction = {
            areaId: 0,
            functionId: 0,
            roleId: $("#roleId").data("id"),
            roles: ""
        };

        var dataObj = {
            roleId: $("#roleId").data("id"),
            areas: [{ id: 0, functions: [] }]
        }
        var successfullySaved = function (res, isReload) {
            $.toast({
                heading: "Thành công",
                text: res.message,
                position: "top-center",
                loaderBg: "#4dd4a8",
                icon: "success",
                hideAfter: 3500
            });

            if (isReload)
                setTimeout(() => { document.location.reload(); }, 1100);
        };

        var unSuccessfullySaved = function (res) {
            $.toast({
                heading: "Không thành công",
                text: res.message,
                position: "top-center",
                loaderBg: "#4dd4a8",
                icon: "error",
                hideAfter: 3500
            });
        };


        const init = function () {

            $(".childFunction").on("change", function () {
                const parent = $(this).data("parent");
                const area = $(this).data("area");
                const mId = $(this).attr("id");
                if (!$(this).is(":checked")) {
                    if ($(`.childOfFunction_${parent}${area}:checked`).length === 0) {
                        $(`#${area}${parent}`).prop("checked", false);
                    }
                } else {
                    if ($(`.childOfFunction_${parent}${area}:checked`).length === $(`.childOfFunction_${parent}${area}`).length) {
                        $(`#${area}${parent}`).prop("checked", true);
                    }
                }
                    
            });

            $(".parent").on("change",
                function () {
                    const fId = $(this).data("id");
                    const areaId = $(this).data("area");
                    const className = `.childOfFunction_${fId}${areaId}`;
                    if ($(this).is(":checked")) {
                        $.each($(className),
                            function () {
                                $(this).prop("checked", true);
                            });

                    } else {
                        $.each($(className),
                            function () {
                                $(this).prop("checked", false);
                            });
                        const inputLenght = $(`.childOfArea_${areaId}:checked`).length;
                        if (inputLenght === 0) {
                            $(`#area_${areaId}`).prop("checked", false);
                            return;
                        }
                    }
                });

            $(".edit-action").on("click",
                function (e) {
                    e.preventDefault();
                    roleFunction.areaId = $(this).data("area");
                    roleFunction.functionId = $(this).data("id");

                    $.ajax({
                        url: `
                            /SystemAdmin/RoleFunction/GetRole?roleId=${roleFunction.roleId}&areaId=${
                            roleFunction.areaId}&functionId=${roleFunction.functionId}`,
                        success: function (res) {
                            if (res.status) {
                                $(".select2").val(res.data).trigger("change");
                            }
                        },
                        error: function (res) {
                            unSuccessfullySaved(res);
                        }
                    });

                    $("#myModal").modal();
                });

            $(".select2").on("change",
                function (e) {
                    const role = $(e.currentTarget).val();
                    roleFunction.roles = role;
                });

            $("#reload").on("click",
                function () {
                    document.location.reload(true);
                });

            $.each($(".childName"),
                function (i, el) {
                    const name = $(this).data("childname");
                    functionName.push(name.toLowerCase());
                    const inputInGroup = $(this).find("input");
                    $.each(inputInGroup,
                        function (i, el) {
                            const checkedL = $(`.${el.className}:checked`).length;
                            if (checkedL === 0) {
                                el.removeAttribute("disabled");
                            }
                        });
                });

            $.each($(".group_header"),
                function () {
                    const parentName = $(this).data("parentname");

                    if (jQuery.inArray(parentName.toLowerCase(), functionName) !== -1) {
                        $(this).remove();
                    }
                    const inputInGroup = $(this).find("input");
                    $.each(inputInGroup,
                        function (i, el) {
                            const checkedL = $(`.${el.className}:checked`).length;
                            if (checkedL === 0) {
                                el.removeAttribute("disabled");
                            }
                        });
                });
        };

        const saveChanges = function () {
            $("#save").on("click",
                function (e) {
                    var me = $(this);
                    e.preventDefault();

                    if (me.data('requestRunning')) {
                        return;
                    }

                    me.data('requestRunning', true);
                    $.each($(".functionId:checked"),
                        function () {
                            const areaId = $(this).data("area");
                            const index = dataObj.areas.map(function (e) { return e.id; }).indexOf(areaId);
                            if (index === -1)
                                dataObj.areas.push({ id: areaId, functions: [] });
                        });

                    $.each($(".functionId:checked"),
                        function () {
                            const areaId = $(this).data("area");
                            const fId = $(this).data("id");

                            for (let i = 0; i < dataObj.areas.length; i++) {
                                if (areaId === dataObj.areas[i].id) {
                                    const ii = dataObj.areas[i].functions.indexOf(fId);
                                    if (ii === -1)
                                        dataObj.areas[i].functions.push(fId);
                                }
                            }
                        });

                    const index = dataObj.areas.map(function (e) { return e.id; }).indexOf(0);
                    if (index !== -1)
                        dataObj.areas.splice(index, 1);

                    $.ajax({
                        url: "/SystemAdmin/RoleFunction/SaveChanges",
                        data: {
                            json: JSON.stringify(dataObj)
                        },
                        success: function (res) {
                            if (res.status)
                                successfullySaved(res, false);
                            else
                                unSuccessfullySaved(res);

                            dataObj = {
                                roleId: $("#roleId").data("id"),
                                areas:
                                    [
                                        {
                                            id: 0,
                                            functions: []
                                        }
                                    ]
                            };
                        },
                        error: function (res) {
                            unSuccessfullySaved(res);
                        },
                        complete: function () {
                            me.data("requestRunning", false);
                        }
                    });
                });

            $("#saveRFChange").on("click",
                function (e) {
                    var me = $(this);
                    e.preventDefault();

                    if (me.data('requestRunning')) {
                        return;
                    }

                    me.data('requestRunning', true);
                    if ($(".select2").val() === null) {
                        roleFunction.roles = [];
                    }
                    $.ajax({
                        url: "/SystemAdmin/RoleFunction/UpdateRole",
                        type: "POST",
                        data: {
                            json: JSON.stringify(roleFunction)
                        },
                        success: function (res) {
                            if (res.status) {
                                successfullySaved(res, false);
                                $("#myModal").modal("hide");
                                $("#load").click();
                            }
                            else
                                unSuccessfullySaved(res);
                        },
                        error: function (res) {
                            unSuccessfullySaved(res);
                        },
                        complete: function () {
                            me.data("requestRunning", false);
                        }
                    });
                });
        };

        $(".areaInput").on("change",
            function () {
                const areaId = $(this).data("id");
                const childInput = document.getElementsByClassName("childOfArea_" + areaId + "");
                if (!$(this).is(":checked")) {
                    $.each(childInput,
                        function () {
                            $(this).prop("checked", false);
                        });
                }

            });


        $(".functionId").on("change",
            function () {
                const areaId = $(this).data("area");

                if ($(this).is(":checked")) {
                    $(`#area_${areaId}`).prop("checked", true);

                } else {
                    const inputLenght = $(`.childOfArea_${areaId}:checked`).length;
                    if (inputLenght === 0) {
                        $(`#area_${areaId}`).prop("checked", false);
                        return;
                    }
                }
            });

        saveChanges();

        return {
            init: init
        };
    }();
    areaFunctionController.init();
});