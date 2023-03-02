#import <Foundation/Foundation.h>
#import <AuthenticationServices/ASWebAuthenticationSession.h>
#import <UnityFramework/UnityFramework-Swift.h>

extern "C" {
    void launch_face_webview(const char *url, const char *redirectUri, const char *objectName) {
        [WebAuthenticate launch:[NSString stringWithUTF8String:url] :[NSString stringWithUTF8String:redirectUri] :[NSString stringWithUTF8String:objectName]];
    }
    
    void expand_background_time(const char *objectName) {
        [WebAuthenticate expand_background_time :[NSString stringWithUTF8String:objectName]];
    }
    
    void normalize_background_time() {
        [WebAuthenticate normalize_background_time];
    }
    
    void pause_unity() {
        [WebAuthenticate pause];
    }
}
