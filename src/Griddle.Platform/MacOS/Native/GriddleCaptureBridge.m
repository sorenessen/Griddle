#import "GriddleCaptureBridge.h"

#import <AppKit/AppKit.h>
#import <ScreenCaptureKit/ScreenCaptureKit.h>
#import <unistd.h>

static void complete_capture(
    CGImageRef image,
    NSError *error,
    GriddleCaptureCallback callback,
    void *context)
{
    if (error != nil)
    {
        callback(
            NULL,
            0,
            0,
            0,
            error.localizedDescription.UTF8String,
            context);

        return;
    }

    if (image == NULL)
    {
        callback(
            NULL,
            0,
            0,
            0,
            "ScreenCaptureKit returned no image.",
            context);

        return;
    }

    NSBitmapImageRep *bitmap =
        [[NSBitmapImageRep alloc]
            initWithCGImage:image];

    NSData *pngData =
        [bitmap representationUsingType:
            NSBitmapImageFileTypePNG
            properties:@{}];

    if (pngData == nil)
    {
        callback(
            NULL,
            0,
            0,
            0,
            "Unable to encode screenshot as PNG.",
            context);

        return;
    }

    callback(
        pngData.bytes,
        (int32_t)pngData.length,
        (int32_t)CGImageGetWidth(image),
        (int32_t)CGImageGetHeight(image),
        NULL,
        context);
}

void griddle_capture_region(
    int32_t x,
    int32_t y,
    int32_t width,
    int32_t height,
    int32_t includeApplicationWindows,
    GriddleCaptureCallback callback,
    void *context)
{
    if (callback == NULL)
    {
        return;
    }

    CGRect rect =
        CGRectMake(
            x,
            y,
            width,
            height);

    if (@available(macOS 14.0, *))
    {
        if (includeApplicationWindows != 0)
        {
            [SCScreenshotManager
                captureImageInRect:rect
                completionHandler:^(
                    CGImageRef image,
                    NSError *error)
                {
                    complete_capture(
                        image,
                        error,
                        callback,
                        context);
                }];

            return;
        }

        [SCShareableContent
            getShareableContentExcludingDesktopWindows:NO
            onScreenWindowsOnly:NO
            completionHandler:^(
                SCShareableContent *content,
                NSError *error)
            {
                if (error != nil)
                {
                    callback(
                        NULL,
                        0,
                        0,
                        0,
                        error.localizedDescription.UTF8String,
                        context);

                    return;
                }

                CGPoint regionCenter =
                    CGPointMake(
                        CGRectGetMidX(rect),
                        CGRectGetMidY(rect));

                SCDisplay *targetDisplay =
                    nil;

                for (SCDisplay *display
                    in content.displays)
                {
                    CGRect displayBounds =
                        CGDisplayBounds(
                            display.displayID);

                    if (CGRectContainsPoint(
                            displayBounds,
                            regionCenter))
                    {
                        targetDisplay =
                            display;

                        break;
                    }
                }

                if (targetDisplay == nil)
                {
                    callback(
                        NULL,
                        0,
                        0,
                        0,
                        "Could not identify the target display.",
                        context);

                    return;
                }

                SCRunningApplication *griddleApp =
                    nil;

                pid_t currentProcessId =
                    getpid();

                for (SCRunningApplication *application
                    in content.applications)
                {
                    if (application.processID ==
                        currentProcessId)
                    {
                        griddleApp =
                            application;

                        break;
                    }
                }

                NSArray<SCRunningApplication *> *
                    excludedApplications =
                        griddleApp == nil
                            ? @[]
                            : @[griddleApp];

                SCContentFilter *filter =
                    [[SCContentFilter alloc]
                        initWithDisplay:targetDisplay
                        excludingApplications:
                            excludedApplications
                        exceptingWindows:@[]];

                SCStreamConfiguration *configuration =
                    [[SCStreamConfiguration alloc]
                        init];

                CGRect displayBounds =
                    CGDisplayBounds(
                        targetDisplay.displayID);

                configuration.sourceRect =
                    CGRectMake(
                        rect.origin.x -
                            displayBounds.origin.x,
                        rect.origin.y -
                            displayBounds.origin.y,
                        rect.size.width,
                        rect.size.height);

                configuration.width =
                    width;

                configuration.height =
                    height;

                configuration.showsCursor =
                    YES;

                [SCScreenshotManager
                    captureImageWithFilter:filter
                    configuration:configuration
                    completionHandler:^(
                        CGImageRef image,
                        NSError *captureError)
                    {
                        complete_capture(
                            image,
                            captureError,
                            callback,
                            context);
                    }];
            }];
    }
    else
    {
        callback(
            NULL,
            0,
            0,
            0,
            "Screen capture requires macOS 14 or later.",
            context);
    }
}