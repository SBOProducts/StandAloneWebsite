

function Notify(text, successful) {
    
    var container = jQuery("#Notifications");
    var message = jQuery('<div style="display:none;" />');
    message.addClass("message");
    message.addClass(successful ? "success" : "error");
    var timestamp = new Date().getTime();

    message.text(text);
    message.attr("data-time-stamp", timestamp);

    container.append(message);

    message.slideDown(400, function () {
        setTimeout("ClearNotification(" + timestamp + ");", 2000);
    });

}

function ClearNotification(timestamp){
    var message = jQuery("#Notifications div[data-time-stamp='" + timestamp + "']");
    message.slideUp(400);
}



(function ($) {

    $("document").ready(function () {

        /* CONFIRM CLASS: prompt users to verify save and delete etc */
        $(".confirm").click(function () {
            var text = $(this).text();
            if (!confirm(text + " - are you sure?"))
                return false;

            return true;
        });

        /* PAGE LOAD NOTIFICATION: notifies text from the #Message if present */
        var text = $("#Message").val();
        if(text.length > 0){
            Notify(text, true);
        }


        $("form[data-ajax-form], form.data-ajax-form").ajaxForm();
        

    });


})(jQuery);


jQuery.fn.ajaxForm = function () {
    var form = $(this);
    form.on("submit", function(event){
        var form = $(this);
        var data = form.serialize();
        var url = form.attr("action");

        $.ajax({
            data: data,
            url: url,
            type: "POST"
        }).done(function(data){
            form.after(data);
            form.remove();
            $("#ContactFormMessage").fadeOut(5000);
            $("form[data-ajax-form]").ajaxForm();
        });

        return false;
    });

}


// opens & closes a dialog showing activity is taking place
jQuery.showProgress = function (command) {


    if (command == null || command == "" || command == "show")
        return $("#ProgressIndicator").fadeIn(1000);

    if (command == "hide")
        return $("#ProgressIndicator").fadeOut(1000);

}
