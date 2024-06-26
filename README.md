# Electronic-Shelf-Label-Management-System
The ESL management system from this repository incorporates the electronic shelf labels and the ESL management software.

An electronic shelf label is a device that displays relevant product information, they're usually attached to the front edge of retail shelving. A typical ESL consists of a display and a simple microcontroller that controls the display.

The ESL management software enables you to manage your electronic shelf labels. It accomplishes this by maintaining a database that stores the credentials of each ESL along with the last image that was sent to it. When you choose the image to be displayed on the ESL and hit the "Send Image To ESL" button, the software encrypts that image, sends it to the specified ESL over Wi-Fi using the UDP protocol, and then saves it to the database.

Moreover, all ESL-relevant information stored in the ESL Management Software is encrypted with AES-256 in CBC mode (not to be confused with the Serpent in CBC mode which only encrypts the data that's sent over the air).

</br>
SourceForge page: https://sourceforge.net/projects/esl-management-system/
</br>
</br>
</br>
Tutorial for V2 is available at https://www.instructables.com/DIY-Electronic-Shelf-Label-Management-System-V20/
</br>

![image text](https://github.com/Northstrix/Electronic-Shelf-Label-Management-System/blob/main/V2.0/Pictures/IMG_0378.jpg)
![image text](https://github.com/Northstrix/Electronic-Shelf-Label-Management-System/blob/main/V2.0/Pictures/IMG_20240607_112111.jpg)
![image text](https://github.com/Northstrix/Electronic-Shelf-Label-Management-System/blob/main/V2.0/Pictures/AES%20in%20CBC.png)
![image text](https://github.com/Northstrix/Electronic-Shelf-Label-Management-System/blob/main/V2.0/Pictures/Circuit%20Diagram%20for%20ESL%20with%20ILI9341.png)
![image text](https://github.com/Northstrix/Electronic-Shelf-Label-Management-System/blob/main/V2.0/Pictures/Circuit%20Diagram%20for%20ESL%20with%20ST7789.png)


</br>
</br>
Tutorial for V1 is available at www.instructables.com/DIY-Electronic-Shelf-Label-Management-System/
</br>
</br>

![image text](https://github.com/Northstrix/Electronic-Shelf-Label-Management-System/blob/main/V1.0/Pictures/Thumbnail.JPG)
![image text](https://github.com/Northstrix/Electronic-Shelf-Label-Management-System/blob/main/V1.0/Pictures/Serpent%20in%20CBC.png)
![image text](https://github.com/Northstrix/Electronic-Shelf-Label-Management-System/blob/main/V1.0/Pictures/Circuit%20Diagram%20for%20ESL%20with%20ILI9341.png)
![image text](https://github.com/Northstrix/Electronic-Shelf-Label-Management-System/blob/main/V1.0/Pictures/Circuit%20Diagram%20for%20ESL%20with%20SSD1351.png)
