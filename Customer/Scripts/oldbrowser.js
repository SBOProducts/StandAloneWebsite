// this script should only be included if the browser doesn't support media queries

function setCookie(c_name, value, exdays) {
	var exdate = new Date();
	exdate.setDate(exdate.getDate() + exdays);
	var c_value = escape(value) + ((exdays == null) ? "" : "; expires=" + exdate.toUTCString());
	document.cookie = c_name + "=" + c_value;
}

function getCookie(c_name) {
	var c_value = document.cookie;
	var c_start = c_value.indexOf(" " + c_name + "=");
	if (c_start == -1) {
		c_start = c_value.indexOf(c_name + "=");
	}
	if (c_start == -1) {
		c_value = null;
	}
	else {
		c_start = c_value.indexOf("=", c_start) + 1;
		var c_end = c_value.indexOf(";", c_start);
		if (c_end == -1) {
			c_end = c_value.length;
		}
		c_value = unescape(c_value.substring(c_start, c_end));
	}
	return c_value;
}

var cookie = getCookie("notified");
if (cookie == null) {
	
	if (jQuery) {
		var div = jQuery("<div><p style='font-size:10pt;'>The browser you are using does not support many features of this website. For best viewing experience please consider upgrading to a modern browser.</p> <p><a href='http://www.google.com/chrome'><img src='/content/images/chrome.png' alt='Google Chrome'/>Google Chrome</a></p> <p><a href='http://downloads.yahoo.com/firefox/'><img src='/content/images/firefox.png' alt='Firefox'/> Firefox</a></p> <p><a href='http://windows.microsoft.com/en-us/internet-explorer/download-ie'><img src='/content/images/ie.png' alt='Internet Explorer' />Internet Explorer</a></p> </div>");
		jQuery("body").append(div);
		div.dialog({ width: 400, modal: true, title: "Outdated Browser Detected", buttons: { "close": function () { jQuery(this).dialog("close"); } } });
	} else {
		alert("The browser you are using does not support many features of this website. For best viewing experience please consider upgrading to a modern browser such as Google Chrome, Mozilla Firefox, or Microsoft Internet Explorer.");
	}
	setCookie("notified", "true", 1);
}
