#import "GriddleCaptureBridge.h"

#import <AppKit/AppKit.h>
#import <ScreenCaptureKit/ScreenCaptureKit.h>

void griddle_capture_region(
    int32_t x,
    int32_t y,
    int32_t width,
    int32_t height,
    GriddleCaptureCallback callback,
    void *context)
{
    if (callback == NULL)
    {
        return;
    }

    CGRect rect = CGRectMake(
        x,
        y,
        width,
        height);

    if (@available(macOS 14.0, *))
    {
        [SCScreenshotManager
            captureImageInRect:rect
            completionHandler:^(
                CGImageRef image,
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