#import "GriddleRecordingBridge.h"

#import <AppKit/AppKit.h>
#import <AVFoundation/AVFoundation.h>
#import <CoreMedia/CoreMedia.h>
#import <ScreenCaptureKit/ScreenCaptureKit.h>
#import <unistd.h>


static BOOL GriddleRecordingActive = NO;

static SCStream *GriddleRecordingStream = nil;

static id GriddleRecordingStreamHandler = nil;

static dispatch_queue_t GriddleRecordingQueue = nil;

static SCRecordingOutput *GriddleRecordingOutput = nil;

static id GriddleRecordingOutputDelegate = nil;

static GriddleRecordingCallback
    GriddlePendingStopCallback = NULL;

static void *
    GriddlePendingStopContext = NULL;


static void clear_recording_state(void)
{
    GriddleRecordingActive = NO;

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
    /*
     * SCRecordingOutput writes the recording file.
     * We retain the screen stream output so Griddle
     * can process individual frames in the future.
     */
    (void)stream;
    (void)sampleBuffer;
    (void)type;
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

    GriddleRecordingCallback callback =
        GriddlePendingStopCallback;

    void *context =
        GriddlePendingStopContext;

    clear_pending_stop();

    clear_recording_state();

    if (callback != NULL)
    {
        callback(
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
    (void)recordingOutput;

    GriddleRecordingCallback callback =
        GriddlePendingStopCallback;

    void *context =
        GriddlePendingStopContext;

    clear_pending_stop();

    clear_recording_state();

    if (callback != NULL)
    {
        callback(
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

    GriddleRecordingCallback callback =
        GriddlePendingStopCallback;

    void *context =
        GriddlePendingStopContext;

    clear_pending_stop();

    clear_recording_state();

    if (callback != NULL)
    {
        callback(
            error.localizedDescription.UTF8String,
            context);
    }
}

@end


void griddle_recording_start(
    int32_t x,
    int32_t y,
    int32_t width,
    int32_t height,
    int32_t includeApplicationWindows,
    int32_t captureSystemAudio,
    int32_t captureMicrophone,
    int32_t framesPerSecond,
    const char *outputFilePath,
    GriddleRecordingCallback callback,
    void *context)
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

    /*
     * Audio wiring is intentionally deferred until
     * the basic video-file recording path is complete.
     */
    (void)captureSystemAudio;
    (void)captureMicrophone;

    if (outputFilePath == NULL)
    {
        callback(
            "Recording output path is required.",
            context);

        return;
    }

    /*
     * Make a durable Objective-C copy before this
     * native call returns. The original C string is
     * owned by the .NET P/Invoke marshaler and cannot
     * safely be referenced later from an async block.
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

    if (@available(macOS 15.0, *))
    {
        CGRect rect =
            CGRectMake(
                x,
                y,
                width,
                height);

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

                NSURL *outputURL =
                    [NSURL fileURLWithPath:
                        requestedOutputPath];

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

                GriddleRecordingStream =
                    stream;

                GriddleRecordingStreamHandler =
                    streamHandler;

                GriddleRecordingQueue =
                    queue;

                GriddleRecordingOutput =
                    recordingOutput;

                GriddleRecordingOutputDelegate =
                    recordingHandler;

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
    GriddleRecordingCallback callback,
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
            "No screen recording is active.",
            context);

        return;
    }

    if (GriddlePendingStopCallback != NULL)
    {
        callback(
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
                GriddleRecordingCallback
                    pendingCallback =
                        GriddlePendingStopCallback;

                void *pendingContext =
                    GriddlePendingStopContext;

                clear_pending_stop();

                clear_recording_state();

                if (pendingCallback != NULL)
                {
                    pendingCallback(
                        error.localizedDescription.UTF8String,
                        pendingContext);
                }

                return;
            }

            /*
             * Do not release SCRecordingOutput here.
             *
             * The recording output still needs to
             * finalize the MP4. StopAsync completes
             * from recordingOutputDidFinishRecording:.
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