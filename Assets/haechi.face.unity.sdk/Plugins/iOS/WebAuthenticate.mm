#import <Foundation/Foundation.h>
#import <AuthenticationServices/ASWebAuthenticationSession.h>
#import <UnityFramework/UnityFramework-Swift.h>

extern "C" {
    void launch_face_webview(const char *url, const char *redirectUri, const char *objectName) {
        [WebAuthenticate launch:[NSString stringWithUTF8String:url] :[NSString stringWithUTF8String:redirectUri] :[NSString stringWithUTF8String:objectName]];
    }
}
