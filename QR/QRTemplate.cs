using QR;
using System;
using System.Text;
using System.Text.RegularExpressions;

public class QRTemplate {
	const int L_ENCODING = 1, M_ENCODING = 0, Q_ENCODING = 3, H_ENCODING = 2;
	const int L_VERSION = 0, M_VERSION = 1, Q_VERSION = 2, H_VERSION = 3;
	const int NUMERIC = 1, ALPHANUMERIC = 2, BYTE = 4;
	string _inputstring;
	int _errorcorrectionlevel;
	int _encodingtype;
	int _charactercount;
	int _versionnumber;
	int _versionindex;
	int _messagecodewordcount;
	int _errorcodewordcount;
	int[] _errorcodewordarray;
	int[] _messagearray;
	int[] _fullmessagearray;
	public int maskValue { get => _maskvalue; set => _maskvalue = value; }
	int _maskvalue;
	public int formatString { get => _formatstring; set => _formatstring = value; }
	int _formatstring;
	public int[] resultArray { get => _resultarray; set => _resultarray = value; }
	int[] _resultarray;

	public QRTemplate(string input, string errorLevel) {
		_inputstring = input;
		switch (errorLevel) {
			case "L":
				_errorcorrectionlevel = L_ENCODING;
				break;
			case "M":
				_errorcorrectionlevel = M_ENCODING;
				break;
			case "Q":
				_errorcorrectionlevel = Q_ENCODING;
				break;
			case "H":
				_errorcorrectionlevel = H_ENCODING;
				break;
			default:
				throw new Exception("bad error level");
		}
	}

	public void parse_message_to_class_variables() {
		_encodingtype = encoding_mode();
		_charactercount = character_count();
		_versionnumber = version();
		_versionindex = _versionnumber - 1;
		_formatstring = format_string();
		_errorcodewordcount = error_codeword_count();
		_maskvalue = 0; //@TODO: MAKE THIS AUTOMATED
		_messagearray = message_array();
		_fullmessagearray = add_terminator();
		_errorcodewordarray = error_array();
		_resultarray = final_array_combine();
	}

	int encoding_mode() {
		Regex numeric = new Regex("^[0-9]+$");
		Regex alpaphaNumeric = new Regex(@"^[A-Z0-9 $%*+\-./:]*$");
		if (numeric.IsMatch(_inputstring)) {
			return NUMERIC;
		}
		if (alpaphaNumeric.IsMatch(_inputstring)) {
			return ALPHANUMERIC;
		}
		return BYTE;
	}

	int character_count() {
		return _inputstring.Length;
	}

	int encoding_to_version_const() {
        switch (_errorcorrectionlevel) {
			case L_ENCODING:
				return L_VERSION;
			case M_ENCODING:
				return M_VERSION;
			case Q_ENCODING:
				return Q_VERSION;
			case H_ENCODING:
				return H_VERSION;
			default:
				throw new Exception("literally impossible to get here");
        }
    }

	int[,] versionsByte = new int[,] {
		{ 17, 14, 11, 7 },
		{ 32, 26, 20, 14 },
		{ 53, 42, 32, 24 },
		{ 78, 62, 46, 34 },
		{ 106, 84, 60, 44 },
		{ 134, 106, 74, 58 }
	};


	int version() {
		//CHANGE TO 40 LATER WHEN ARRAY IS BIGGER
		int[,] versionsArray = new int[6, 3];
        switch(_encodingtype) {
			case NUMERIC:
				//versionsArray = versionsNumeric;
				break;
			case ALPHANUMERIC:
				//versionsArray = versionsAlphanumeric;
				break;
			case BYTE:
				versionsArray = versionsByte;
				break;
			default:
				throw new Exception("Version broke somehow");
        }
		int encoding = encoding_to_version_const();
		for (int i = 0; i < versionsArray.GetLength(0); i += 1) {
			if (_charactercount <= versionsArray[i, encoding]) {
				//return i + 1 instead of i because versions start at 1.
				_messagecodewordcount = versionsArray[i, encoding];
				return i + 1;
            }
        }
		throw new Exception("Character count too large");
    }

	int[] formatBits = new int[] {
        //This is a table of the 5 data bits (0 - 32)
        //and the error correction through BCH (15, 5)
        //error correction. This is the full string
        //and it is already masked.
        0x5412, 0x5125, 0x5E7C, 0x5B4B, 0x45F9,
		0x40CE, 0x4F97, 0x4AA0, 0x77C4, 0x72F3,
		0x7DAA, 0x789D, 0x662F, 0x6318, 0x6C41,
		0x6976, 0x1689, 0x13BE, 0x1CE7, 0x19D0,
		0x0762, 0x0255, 0x0D0C, 0x083B, 0x355F,
		0x3068, 0x3F31, 0x3A06, 0x24B4, 0x2183,
		0x2EDA, 0x2BED,
	};

	int format_string() {
		int formatNumber = encoding_to_version_const() * _maskvalue;
		return formatBits[formatNumber];
    }

	int[] message_array() {
		int[] intArray = new int[_charactercount];
		Encoding latin1 = Encoding.GetEncoding(28591);
		byte[] bytes = latin1.GetBytes(_inputstring);
		
		switch (_encodingtype) {
			//case NUMERIC:
			//	return message_array_num(charArray);
			//case ALPHANUMERIC:
			//	return message_array_alphanum(strArray);
			case BYTE:
				int index = 0;
				foreach (byte b in bytes) {
					intArray[index] = Convert.ToInt32(b);
					index += 1;
				}
				return intArray;
			default:
				throw new Exception("shouldn't happen");
        }

    }

	int[] combine_and_add_padding() {
		int[] result = new int[_messagearray.Length + 2];
		//extra room so a padding byte can be added
		int[] tempMessageArray = new int[_messagearray.Length + 3];
		tempMessageArray[0] = _encodingtype;
		tempMessageArray[1] = _charactercount;
		for (int i = 2; i < tempMessageArray.Length - 1; i += 1) {
			tempMessageArray[i] = _messagearray[i - 2];
        }
		//this empty final "byte" is so padding bits get automatically added at the end.
		tempMessageArray[tempMessageArray.Length - 1] = 0;
		for (int i = 0; i < tempMessageArray.Length - 1; i += 1) {
			result[i] = nibble_and_nibble(tempMessageArray[i], tempMessageArray[i + 1]);
        }
		return result;
	}

	int[] add_terminator() {
		const int PADDINGCOUNT = 2;
		int[] paddedMessageArray = combine_and_add_padding();
		int[] terminatorArray = new int[] { 236, 17 };
		int[] result = new int[_messagecodewordcount + PADDINGCOUNT];
		int bitsToFill = _messagecodewordcount - _messagearray.Length;
		for (int i = 0; i < paddedMessageArray.Length; i += 1) {
			result[i] = paddedMessageArray[i];
        }
		for (int i = 0; i < bitsToFill; i += 1) {
			result[i + paddedMessageArray.Length] = terminatorArray[i % 2];
        }
		return result;
    }

	static int nibble_and_nibble(int numFirst, int numSecond) {
		int result;
		//get first 4 bits
		numFirst = (numFirst & 0b_00001111) << 4;
		//get last 4 bits
		numSecond = (numSecond & 0b_11110000) >> 4;
		//add
		result = numFirst + numSecond;
		return result;
	}

	int[,] errorCodewordsTable = {
		{ 7, 10, 13, 17 },
		{ 10, 16, 22, 28 },
		{ 15, 26, 36, 44 },
		{ 20, 36, 52, 64 },
		{ 26, 48, 72, 88 },
		{ 36, 64, 96, 112 }
	};

	int error_codeword_count() {
		return errorCodewordsTable[_versionindex, encoding_to_version_const()];
    }

    int[] error_array() {
		int[] generator = GaloisField.GF_generator_polynomial(_errorcodewordcount);
		int[] array = GaloisField.GF_error_codewords(_fullmessagearray, generator);
		return array;
	}

	int[] final_array_combine() {
		int[] result = new int[_fullmessagearray.Length + _errorcodewordarray.Length];
		for (int i = 0; i < _fullmessagearray.Length; i += 1) {
			result[i] = _fullmessagearray[i];
        }
		for (int i = 0; i < _errorcodewordarray.Length; i += 1) {
			result[i + _fullmessagearray.Length] = _errorcodewordarray[i];
        }
		return result;
    }

    //static int[] message_array_num(char[] charArray) {
    //	string tempString = "";
    //	int count = 1;
    //	foreach(string s in strArray) {
    //		if (count == 3) {
    //			tempString += "s%";
    //           }
    //		else {
    //			tempString += "s";
    //           }
    //       }
    //	string[] tempArray = tempString.Split("%");
    //	int[] intArray = Array.ConvertAll(tempArray, int.Parse);
    //	return intArray;
    //}

    //static int[] message_array_alphanum(string[] strArray) {

    //   }
}
