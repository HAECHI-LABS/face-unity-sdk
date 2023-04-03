import AuthenticationServices
import SafariServices
import UIKit

@objc public class WebAuthenticate: NSObject {
    private var unity: UnityFramework? = UnityFramework.getInstance();
    private var bgTaskIdentifier: UIBackgroundTaskIdentifier = .invalid
    private var foregroundNotificationObserver: Any?
    private var backgroundNotificationObserver: Any?
        
    public static let instance = WebAuthenticate();
    public static var authSession: ASWebAuthenticationSession? = nil;
    
    @objc public func call(_ url: String,_ redirectUri: String,_ objectName: String) {
        WebAuthenticate.authSession = ASWebAuthenticationSession(
            url: URL(string: url)!, callbackURLScheme: redirectUri) { callbackURL, authError in
                guard authError == nil, let callbackURL = callbackURL else {
                    return
                }

                self.unity?.sendMessageToGO(withName: objectName, functionName: "onDeepLinkActivated", message: callbackURL.absoluteString);
        }
        
        if #available(iOS 13.0, *) {
            WebAuthenticate.authSession?.presentationContextProvider = self
        }
        
        WebAuthenticate.authSession?.start();
    }
    
    @objc public func add_observers(_ objectName: String) {
        self.backgroundNotificationObserver = NotificationCenter.default.addObserver(
            forName: UIApplication.didEnterBackgroundNotification,
            object: nil,
            queue: OperationQueue.main
        ) { [weak self] _ in
            self?.requestBackgroundExecutionTime();
        }

        self.foregroundNotificationObserver = NotificationCenter.default.addObserver(
            forName: UIApplication.didBecomeActiveNotification,
            object: nil,
            queue: OperationQueue.main
        ) { [weak self] _ in
            self?.endBackgroundExecutionTime()
        }
    }
    
    @objc public func remove_observers() {
        if let observer = self.foregroundNotificationObserver {
            NotificationCenter.default.removeObserver(observer)
        }
        if let observer = self.backgroundNotificationObserver {
            NotificationCenter.default.removeObserver(observer)
        }
    }
    
    @objc func requestBackgroundExecutionTime() {
        if bgTaskIdentifier != .invalid {
           endBackgroundExecutionTime()
        }

        bgTaskIdentifier = UIApplication.shared.beginBackgroundTask() {
            [weak self] in
            self?.endBackgroundExecutionTime()
        }
    }

    @objc func endBackgroundExecutionTime() {
        guard bgTaskIdentifier != .invalid else { return }
        UIApplication.shared.endBackgroundTask(bgTaskIdentifier)
        bgTaskIdentifier = .invalid
    }
    
    @objc public static func launch(_ url: String,_ redirectUri: String,_ objectName: String) {
        instance.call(url, redirectUri, objectName);
    }
    
    @objc public static func expand_background_time(_ objectName: String) {
        instance.add_observers(objectName);
    }
    
    @objc public static func normalize_background_time() {
        instance.remove_observers();
    }
    
    @objc public static func pause() {
        instance.unity?.pause(true);
    }
}


@available(iOS 12.0, *)
extension WebAuthenticate: ASWebAuthenticationPresentationContextProviding {
    public func presentationAnchor(for session: ASWebAuthenticationSession) -> ASPresentationAnchor {
        return UnityFramework.getInstance().appController().window;
    }
}
