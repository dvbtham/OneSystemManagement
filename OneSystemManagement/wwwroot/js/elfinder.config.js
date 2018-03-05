$(function () {
    var myCommands = elFinder.prototype._options.commands;
    elFinder.prototype._options.commands.push("getimageurl");
    const subFolder = $("#elfinder").data("subfolder");
    const folder = $("#elfinder").data("folder");

    elFinder.prototype.commands.getimageurl = function () {
        this.exec = function (hashes) {
            const file = this.files(hashes);

            const path = this.fm.path(file[0].hash);
            const fullPath = path.replace("Files/", "Files/UserUploads/");

            const elToSetUrl = $("input[type='hidden']");

            var id = elToSetUrl.attr('id');
            id = "#" + id;
            $(id, opener.window.document).val(`/${fullPath}`);

            var ckeditorInputs = opener.window.document.getElementsByClassName("cke_dialog_ui_input_text");

            var elToSetImage = opener.window.document.getElementById("Img" + elToSetUrl.attr("id"));
            var imageDiv = opener.window.document.getElementById("image-show");
            var imageDiv2 = opener.window.document.getElementsByClassName("image-with-action");
            if (elToSetImage !== null && elToSetImage !== undefined) {
                elToSetImage.src = `/${fullPath}`;
            }
            if (imageDiv2[0])
                imageDiv2[0].style.display = "inline-block";

            if (imageDiv) {
                imageDiv.classList.remove("hide");
                imageDiv.style.display = "inline-block";
            }


            if (!ckeditorInputs[1]) {
                window.close();
                return;
            }

            var input = opener.window.document.getElementById(ckeditorInputs[1].id);
            if (input) input.value = `/${fullPath}`;
            elToSetUrl.remove();

            window.close();
        }
        this.getstate = function () {
            //return 0 to enable, -1 to disable icon access
            return 0;
        }
    }
    var disabled = ['extract', 'archive', 'resize', 'help', 'select']; // Not yet implemented commands in ElFinder.Net

    $.each(disabled, function (i, cmd) {
        (idx = $.inArray(cmd, myCommands)) !== -1 && myCommands.splice(idx, 1);
    });

    var selectedFile = null;

    var options = {
        url: '/file/connector',
        customData: { folder: folder, subFolder: subFolder },
        rememberLastDir: false,
        commands: myCommands,
        contextmenu: {
            files: ['getfile', '|', 'getimageurl', 'quicklook', '|', '|', 'copy', 'cut', 'paste', 'duplicate', '|', 'rm', '|', 'edit', 'rename', 'resize', '|', 'archive', 'extract', '|', 'info']
        },
        lang: 'vi',
        uiOptions: {
            toolbar: [
                ['back', 'forward'],
                ['reload'],
                ['home', 'up'],
                ['mkdir', 'mkfile', 'upload'],
                ['open', 'download', 'getfile'],
                ['info'],
                ['quicklook'],
                ['copy', 'cut', 'paste'],
                ['rm'],
                ['duplicate', 'rename', 'edit'],
                ['view', 'sort']
            ]
        },
        handlers: {
            select: function (event, elfinderInstance) {
                if (event.data.selected.length === 1) {
                    var item = $('#' + event.data.selected[0]);
                    if (!item.hasClass("directory")) {
                        selectedFile = event.data.selected[0];
                        $('#elfinder-selectFile').show();
                        return;
                    }
                }
                $('#elfinder-selectFile').hide();
                selectedFile = null;
            }
        }
    };
    $('#elfinder').elfinder(options).elfinder('instance');

    $('.elfinder-toolbar:first').append('<div class="ui-widget-content ui-corner-all elfinder-buttonset" id="elfinder-selectFile" style="display:none; float:right;">' +
        '<div class="ui-state-default elfinder-button" title="Select" style="width: 100px;"></div>');
    $('#elfinder-selectFile').click(function () {
        if (selectedFile !== null)
            $.post('file/select-file', { target: selectedFile }, function (response) {
                alert(response);
            });
    });
});       