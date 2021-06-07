using System;

namespace QR {

    class GaloisField {

        public static int GF_add(int x, int y) {
            return x ^ y;
        }

        public static int GF_multiply(int x, int y) {
            int currentValue = 0;
            int currentBitDigit;
            for (int i = 0; i < 8; i += 1) {
                currentBitDigit = 1 << i;
                if ((y & currentBitDigit) == currentBitDigit) {
                    currentValue = GF_add(x << i, currentValue);
                }
            }
            //mod shift for numbers over a byte.
            if (GF_find_sig_bit(currentValue) > 8) {
                    currentValue = GF_multiply_mod(currentValue);
                }
            return currentValue;
        }
        static int GF_find_sig_bit(int num) {
            int count = 0;
            while (num > 0) {
                num = num / 2;
                count += 1;
            }
            return count;
        }

        static int GF_multiply_mod(int result) {
            const int irreduciblePolynomial = 285;
            
            int IP_SigBit = GF_find_sig_bit(285);
            int resultSigBit;
            int shiftAmount;
            int IP_Shift;

            void GF_multiply_shift() {
                resultSigBit = GF_find_sig_bit(result);
                shiftAmount = resultSigBit - IP_SigBit;
                IP_Shift = irreduciblePolynomial << shiftAmount;
 
            }
            GF_multiply_shift();

            while (resultSigBit >= IP_SigBit) {
                result = GF_add(result, IP_Shift);
                GF_multiply_shift();
            }
            return result;
        }

        public static int[] GF_error_codewords(int[] messageArray, int[] generatorArray) {
            int firstMessageTerm;
            int loopCount = messageArray.Length;
            int amountOfCodewords = generatorArray.Length - 1;

            int[] resizedMessageArray = new int[amountOfCodewords + messageArray.Length];
            for (int i = 0; i < messageArray.Length; i += 1) {
                resizedMessageArray[i] = messageArray[i];
            }

            for (int i = 0; i < loopCount; i += 1) {
                firstMessageTerm = resizedMessageArray[i];
                for (int j = 0; j < generatorArray.Length; j += 1) {
                    int multipliedGenerator = GF_multiply(generatorArray[j], firstMessageTerm);
                    resizedMessageArray[j + i] = GF_add(resizedMessageArray[j + i], multipliedGenerator);
                }
            }

            int[] result = new int[amountOfCodewords];
            int count = 0;
            for (int i = loopCount; i < resizedMessageArray.Length; i += 1) {
                result[count] = resizedMessageArray[i];
                count += 1;
            }
            return result;
        }

        public static int[] GF_generator_polynomial(int codewordCount) {
            int[] startingPoly = new int[] { 1, 1 };
            return GF_generate_recur(codewordCount, startingPoly);
        }

        static int[] GF_generate_recur(int codewordCount, int[] currentArray, int numberofIterations = 1, int lastAlpha = 1) {
            if (numberofIterations == codewordCount) {
                return currentArray;
            }
            lastAlpha = GF_multiply(lastAlpha, 2);
            int[] newArrayFirst = new int[currentArray.Length + 1];
            int[] newArraySecond = new int[currentArray.Length + 1];
            int[] newArrayFinal = new int[currentArray.Length + 1];
            int[] multiplier = new int[] { 1, lastAlpha };

            for (int i = 0; i < currentArray.Length; i += 1) {
                newArrayFirst[i] = GF_multiply(multiplier[0], currentArray[i]);
                newArraySecond[i + 1] = GF_multiply(multiplier[1], currentArray[i]);
            }
            for (int i = 0; i < newArrayFinal.Length; i += 1) {
                newArrayFinal[i] = GF_add(newArrayFirst[i], newArraySecond[i]);
            }
            numberofIterations += 1;
            return GF_generate_recur(codewordCount, newArrayFinal, numberofIterations, lastAlpha);
        }

    }
}
