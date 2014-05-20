
(function ($) {


    $(document).ready(function () {

        // the delete button is disabled until an image is selected
        $("#DeleteImages").attr("disabled", "disabled");

        // make images sortable
        AddSorting();


        // setup mouse & keyboard feature to select images in the panel
        $("#previews").selectableItems({
            itemSelected: function (item) {
                $("#DeleteImages").removeAttr("disabled");
                
            },
            selectionCleared: function () {
                $("#DeleteImages").attr("disabled", "disabled");
            }
        });

        // handle the delete button click
        $("#DeleteImages").click(function (event) {
            event.preventDefault();

            var ids = new Array();
            var items = $("#previews").selectableItems("GetSelectedItems");

            items.each(function () {
                var id = $(this).attr("data-id");
                ids.push(id);
            });

            // send to server
            $.ajax({
                type: "POST",
                url: $("#previews").attr("data-delete-url"),
                data: JSON.stringify({ ids: ids }),
                contentType: 'application/json; charset=utf-8',
            }).done(function (data) {
                if (data == "OK")
                    items.remove();
                else
                    alert("Sorry, an error occurred");
            });

        });

    });





    // gets the path to the image for the requested settings
    function GetImagePath(originalImgSrc, imgSetting) {
        var parts = originalImgSrc.split('/');
        parts.splice(parts.length - 1, 0, imgSetting);
        return parts.join('/');
    }



    // adds sorting to the images
    function AddSorting() {
        $("#previews").sortable({
            stop: function (event, ui) {
                var list = new Array();

                // build an array of all image paths
                $("#previews img").each(function () {
                    var id = $(this).attr("data-id");
                    list.push(id);
                });

                // send to server
                $.ajax({
                    type: "POST",
                    url: $("#previews").attr("data-sort-url"),
                    data: JSON.stringify({ ids: list }),
                    contentType: 'application/json; charset=utf-8',
                    error: function (data) { alert("Error\n\n" + data); },
                    success: function (data) {
                        if (data != "OK")
                            Notify(data, false);
                    }
                });

            }
        });

        $("#previews").disableSelection();
    }


    // provides keyboard functionality
    jQuery.fn.keyboard = function (options) {

        var elem = $(window);

        // setup defaults
        var defaults = {
            ctrlOn: function () { },
            ctrlOff: function () { },
            altOn: function () { },
            altOff: function () { },
            shiftOn: function () { },
            shiftOff: function () { },
            deleteOn: function () { },
            deleteOff: function () { }
        };


        // extend the settings
        var settings = $.extend({}, defaults, options);


        // handles key down event
        elem.keydown(function (event) {
            var code = event.which;

            if (code == 16)
                settings.shiftOn();
            if (code == 17)
                settings.ctrlOn();
            if (code == 18)
                settings.altOn();
            if (code == 46)
                settings.deleteOn();
        });


        // handles focous out event
        elem.focusout(function (event) { });


        // handles key up event
        elem.keyup(function (event) {
            var code = event.which;
            if (code == 16)
                settings.shiftOff();
            if (code == 17)
                settings.ctrlOff();
            if (code == 18)
                settings.altOff();
        });


        // execute commands
        if (typeof (options) == "string") {


        }


    }


    // makes images selectable 
    jQuery.fn.selectableItems = function (options) {

        // default settings            
        var defaults = {
            activeClass: "selected",
            itemSelector: "img",
            itemSelected: function (item) { },
            selectionCleared: function () { }
        };

        var settings = $.extend({}, defaults, options);

        // execute commands
        if (typeof (options) == "string") {

            // get all selected items
            if (options == "GetSelectedItems") {
                return $(this).find("." + settings.activeClass);
            }
        }


        // keyboard detection
        var ctrlDown = false;
        var shiftDown = false;
        jQuery.fn.keyboard({
            ctrlOn: function () { ctrlDown = true; },
            ctrlOff: function () { ctrlDown = false; },
            shiftOn: function () { shiftDown = true; },
            shiftOff: function () { shiftDown = false; },
            deleteOn: function () { $("#DeleteImages").click(); }
        });


        // apply selectable Items to each element in the array
        return $(this).each(function () {

            var elem = $(this);
            var lastClicked = null;

            // unselect all items
            function ClearSelection() {
                elem.find("." + settings.activeClass).removeClass(settings.activeClass);
            }

            // clears the selection when 
            
            $(window).click(function(){
                ClearSelection();
                lastClicked = null;
                settings.selectionCleared();
            });
            


            // when the user clicks an image
            elem.on("click", function (event) {

                // get the item from the event (avoids rebind issues for dynamically added items)
                var item = $(event.target);
                if (item[0].tagName != "IMG")
                    return;

                // don't bubble up events on images
                event.stopPropagation();

                // if ctrl is not pressed then clear selection
                if (!ctrlDown && !shiftDown)
                    ClearSelection();

                // add the item to the selection
                if (item.hasClass(settings.activeClass))
                    item.removeClass(settings.activeClass)
                else
                    item.addClass(settings.activeClass);

                // if shift is held down then add all in the range to the selection
                if (shiftDown && lastClicked != null) {
                    var li = lastClicked.index();
                    var ci = item.index();
                    if (li > ci)
                        elem.find(settings.itemSelector).slice(ci, li).addClass(settings.activeClass);
                    else
                        elem.find(settings.itemSelector).slice(li, ci).addClass(settings.activeClass);
                }

                // remember the last item for shift down selections
                lastClicked = item;

                // call the optional callback
                settings.itemSelected(item);
            });




        });



    };



})(jQuery);

