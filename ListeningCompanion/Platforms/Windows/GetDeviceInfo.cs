
using Windows.Security.ExchangeActiveSyncProvisioning;

namespace ListeningCompanion
{
    public partial class GetDeviceInfo
    {
        public partial string GetDeviceID()
        {
            var deviceInformation = new EasClientDeviceInformation();

            string id = deviceInformation.Id.ToString();

            return id;
        }
    }
}