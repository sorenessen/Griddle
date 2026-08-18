#ifndef GRIDDLE_RECORDING_BRIDGE_H
#define GRIDDLE_RECORDING_BRIDGE_H

#include <stdint.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef void (*GriddleRecordingCallback)(
    const char *errorMessage,
    void *context);

typedef void (*GriddleRecordingStopCallback)(
    double durationSeconds,
    const char *errorMessage,
    void *context);

typedef void (*GriddleMicrophonePermissionCallback)(
    int32_t granted,
    const char *errorMessage,
    void *context);

typedef void (*GriddleScreenPermissionCallback)(
    int32_t granted,
    void *context);

void griddle_request_screen_access(
    GriddleScreenPermissionCallback callback,
    void *context);

void griddle_request_microphone_access(
    GriddleMicrophonePermissionCallback callback,
    void *context);

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
    void *context);

void griddle_recording_stop(
    GriddleRecordingStopCallback callback,
    void *context);

int32_t griddle_recording_is_active(void);

#ifdef __cplusplus
}
#endif

#endif