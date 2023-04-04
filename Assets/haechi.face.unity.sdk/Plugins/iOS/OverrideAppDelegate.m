#import "UnityAppController.h"

@interface OverrideAppDelegate : UnityAppController
@end

IMPL_APP_CONTROLLER_SUBCLASS(OverrideAppDelegate)

@implementation OverrideAppDelegate

NSMutableArray<NSNumber *> *backgroundTaskQueue;
BOOL isWalletConnectV1 = NO;

- (void) startUnitySendBackgroundTask {
    if (!backgroundTaskQueue) {
        backgroundTaskQueue = [NSMutableArray array];
    }

    UIBackgroundTaskIdentifier backgroundTaskID = [[UIApplication sharedApplication] beginBackgroundTaskWithExpirationHandler:^{
        // This block will be called if the task is terminated by the system
        [self endBackgroundTask];
    }];

    [backgroundTaskQueue addObject:@(backgroundTaskID)];

    UnitySendMessage("WalletConnectV1Client", "OnAdditionalFrame", "");
    [self endBackgroundTask];
}

- (void) endBackgroundTask {
    if (backgroundTaskQueue.count > 0) {
        UIBackgroundTaskIdentifier backgroundTaskID = [backgroundTaskQueue firstObject].unsignedIntegerValue;
        [[UIApplication sharedApplication] endBackgroundTask:backgroundTaskID];
        [backgroundTaskQueue removeObjectAtIndex:0];
    }
}

- (void) applicationWillResignActive:(UIApplication *)application
{
    if (isWalletConnectV1) {
        // Avoid calling super.applicationWillResignActive, as it may result in WalletConnect V1 becoming unavailable.
        UnityPause(false);
    } else {
        [super applicationWillResignActive: application];
    }
}

- (void) applicationDidEnterBackground:(UIApplication *)application
{
    /*
    * When app goes background, app will be paused so cannot send websocket message to the browser.
    * This background task will enable not pausing the app forcibly and send the message by unity
    * So, user must return to the browser that has opened the dapp page such as OpenSea.
    * After
    */
    [super applicationDidEnterBackground: application];
    if (isWalletConnectV1) {
        [self startUnitySendBackgroundTask];
    }
}

- (void) applicationWillEnterForeground:(UIApplication *)application
{
    [super applicationWillEnterForeground: application];
}

- (void) applicationDidBecomeActive:(UIApplication *)application
{
    [super applicationDidBecomeActive: application];
    if (isWalletConnectV1) {
        UnityPause(false);
    }
}

void set_wc_v1_true(const char* objectName)
{
    NSString *objcObjectName = [NSString stringWithUTF8String:objectName];

    if (![objcObjectName isEqualToString:@"WalletConnectV1Client"]) {
        return;
    }
    isWalletConnectV1 = YES;
}

void set_wc_v1_false()
{
    isWalletConnectV1 = NO;
}

@end
