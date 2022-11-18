var OpenWindowPlugin = {
    openWindow: function(link)
    {
    	var url = UTF8ToString(link);
        document.onmouseup = function()
        {
        	window.open(url);
        	document.onmouseup = null;
        }
    }
};

mergeInto(LibraryManager.library, OpenWindowPlugin);