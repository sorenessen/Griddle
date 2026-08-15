#ifndef GRIDDLE_CAPTURE_BRIDGE_H
#define GRIDDLE_CAPTURE_BRIDGE_H

#include <stdint.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef void (*GriddleCaptureCallback)(
    const uint8_t *data,
    int32_t dataLength,
    int32_t width,
    int32_t height,
    const char *errorMessage,
    void *context);

void griddle_capture_region(
    int32_t x,
    int32_t y,
    int32_t width,
    int32_t height,
    GriddleCaptureCallback callback,
    void *context);

#ifdef __cplusplus
}
#endif

#endif
