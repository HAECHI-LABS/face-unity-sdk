#import "UnityAppController.h"

@interface OverrideAppDelegate : UnityAppController
@end

IMPL_APP_CONTROLLER_SUBCLASS(OverrideAppDelegate)

@implementation OverrideAppDelegate

NSMutableArray<NSNumber *> *backgroundTaskQueue;

- (void)startUnitySendBackgroundTask {
    if (!backgroundTaskQueue) {
        backgroundTaskQueue = [NSMutableArray array];
    }

    UIBackgroundTaskIdentifier backgroundTaskID = [[UIApplication sharedApplication] beginBackgroundTaskWithExpirationHandler:^{
        // This block will be called if the task is terminated by the system
        [self endBackgroundTask];
    }];

    [backgroundTaskQueue addObject:@(backgroundTaskID)];

    UnityPause(false);
    UnitySendMessage("WalletConnectV1Client", "OnAdditionalFrame", "");

    [self endBackgroundTask];
}

- (void)startBackgroundTask {
    if (!backgroundTaskQueue) {
        backgroundTaskQueue = [NSMutableArray array];
    }

    UIBackgroundTaskIdentifier backgroundTaskID = [[UIApplication sharedApplication] beginBackgroundTaskWithExpirationHandler:^{
        // This block will be called if the task is terminated by the system
        [self endBackgroundTask];
    }];

    [backgroundTaskQueue addObject:@(backgroundTaskID)];

    [self endBackgroundTask];
}

- (void)endBackgroundTask {
    if (backgroundTaskQueue.count > 0) {
        UIBackgroundTaskIdentifier backgroundTaskID = [backgroundTaskQueue firstObject].unsignedIntegerValue;
        [[UIApplication sharedApplication] endBackgroundTask:backgroundTaskID];
        [backgroundTaskQueue removeObjectAtIndex:0];
    }
}

- (void) applicationWillResignActive:(UIApplication *)application
{
    [self startBackgroundTask];
}

- (void) applicationDidEnterBackground:(UIApplication *)application
{
    /*
     * When app goes background, app will be paused so cannot send websocket message to the browser.
     * This background task will enable not pausing the app forcibly and send the message by unity
     * So, user must return to the browser that has opened the dapp page such as OpenSea.
     * After 
     */
    [self startUnitySendBackgroundTask];
}

@end
