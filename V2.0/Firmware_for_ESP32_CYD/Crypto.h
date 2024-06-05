/**
 * An extremely minimal crypto library for Arduino devices.
 * 
 * The SHA256 and AES implementations are derived from axTLS 
 * (http://axtls.sourceforge.net/), Copyright (c) 2008, Cameron Rich.
 * 
 * Ported and refactored by Chris Ellis 2016.
 * pkcs7 padding routines added by Mike Killewald Nov 26, 2017 (adopted from https://github.com/spaniakos/AES).
 * 
 */

#ifndef CRYPTO_h
#define CRYPTO_h

#include <Arduino.h>

class RNG
{
    public:
        static byte get();
    private:
};
#endif
