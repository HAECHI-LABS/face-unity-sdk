mergeInto(LibraryManager.library, {
	SignInWithGoogle: function (objectName, callback) {

    var parsedObjectName = UTF8ToString(objectName);
    var parsedCallback = UTF8ToString(callback);

    try {
        google.accounts.id.initialize({
          client_id: '965931844205-q64ebkmn6atutksvi6b0rv2hkdm9gor5.apps.googleusercontent.com',
          callback: (res)=>{
            console.log(res);
            SendMessage(parsedObjectName, parsedCallback, res.credential);
          }
        });
        google.accounts.id.prompt();
    } catch (error) {
        console.log(error);
    }
  }
});
