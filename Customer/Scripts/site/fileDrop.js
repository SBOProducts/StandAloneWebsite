
//https: //github.com/weixiyen/jquery-filedrop/blob/master/README.md

(function ($) {

    $("document").ready(function () {

        /* data-file-drop attribute: uploads a file and returns the url */
        $("*[data-file-drop]").each(function () {
            $(this).fileDrop();
        });

    });


})(jQuery);



jQuery.fn.fileDrop = function () {

    var attr = $(this).attr("data-file-dropper");

    if (typeof attr == 'undefined' || attr == false) {

        $(this).attr("data-file-dropper", "true");

        var dropbox = $(this);
        var types = eval(dropbox.attr("data-file-drop"));

        dropbox.filedrop({
            url: dropbox.attr("data-url"),
            paramname: 'files',
            data: { ImageSettings: dropbox.attr("data-image-settings") },
            maxFiles: 20,
            maxFileSize: 20,
            allowedfiletypes: types,
            dragOver: function () {
                // user dragging files over #dropzone
                dropbox.addClass("hover");
            },
            dragLeave: function () {
                // user dragging files out of #dropzone
                dropbox.removeClass("hover");
            },
            uploadStarted: function (i, file, len) {
                jQuery.showProgress("show");
            },
            afterAll: function () {
                // runs after all files have been uploaded or otherwise dealt with
                dropbox.removeClass("hover");
                jQuery.showProgress("hide");
            },
            uploadFinished: function (i, file, response) {
                dropbox.trigger("upload-complete", [file, response]);
            },
            error: function (err, file) {
                switch (err) {
                    case 'BrowserNotSupported':
                        alert('Your browser does not support HTML5 file uploads!');
                        break;
                    case 'TooManyFiles':
                        alert('Too many files! Please select 10 at most!');
                        break;
                    case 'FileTooLarge':
                        alert(file.name + ' is too large! Please upload files up to 4mb');
                        break;
                    case 'FileTypeNotAllowed':
                        alert(file.name + ' is not supported.');
                        break;
                    default:
                        alert(err);
                }
            }
        });

    }

}

