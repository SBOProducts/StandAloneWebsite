CKEDITOR.plugins.add('ajaxsave',
{
    init: function (editor) {

        editor.addCommand('ajaxSave',
	    {
	        exec: function (editor) {

	            //if (!confirm("Save these changes?"))
	            //    return;

	            var data = {
	                ID: editor.name.replace("HtmlEditor-", ""),
	                HTML: editor.getData()
	            };

	            var url = '/Content/Update/' + data.ID;


	            // ajax post data
	            jQuery.ajax({
	                url: url,
	                data: data,
	                type: "POST",
	                success: function (data, textStatus, jqXHR) {
	                    Notify("Your changes have been saved", true);
	                    //jQuery("#HideFocus").focus();
	                },
	                error: function (jqXHR, textStatus, errorThrown) {
	                    Notify("An error occurred while saving your changes", false);
	                }
	            });

	        }
	    });

        editor.ui.addButton('Save',
        {
            label: 'Save',
            command: 'ajaxSave',
            icon: this.path + 'images/save.png'
        });

    }
});