using System;

public class Versions
{
    public static void QR_print(int[,] array, int dimensions) {
        for (int i = dimensions - 1; i >= 0; i += -1) {
            for (int j = dimensions - 1; j >= 0; j += -1) {
                Console.Write(array[j, i] + " ");
            }
            Console.WriteLine("");
        }
    }

	public static void QR1(QRTemplate QRCode) {
        const bool UP = true, DOWN = false;
        const bool NOTFULLLINE = false;
        Pointer pointer = new Pointer();
        int[,] QRArray = new int[21, 21];
        int[] messageArray = QRCode.resultArray;
        int formatString = QRCode.formatString;
        int mask = QRCode.maskValue;

        QRData.QR_up_or_down(UP, 3, pointer, messageArray, QRArray);
        QRData.QR_up_or_down(DOWN, 3, pointer, messageArray, QRArray);

        QRData.QR_up_or_down(UP, 3, pointer, messageArray, QRArray);
        QRData.QR_up_or_down(DOWN, 3, pointer, messageArray, QRArray);

        QRData.QR_up_or_down(UP, 3, pointer, messageArray, QRArray, NOTFULLLINE);
        QRData.QR_middle_skip(UP, 1, pointer, messageArray, QRArray, NOTFULLLINE);
        QRData.QR_up_or_down(UP, 1, pointer, messageArray, QRArray);
        QRData.QR_up_or_down(DOWN, 1, pointer, messageArray, QRArray, NOTFULLLINE);
        QRData.QR_middle_skip(DOWN, 1, pointer, messageArray, QRArray, NOTFULLLINE);
        QRData.QR_up_or_down(DOWN, 3, pointer, messageArray, QRArray);

        QRData.QR_skip_byte(UP, 2, pointer);
        QRData.QR_up_or_down(UP, 1, pointer, messageArray, QRArray);
        pointer.Xpointer += 1;
        QRData.QR_up_or_down(DOWN, 1, pointer, messageArray, QRArray);

        QRData.QR_up_or_down(UP, 1, pointer, messageArray, QRArray);
        QRData.QR_up_or_down(DOWN, 1, pointer, messageArray, QRArray);

        
        QRData.masking(mask, QRArray);
        QRData.draw_eyes(QRArray);
        QRData.white_border(QRArray);
        QRData.format_information(formatString, QRArray);
        QRData.timing_bits(QRArray);
        QRData.to_png(QRArray);
        QR_print(QRArray, 21);
    }
}
