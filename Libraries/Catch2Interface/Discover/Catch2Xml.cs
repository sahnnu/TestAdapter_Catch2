﻿/** Basic Info **

Copyright: 2019 Johnny Hendriks

Author : Johnny Hendriks
Year   : 2019
Project: VSTestAdapter for Catch2
Licence: MIT

Notes: None

** Basic Info **/

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Catch2Interface.Discover
{
/*YAML
Class :
  Description : >
    This class is intended for use in discovering tests via Catch2 test executables.
*/
    public class Catch2Xml
    {
        #region Fields

        private StringBuilder  _logbuilder = new StringBuilder();
        private Settings       _settings;
        private List<TestCase> _testcases;

        #endregion // Fields

        #region Properties

        public string Log { get; private set; } = string.Empty;

        #endregion // Properties

        #region Constructor

        public Catch2Xml(Settings settings)
        {
            _settings = settings ?? new Settings();
        }

        #endregion // Constructor

        #region Public Methods

        public List<TestCase> ExtractTests(string output, string source)
        {
            _logbuilder.Clear();
            _testcases = new List<TestCase>();

            LogDebug($"  XML Discovery:{Environment.NewLine}{output}");
            Process(output, source);

            Log = _logbuilder.ToString();

            return _testcases;
        }

        #endregion // Public Methods

        #region Private Methods

        private void Process(string output, string source)
        {
            try
            {
                // Parse Xml
                var xml = new XmlDocument();
                xml.LoadXml(output);

                // Get TestCases
                var nodeGroup = xml.SelectSingleNode("//Group");

                var reportedTestCases = new List<Reporter.TestCase>();
                foreach(XmlNode child in nodeGroup)
                {
                    if(child.Name == Constants.NodeName_TestCase)
                    {
                        reportedTestCases.Add(new Reporter.TestCase(child));
                    }
                }

                // Convert found Xml testcases and add them to TestCase list to be returned
                foreach (var reportedTestCase in reportedTestCases)
                {
                    // Create testcase
                    var testcase = new TestCase();
                    testcase.Name = reportedTestCase.Name;
                    testcase.Source = source;
                    testcase.Filename = reportedTestCase.Filename;
                    testcase.Line = reportedTestCase.Line;
                    testcase.Tags = reportedTestCase.Tags;

                    // Add testcase
                    if(CanAddTestCase(testcase))
                    {
                        _testcases.Add(testcase);
                    }
                }
            }
            catch(XmlException)
            {
                // For now ignore Xml parsing errors
            }
        }

        private bool CanAddTestCase(TestCase testcase)
        {
            if( string.IsNullOrEmpty(testcase.Name) )
            {
                return false;
            }

            if(_settings.IncludeHidden)
            {
                return true;
            }

            // Check tags for hidden signature
            foreach(var tag in testcase.Tags)
            {
                if(Constants.Rgx_IsHiddenTag.IsMatch(tag))
                {
                    return false;
                }
            }
            return true;
        }

        #endregion // Private Methods

        #region Private Logging Methods

        private void LogDebug(string msg)
        {
            if (_settings == null
             || _settings.LoggingLevel == LoggingLevels.Debug)
            {
                _logbuilder.Append(msg);
            }
        }

        private void LogNormal(string msg)
        {
            if (_settings == null
             || _settings.LoggingLevel == LoggingLevels.Normal
             || _settings.LoggingLevel == LoggingLevels.Verbose
             || _settings.LoggingLevel == LoggingLevels.Debug)
            {
                _logbuilder.Append(msg);
            }
        }

        private void LogVerbose(string msg)
        {
            if (_settings == null
             || _settings.LoggingLevel == LoggingLevels.Verbose
             || _settings.LoggingLevel == LoggingLevels.Debug)
            {
                _logbuilder.Append(msg);
            }
        }

        #endregion // Private Logging Methods

    }
}
