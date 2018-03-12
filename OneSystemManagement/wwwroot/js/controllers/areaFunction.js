$(document).ready(function () {
    $(".select2").select2();
    const areaFunctionController = function () {
        var functionName = [];
        var data = [];
        var functionNoArea = [];
        var dataWithFunctionParent = {
            id: 0,
            parentId: $(".select2 :selected").val()
        };

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

            $(".edit-action").on("click",
                function (e) {
                    e.preventDefault();
                    $("#function-lbl").text($(this).data("parentname"));
                    $(".select2").val($(this).data("parentid")).trigger("change");
                    dataWithFunctionParent.id = $(this).data("id");
                    dataWithFunctionParent.parentId = $(".select2 :selected").val();
                    $("#myModal").modal();
                });

            $(".select2").on("select2:select", function (e) {
                var parentId = $(e.currentTarget).val();
                dataWithFunctionParent.parentId = parentId;
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
                        error: function(res) {
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
                        url: "/SystemAdmin/AreaFunction/ModifyFunction",
                        data: {
                            jsonObject: JSON.stringify(data),
                            functionNoArea: JSON.stringify(functionNoArea)
                        },
                        success: function (res) {
                            successfullySaved(res, false);
                        },
                        error: function(res) {
                            unSuccessfullySaved(res);
                        }
                    });
                });
        };

        var toggleCheckbox = function (el) {

            const elId = el.attr("id");
            const areaId = el.data("area");
            const fId = el.data("id");

            const parentFunctionHtml = document.getElementsByClassName(`parentFunction_${fId}`);
            const childFunctionHtml = document.getElementsByClassName(`childFunction_${fId}`);

            if (el.is(":checked")) {
                toggleDisabled(parentFunctionHtml, elId, true);
                toggleDisabled(childFunctionHtml, elId, true);

                for (let k = 0; k < data.length; k++) {
                    if (data[k].areaId === areaId)
                        data[k].functionIds.push(fId);
                }
                const index = functionNoArea.indexOf(fId);
                functionNoArea.splice(index, 1);
            } else {
                toggleDisabled(parentFunctionHtml, elId, false);
                toggleDisabled(childFunctionHtml, elId, false);

                for (let m = 0; m < data.length; m++) {
                    if (data[m].areaId === areaId) {
                        const index = data[m].functionIds.indexOf(fId);
                        data[m].functionIds.splice(index, 1);
                    }
                }
                functionNoArea.push(fId);
            }
        };
        saveChanges();
        return {
            init: init
        };
    }();
    areaFunctionController.init();
})