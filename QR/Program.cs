using QR;
using System;

public class Program
{

    static void Main(string[] args) {
        QRTemplate QRtest = new QRTemplate("Hello World", "Q");
        QRtest.parse_message_to_class_variables();

        Versions.QR1(QRtest);

        //Error correcting works!
        //Masking works!
        //Generator polynomials will be too much of a pain in the ass to make a table for
        //so doing them with polynomial multiplication is the last step
        //Draw is an array that has a different symbol for each shape and each version has its own array.



        //int[] messagePolynomial = new int[] { 0b_01000000, 0b_10110100, 0b_10000110, 0b_01010110, 0b_11000110, 0b_11000110, 0b_11110010,
        //        0b_00000101, 0b_01110110, 0b_11110111, 0b_00100110, 0b_11000110, 0b_01000000 };
        //int[] generatorPolynomial = new int[] { 1, 137, 73, 227, 17, 177, 17, 52, 13, 46, 43, 83, 132, 120 };
        //foreach (int item in GaloisField.GF_error_codewords(messagePolynomial, generatorPolynomial)) {
        //    Console.WriteLine(item.ToString());
        //}

        //int[] possibleGenerator = new int[14];
        //foreach (int item in GaloisField.GF_error_codewords(messagePolynomial, generatorPolynomial)) {
        //    Console.WriteLine(item.ToString());
        //}
    }
}
