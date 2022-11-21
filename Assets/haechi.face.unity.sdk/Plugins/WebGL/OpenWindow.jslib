var OpenWindowPlugin = {
    openWindow: function(link)
    {
        window.open(UTF8ToString(link));
    }
};

mergeInto(LibraryManager.library, OpenWindowPlugin);