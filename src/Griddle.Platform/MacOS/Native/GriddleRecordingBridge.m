#import "GriddleRecordingBridge.h"

#import <AppKit/AppKit.h>
#import <AVFoundation/AVFoundation.h>
#import <CoreMedia/CoreMedia.h>
#import <CoreVideo/CoreVideo.h>
#import <ScreenCaptureKit/ScreenCaptureKit.h>
#import <math.h>
#import <unistd.h>
#import <string.h>


static BOOL GriddleRecordingActive = NO;

static uint64_t
    GriddleScreenFrameCount = 0;

static BOOL
    GriddleLoggedPixelBufferFormat = NO;

static SCStream *GriddleRecordingStream = nil;

static id GriddleRecordingStreamHandler = nil;

static dispatch_queue_t GriddleRecordingQueue = nil;

static SCRecordingOutput *GriddleRecordingOutput = nil;

static BOOL
    GriddleDiagnosticRawStreamOnly = YES;

static id GriddleRecordingOutputDelegate = nil;

static AVAssetWriter *
    GriddleAssetWriter = nil;

static AVAssetWriterInput *
    GriddleVideoWriterInput = nil;

static AVAssetWriterInput *
    GriddleMicrophoneWriterInput = nil;

static AVAssetWriterInputPixelBufferAdaptor *
    GriddleVideoPixelBufferAdaptor = nil;

static BOOL
    GriddleAssetWriterSessionStarted = NO;

static CMTime
    GriddleAssetWriterStartTime;

static CMTime
    GriddleAssetWriterLastVideoTime;

static GriddleRecordingStopCallback
    GriddlePendingStopCallback = NULL;

static void *
    GriddlePendingStopContext = NULL;

static id
    GriddleMicrophoneDisconnectObserver = nil;

static NSString *
    GriddleActiveMicrophoneDeviceId = nil;

static GriddleMicrophoneDisconnectedCallback
    GriddleMicrophoneDisconnectedHandler = NULL;

static void *
    GriddleMicrophoneDisconnectedContext = NULL;

static void clear_recording_state(void)
{
    if (GriddleMicrophoneDisconnectObserver != nil)
    {
        [[NSNotificationCenter defaultCenter]
            removeObserver:
                GriddleMicrophoneDisconnectObserver];

        GriddleMicrophoneDisconnectObserver =
            nil;
    }

    GriddleActiveMicrophoneDeviceId =
        nil;

    GriddleMicrophoneDisconnectedHandler =
        NULL;

    GriddleMicrophoneDisconnectedContext =
        NULL;

    GriddleAssetWriter =
    nil;

    GriddleVideoWriterInput =
        nil;

    GriddleMicrophoneWriterInput =
        nil;

    GriddleVideoPixelBufferAdaptor =
        nil;

    GriddleAssetWriterSessionStarted =
        NO;

    GriddleAssetWriterStartTime =
        kCMTimeInvalid;

    GriddleAssetWriterLastVideoTime =
        kCMTimeInvalid;

    GriddleRecordingActive = NO;

    GriddleScreenFrameCount = 0;

    GriddleLoggedPixelBufferFormat =
        NO;

    GriddleRecordingStream = nil;

    GriddleRecordingStreamHandler = nil;

    GriddleRecordingQueue = nil;

    GriddleRecordingOutput = nil;

    GriddleRecordingOutputDelegate = nil;
}


static void clear_pending_stop(void)
{
    GriddlePendingStopCallback = NULL;

    GriddlePendingStopContext = NULL;
}


@interface GriddleStreamHandler
    : NSObject <SCStreamOutput, SCStreamDelegate>
@end


@interface GriddleRecordingOutputHandler
    : NSObject <SCRecordingOutputDelegate>
@end


@implementation GriddleStreamHandler

- (void)stream:(SCStream *)stream
    didOutputSampleBuffer:(CMSampleBufferRef)sampleBuffer
    ofType:(SCStreamOutputType)type
{
    (void)stream;

    if (type == SCStreamOutputTypeMicrophone)
    {
        if (!GriddleAssetWriterSessionStarted ||
            GriddleMicrophoneWriterInput == nil)
        {
            return;
        }

        if (GriddleMicrophoneWriterInput
                .readyForMoreMediaData)
        {
            if (![GriddleMicrophoneWriterInput
                    appendSampleBuffer:
                        sampleBuffer])
            {
                NSLog(
                    @"Griddle microphone append failed -- status=%ld error=%@",
                    (long)GriddleAssetWriter.status,
                    GriddleAssetWriter.error);
            }
        }

        return;
    }

    if (type != SCStreamOutputTypeScreen)
    {
        return;
    }

    GriddleScreenFrameCount++;

    CFArrayRef attachmentsArray =
        CMSampleBufferGetSampleAttachmentsArray(
            sampleBuffer,
            false);

    NSInteger frameStatus =
        -1;

    if (attachmentsArray != NULL &&
        CFArrayGetCount(attachmentsArray) > 0)
    {
        CFDictionaryRef attachments =
            CFArrayGetValueAtIndex(
                attachmentsArray,
                0);

        CFNumberRef statusNumber =
            CFDictionaryGetValue(
                attachments,
                (__bridge const void *)
                    SCStreamFrameInfoStatus);

        if (statusNumber != NULL)
        {
            CFNumberGetValue(
                statusNumber,
                kCFNumberNSIntegerType,
                &frameStatus);
        }
    }

    CMTime presentationTime =
        CMSampleBufferGetPresentationTimeStamp(
            sampleBuffer);

    NSLog(
        @"Griddle screen frame #%llu status=%ld pts=%.3f",
        GriddleScreenFrameCount,
        (long)frameStatus,
        CMTimeGetSeconds(
            presentationTime));

    if (frameStatus != SCFrameStatusComplete)
    {
        return;
    }

    if (!GriddleLoggedPixelBufferFormat)
    {
        CVImageBufferRef imageBuffer =
            CMSampleBufferGetImageBuffer(
                sampleBuffer);

        if (imageBuffer != NULL)
        {
            OSType pixelFormat =
                CVPixelBufferGetPixelFormatType(
                    imageBuffer);

            size_t pixelWidth =
                CVPixelBufferGetWidth(
                    imageBuffer);

            size_t pixelHeight =
                CVPixelBufferGetHeight(
                    imageBuffer);

            char formatString[5] =
            {
                (char)((pixelFormat >> 24) & 0xff),
                (char)((pixelFormat >> 16) & 0xff),
                (char)((pixelFormat >> 8) & 0xff),
                (char)(pixelFormat & 0xff),
                '\0'
            };

            NSLog(
                @"Griddle source pixel buffer -- format=%s (%u) size=%zux%zu planes=%zu",
                formatString,
                (unsigned int)pixelFormat,
                pixelWidth,
                pixelHeight,
                CVPixelBufferGetPlaneCount(
                    imageBuffer));

            GriddleLoggedPixelBufferFormat =
                YES;
        }
    }

    if (!GriddleAssetWriterSessionStarted)
    {
        if (![GriddleAssetWriter startWriting])
        {
            NSLog(
                @"Griddle AVAssetWriter failed to start: %@",
                GriddleAssetWriter.error);

            return;
        }

        GriddleAssetWriterStartTime =
            presentationTime;

        [GriddleAssetWriter
            startSessionAtSourceTime:
                GriddleAssetWriterStartTime];

        GriddleAssetWriterSessionStarted =
            YES;

        NSLog(
            @"Griddle writer started -- status=%ld ready=%d pool=%p error=%@",
            (long)GriddleAssetWriter.status,
            GriddleVideoWriterInput.readyForMoreMediaData
                ? 1
                : 0,
            GriddleVideoPixelBufferAdaptor.pixelBufferPool,
            GriddleAssetWriter.error);
    }

    // if (GriddleVideoWriterInput.readyForMoreMediaData)
    // {
    //     if (![GriddleVideoWriterInput
    //             appendSampleBuffer:
    //                 sampleBuffer])
    //     {
    //         NSLog(
    //             @"Griddle AVAssetWriter video append failed: %@",
    //             GriddleAssetWriter.error);
    //     }
    //     else
    //     {
    //         GriddleAssetWriterLastVideoTime =
    //             presentationTime;
    //     }
    // }

    if (!GriddleVideoWriterInput.readyForMoreMediaData)
    {
        NSLog(
            @"Griddle writer input not ready -- status=%ld error=%@",
            (long)GriddleAssetWriter.status,
            GriddleAssetWriter.error);

        return;
    }

    CVPixelBufferPoolRef pixelBufferPool =
        GriddleVideoPixelBufferAdaptor.pixelBufferPool;

    if (pixelBufferPool == NULL)
    {
        NSLog(
            @"Griddle writer pixel buffer pool is NULL -- status=%ld error=%@",
            (long)GriddleAssetWriter.status,
            GriddleAssetWriter.error);

        return;
    }

    CVImageBufferRef sourceBuffer =
        CMSampleBufferGetImageBuffer(
            sampleBuffer);

    if (sourceBuffer == NULL)
    {
        NSLog(
            @"Griddle screen sample did not contain an image buffer.");

        return;
    }

    CVPixelBufferRef destinationBuffer =
        NULL;

    CVReturn createResult =
        CVPixelBufferPoolCreatePixelBuffer(
            kCFAllocatorDefault,
            pixelBufferPool,
            &destinationBuffer);

    if (createResult != kCVReturnSuccess ||
        destinationBuffer == NULL)
    {
        NSLog(
            @"Griddle could not allocate writer pixel buffer: %d",
            createResult);

        return;
    }

    CVReturn sourceLockResult =
        CVPixelBufferLockBaseAddress(
            sourceBuffer,
            kCVPixelBufferLock_ReadOnly);

    CVReturn destinationLockResult =
        CVPixelBufferLockBaseAddress(
            destinationBuffer,
            0);

    BOOL copySucceeded =
        sourceLockResult == kCVReturnSuccess &&
        destinationLockResult == kCVReturnSuccess;

    if (copySucceeded)
    {
        size_t sourcePlaneCount =
            CVPixelBufferGetPlaneCount(
                sourceBuffer);

        size_t destinationPlaneCount =
            CVPixelBufferGetPlaneCount(
                destinationBuffer);

        copySucceeded =
            sourcePlaneCount ==
            destinationPlaneCount;

        if (!copySucceeded)
        {
            NSLog(
                @"Griddle pixel buffer plane mismatch -- source=%zu destination=%zu",
                sourcePlaneCount,
                destinationPlaneCount);
        }
        else
        {
            for (size_t plane = 0;
                plane < sourcePlaneCount;
                plane++)
            {
                uint8_t *source =
                    CVPixelBufferGetBaseAddressOfPlane(
                        sourceBuffer,
                        plane);

                uint8_t *destination =
                    CVPixelBufferGetBaseAddressOfPlane(
                        destinationBuffer,
                        plane);

                size_t sourceBytesPerRow =
                    CVPixelBufferGetBytesPerRowOfPlane(
                        sourceBuffer,
                        plane);

                size_t destinationBytesPerRow =
                    CVPixelBufferGetBytesPerRowOfPlane(
                        destinationBuffer,
                        plane);

                size_t sourceHeight =
                    CVPixelBufferGetHeightOfPlane(
                        sourceBuffer,
                        plane);

                size_t destinationHeight =
                    CVPixelBufferGetHeightOfPlane(
                        destinationBuffer,
                        plane);

                if (source == NULL ||
                    destination == NULL ||
                    sourceHeight != destinationHeight)
                {
                    copySucceeded =
                        NO;

                    NSLog(
                        @"Griddle pixel buffer plane copy mismatch -- plane=%zu sourceHeight=%zu destinationHeight=%zu",
                        plane,
                        sourceHeight,
                        destinationHeight);

                    break;
                }

                size_t bytesToCopy =
                    MIN(
                        sourceBytesPerRow,
                        destinationBytesPerRow);

                for (size_t row = 0;
                    row < sourceHeight;
                    row++)
                {
                    memcpy(
                        destination +
                            (row *
                                destinationBytesPerRow),
                        source +
                            (row *
                                sourceBytesPerRow),
                        bytesToCopy);
                }
            }
        }
    }
    else
    {
        NSLog(
            @"Griddle pixel buffer lock failed -- source=%d destination=%d",
            sourceLockResult,
            destinationLockResult);
    }

    if (destinationLockResult == kCVReturnSuccess)
    {
        CVPixelBufferUnlockBaseAddress(
            destinationBuffer,
            0);
    }

    if (sourceLockResult == kCVReturnSuccess)
    {
        CVPixelBufferUnlockBaseAddress(
            sourceBuffer,
            kCVPixelBufferLock_ReadOnly);
    }

    if (copySucceeded)
    {
        BOOL appended =
            [GriddleVideoPixelBufferAdaptor
                appendPixelBuffer:
                    destinationBuffer
                withPresentationTime:
                    presentationTime];

        if (appended)
        {
            GriddleAssetWriterLastVideoTime =
                presentationTime;
        }
        else
        {
            NSLog(
                @"Griddle copied video frame append failed -- status=%ld error=%@",
                (long)GriddleAssetWriter.status,
                GriddleAssetWriter.error);
        }
    }

    CVPixelBufferRelease(
        destinationBuffer);
}

- (void)stream:(SCStream *)stream
    didStopWithError:(NSError *)error
{
    (void)stream;

    if (error == nil)
    {
        return;
    }

    NSLog(
        @"Griddle recording stream stopped with error: %@",
        error);

    GriddleRecordingStopCallback callback =
        GriddlePendingStopCallback;

    void *context =
        GriddlePendingStopContext;

    clear_pending_stop();

    clear_recording_state();

    if (callback != NULL)
    {
        callback(
            0.0,
            error.localizedDescription.UTF8String,
            context);
    }
}

@end


@implementation GriddleRecordingOutputHandler

- (void)recordingOutputDidStartRecording:
    (SCRecordingOutput *)recordingOutput
{
    (void)recordingOutput;
}


- (void)recordingOutputDidFinishRecording:
    (SCRecordingOutput *)recordingOutput
{
    double durationSeconds =
        CMTimeGetSeconds(
            recordingOutput.recordedDuration);

    if (!isfinite(durationSeconds) ||
        durationSeconds < 0.0)
    {
        durationSeconds =
            0.0;
    }

    GriddleRecordingStopCallback callback =
        GriddlePendingStopCallback;

    void *context =
        GriddlePendingStopContext;

    clear_pending_stop();

    clear_recording_state();

    if (callback != NULL)
    {
        callback(
            durationSeconds,
            NULL,
            context);
    }
}


- (void)recordingOutput:
    (SCRecordingOutput *)recordingOutput
    didFailWithError:(NSError *)error
{
    (void)recordingOutput;

    NSLog(
        @"Griddle recording output failed: %@",
        error);

    GriddleRecordingStopCallback callback =
        GriddlePendingStopCallback;

    void *context =
        GriddlePendingStopContext;

    clear_pending_stop();

    clear_recording_state();

    if (callback != NULL)
    {
        callback(
            0.0,
            error.localizedDescription.UTF8String,
            context);
    }
}

@end

void griddle_request_screen_access(
    GriddleScreenPermissionCallback callback,
    void *context)
{
    if (callback == NULL)
    {
        return;
    }

    if (CGPreflightScreenCaptureAccess())
    {
        callback(
            1,
            context);

        return;
    }

    BOOL granted =
        CGRequestScreenCaptureAccess();

    callback(
        granted ? 1 : 0,
        context);
}

void griddle_request_microphone_access(
    GriddleMicrophonePermissionCallback callback,
    void *context)
{
    if (callback == NULL)
    {
        return;
    }

    AVAuthorizationStatus status =
        [AVCaptureDevice
            authorizationStatusForMediaType:
                AVMediaTypeAudio];

    switch (status)
    {
        case AVAuthorizationStatusAuthorized:
        {
            callback(
                1,
                NULL,
                context);

            return;
        }

        case AVAuthorizationStatusDenied:
        {
            callback(
                0,
                "Microphone access was denied.",
                context);

            return;
        }

        case AVAuthorizationStatusRestricted:
        {
            callback(
                0,
                "Microphone access is restricted.",
                context);

            return;
        }

        case AVAuthorizationStatusNotDetermined:
        {
            [AVCaptureDevice
                requestAccessForMediaType:
                    AVMediaTypeAudio
                completionHandler:^(
                    BOOL granted)
                {
                    if (granted)
                    {
                        callback(
                            1,
                            NULL,
                            context);
                    }
                    else
                    {
                        callback(
                            0,
                            "Microphone access was denied.",
                            context);
                    }
                }];

            return;
        }
    }

    callback(
        0,
        "Unable to determine microphone permission status.",
        context);
}

void griddle_get_microphone_devices(
    GriddleMicrophoneDevicesCallback callback,
    void *context)
{
    if (callback == NULL)
    {
        return;
    }

    AVCaptureDeviceDiscoverySession *session =
        [AVCaptureDeviceDiscoverySession
            discoverySessionWithDeviceTypes:
                @[AVCaptureDeviceTypeMicrophone]
            mediaType:
                AVMediaTypeAudio
            position:
                AVCaptureDevicePositionUnspecified];

    NSMutableArray *devices =
        [NSMutableArray array];

    for (AVCaptureDevice *device in session.devices)
    {
        [devices addObject:
            @{
                @"id": device.uniqueID,
                @"name": device.localizedName
            }];
    }

    NSError *error =
        nil;

    NSData *jsonData =
        [NSJSONSerialization
            dataWithJSONObject:devices
            options:0
            error:&error];

    if (error != nil ||
        jsonData == nil)
    {
        callback(
            "[]",
            context);

        return;
    }

    NSString *json =
        [[NSString alloc]
            initWithData:jsonData
            encoding:NSUTF8StringEncoding];

    callback(
        json.UTF8String,
        context);
}

void griddle_recording_start(
    int32_t x,
    int32_t y,
    int32_t width,
    int32_t height,
    int32_t includeApplicationWindows,
    int32_t captureSystemAudio,
    int32_t captureMicrophone,
    const char *microphoneDeviceId,
    int32_t framesPerSecond,
    const char *outputFilePath,
    GriddleRecordingCallback callback,
    void *context,
    GriddleMicrophoneDisconnectedCallback microphoneDisconnectedCallback,
    void *microphoneDisconnectedContext)
{
    if (callback == NULL)
    {
        return;
    }

    if (GriddleRecordingActive ||
        GriddleRecordingStream != nil)
    {
        callback(
            "A screen recording is already active.",
            context);

        return;
    }

    if (outputFilePath == NULL)
    {
        callback(
            "Recording output path is required.",
            context);

        return;
    }

    /*
     * Copy the P/Invoke string before entering
     * asynchronous native work.
     */
    NSString *requestedOutputPath =
        [[NSString alloc]
            initWithUTF8String:
                outputFilePath];

    if (requestedOutputPath == nil ||
        requestedOutputPath.length == 0)
    {
        callback(
            "Recording output path is invalid.",
            context);

        return;
    }

    NSString *requestedMicrophoneDeviceId =
        nil;

    if (microphoneDeviceId != NULL &&
        microphoneDeviceId[0] != '\0')
    {
        requestedMicrophoneDeviceId =
            [[NSString alloc]
                initWithUTF8String:
                    microphoneDeviceId];
    }

    if (@available(macOS 15.0, *))
    {
        CGRect rect =
            CGRectMake(
                x,
                y,
                width,
                height);

        /*
         * H.264 requires even frame dimensions. The built-in Retina
         * display can report an odd point height (for example 1117).
         * Keep the full sourceRect, but have ScreenCaptureKit scale the
         * stream output by at most one pixel so the encoder receives
         * legal dimensions. External 1280x720 displays are unchanged.
         */
        int32_t encodedWidth =
            width > 1
                ? (width & ~1)
                : width;

        int32_t encodedHeight =
            height > 1
                ? (height & ~1)
                : height;

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
                        "Could not identify the target display.",
                        context);

                    return;
                }

                NSLog(
                    @"Griddle target display -- id=%u bounds=(%.0f, %.0f, %.0f, %.0f)",
                    targetDisplay.displayID,
                    CGDisplayBounds(
                        targetDisplay.displayID).origin.x,
                    CGDisplayBounds(
                        targetDisplay.displayID).origin.y,
                    CGDisplayBounds(
                        targetDisplay.displayID).size.width,
                    CGDisplayBounds(
                        targetDisplay.displayID).size.height);

                NSArray<SCRunningApplication *> *
                    excludedApplications =
                        @[];

                if (includeApplicationWindows == 0)
                {
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

                    if (griddleApp != nil)
                    {
                        excludedApplications =
                            @[griddleApp];
                    }
                }

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

                NSLog(
                    @"Griddle recording geometry -- "
                     "request=(%.0f, %.0f, %.0f, %.0f) "
                     "displayBounds=(%.0f, %.0f, %.0f, %.0f) "
                     "filterContentRect=(%.0f, %.0f, %.0f, %.0f) "
                     "pointPixelScale=%.3f",
                    rect.origin.x,
                    rect.origin.y,
                    rect.size.width,
                    rect.size.height,
                    displayBounds.origin.x,
                    displayBounds.origin.y,
                    displayBounds.size.width,
                    displayBounds.size.height,
                    filter.contentRect.origin.x,
                    filter.contentRect.origin.y,
                    filter.contentRect.size.width,
                    filter.contentRect.size.height,
                    filter.pointPixelScale);

                configuration.sourceRect =
                    CGRectMake(
                        rect.origin.x -
                            displayBounds.origin.x,
                        rect.origin.y -
                            displayBounds.origin.y,
                        rect.size.width,
                        rect.size.height);

                configuration.width =
                    encodedWidth;

                configuration.height =
                    encodedHeight;

                NSLog(
                    @"Griddle encoder dimensions -- source=%dx%d encoded=%dx%d",
                    width,
                    height,
                    encodedWidth,
                    encodedHeight);

                configuration.showsCursor =
                    YES;

                configuration.capturesAudio =
                    captureSystemAudio != 0;

                configuration.captureMicrophone =
                    captureMicrophone != 0;

                if (requestedMicrophoneDeviceId != nil &&
                    requestedMicrophoneDeviceId.length > 0)
                {
                    configuration.microphoneCaptureDeviceID =
                        requestedMicrophoneDeviceId;
                }

                int32_t effectiveFramesPerSecond =
                    framesPerSecond > 0
                        ? framesPerSecond
                        : 30;

                configuration.minimumFrameInterval =
                    CMTimeMake(
                        1,
                        effectiveFramesPerSecond);

                configuration.queueDepth =
                    3;

                GriddleStreamHandler *streamHandler =
                    [[GriddleStreamHandler alloc]
                        init];

                SCStream *stream =
                    [[SCStream alloc]
                        initWithFilter:filter
                        configuration:configuration
                        delegate:streamHandler];

                dispatch_queue_t queue =
                    dispatch_queue_create(
                        "com.griddle.recording.screen",
                        DISPATCH_QUEUE_SERIAL);

                NSError *streamOutputError =
                    nil;

                BOOL streamOutputAdded =
                    [stream
                        addStreamOutput:streamHandler
                        type:SCStreamOutputTypeScreen
                        sampleHandlerQueue:queue
                        error:&streamOutputError];

                if (!streamOutputAdded)
                {
                    callback(
                        streamOutputError
                            .localizedDescription
                            .UTF8String,
                        context);

                    return;
                }

                if (captureSystemAudio != 0)
                {
                    NSError *audioOutputError =
                        nil;

                    BOOL audioOutputAdded =
                        [stream
                            addStreamOutput:streamHandler
                            type:SCStreamOutputTypeAudio
                            sampleHandlerQueue:queue
                            error:&audioOutputError];

                    if (!audioOutputAdded)
                    {
                        callback(
                            audioOutputError
                                .localizedDescription
                                .UTF8String,
                            context);

                        return;
                    }
                }

                if (captureMicrophone != 0)
                {
                    NSError *microphoneOutputError =
                        nil;

                    BOOL microphoneOutputAdded =
                        [stream
                            addStreamOutput:streamHandler
                            type:SCStreamOutputTypeMicrophone
                            sampleHandlerQueue:queue
                            error:&microphoneOutputError];

                    if (!microphoneOutputAdded)
                    {
                        callback(
                            microphoneOutputError
                                .localizedDescription
                                .UTF8String,
                            context);

                        return;
                    }
                }

                if (captureMicrophone != 0 &&
                    requestedMicrophoneDeviceId != nil)
                {
                    GriddleActiveMicrophoneDeviceId =
                        [requestedMicrophoneDeviceId copy];

                    GriddleMicrophoneDisconnectedHandler =
                        microphoneDisconnectedCallback;

                    GriddleMicrophoneDisconnectedContext =
                        microphoneDisconnectedContext;

                    GriddleMicrophoneDisconnectObserver =
                        [[NSNotificationCenter defaultCenter]
                            addObserverForName:
                                AVCaptureDeviceWasDisconnectedNotification
                            object:nil
                            queue:
                                [NSOperationQueue mainQueue]
                            usingBlock:^(
                                NSNotification *notification)
                            {
                                AVCaptureDevice *device =
                                    (AVCaptureDevice *)
                                        notification.object;

                                if (device == nil)
                                {
                                    return;
                                }

                                if (![device.uniqueID
                                        isEqualToString:
                                            GriddleActiveMicrophoneDeviceId])
                                {
                                    return;
                                }

                                NSLog(
                                    @"Griddle active microphone disconnected: %@ (%@)",
                                    device.localizedName,
                                    device.uniqueID);

                                if (GriddleMicrophoneDisconnectedHandler != NULL)
                                {
                                    GriddleMicrophoneDisconnectedHandler(
                                        device.uniqueID.UTF8String,
                                        device.localizedName.UTF8String,
                                        GriddleMicrophoneDisconnectedContext);
                                }
                            }];
                }

                NSURL *outputURL =
                    [NSURL fileURLWithPath:
                        requestedOutputPath];

                NSError *assetWriterError =
                    nil;

                GriddleAssetWriter =
                    [[AVAssetWriter alloc]
                        initWithURL:outputURL
                        fileType:AVFileTypeMPEG4
                        error:&assetWriterError];

                if (GriddleAssetWriter == nil ||
                    assetWriterError != nil)
                {
                    callback(
                        assetWriterError
                            .localizedDescription
                            .UTF8String,
                        context);

                    return;
                }

                NSDictionary *videoSettings =
                    @{
                        AVVideoCodecKey:
                            AVVideoCodecTypeH264,

                        AVVideoWidthKey:
                            @(encodedWidth),

                        AVVideoHeightKey:
                            @(encodedHeight)
                    };

                GriddleVideoWriterInput =
                    [[AVAssetWriterInput alloc]
                        initWithMediaType:
                            AVMediaTypeVideo
                        outputSettings:
                            videoSettings];

                GriddleVideoWriterInput.expectsMediaDataInRealTime =
                    YES;

                NSDictionary *pixelBufferAttributes =
                    @{
                        (NSString *)
                            kCVPixelBufferPixelFormatTypeKey:
                                @(kCVPixelFormatType_420YpCbCr8BiPlanarVideoRange),

                        (NSString *)
                            kCVPixelBufferWidthKey:
                                @(encodedWidth),

                        (NSString *)
                            kCVPixelBufferHeightKey:
                                @(encodedHeight),

                        (NSString *)
                            kCVPixelBufferIOSurfacePropertiesKey:
                                @{}
                    };

                GriddleVideoPixelBufferAdaptor =
                    [[AVAssetWriterInputPixelBufferAdaptor alloc]
                        initWithAssetWriterInput:
                            GriddleVideoWriterInput
                        sourcePixelBufferAttributes:
                            pixelBufferAttributes];

                if (![GriddleAssetWriter
                        canAddInput:
                            GriddleVideoWriterInput])
                {
                    callback(
                        "Could not add video input to AVAssetWriter.",
                        context);

                    return;
                }

                [GriddleAssetWriter
                    addInput:
                        GriddleVideoWriterInput];

                if (captureMicrophone != 0)
                {
                    AudioChannelLayout channelLayout =
                        {0};

                    channelLayout.mChannelLayoutTag =
                        kAudioChannelLayoutTag_Mono;

                    NSData *channelLayoutData =
                        [NSData
                            dataWithBytes:
                                &channelLayout
                            length:
                                sizeof(channelLayout)];

                    NSDictionary *microphoneSettings =
                        @{
                            AVFormatIDKey:
                                @(kAudioFormatMPEG4AAC),

                            AVSampleRateKey:
                                @48000,

                            AVNumberOfChannelsKey:
                                @1,

                            AVEncoderBitRateKey:
                                @128000,

                            AVChannelLayoutKey:
                                channelLayoutData
                        };

                    GriddleMicrophoneWriterInput =
                        [[AVAssetWriterInput alloc]
                            initWithMediaType:
                                AVMediaTypeAudio
                            outputSettings:
                                microphoneSettings];

                    GriddleMicrophoneWriterInput
                        .expectsMediaDataInRealTime =
                            YES;

                    if (![GriddleAssetWriter
                            canAddInput:
                                GriddleMicrophoneWriterInput])
                    {
                        callback(
                            "Could not add microphone input to AVAssetWriter.",
                            context);

                        return;
                    }

                    [GriddleAssetWriter
                        addInput:
                            GriddleMicrophoneWriterInput];
                }


                SCRecordingOutputConfiguration *
                    recordingConfiguration =
                        [[SCRecordingOutputConfiguration alloc]
                            init];

                recordingConfiguration.outputURL =
                    outputURL;

                recordingConfiguration.outputFileType =
                    AVFileTypeMPEG4;

                recordingConfiguration.videoCodecType =
                    AVVideoCodecTypeH264;

                GriddleRecordingOutputHandler *
                    recordingHandler =
                        [[GriddleRecordingOutputHandler alloc]
                            init];

                SCRecordingOutput *recordingOutput =
                    nil;

                if (!GriddleDiagnosticRawStreamOnly)
                {

                    recordingOutput =
                        [[SCRecordingOutput alloc]
                            initWithConfiguration:
                                recordingConfiguration
                            delegate:
                                recordingHandler];

                    NSError *recordingOutputError =
                        nil;

                    BOOL recordingOutputAdded =
                        [stream
                            addRecordingOutput:
                                recordingOutput
                            error:
                                &recordingOutputError];

                    if (!recordingOutputAdded)
                    {
                        callback(
                            recordingOutputError
                                .localizedDescription
                                .UTF8String,
                            context);

                        return;
                    }
                }


                GriddleRecordingStream =
                    stream;

                GriddleRecordingStreamHandler =
                    streamHandler;

                GriddleRecordingQueue =
                    queue;

                if (!GriddleDiagnosticRawStreamOnly)
                {
                    GriddleRecordingOutput =
                        recordingOutput;

                    GriddleRecordingOutputDelegate =
                        recordingHandler;
                }

                [stream
                    startCaptureWithCompletionHandler:^(
                        NSError *startError)
                    {
                        if (startError != nil)
                        {
                            clear_recording_state();

                            callback(
                                startError
                                    .localizedDescription
                                    .UTF8String,
                                context);

                            return;
                        }

                        GriddleRecordingActive =
                            YES;

                        callback(
                            NULL,
                            context);
                    }];
            }];
    }
    else
    {
        callback(
            "Screen recording output requires macOS 15 or later.",
            context);
    }
}


void griddle_recording_stop(
    GriddleRecordingStopCallback callback,
    void *context)
{
    if (callback == NULL)
    {
        return;
    }

    if (!GriddleRecordingActive ||
        GriddleRecordingStream == nil)
    {
        callback(
            0.0,
            "No screen recording is active.",
            context);

        return;
    }

    if (GriddlePendingStopCallback != NULL)
    {
        callback(
            0.0,
            "Screen recording is already stopping.",
            context);

        return;
    }

    GriddlePendingStopCallback =
        callback;

    GriddlePendingStopContext =
        context;

    SCStream *stream =
        GriddleRecordingStream;

    [stream
        stopCaptureWithCompletionHandler:^(
            NSError *error)
        {
            if (error != nil)
            {
                GriddleRecordingStopCallback
                    pendingCallback =
                        GriddlePendingStopCallback;

                void *pendingContext =
                    GriddlePendingStopContext;

                clear_pending_stop();

                clear_recording_state();

                if (pendingCallback != NULL)
                {
                    pendingCallback(
                        0.0,
                        error.localizedDescription.UTF8String,
                        pendingContext);
                }

                return;
            }

            if (GriddleDiagnosticRawStreamOnly)
            {
                GriddleRecordingStopCallback
                    pendingCallback =
                        GriddlePendingStopCallback;

                void *pendingContext =
                    GriddlePendingStopContext;

                clear_pending_stop();

                if (GriddleVideoWriterInput != nil)
                {
                    [GriddleVideoWriterInput
                        markAsFinished];
                }

                if (GriddleMicrophoneWriterInput != nil)
                {
                    [GriddleMicrophoneWriterInput
                        markAsFinished];
                }

                if (GriddleAssetWriter != nil &&
                    GriddleAssetWriterSessionStarted)
                {
                    [GriddleAssetWriter
                        finishWritingWithCompletionHandler:^
                        {
                            NSError *writerError =
                                GriddleAssetWriter.error;

                            NSLog(
                                @"Griddle AVAssetWriter finish -- status=%ld error=%@",
                                (long)GriddleAssetWriter.status,
                                writerError);

                            double durationSeconds =
                                0.0;

                            if (CMTIME_IS_VALID(
                                    GriddleAssetWriterStartTime))
                            {
                                if (CMTIME_IS_VALID(
                                        GriddleAssetWriterLastVideoTime))
                                {
                                    CMTime duration =
                                        CMTimeSubtract(
                                            GriddleAssetWriterLastVideoTime,
                                            GriddleAssetWriterStartTime);

                                    durationSeconds =
                                        CMTimeGetSeconds(
                                            duration);
                                }
                            }

                            clear_recording_state();

                            if (pendingCallback != NULL)
                            {
                                pendingCallback(
                                    durationSeconds,
                                    writerError != nil
                                        ? writerError
                                            .localizedDescription
                                            .UTF8String
                                        : NULL,
                                    pendingContext);
                            }
                        }];
                }
                else
                {
                    clear_recording_state();

                    if (pendingCallback != NULL)
                    {
                        pendingCallback(
                            0.0,
                            NULL,
                            pendingContext);
                    }
                }

                return;
            }

            /*
             * Keep SCRecordingOutput alive until it
             * finishes finalizing the MP4.
             *
             * recordingOutputDidFinishRecording:
             * returns the real duration to C#.
             */
            GriddleRecordingActive =
                NO;
        }];
}


int32_t griddle_recording_is_active(void)
{
    return GriddleRecordingActive
        ? 1
        : 0;
}
