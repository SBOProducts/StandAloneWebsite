CKEDITOR.plugins.add('ajaxcancel',
{
    init: function (editor) {

        editor.addCommand('ajaxCancel',
	    {
	        exec: function (editor) {
	            var id = editor.name;
	            var contentId = id.split("-")[1];
	            var data = editor.getData();

	            var hasChanged = jQuery("#" + id).attr("data-changed");


	            // if nothing's changed then just hide editor
	            if (!hasChanged) {
	                //jQuery("#HideFocus").focus();
	                return;
	            }

	            // if something has changed confirm to lose them
	            if (confirm("You have made changes, do you wish to discard them?")) {
	                location.reload();
	            }
	        }
	    });

        editor.ui.addButton('Cancel',
        {
            label: 'Cancel',
            command: 'ajaxCancel',
            icon: this.path + 'images/cancel.png'
        });

    }
});