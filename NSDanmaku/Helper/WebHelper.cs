using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace NSDanmaku.Helper
{
    static class WebHelper
    {
        public static async Task<string> GetResults(Uri url)
        {
            HttpBaseProtocolFilter fiter = new HttpBaseProtocolFilter();
            fiter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.Expired);
            using (HttpClient hc = new HttpClient(fiter))
            {
                HttpResponseMessage hr = await hc.GetAsync(url);
                hr.EnsureSuccessStatusCode();
                var encodeResults = await hr.Content.ReadAsBufferAsync();
                string results = Encoding.UTF8.GetString(encodeResults.ToArray());
                return results;
            }
        }
    }
}
