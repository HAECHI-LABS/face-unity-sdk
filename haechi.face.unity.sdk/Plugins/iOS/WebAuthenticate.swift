import AuthenticationServices
import SafariServices
import UIKit

@objc public class WebAuthenticate: NSObject {
    private var unity: UnityFramework? = UnityFramework.getInstance();
    public static let instance = WebAuthenticate();
    public static var authSession: ASWebAuthenticationSession? = nil;
    
    @objc public func call(_ url: String,_ redirectUri: String,_ objectName: String) {
        WebAuthenticate.authSession = ASWebAuthenticationSession(
            url: URL(string: url)!, callbackURLScheme: redirectUri) { callbackURL, authError in
                if let authError = authError as? ASWebAuthenticationSessionError, authError.code == .canceledLogin, callbackURL == nil {
                    self.unity?.sendMessageToGO(withName: objectName, functionName: "OnWebviewCanceled", message: "")
                } else if let callbackURL = callbackURL, authError == nil {
                    self.unity?.sendMessageToGO(withName: objectName, functionName: "OnDeepLinkActivated", message: callbackURL.absoluteString)
                } else {
                    print("authError: \(authError)")
                    return
                }
        }
        
        if #available(iOS 13.0, *) {
            WebAuthenticate.authSession?.presentationContextProvider = self
        }
        
        WebAuthenticate.authSession?.start();
    }
    
    @objc public static func launch(_ url: String,_ redirectUri: String,_ objectName: String) {
        instance.call(url, redirectUri, objectName);
    }
}


@available(iOS 12.0, *)
extension WebAuthenticate: ASWebAuthenticationPresentationContextProviding {
    public func presentationAnchor(for session: ASWebAuthenticationSession) -> ASPresentationAnchor {
        return UnityFramework.getInstance().appController().window;
    }
}
