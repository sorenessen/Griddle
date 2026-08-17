#ifndef GRIDDLE_RECORDING_BRIDGE_H
#define GRIDDLE_RECORDING_BRIDGE_H

#include <stdint.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef void (*GriddleRecordingCallback)(
    const char *errorMessage,
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
    GriddleRecordingCallback callback,
    void *context);

int32_t griddle_recording_is_active(void);

#ifdef __cplusplus
}
#endif

#endif