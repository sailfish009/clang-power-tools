﻿using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;

namespace ClangPowerTools
{
  [Serializable]
  public class TidyOptions : DialogPage
  {
    #region Members

    private string[] mTidyChecks;
    private const string kTidyOptionsFileName = "TidyOptionsConfiguration.config";
    private SettingsPathBuilder mSettingsPathBuilder = new SettingsPathBuilder();

    #endregion

    #region Properties

    [Category(" Tidy")]
    [DisplayName("Custom Checks")]
    [Description("Specify clang-tidy checks to run using the standard tidy syntax. You can use wildcards to match multiple checks, combine them, etc (Eg. \"modernize - *, readability - *\"). When custom checks are specified, the individual tidy checks enabled (true/false) are ignored.")]
    [TypeConverter(typeof(StringArrayConverter))]
    public string[] TidyChecks
    {
      get => mTidyChecks;
      set => mTidyChecks = value;
    }

    [Category(" Tidy")]
    [DisplayName("Fix")]
    [Description("Automatically applies clang-tidy fixes to selected source files, affected header files and saves them to disk.")]
    public bool Fix { get; set; }

    #endregion

    #region DialogPage Save and Load implementation 

    public override void SaveSettingsToStorage()
    {
      string path = mSettingsPathBuilder.GetPath(kTidyOptionsFileName);

      var currentSettings = new ClangTidyOptions
      {
        Fix = this.Fix,
        TidyChecks = this.TidyChecks.ToList()
      };

      XmlSerializer serializer = new XmlSerializer();
      serializer.SerializeToFile(path, currentSettings);
    }

    public override void LoadSettingsFromStorage()
    {
      string path = mSettingsPathBuilder.GetPath(kTidyOptionsFileName);
      XmlSerializer serializer = new XmlSerializer();

      var loadedConfig = File.Exists(path)
        ? serializer.DeserializeFromFIle<ClangTidyOptions>(path)
        : new ClangTidyOptions();

      this.TidyChecks = loadedConfig.TidyChecks.ToArray();
      this.Fix = loadedConfig.Fix;
    }

    #endregion


  }
}
