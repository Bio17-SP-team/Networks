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
        string[] requestLines;
        string[] header;
        string[] headerLines_betweenvalu;
        string[] header_value_tile;
        string[] header_linessplit;
        string[] header_linespre;
        string BlankLine;
        string uri;



        string requestLineString;
        string headerLineString;

        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

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

            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)

            // Parse Request line

            // Validate blank line exists

            // Load header lines into HeaderLines dictionary
            ///req/r1/nb2 header/r3/n4/r5/n6/r7/n8

            Lines = requestString.Split('\r', '\n');
            //Line= request - header - blank 

            if (Lines.Length == 3)
            {
                requestLineString = Lines[0];
                BlankLine = Lines[4];
                headerLineString = Lines[2];
                ParseRequestLine();
                ValidateBlankLine();
                ValidateIsURI(uri);
                LoadHeaderLines();
                
                return true;

            }
            
            

            //throw new NotImplementedException();


            return false;
        }

        private bool ParseRequestLine()
        {


            requestLines = requestLineString.Split(' ');
            uri = requestLines[1];

            // HTTPVersion myvar = HTTPVersion.getnames;
            //bool exist= Enum.IsDefined(typeof(HTTPVersion), requestLines[0]);
            // string my = requestLines[0];
            // HTTPVersion myvar= requestLines[0].p



            if (requestLines.Length == 3)
            {
                if (requestLines[0] == "GET")
                    method = RequestMethod.GET;
                else if (requestLines[0] == "POST")
                    method = RequestMethod.POST;
                else if (requestLines[0] == "HEAD")
                    method = RequestMethod.HEAD;

                if (requestLines[2] == "HTTP/1.0")
                    httpVersion = HTTPVersion.HTTP10;
                else if (requestLines[2] == "HTTP/1.1")
                    httpVersion = HTTPVersion.HTTP11;
                else if (requestLines[2] == "HTTP/0.9")
                    httpVersion = HTTPVersion.HTTP09;
                return true;

            }

            else
            {

                return false;

            }

            //throw new NotImplementedException();





        }

        private bool ValidateIsURI(string uri)
        {

            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);

        }

        private bool LoadHeaderLines()
        {


            header_linespre = headerLineString.Split('\n');
            for (int i = 0; i <= header_linespre.Length; i = i + 1)
            {
                header_linessplit = header_linespre[i].Split(':');
                headerLines.Add(header_linessplit[0], header_linessplit[1]);
                string var = header_linessplit[0];


            }
            //dictionary number will be equals to number
            if (headerLines.Count == header_linespre.Length)
            {
                return true;
            }
            //throw new NotImplementedException();


            return false;
        }

        private bool ValidateBlankLine()
        {

            if (String.IsNullOrEmpty(BlankLine))
            {
                return true;
            }

            //throw new NotImplementedException();
            return false;
        }

    }
}
