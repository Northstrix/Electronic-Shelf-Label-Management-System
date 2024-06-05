#include "Crypto.h"

#if defined ESP8266 || defined ESP32
/**
 * ESP8266 and ESP32 specific hardware true random number generator.
 * 
 * Acording to the ESP32 documentation, you should not call the tRNG 
 * faster than 5MHz
 * 
 */

byte RNG::get()
{
#if defined ESP32
    // ESP32 only
    uint32_t* randReg = (uint32_t*) 0x3FF75144;
    return (byte) *randReg;
#elif defined ESP8266
    // ESP8266 only
    uint32_t* randReg = (uint32_t*) 0x3FF20E44L;
    return (byte) *randReg;
#else
    // NOT SUPPORTED
    return 0;
#endif
}
#endif
