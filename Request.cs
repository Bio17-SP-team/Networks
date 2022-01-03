using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;


namespace HTTPServer
{
    public enum RequestMethod
    {

        GET,
        POST,
        HEAD

    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] Lines;
        string requestLineString;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines = new Dictionary<string, string>();

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            //TODO: parse the receivedRequest using the \r\n delimeter
            string[] lineSeparators = new string[] { "\r\n" };
            this.Lines = this.requestString.Split(lineSeparators, StringSplitOptions.None);
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (Lines.Length < 3) return false;
            // Parse Request line
            if (!ParseRequestLine()) return false;
            // Validate blank line exists
            if (!ValidateBlankLine()) return false;
            // Load header lines into HeaderLines dictionary
            if (!LoadHeaderLines()) return false;
            return true;
        }

        private bool ParseRequestLine()
        {
            string[] requestLines = this.Lines[0].Split(' ');
            if (requestLines.Length < 3) return false;
            this.method = (RequestMethod)Enum.Parse(typeof(RequestMethod), requestLines[0]);
            this.relativeURI = requestLines[1].Substring(1);

            if (requestLines[2] == "HTTP/1.0")
                this.httpVersion = HTTPVersion.HTTP10;
            else if (requestLines[2] == "HTTP/1.1")
                this.httpVersion = HTTPVersion.HTTP11;
            else if (requestLines[2] == "HTTP/0.9")
                this.httpVersion = HTTPVersion.HTTP09;

            if (!ValidateIsURI(relativeURI)) return false;
            return true;
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {

            string[] separator = new string[] { ": " };
            for (int i = 1; i < this.Lines.Length - 2; i++)
            {
                string[] headerSplit = Lines[i].Split(separator, StringSplitOptions.None);
                if (headerSplit.Length < 2) return false;
                this.HeaderLines.Add(headerSplit[0], headerSplit[1]);
            }
            return true;
        }

        private bool ValidateBlankLine()
        {

            if (String.IsNullOrEmpty(Lines[Lines.Length - 2]))
            {
                return true;
            }
            return false;
        }

    }
}
