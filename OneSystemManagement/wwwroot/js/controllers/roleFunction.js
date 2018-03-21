$(document).ready(function () {
    $(".select2").select2();
    const areaFunctionController = function () {
        var functionName = [];
        var data = [];
        var dataWithFunctionParent = {
            id: 0,
            parentId: $(".select2 :selected").val()
        };

        var roleFunction = {
            areaId: 0,
            functionId: 0,
            roleId: $("#roleId").data("id"),
            roles: ""
        };

        var dataObj = {
            roleId: $("#roleId").data("id"),
            areas: []
        }
        
        var areaInput = document.getElementsByClassName("areaInput");
        var functionInput = document.getElementsByClassName("functionId");

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

            $("input[type='checkbox']").change(function () {
                toggleCheckbox($(this));
            });

            $.each(areaInput,
                function (i, el) {
                    const areaId = parseInt(el.dataset.id);
                    if (el.checked) {
                        let area = {
                            id: areaId,
                            functions: []
                        }
                        dataObj.areas.push(area);
                    } else {
                        const childInput = document.getElementsByClassName("childOfArea_" + areaId + "");
                        $.each(childInput,
                            function () {
                                $(this).attr("disabled", true);
                            });
                    }
                });

            $.each(functionInput,
                function (index, el) {
                    const funcId = parseInt(el.dataset.id);
                    const areaId = parseInt(el.dataset.area);

                    if (el.checked) {
                        for (let i = 0; i < dataObj.areas.length; i++) {
                            if (areaId === dataObj.areas[i].id) {
                                if (dataObj.areas[i].functions.indexOf(funcId) === -1) {
                                    dataObj.areas[i].functions.push(funcId);
                                }
                            }
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
/SystemAdmin/RoleFunction/GetRole?roleId=${roleFunction.roleId}&areaId=${roleFunction.areaId}&functionId=${roleFunction.functionId}`,
                       success: function (res) {
                           if (res.status) {
                               $(".select2").val(res.data).trigger("change");
                                console.log(res.data);
                            }
                        },
                        error: function (res) {
                            unSuccessfullySaved(res);
                        }
                    });

                    $("#myModal").modal();
                });

            $(".select2").on("change", function (e) {
                const role = $(e.currentTarget).val();
                roleFunction.roles = role;
            });

            $("#saveParentChange").on("click",
                function () {
                    $.ajax({
                        url: "/SystemAdmin/AreaFunction/ModifyParentFunction",
                        data: {
                            data: JSON.stringify(dataWithFunctionParent)
                        },
                        success: function (res) {
                            successfullySaved(res, true);
                        },
                        error: function (res) {
                            unSuccessfullySaved(res);
                        }
                    });
                });

            $("#reload").on("click", function () {
                document.location.reload(true);
            });

            $.each($(".text-area"),
                function () {
                    let mData = {
                        areaId: $(this).data("area"),
                        functionIds: []
                    }
                    data.push(mData);
                });

            $.each($(".childName"),
                function (i, el) {
                    var name = $(this).data("childname");
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
                function (i, el) {
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

        var toggleDisabled = function (el, elId, isChecked) {
            if (isChecked) {
                for (let i = 0; i < el.length; i++) {

                    if (el[i].id !== elId) {
                        el[i].setAttribute("disabled", "");
                    } else {
                        el[i].removeAttribute("disabled");
                    }
                }
            } else {
                for (let j = 0; j < el.length; j++) {

                    if (el[j].id !== elId) {
                        el[j].removeAttribute("disabled");
                    }
                }
            }
        };

        const saveChanges = function () {
            $("#save").on("click",
                function () {
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
                        },
                        error: function (res) {
                            unSuccessfullySaved(res);
                        }
                    });
                });

            $("#saveRFChange").on("click",
                function () {
                    $.ajax({
                        url: "/SystemAdmin/RoleFunction/UpdateRole",
                        type: "POST",
                        data: {
                            json: JSON.stringify(roleFunction)
                        },
                        success: function (res) {
                            if (res.status)
                                successfullySaved(res, false);
                            else
                                unSuccessfullySaved(res);
                        },
                        error: function (res) {
                            unSuccessfullySaved(res);
                        }
                    });
                });
        };

        $(".areaInput").on("change",
            function () {
                const areaId = $(this).data("id");
                const childInput = document.getElementsByClassName("childOfArea_" + areaId + "");
                if ($(this).is(":checked")) {

                    if (dataObj.areas.some(x => x.id !== areaId) || dataObj.areas.length === 0)
                        dataObj.areas.push({ id: areaId, functions: [] });
                } else {
                    const index = dataObj.areas.map(function (e) { return e.id; }).indexOf(areaId);

                    if (index !== -1)
                        dataObj.areas.splice(index, 1);

                    $.each(childInput,
                        function () {
                            $(this).prop("checked", false);
                        });
                    let area = dataObj.areas.filter(x => x.id === areaId).map(x => x);
                    area.functions = [];

                    $.ajax({
                        url: `/SystemAdmin/RoleFunction/DeleteArea/${areaId}`,
                        type: "Delete",
                        success: function (res) {
                            if (res.status)
                                successfullySaved(res, false);
                            else
                                unSuccessfullySaved(res);
                        },
                        error: function (res) {
                            unSuccessfullySaved(res);
                        }
                    });
                }

            });

        $(".functionId").on("change",
            function () {
                const functionId = $(this).data("id");
                const areaId = $(this).data("area");
                if ($(this).is(":checked")) {
                    if (dataObj.areas.length === 0 || dataObj.areas.map(function (e) { return e.id; }).indexOf(areaId) === -1) {
                        dataObj.areas.push({ id: areaId, functions: [] });
                    }
                    $("#area_" + areaId + "").prop("checked", true);
                    for (let i = 0; i < dataObj.areas.length; i++) {
                        if (areaId === dataObj.areas[i].id) {
                            dataObj.areas[i].functions.push(functionId);
                        }
                    }
                } else {
                    for (let i = 0; i < dataObj.areas.length; i++) {
                        if (areaId === dataObj.areas[i].id) {
                            const index = dataObj.areas[i].functions.indexOf(functionId);
                            dataObj.areas[i].functions.splice(index, 1);
                        }
                    }
                    const roleId = $("#roleId").data("id");
                    const mData = {
                        roleId: roleId,
                        areaId: areaId,
                        functionId: functionId
                    };

                    $.ajax({
                        url: `/SystemAdmin/RoleFunction/Delete`,
                        type: "POST",
                        data: {
                            json: JSON.stringify(mData)
                        },
                        success: function (res) {
                            if (!res.status)
                                unSuccessfullySaved(res);
                        },
                        error: function (res) {
                            unSuccessfullySaved(res);
                        }
                    });
                }
                console.log(dataObj);
            });

        var toggleCheckbox = function (el) {

            const elId = el.attr("id");
            const fId = el.data("id");

            const parentFunctionHtml = document.getElementsByClassName(`parentFunction_${fId}`);
            const childFunctionHtml = document.getElementsByClassName(`childFunction_${fId}`);

            if (el.is(":checked")) {
                toggleDisabled(parentFunctionHtml, elId, true);
                toggleDisabled(childFunctionHtml, elId, true);

            } else {
                toggleDisabled(parentFunctionHtml, elId, false);
                toggleDisabled(childFunctionHtml, elId, false);
            }
        };
        saveChanges();

        return {
            init: init
        };
    }();
    areaFunctionController.init();
})