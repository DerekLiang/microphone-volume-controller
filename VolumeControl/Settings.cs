using System.ComponentModel;
using System.Configuration;
using System.Windows.Media;

namespace VolumeControl
{
    public class Settings : ApplicationSettingsBase
    {
        [UserScopedSetting()]
        [DefaultSettingValue("")]
        public int DefaultVolume
        {
            get
            {
                if (this["DefaultVolume"] is string)
                {
                    var returnVal = 50;
                    int.TryParse((string) this["DefaultVolume"], out returnVal);
                    return returnVal;
                }
                return ((int)this["DefaultVolume"]);
            }
            set
            {
                this["DefaultVolume"] = value;
            }
        }
    }
}