//http: //docs.cksource.com/CKEditor_3.x/Tutorials/SimpleLink_Plugin_Part_1

CKEDITOR.plugins.add('tabs',
{
    init: function (editor) {
        editor.addCommand('tabsDialog', new CKEDITOR.dialogCommand('tabsDialog'));

        editor.ui.addButton('Tabs',
		{
		    label: 'Insert Tabs',
		    command: 'tabsDialog',
		    icon: this.path + 'images/tabs.gif'
		});

        CKEDITOR.dialog.add('tabsDialog', function (editor) {
            return {
                title: 'Tab Properties',
                minWidth: 400,
                minHeight: 200,
                contents:
				[
					{
					    id: 'general',
					    label: 'Settings',
					    elements:
						[
							{
							    type: 'html',
							    html: 'This dialog window lets you create tabs for your website.'
							},
							{
							    type: 'textarea',
							    id: 'titles',
							    label: 'Tab Titles Separated by Commas',
							    validate: CKEDITOR.dialog.validate.notEmpty('The Tab Titles text field cannot be empty.'),
							    required: true,
							    commit: function (data) {
							        data.titles = this.getValue();
							    }
							}
						]
					}
				],
                onOk: function () {
                    var dialog = this,
						data = {};

                    this.commitContent(data);

                    var html = jQuery('<div class="tabs"/>');
                    var ul = jQuery('<ul/>');
                    html.append(ul);

                    var titles = data.titles.split(',');
                    for (var a in titles) {
                        var title = titles[a];
                        var li = jQuery('<li><a href="#tab-' + a + '">' + title + '</a></li>');
                        var div = jQuery('<div id="tab-' + a + '">Enter your content for Tab ' + a + '</div>');
                        ul.append(li);
                        html.append(div);
                    }


                    var container = jQuery("<div/>");
                    container.append(html);
                    editor.insertHtml(container.html());

                    jQuery("#" + editor.name).find(".tabs").tabs();


                }
            };
        });
    }
});