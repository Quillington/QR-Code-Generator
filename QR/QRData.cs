using System;
using System.Drawing;

public class QRData
{
    
    public static int digit_to_bit(int num, int digit) {
        int result;
        int shift = 1 << digit;
        if ((num & shift) == shift) {
            result = 1;
        }
        else {
            result = 0;
        }
        return result;
    }

    private static void QR_up(Pointer pointer, int message, int[,] array) {
        array[0 + pointer.Xpointer, 0 + pointer.Ypointer] = digit_to_bit(message, 7);
        array[1 + pointer.Xpointer, 0 + pointer.Ypointer] = digit_to_bit(message, 6);
        array[0 + pointer.Xpointer, 1 + pointer.Ypointer] = digit_to_bit(message, 5);
        array[1 + pointer.Xpointer, 1 + pointer.Ypointer] = digit_to_bit(message, 4);
        array[0 + pointer.Xpointer, 2 + pointer.Ypointer] = digit_to_bit(message, 3);
        array[1 + pointer.Xpointer, 2 + pointer.Ypointer] = digit_to_bit(message, 2);
        array[0 + pointer.Xpointer, 3 + pointer.Ypointer] = digit_to_bit(message, 1);
        array[1 + pointer.Xpointer, 3 + pointer.Ypointer] = digit_to_bit(message, 0);
    }


    private static void QR_down(Pointer pointer, int message, int[,] array) {
        array[0 + pointer.Xpointer, 3 + pointer.Ypointer] = digit_to_bit(message, 7);
        array[1 + pointer.Xpointer, 3 + pointer.Ypointer] = digit_to_bit(message, 6);
        array[0 + pointer.Xpointer, 2 + pointer.Ypointer] = digit_to_bit(message, 5);
        array[1 + pointer.Xpointer, 2 + pointer.Ypointer] = digit_to_bit(message, 4);
        array[0 + pointer.Xpointer, 1 + pointer.Ypointer] = digit_to_bit(message, 3);
        array[1 + pointer.Xpointer, 1 + pointer.Ypointer] = digit_to_bit(message, 2);
        array[0 + pointer.Xpointer, 0 + pointer.Ypointer] = digit_to_bit(message, 1);
        array[1 + pointer.Xpointer, 0 + pointer.Ypointer] = digit_to_bit(message, 0);
    }

    public static void QR_up_or_down(bool isUP, int timesToRepeat, Pointer pointer, 
        int[] messageArray, int[,] array, bool fullLine = true) {

        for (int i = 0; i < timesToRepeat; i += 1) {
            if (isUP) {
                QR_up(pointer, messageArray[pointer.messageIndex], array);
                if (i < (timesToRepeat - 1)) {
                    //last round moves left and not up
                    pointer.Y_up();
                }
            }
            else {
                QR_down(pointer, messageArray[pointer.messageIndex], array);
                if (i < (timesToRepeat - 1)) {
                    //last round moves left and not up
                    pointer.Y_down();
                }
            }
            pointer.messageIndex += 1;
        }
        if (fullLine) {
            pointer.X_left();
        }
        else {
            if (isUP) {
                pointer.Y_up();
            }
            else {
                pointer.Y_down();
            }
        }
    }

    static void QR_up_middle_skip(Pointer pointer, int message, int[,] array) {
        array[0 + pointer.Xpointer, 0 + pointer.Ypointer] = digit_to_bit(message, 7);
        array[1 + pointer.Xpointer, 0 + pointer.Ypointer] = digit_to_bit(message, 6);
        array[0 + pointer.Xpointer, 1 + pointer.Ypointer] = digit_to_bit(message, 5);
        array[1 + pointer.Xpointer, 1 + pointer.Ypointer] = digit_to_bit(message, 4);
        pointer.Ypointer += 1;
        array[0 + pointer.Xpointer, 2 + pointer.Ypointer] = digit_to_bit(message, 3);
        array[1 + pointer.Xpointer, 2 + pointer.Ypointer] = digit_to_bit(message, 2);
        array[0 + pointer.Xpointer, 3 + pointer.Ypointer] = digit_to_bit(message, 1);
        array[1 + pointer.Xpointer, 3 + pointer.Ypointer] = digit_to_bit(message, 0);
    }

    static void QR_down_middle_skip(Pointer pointer, int message, int[,] array) {
        array[0 + pointer.Xpointer, 3 + pointer.Ypointer] = digit_to_bit(message, 7);
        array[1 + pointer.Xpointer, 3 + pointer.Ypointer] = digit_to_bit(message, 6);
        array[0 + pointer.Xpointer, 2 + pointer.Ypointer] = digit_to_bit(message, 5);
        array[1 + pointer.Xpointer, 2 + pointer.Ypointer] = digit_to_bit(message, 4);
        pointer.Ypointer += -1;
        array[0 + pointer.Xpointer, 1 + pointer.Ypointer] = digit_to_bit(message, 3);
        array[1 + pointer.Xpointer, 1 + pointer.Ypointer] = digit_to_bit(message, 2);
        array[0 + pointer.Xpointer, 0 + pointer.Ypointer] = digit_to_bit(message, 1);
        array[1 + pointer.Xpointer, 0 + pointer.Ypointer] = digit_to_bit(message, 0);
    }

    public static void QR_middle_skip(bool isUP, int timesToRepeat, Pointer pointer,
        int[] messageArray, int[,] array, bool fullLine = true) {

        for (int i = 0; i < timesToRepeat; i += 1) {
            if (isUP) {
                QR_up_middle_skip(pointer, messageArray[pointer.messageIndex], array);
                if (i < (timesToRepeat - 1)) {
                    //last round moves left and not up
                    pointer.Y_up();
                }
            }
            else {
                QR_down_middle_skip(pointer, messageArray[pointer.messageIndex], array);
                if (i < (timesToRepeat - 1)) {
                    //last round moves left and not up
                    pointer.Y_down();
                }
            }
            pointer.messageIndex += 1;
        }
        if (fullLine) {
            pointer.X_left();
        }
        else {
            if (isUP) {
                pointer.Y_up();
            }
            else {
                pointer.Y_down();
            }
        }
    }

    public static void QR_skip_byte(bool isUP, int timesToRepeat, Pointer pointer) {
        for (int i = 0; i < timesToRepeat; i += 1) {
            if (isUP) {
                pointer.Y_up();
            }
            else {
                pointer.Y_down();
            }
        }
    }

    static void draw_eye(int[,] array, int Xmodifier, int Ymodifier) {
        void draw_pattern(int pattern, int[,] array, int Yvalue, int Xmodifier, int Ymodifier) {
            int[] patternArrayOne = { 1, 1, 1, 1, 1, 1, 1 };
            int[] patternArrayTwo = { 1, 0, 0, 0, 0, 0, 1 };
            int[] patternArrayThree = { 1, 0, 1, 1, 1, 0, 1 };
            for (int i = 0; i < 7; i += 1) {
                if (pattern == 1) {
                    array[i + Xmodifier, Yvalue + Ymodifier] = patternArrayOne[i];  
                }
                else if (pattern == 2) {
                    array[i + Xmodifier, Yvalue + Ymodifier] = patternArrayTwo[i];
                }
                else {
                    array[i + Xmodifier, Yvalue + Ymodifier] = patternArrayThree[i];
                }
            }
        }
        draw_pattern(1, array, 0, Xmodifier, Ymodifier);
        draw_pattern(2, array, 1, Xmodifier, Ymodifier);
        draw_pattern(3, array, 2, Xmodifier, Ymodifier);
        draw_pattern(3, array, 3, Xmodifier, Ymodifier);
        draw_pattern(3, array, 4, Xmodifier, Ymodifier);
        draw_pattern(2, array, 5, Xmodifier, Ymodifier);
        draw_pattern(1, array, 6, Xmodifier, Ymodifier);
    }

    public static void draw_eyes(int[,] array) {
        int sizeMinusEyeSize = array.GetLength(0) - 7;
        draw_eye(array, 0, sizeMinusEyeSize);
        draw_eye(array, sizeMinusEyeSize, 0);
        draw_eye(array, sizeMinusEyeSize, sizeMinusEyeSize);
    }

    static int masking_type(int type, int x, int y, int dimensions) {
        x = dimensions - x;
        switch(type) {
            case 0:
                return (x + y) % 2;
            case 1:
                return x % 2;
            case 2:
                return y % 3;
            case 3:
                return (x + y) % 3;
            case 4:
                return ((x / 2) + (y / 3)) % 2;
            case 5:
                return ((x * y) % 2) + ((x * y) % 3);
            case 6:
                return (((x * y) % 3) + (x * y)) % 2;
            case 7:
                return ((x * y) % 3 + x + y) % 2;
        }
        throw new Exception("bad");
    }

    public static void masking(int type, int[,] array) {
        for (int i = 0; i < array.GetLength(0); i += 1) {
            for (int j = 0; j < array.GetLength(1); j += 1) {
                array[j, i] = array[j, i] ^ masking_type(type, j, i, array.GetLength(0));
            }
        }
    }

    static void white_border_up(int Xmodifier, int Ymodifier, int[,] array) {
        for (int i = 0; i < 8; i += 1) {
            array[Xmodifier, Ymodifier + i] = 0;
        }
    }

    static void white_border_accross(int Xmodifier, int Ymodifier, int[,] array) {
        for (int i = 0; i < 8; i += 1) {
            array[Xmodifier + i, Ymodifier] = 0;
        }
    }

    public static void white_border(int[,] array) {
        int eye = 8;
        int sizeMinusEye = array.GetLength(0) - eye;

        white_border_accross(0, sizeMinusEye, array);
        white_border_up(eye - 1, sizeMinusEye, array);

        white_border_accross(sizeMinusEye, eye - 1, array);
        white_border_up(sizeMinusEye, 0, array);

        white_border_accross(sizeMinusEye, sizeMinusEye, array);
        white_border_up(sizeMinusEye, sizeMinusEye, array);
    }

    public static void timing_bits(int[,] array) {
        int eye = 6;
        int length = array.GetLength(0) - 1;

        for (int i = 8; i < length - 7; i += 1) {
            //horizontal
            array[length - i, length - eye] = (i + 1) % 2;
            //vertical
            array[length - eye, i] = (i + 1) % 2;
        }
    }

    public static void format_information(int message, int[,] array) {
        int eye = 9;
        int sizeMinusEye = array.GetLength(0) - eye;

        //start at top left, go downwards
        array[sizeMinusEye, array.GetLength(0) - 1] = digit_to_bit(message, 0);
        array[sizeMinusEye, array.GetLength(0) - 2] = digit_to_bit(message, 1);
        array[sizeMinusEye, array.GetLength(0) - 3] = digit_to_bit(message, 2);
        array[sizeMinusEye, array.GetLength(0) - 4] = digit_to_bit(message, 3);
        array[sizeMinusEye, array.GetLength(0) - 5] = digit_to_bit(message, 4);
        array[sizeMinusEye, array.GetLength(0) - 6] = digit_to_bit(message, 5);
        //skip a bit for timings
        array[sizeMinusEye, array.GetLength(0) - 8] = digit_to_bit(message, 6);
        array[sizeMinusEye, sizeMinusEye] = digit_to_bit(message, 7);
        //wrap other direction
        array[sizeMinusEye + 1, sizeMinusEye] = digit_to_bit(message, 8);
        //skip for timings
        array[sizeMinusEye + 3, sizeMinusEye] = digit_to_bit(message, 9);
        array[sizeMinusEye + 4, sizeMinusEye] = digit_to_bit(message, 10);
        array[sizeMinusEye + 5, sizeMinusEye] = digit_to_bit(message, 11);
        array[sizeMinusEye + 6, sizeMinusEye] = digit_to_bit(message, 12);
        array[sizeMinusEye + 7, sizeMinusEye] = digit_to_bit(message, 13);
        array[array.GetLength(0) - 1, sizeMinusEye] = digit_to_bit(message, 14);

        //do it again for the other two sides
        //start at top right
        int temp = digit_to_bit(message, 0);
        array[0, sizeMinusEye] = temp;
        array[1, sizeMinusEye] = digit_to_bit(message, 1);
        array[2, sizeMinusEye] = digit_to_bit(message, 2);
        array[3, sizeMinusEye] = digit_to_bit(message, 3);
        array[4, sizeMinusEye] = digit_to_bit(message, 4);
        array[5, sizeMinusEye] = digit_to_bit(message, 5);
        array[6, sizeMinusEye] = digit_to_bit(message, 6);
        array[7, sizeMinusEye] = digit_to_bit(message, 7);

        //now go to bottom left
        //random dark module for good luck
        array[sizeMinusEye, 7] = 1;
        //now the data for real
        array[sizeMinusEye, 6] = digit_to_bit(message, 8);
        array[sizeMinusEye, 5] = digit_to_bit(message, 9);
        array[sizeMinusEye, 4] = digit_to_bit(message, 10);
        array[sizeMinusEye, 3] = digit_to_bit(message, 11);
        array[sizeMinusEye, 2] = digit_to_bit(message, 12);
        array[sizeMinusEye, 1] = digit_to_bit(message, 13);
        array[sizeMinusEye, 0] = digit_to_bit(message, 14);
    }

    public static void to_png(int[,] array) {
        int dimensions = array.GetLength(0);

        Bitmap image = new Bitmap(dimensions, dimensions);
        for (int y = 0; y < dimensions; y += 1)
            for (int x = 0; x < dimensions; x += 1) {
                if (array[x, y] == 1) {
                    image.SetPixel(dimensions - 1 - x, dimensions - 1 - y, Color.FromName("Black"));
                }
                else {
                    image.SetPixel(dimensions - 1 - x, dimensions - 1 - y, Color.FromName("White"));
                }
                
            }
        image.Save(@"y:\tmp.png");
    }


    public static int[] message_string() {
        //fill this in later hate documentation
        //will add all the other functions together into one big thing
        //need 1) encoding 2) version 3) character count 4) message to bits
        return new int [] { 0b_01000000, 0b_10110100, 0b_10000110, 0b_01010110, 0b_11000110, 0b_11000110, 0b_11110010,
                0b_00000101, 0b_01110110, 0b_11110111, 0b_00100110, 0b_11000110, 0b_01000000, 186, 102, 131, 211, 101, 92, 152, 1, 196, 221, 75, 252, 112};
    }
}
