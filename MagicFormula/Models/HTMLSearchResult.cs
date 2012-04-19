// ***************************************************************
//  HTMLSearchResult   version:  1.0     date: 08/18/2007
//  -------------------------------------------------------------
//  This file contains class definition for HTMLSearchParser, to extract
//  parts of a HTML code.

//  This code has been written completely by me without anyone's help.
//  Disclaimer: Use this code at your own risk. Any problmes arising out 
//              due to this, please don't blame me.

//  Author: Ashutosh Bhawasinka
//  Email : expert@ashutosh.info
//  -------------------------------------------------------------
//  Copyright (C) 2007 - Ashutosh Bhawasinka
// ***************************************************************
// 
// ***************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace HTMLParser
{
    class HTMLSearchResult
    {
        public static bool EnableTracing = false;
        private Stack<string> TAGStack;
        //public static StreamWriter sw = new StreamWriter(@"C:\trace.log", true);

        public HTMLSearchResult()
        {
            TAGStack = null;
            sSearchResult = "";
            sTAGAttribute = "";
        }
        public HTMLSearchResult(string HTMLData)
        {
            TAGStack = null;
            sSearchResult = HTMLData;
            sTAGAttribute = "";
        }
        private string sSearchResult;
        /// <summary>
        /// Represents the TAG data for the last searched TAG.
        /// </summary>
        public string TAGData
        {
            get { return sSearchResult; }
        }
        private string sTAGAttribute;
        /// <summary>
        /// Represents the TAG attribute for the last searched TAG.
        /// </summary>
        public string TAGAttribute
        {
            get { return sTAGAttribute; }
        }

        private HTMLSearchResult Result(string str)
        {
            HTMLSearchResult ret = new HTMLSearchResult();
            ret.sSearchResult = str;
            return ret;
        }
        public HTMLSearchResult GetTagDataBetweenStrings(string sFileData, string sSearchStartText, string sSearchEndText)
        {
            string sResult = "";
            int nPos1 = sFileData.IndexOf(sSearchStartText);
            if (nPos1 >= 0)
            {
                int nPos2 = sFileData.IndexOf(sSearchEndText);
                if (nPos2 > nPos1 + sSearchStartText.Length)
                {
                    sResult = sFileData.Substring(nPos1 + sSearchStartText.Length, nPos2 - 1);
                }
            }
            return Result(sResult);
        }
/// <summary>
        ///Extracts the first occurance of specified tag from the current HTML data.
        /// <param name="sSearchTag">TAG to search.</param>
        /// <returns>Returns an HTMLSearchResult object containing the tag data.</returns>
/// </summary>
        public HTMLSearchResult GetTagData(string sSearchTag)
        {
            return GetTagData(sSearchResult, sSearchTag, 1);
        }
        /// <summary>
        ///Extracts the Nth occurance of specified tag from the current HTML data.
        /// <param name="sSearchTag">TAG to search</param>
        /// <param name="nOccurance">The Nth occurance to search for</param>
        /// <returns>Returns an HTMLSearchResult object containing the tag data.</returns>
        /// </summary>

        public HTMLSearchResult GetTagData(string sSearchTag, int nOccurance)
        {
            return GetTagData(sSearchResult, sSearchTag, nOccurance);
        }

        //This function returns the html between given two search strings
        /// <summary>
        ///Extracts the Nth occurance of specified tag from the current HTML data.
        /// <param name="sFileData">The HTML page data, that contains the entire html page</param>
        /// <param name="sSearchTag">TAG to search</param>
        /// <param name="nOccurance">The Nth occurance to search for</param>
        /// <returns>Returns an HTMLSearchResult object containing the tag data.</returns>
        /// </summary>

        public HTMLSearchResult GetTagData(string sFileData, string sSearchTag, int nOccurance)
        {
            #region General_Variables_&_Validation
            string sTAGData = "";
            int nStartPos = -1;
            int nEndPos = -1;

            TAGStack = new Stack<string>();
            int nFoundOccurance = 0; // keep track of the no of instances we found, of the search tag.
            int nLen = sFileData.Length;
            int nLevel = 0;
            bool bFound = false;

            TAGStack.Clear();

            if (nLen < 1)
            {
                throw (new ArgumentNullException("File Data cannot be null or blank string"));
            }
            if (sSearchTag.Length < 1)
            {
                throw (new ArgumentNullException("Search Tag cannot be null or blank string"));
            }
            if (nOccurance < 1)
            {
                throw (new ArgumentOutOfRangeException("The occurance number cannot be less than zero."));
            }
            //--------------------START THE SEARCH-----------------
            #endregion
            try
            {
                nLen = nLen - sSearchTag.Length + 1; //the part where we can compare
                for (int i = 0; i < nLen; i++)
                {
                    if (bFound == false)
                    {
                        if (sFileData[i] == '<' && sFileData[i + 1] != '/' && sFileData[i + 1] != '!')  //found some tag...
                        {
                            i++;
                            int nLastPos = i;
                            sTAGAttribute = ""; //its class member.
                            string sTAGName = ReadTillEndOfTag(sFileData, ref i, out sTAGAttribute);
                            if (string.Compare(sSearchTag, sTAGName, true) == 0)
                            {
                                ShowPos(sSearchTag, nLevel, true);
                                bFound = true;
                                nStartPos = i+1;//Need to fix this...
                            }
                        }
                        else if (sFileData[i] == '<' && sFileData[i + 1] == '/') //end tag found
                        {
                            string sEndTAG = "";
                            i += 2;
                            while (sFileData[i] != '>')
                                sEndTAG += sFileData[i++];

                            if (String.Compare(sEndTAG, sSearchTag, true) == 0 && nLevel == 0)
                            {
                                throw (new Exception("The Start tag was not found, however its end tag was found"));
                            }

                        }
                        else
                        {
                            continue;
                        }
                    }//bFound==false
                    else
                    {
                        if (sFileData[i] == '<' && sFileData[i + 1] != '/' && sFileData[i + 1] != '!')  //found some tag...
                        {
                            i++;
                            string sTagAttribute = "";
                            string sTAGName = ReadTillEndOfTag(sFileData, ref i, out sTagAttribute);
                            if (String.Compare(sTAGName, "script", true) == 0)
                            {
                                int k =i+1;
                                while (i < sFileData.Length)
                                {
                                    string substr = sFileData.Substring(k, 9);
                                    if (String.Compare(substr, "</script>", true) == 0)
                                    {
                                        i = k+9;
                                        break;
                                    }
                                    k++;
                                }
                            }
                            else if (String.Compare(sTAGName, "input", true) != 0 && String.Compare(sTAGName, "link", true) != 0 && String.Compare(sTAGName, "br", true) != 0 && String.Compare(sTAGName, "meta", true) != 0 && String.Compare(sTAGName, "img", true) != 0)
                            {
                                TAGStack.Push(sTAGName);
                                nLevel++;
                                ShowPos(sTAGName, nLevel, true);
                            }

                        }
                        else if (sFileData[i] == '<' && sFileData[i + 1] == '/')  //end tag found
                        {
                            int nLastCharPos = i - 1;
                            string sEndTAG = "";
                            i += 2;
                            while (sFileData[i] != '>')
                                sEndTAG += sFileData[i++];
                            ShowPos(sEndTAG, nLevel, false);
                            if (String.Compare(sEndTAG, sSearchTag, true) == 0 && nLevel == 0)
                            {
                                nFoundOccurance++;
                                nEndPos = nLastCharPos;
                                bFound = false;

                                if (nFoundOccurance == nOccurance)
                                {
                                    sTAGData = sFileData.Substring(nStartPos, nEndPos - nStartPos + 1);
                                    //Console.Write("\nSearch String\n======================================" + sTAGData + "\n==============================");
                                    break;
                                }
                                //break;
                            }
                            else
                            {
                                string sPopedTag = TAGStack.Pop();
                                if (string.Compare(sPopedTag, sEndTAG, true) != 0)
                                {
                                    //throw (new Exception("Unknown tag end"));
                                    TAGStack.Push(sPopedTag);
                                    nLevel++;
                                }
                                nLevel--;
                            }
                        }
                        else
                        {
                            continue;
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
            HTMLSearchResult result = new HTMLSearchResult();
            result.sSearchResult = sTAGData;
            return result;
        }

        private string ReadTillEndOfTag(string sFileData, ref int i, out string sTagAttribute)
        {
            string sTagName = "";
            sTagAttribute = "";

            //first skip all white spaces...

            while (sFileData[i] == ' ' || sFileData[i] == '\n' || sFileData[i] == '\r' || sFileData[i] == '\t' || sFileData[i] == '\v')
            {
                if (sFileData[i] == '>')
                    throw (new ArgumentException("The file data doesn't start a valid tag"));

                i++;
            }
            if (i >= sFileData.Length)
            {
                return sTagName;
            }
            //now read the TAG name
            while (sFileData[i] != '>' && sFileData[i] != ' ' && sFileData[i] != '\n' && sFileData[i] != '\r' && sFileData[i] != '\t' && sFileData[i] != '\v')
            {
                sTagName += sFileData[i++];
            }
            //Tag Name extracted...

            //Now read the Tag attribute...
            while (sFileData[i] != '>')
            {
                sTagAttribute += sFileData[i++];
            }
            return sTagName;
        }
        static private void ShowPos(string str, int nIndent, bool bEntry)
        {
            if (EnableTracing == false)
                return;

            string os = "";
            while (nIndent > 0)
            {
                nIndent--;
                os += " ";
            }
            if (bEntry)
            {
                os += ">";
            }
            else
            {
                os += "<";
            }
            os += str + "\r\n";
            //Console.Write(os);
            /*if (sw == null)
            {
                sw = new StreamWriter(@"C:\trace.log", true);
            }
            sw.Write(os);*/
        }
    }
}
