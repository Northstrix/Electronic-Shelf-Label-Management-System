/*
DIY Electronic Shelf Label Management System
Distributed under the MIT License
© Copyright Maxim Bortnikov 2024
For more information please visit
https://sourceforge.net/projects/esl-management-system/
https://github.com/Northstrix/Electronic-Shelf-Label-Management-System
Required libraries:
https://github.com/peterferrie/serpent
https://github.com/me-no-dev/ESPAsyncUDP
https://github.com/adafruit/Adafruit-GFX-Library
https://github.com/adafruit/Adafruit_ILI9341
https://github.com/adafruit/Adafruit-SSD1351-library
https://github.com/adafruit/Adafruit_BusIO
*/
#include "ESP8266TrueRandom.h"
#include <EEPROM.h>
#include "serpent.h"
#include <ESP8266WiFi.h>
#include "ESPAsyncUDP.h"
#include "FS.h"
#include <Adafruit_GFX.h>
#include <Adafruit_SSD1351.h>

#define SCLK_PIN D5
#define MOSI_PIN D7
#define DC_PIN   D4
#define CS_PIN   D8
#define RST_PIN  D6

#define DISPLAY_WIDTH  128
#define DISPLAY_HEIGHT 128

#define EEPROM_SIZE 4095
#define BUTTON_PIN 5

const char * ssid = "Access-Point-Name";
const char * pass = "access_point_password";

uint8_t serp_key[32];
byte tmp_st[8];
int decract;
char array_for_CBC_mode[16];
uint8_t back_serp_key[32];
String string_for_data;

Adafruit_SSD1351 tft = Adafruit_SSD1351(DISPLAY_WIDTH, DISPLAY_HEIGHT, &SPI, CS_PIN, DC_PIN, RST_PIN);
AsyncUDP udp;

int x;
int y;
uint16_t udp_port;

void back_serp_k() {
  for (int i = 0; i < 32; i++) {
    back_serp_key[i] = serp_key[i];
  }
}

void rest_serp_k() {
  for (int i = 0; i < 32; i++) {
    serp_key[i] = back_serp_key[i];
  }
}

void incr_serp_key() {
  if (serp_key[15] == 255) {
    serp_key[15] = 0;
    if (serp_key[14] == 255) {
      serp_key[14] = 0;
      if (serp_key[13] == 255) {
        serp_key[13] = 0;
        if (serp_key[12] == 255) {
          serp_key[12] = 0;
          if (serp_key[11] == 255) {
            serp_key[11] = 0;
            if (serp_key[10] == 255) {
              serp_key[10] = 0;
              if (serp_key[9] == 255) {
                serp_key[9] = 0;
                if (serp_key[8] == 255) {
                  serp_key[8] = 0;
                  if (serp_key[7] == 255) {
                    serp_key[7] = 0;
                    if (serp_key[6] == 255) {
                      serp_key[6] = 0;
                      if (serp_key[5] == 255) {
                        serp_key[5] = 0;
                        if (serp_key[4] == 255) {
                          serp_key[4] = 0;
                          if (serp_key[3] == 255) {
                            serp_key[3] = 0;
                            if (serp_key[2] == 255) {
                              serp_key[2] = 0;
                              if (serp_key[1] == 255) {
                                serp_key[1] = 0;
                                if (serp_key[0] == 255) {
                                  serp_key[0] = 0;
                                } else {
                                  serp_key[0]++;
                                }
                              } else {
                                serp_key[1]++;
                              }
                            } else {
                              serp_key[2]++;
                            }
                          } else {
                            serp_key[3]++;
                          }
                        } else {
                          serp_key[4]++;
                        }
                      } else {
                        serp_key[5]++;
                      }
                    } else {
                      serp_key[6]++;
                    }
                  } else {
                    serp_key[7]++;
                  }
                } else {
                  serp_key[8]++;
                }
              } else {
                serp_key[9]++;
              }
            } else {
              serp_key[10]++;
            }
          } else {
            serp_key[11]++;
          }
        } else {
          serp_key[12]++;
        }
      } else {
        serp_key[13]++;
      }
    } else {
      serp_key[14]++;
    }
  } else {
    serp_key[15]++;
  }
}

size_t hex2bin(void * bin) {
  size_t len, i;
  int x;
  uint8_t * p = (uint8_t * ) bin;
  for (i = 0; i < 32; i++) {
    p[i] = (uint8_t) serp_key[i];
  }
  return 32;
}

uint8_t getNum(char ch) {
  int num = 0;
  if (ch >= '0' && ch <= '9') {
    num = ch - 0x30;
  } else {
    switch (ch) {
    case 'A':
    case 'a':
      num = 10;
      break;
    case 'B':
    case 'b':
      num = 11;
      break;
    case 'C':
    case 'c':
      num = 12;
      break;
    case 'D':
    case 'd':
      num = 13;
      break;
    case 'E':
    case 'e':
      num = 14;
      break;
    case 'F':
    case 'f':
      num = 15;
      break;
    default:
      num = 0;
    }
  }
  return uint8_t(num);
}

void back_key() {
  back_serp_k();
}

void rest_key() {
  rest_serp_k();
}

void clear_variables() {
  decract = 0;
}

bool bttn_is_pressed() {
  bool bt_state = false;
  if (digitalRead(BUTTON_PIN) == LOW) {
    delay(350);
    if (digitalRead(BUTTON_PIN) == LOW) {
      bt_state = true;
    }
  }
  return bt_state;
}

void disp_centered_text(String text, int h) {
  int16_t x1;
  int16_t y1;
  uint16_t width;
  uint16_t height;
  tft.getTextBounds(text, 0, 0, & x1, & y1, & width, & height);
  tft.setCursor((DISPLAY_WIDTH - width) / 2, h);
  tft.print(text);
}

String read_file(fs::FS & fs, String path) {
  File file = fs.open(path, "r");
  if (!file || file.isDirectory()) {
    return "-1";
  }
  String fileContent;
  while (file.available()) {
    fileContent += String((char) file.read());
  }
  file.close();
  return fileContent;
}

void write_to_file_with_overwrite(fs::FS & fs, String path, String content) {
  File file = fs.open(path, "w");
  if (!file) {
    return;
  }
  file.print(content);
  file.close();
}

void display_line_on_display(String img_line) {
  if (img_line == "-1")
    return;
  for (int i = 0; i < (DISPLAY_WIDTH * 4); i += 4) {
    String hex = "";
    for (int j = 0; j < 4; j++) {
      hex += img_line.charAt(i + j);
    }
    uint16_t color = (uint16_t) strtol(hex.c_str(), NULL, 16);
    tft.drawPixel(x, y, color);
    x++;
    if (x == DISPLAY_WIDTH) {
      x = 0;
      y++;
    }
  }
}

void setup() {
  tft.begin();
  tft.fillScreen(0x0000);
  tft.setRotation(0);
  tft.setCursor(0, 5);
  if (!SPIFFS.begin()) {
    tft.println("An Error has occurred while mounting SPIFFS");
    return;
  }
  EEPROM.begin(EEPROM_SIZE);
  pinMode(BUTTON_PIN, INPUT);
  Serial.begin(115200);
  bool disp_set = false;
  if (bttn_is_pressed() == true) {
    disp_centered_text("Open Serial Terminal", 10);
    disp_centered_text("and switch ESL", 30);
    disp_centered_text("to operating mode", 50);
    while (bttn_is_pressed() == true) {
      delay(100);
    }
    tft.fillScreen(0x0000);
    disp_centered_text("The Encryption Key", 10);
    disp_centered_text("and UDP Port", 30);
    disp_centered_text("will be printed to", 50);
    disp_centered_text("Serial Terminal", 70);
    disp_centered_text("in 2 seconds", 90);
    delay(2000);
    tft.fillScreen(0x0000);
    disp_centered_text("Check the", 10);
    disp_centered_text("Serial Terminal", 30);
    EEPROM.begin(EEPROM_SIZE);
    randomSeed(ESP8266TrueRandom.random());
    for (int i = 0; i < 34; i++) {
      EEPROM.write((i), ESP8266TrueRandom.random(0, 256));
    }
    if (EEPROM.read(33) == 0 && EEPROM.read(32) == 0)
      EEPROM.write((33), ESP8266TrueRandom.random(128, 256));

    String extr_eeprom_content;
    for (int i = 0; i < 32; i++) {
      if (EEPROM.read(i) < 16)
        extr_eeprom_content += "0";
      extr_eeprom_content += String(EEPROM.read(i), HEX);
    }
    Serial.println("\nEncryption Key:");
    Serial.println(extr_eeprom_content);

    String extr_udp;
    for (int i = 32; i < 34; i++) {
      if (EEPROM.read(i) < 16)
        extr_udp += "0";
      extr_udp += String(EEPROM.read(i), HEX);
    }
    
    Serial.println("UDP Port:");
    Serial.println(extr_udp);
    Serial.println();
    EEPROM.end();
    disp_set = true;
  } else {
    if (read_file(SPIFFS, "Line1") != "-1") {
      x = 0;
      y = 0;
      for (int i = 0; i < DISPLAY_WIDTH; i++) {
        display_line_on_display(read_file(SPIFFS, "Line" + String(i + 1)));
      }
    }
    else{
      tft.fillScreen(0x0000);
      tft.setTextSize(1);
      tft.setTextColor(65535);
      tft.setCursor(0, 64);
      tft.print("No image is set (yet)");
    }
  }
  x = 0;
  y = 0;
  Serial.println("");
  Serial.println("Connecting to Wi-Fi");
  WiFi.disconnect(true);
  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, pass);
  while (WiFi.status() != WL_CONNECTED) {
    delay(300);
    Serial.print("#");
  }
  delay(24);
  EEPROM.begin(EEPROM_SIZE);
  for (int i = 0; i < 32; i++) {
    serp_key[i] = EEPROM.read(i);
  }
  EEPROM.end();
  char ipadr[20];
  sprintf(ipadr, "IP:%s", WiFi.localIP().toString().c_str());
  //disp_centered_text(ipadr, 40);
  EEPROM.begin(EEPROM_SIZE);
  /*
  String extr_eeprom_content;
  for (int i = 32; i < 34; i++) {
    if (EEPROM.read(i) < 16)
      extr_eeprom_content += "0";
    extr_eeprom_content += String(EEPROM.read(i), HEX);
  }
  */
  udp_port = (EEPROM.read(32) << 8) | EEPROM.read(33);
  EEPROM.end();
  if (udp.listen(udp_port)) {
    Serial.print("\nUDP Listening on IP: ");
    Serial.println(WiFi.localIP());
    udp.onPacket([](AsyncUDPPacket packet) {
      if (packet.length() == 1) {
        x = 0;
        y = 0;
      } else {
        /*
        for (int i = 0; i < packet.length(); i += 2) {
          tft.drawPixel(x, y, (packet.data()[(i + 1)] << 8) | packet.data()[i]);
          x++;
          if (x == DISPLAY_WIDTH){
            x = 0;
            y++;
          }
        }
        */
        back_key();
        clear_variables();
        int ext = 0;
        decract = -1;
        string_for_data = "";
        for (int i = 0; i < packet.length(); i += 16) { // 656 /16 
          int br = false;
          byte res[16];
          byte prev_res[16];
          for (int j = 0; j < 16; j++) {
            res[j] = packet.data()[j + ext];
          }

          if (decract > 16) {
            for (int j = 0; j < 16; j++) {
              prev_res[j] = packet.data()[j + ext - 16];
            }
          }

          if (br == false) {
            if (decract > 16) {
              for (int i = 0; i < 16; i++) {
                array_for_CBC_mode[i] = prev_res[i];
              }
            }
            uint8_t ct1[32], pt1[32], key[64];
            int plen, clen, i, j;
            serpent_key skey;
            serpent_blk ct2;
            uint32_t * p;

            for (i = 0; i < 1; i++) {
              hex2bin(key);

              // set key
              memset( & skey, 0, sizeof(skey));
              p = (uint32_t * ) & skey.x[0][0];

              serpent_setkey( & skey, key);
              //Serial.printf ("\nkey=");

              for (j = 0; j < sizeof(skey) / sizeof(serpent_subkey_t) * 4; j++) {
                if ((j % 8) == 0) putchar('\n');
                //Serial.printf ("%08X ", p[j]);
              }

              for (int i = 0; i < 16; i++)
                ct2.b[i] = res[i];
              /*
              Serial.printf ("\n\n");
              for(int i = 0; i<16; i++){
              Serial.printf("%x", ct2.b[i]);
              Serial.printf(" ");
              */
            }
            //Serial.printf("\n");
            serpent_encrypt(ct2.b, & skey, SERPENT_DECRYPT);
            incr_serp_key();
            if (decract > 2) {
              for (int i = 0; i < 16; i++) {
                ct2.b[i] ^= array_for_CBC_mode[i];
              }

              for (i = 0; i < 16; i += 2) {
                tft.drawPixel(x, y, (ct2.b[i + 1] << 8) | ct2.b[i]);
                /*
                uint16_t color = (ct2.b[i + 1] << 8) | ct2.b[i];
                char hexString[5];
                sprintf(hexString, "%04X", color);
                string_for_data += String(hexString);
                */
                if (ct2.b[i + 1] < 16)
                  string_for_data += "0";
                string_for_data += String(ct2.b[i + 1], HEX);
                if (ct2.b[i] < 16)
                  string_for_data += "0";
                string_for_data += String(ct2.b[i], HEX);
                x++;
                if (x == DISPLAY_WIDTH) {
                  x = 0;
                  y++;
                }
              }
            }

            if (decract == -1) {
              for (i = 0; i < 16; ++i) {
                array_for_CBC_mode[i] = int(ct2.b[i]);
              }
            }
            decract++;
          }
          ext += 16;
          decract += 10;
        }
        rest_key();
        write_to_file_with_overwrite(SPIFFS, "Line" + String(y), string_for_data);
      }
    });
  }
}

void loop() {

}
