// https://github.com/eduherminio/FileParser | https://www.nuget.org/packages/FileParser/

using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Print = System.Diagnostics.Debug;
using System.ComponentModel;
using System.Globalization;

namespace FileParser
{
    [Serializable]
    public class ParsingException : Exception
    {
        private const string _genericMessage = "Exception triggered during parsing process";

        public ParsingException() : base(_genericMessage)
        {
        }

        public ParsingException(string message) : base(message ?? _genericMessage)
        {
        }

        public ParsingException(string message, Exception inner) : base(message ?? _genericMessage, inner)
        {
        }

        protected ParsingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

namespace FileParser
{
    public class ParsedFile : Queue<IParsedLine>, IParsedFile
    {
        public bool Empty { get { return Count == 0; } }

        /// <summary>
        /// Parses a file
        /// </summary>
        /// <param name="parsedFile"></param>
        public ParsedFile(IEnumerable<IParsedLine> parsedFile)
            : base(new Queue<IParsedLine>(parsedFile))
        {
        }

        /// <summary>
        /// Parses a file
        /// </summary>
        /// <param name="parsedFile"></param>
        public ParsedFile(Queue<IParsedLine> parsedFile)
            : base(parsedFile)
        {
        }

        /// <summary>
        /// Parses a file
        /// </summary>
        /// <param name="path">FilePath</param>
        /// <param name="existingSeparator">Word separator (space by default)</param>
        public ParsedFile(string path, char[] existingSeparator)
            : this(path, new string(existingSeparator))
        {
        }

        /// <summary>
        /// Parses a file
        /// </summary>
        /// <param name="path">FilePath</param>
        /// <param name="existingSeparator">Word separator (space by default)</param>
        public ParsedFile(string path, string existingSeparator = null)
            : base(ParseFile(path, existingSeparator))
        {
        }

        public IParsedLine NextLine()
        {
            if (!Empty)
            {
                return Dequeue();
            }

            throw new ParsingException("End of ParsedFile reached");
        }

        public IParsedLine PeekNextLine()
        {
            if (!Empty)
            {
                return Peek();
            }

            throw new ParsingException("End of ParsedFile reached");
        }

        public IParsedLine LineAt(int index)
        {
            return this.ElementAt(index);
        }

        public IParsedLine LastLine()
        {
            return this.Last();
        }

        public List<T> ToList<T>(string lineSeparatorToAdd = null)
        {
            List<T> list = new List<T>();

            if (!string.IsNullOrEmpty(lineSeparatorToAdd))
            {
                foreach (IParsedLine parsedLine in this)
                {
                    parsedLine.Append(lineSeparatorToAdd);
                }
            }

            while (!Empty)
            {
                list.AddRange(NextLine().ToList<T>());
            }

            return list;
        }

        public string ToSingleString(string wordSeparator = " ", string lineSeparator = null)
        {
            StringBuilder stringBuilder = new StringBuilder();

            while (!Empty)
            {
                stringBuilder.Append(NextLine().ToSingleString(wordSeparator));

                if (!Empty)
                {
                    stringBuilder.Append(!string.IsNullOrEmpty(lineSeparator)
                        ? lineSeparator
                        : wordSeparator);
                }
            }

            return stringBuilder.ToString();
        }

        public void Append(IParsedLine parsedLine) => Enqueue(parsedLine);

        #region Private methods

        /// <summary>
        /// Parses a file into a Queue&lt;IParsedLine&gt;
        /// Queue&lt;IParsedLine&gt; ~~ Queues of 'words' inside of a queue of lines
        /// </summary>
        /// <param name="path"></param>
        /// <param name="existingSeparator">Word separator</param>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static Queue<IParsedLine> ParseFile(string path, string existingSeparator = null)
        {
            Queue<IParsedLine> parsedFile = new Queue<IParsedLine>();

            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    while (!reader.EndOfStream)
                    {
                        string original_line = reader.ReadLine();

                        // TODO: Evaluate if is it worth giving the user the option of detecting these kind of lines?
                        if (string.IsNullOrWhiteSpace(original_line))
                        {
                            continue;
                        }
                        // end TODO

                        IParsedLine parsedLine = new ParsedLine(ProcessLine(original_line, existingSeparator));
                        parsedFile.Enqueue(parsedLine);
                    }
                }

                return parsedFile;
            }
            catch (Exception e)
            {
                Print.WriteLine(e.Message);
                Print.WriteLine("(path: {0}", path);
                throw;
            }
        }

        private static ICollection<string> ProcessLine(string original_line, string separator)
        {
            List<string> wordsInLine = original_line
                .Split(separator?.ToCharArray())
                .Select(str => str.Trim()).ToList();

            wordsInLine.RemoveAll(string.IsNullOrWhiteSpace);   // Probably not needed, but just in case

            return wordsInLine;
        }

        #endregion
    }
}

namespace FileParser
{
    /// <summary>
    /// Implementation based on Queue&lt;string&gt;
    /// </summary>
    public class ParsedLine : Queue<string>, IParsedLine
    {
        public bool Empty { get { return Count == 0; } }

        /// <summary>
        /// Parses a line
        /// </summary>
        /// <param name="parsedLine"></param>
        public ParsedLine(IEnumerable<string> parsedLine)
            : base(new Queue<string>(parsedLine))
        {
        }

        /// <summary>
        /// Parses a line
        /// </summary>
        /// <param name="parsedLine"></param>
        public ParsedLine(Queue<string> parsedLine)
            : base(parsedLine)
        {
        }

        public T NextElement<T>()
        {
            if (!Empty)
            {
                return Extract<T>();
            }

            throw new ParsingException("End of ParsedLine reached");
        }

        public T PeekNextElement<T>()
        {
            if (!Empty)
            {
                return Peek<T>();
            }

            throw new ParsingException("End of ParsedLine reached");
        }

        public T ElementAt<T>(int index)
        {
            ValidateSupportedType<T>();

            string element = this.ElementAt(index);

            return StringConverter.Convert<T>(element);
        }

        public T LastElement<T>()
        {
            ValidateSupportedType<T>();

            string element = this.Last();

            return StringConverter.Convert<T>(element);
        }

        public List<T> ToList<T>()
        {
            List<T> list = new List<T>();

            while (!Empty)
            {
                list.Add(NextElement<T>());
            }

            return list;
        }

        public string ToSingleString(string wordSeparator = " ")
        {
            StringBuilder stringBuilder = new StringBuilder();

            while (!Empty)
            {
                stringBuilder.Append(NextElement<string>());
                if (!Empty)
                {
                    stringBuilder.Append(wordSeparator);
                }
            }

            return stringBuilder.ToString();
        }

        public void Append(string str) => Enqueue(str);

        #region Private methods

        /// <summary>
        /// Returns next element of a ParsedLine, converting it to T and removing it from the Queue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private T Extract<T>()
        {
            ValidateSupportedType<T>();

            string stringToConvert = Dequeue();

            if (typeof(T) == typeof(string))
            {
                return (T)(object)stringToConvert;
            }
            else if (typeof(T) == typeof(char))
            {
                if (this.Any())
                {
                    throw new NotSupportedException("Extract<char> can only be used with one-length Queues" +
                       " Try using ExtractChar<string> instead, after parsing each string with Extract<string>()");
                }

                char nextChar = ExtractChar(ref stringToConvert);
                if (stringToConvert.Length > 0)
                {
                    Enqueue(stringToConvert);
                }

                return (T)(object)nextChar;
            }

            return StringConverter.Convert<T>(stringToConvert);
        }

        /// <summary>
        /// Returns next element of a ParsedLine, converting it to T but WITHOUT removing it from the Queue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private T Peek<T>()
        {
            ValidateSupportedType<T>();

            string stringToConvert = Peek();

            return StringConverter.Convert<T>(stringToConvert);
        }

        private static void ValidateSupportedType<T>()
        {
            if (!SupportedTypes.Contains(typeof(T)))
            {
                throw new NotSupportedException("Parsing to " + typeof(T).ToString() + "is not supported yet");
            }
        }

        /// <summary>
        /// Returns next char of a string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static char ExtractChar(ref string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ParsingException("String is empty");
            }

            char nextChar = str[0];
            str = str.Substring(1);

            return nextChar;
        }

        /// <summary>
        /// Supported parsing conversions
        /// </summary>
        private static HashSet<Type> SupportedTypes { get; } = new HashSet<Type>()
        {
            typeof(bool),
            typeof(char),
            typeof(string),
            typeof(short),
            typeof(int),
            typeof(long),
            typeof(double),
            typeof(object)
        };

        #endregion
    }
}

namespace FileParser
{
    public interface IParsedFile
    {
        /// <summary>
        /// Returns the size (number of lines) of ParsedFile
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Returns whether ParsedFile has no elements
        /// </summary>
        bool Empty { get; }

        /// <summary>
        /// Returns next line (IParsedLine), removing it from ParsedFile
        /// </summary>
        /// <exception cref="ParsingException">File is already empty</exception>
        /// <returns>Next line</returns>
        IParsedLine NextLine();

        /// <summary>
        /// Returns next line (IParsedLine), not modifying ParsedFile
        /// </summary>
        /// <exception cref="ParsingException">File is already empty</exception>
        /// <returns>Next line</returns>
        IParsedLine PeekNextLine();

        /// <summary>
        /// Returns line at a specified index
        /// </summary>
        /// <param name="index">zero-based index</param>
        /// <returns>Line at the specified position in file</returns>
        /// <exception cref="System.ArgumentNullException">File is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">index is less than 0 or greater than or equal to the number of lines in file</exception>
        IParsedLine LineAt(int index);

        /// <summary>
        /// Returns last line
        /// </summary>
        /// <returns>Last line</returns>
        /// <exception cref="System.ArgumentNullException">File is null</exception>
        /// <exception cref="System.InvalidOperationException">File is empty</exception>
        IParsedLine LastLine();

        /// <summary>
        /// Returns remaining elements as a list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        List<T> ToList<T>(string lineSeparatorToAdd = null);

        /// <summary>
        /// Returns remaining elements as a single string, separated by given wordSeparator and lineSeparator
        /// </summary>
        /// <param name="wordSeparator"></param>
        /// <param name="lineSeparator"></param>
        /// <returns></returns>
        string ToSingleString(string wordSeparator = " ", string lineSeparator = null);

        /// <summary>
        /// Appends a line to the end of the file
        /// </summary>
        /// <param name="parsedLine"></param>
        void Append(IParsedLine parsedLine);
    }
}

namespace FileParser
{
    public interface IParsedLine
    {
        /// <summary>
        /// Returns the size (number of elements) of ParsedLine
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Returns whether ParsedLine has no elements
        /// </summary>
        bool Empty { get; }

        /// <summary>
        /// Returns next element of type T, removing it from ParsedLine
        /// </summary>
        /// <exception cref="ParsingException">Line is already empty</exception>
        /// <exception cref="System.NotSupportedException">Parsing to chosen type is not supported yet or T is char and line's length > 1</exception>
        /// <returns>Next element</returns>
        T NextElement<T>();

        /// <summary>
        /// Returns next element of type T, not modifying ParsedLine.
        /// This still allows its modification
        /// </summary>
        /// <exception cref="ParsingException">Line is already empty</exception>
        /// <exception cref="System.NotSupportedException">Parsing to chosen type is not supported yet</exception>
        /// <returns>Next element</returns>
        T PeekNextElement<T>();

        /// <summary>
        /// Returns element at a specified index, allowing its modification
        /// </summary>
        /// <param name="index">zero-based index</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>Element at the specified position in line</returns>
        /// <exception cref="System.NotSupportedException">Parsing to selected type is not supported yet</exception>
        /// <exception cref="System.ArgumentNullException">File is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">index is less than 0 or greater than or equal to the number of lines in file</exception>
        T ElementAt<T>(int index);

        /// <summary>
        /// Returns last element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Last element</returns>
        /// <exception cref="System.NotSupportedException">Parsing to selected type is not supported yet</exception>
        /// <exception cref="System.ArgumentNullException">Line is null</exception>
        /// <exception cref="System.InvalidOperationException">Line is empty</exception>
        T LastElement<T>();

        /// <summary>
        /// Returns remaining elements as a list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        List<T> ToList<T>();

        /// <summary>
        /// Returns remaining elements as a single string, separated by wordSeparator
        /// </summary>
        /// <param name="wordSeparator"></param>
        /// <returns></returns>
        string ToSingleString(string wordSeparator = " ");

        /// <summary>
        /// Appends a string to the end of the line
        /// </summary>
        /// <param name="str"></param>
        void Append(string str);
    }
}

namespace FileParser
{
    internal static class StringConverter
    {
        /// <summary>
        /// Converts strings to basic, nullable types
        /// Optional parameter: an already known typeConverter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <param name="typeConverter"></param>
        /// <returns></returns>
        public static T Convert<T>(string str, TypeConverter typeConverter = null)
        {
            if (typeof(T).IsPrimitive)
            {
                return typeConverter == null
                    ? TConverter.ChangeType<T>(str)
                    : TConverter.ChangeType<T>(str, typeConverter);
            }
            else // Avoids exception if T is an object
            {
                object o = str;
                return (T)o;
            }
        }

        public static TypeConverter GetConverter<T>()
        {
            if (typeof(T).IsPrimitive)
            {
                return TConverter.GetTypeConverter(typeof(T));
            }
            else
            {
                throw new NotSupportedException($"Converter for {typeof(T).Name} yet to be implemented");
            }
        }
    }
}

namespace FileParser
{
    /// <summary>
    /// Provides generic type conversions for basic types, including nullable ones
    /// Original method by Tuna Toksoz
    /// Sources:
    /// https://stackoverflow.com/questions/8625/generic-type-conversion-from-string
    /// http://web.archive.org/web/20101214042641/http://dogaoztuzun.com/post/C-Generic-Type-Conversion.aspx
    /// </summary>
    internal static class TConverter
    {
        internal static T ChangeType<T>(object value, TypeConverter typeConverter = null)
        {
            return typeConverter == null
                    ? (T)ChangeType(typeof(T), value)
                    : (T)typeConverter.ConvertFrom(value);
        }

        private static object ChangeType(Type t, object value)
        {
            if (t == typeof(double))
            {
                return ParseDouble(value);
            }
            else
            {
                TypeConverter tc = TypeDescriptor.GetConverter(t);

                return tc.ConvertFrom(value);
            }
        }

        private static object ParseDouble(object value)
        {
            double result;

            string doubleAsString = value.ToString();

            if (doubleAsString == null)
            {
                throw new ParsingException($"Error parsing value as double");
            }

            IEnumerable<char> doubleAsCharList = doubleAsString.ToList();

            if (doubleAsCharList.Count(ch => ch == '.' || ch == ',') <= 1)
            {
                double.TryParse(doubleAsString.Replace(',', '.'),
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out result);
            }
            else
            {
                if (doubleAsCharList.Count(ch => ch == '.') <= 1
                    && doubleAsCharList.Count(ch => ch == ',') > 1)
                {
                    double.TryParse(doubleAsString.Replace(",", string.Empty),
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out result);
                }
                else if (doubleAsCharList.Count(ch => ch == ',') <= 1
                    && doubleAsCharList.Count(ch => ch == '.') > 1)
                {
                    double.TryParse(doubleAsString.Replace(".", string.Empty).Replace(',', '.'),
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out result);
                }
                else
                {
                    throw new ParsingException($"Error parsing {doubleAsString} as double, try removing thousand separators (if any)");
                }
            }

            return result as object;
        }

        internal static TypeConverter GetTypeConverter(Type t)
        {
            return TypeDescriptor.GetConverter(t);
        }

        /// <summary>
        /// Currently unused
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TC"></typeparam>
        internal static void RegisterTypeConverter<T, TC>() where TC : TypeConverter
        {
            TypeDescriptor.AddAttributes(typeof(T), new TypeConverterAttribute(typeof(TC)));
        }
    }
}