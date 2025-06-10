using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnoopAPITechnicalTask.TestData
{
    using SnoopAPITechnicalTask.Utilities;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

    public class CustomerTestData
    {
        #region Fields

        [JsonInclude]
        [JsonPropertyName("NoData")]
        private List<string> NoData { get; set; }

        [JsonInclude]
        [JsonPropertyName("ValidData")]
        private List<string> ValidData { get; set; }

        [JsonInclude]
        [JsonPropertyName("InvalidData")]
        private List<string> InvalidData { get; set; }

        public static IEnumerable<string> InvalidCustomerIds
        {
            get
            {
                var filePath = Path.Combine(FileUtility.GetProjectRootPath(), "TestData", "customerTestData.json");
                var json = File.ReadAllText(filePath);

                var testData = JsonSerializer.Deserialize<CustomerTestData>(json);
                return testData?.GetInvalidCustomers() ?? Enumerable.Empty<string>();
            }
        }

        public static IEnumerable<string> ValidCustomerIds
        {
            get
            {
                var filePath = Path.Combine(FileUtility.GetProjectRootPath(), "TestData", "customerTestData.json");
                var json = File.ReadAllText(filePath);

                var testData = JsonSerializer.Deserialize<CustomerTestData>(json);
                return testData?.GetValidCustomers() ?? Enumerable.Empty<string>();
            }
        }

        public static IEnumerable<string> NoDataCustomerIds
        {
            get
            {
                var filePath = Path.Combine(FileUtility.GetProjectRootPath(), "TestData", "customerTestData.json");
                var json = File.ReadAllText(filePath);

                var testData = JsonSerializer.Deserialize<CustomerTestData>(json);
                return testData?.GetNoDataCustomers() ?? Enumerable.Empty<string>();
            }
        }
        #endregion

        #region Public Methods

        public IReadOnlyList<string> GetNoDataCustomers() => NoData ?? new List<string>();

        public IReadOnlyList<string> GetValidCustomers() => ValidData ?? new List<string>();

        public IReadOnlyList<string> GetInvalidCustomers() => InvalidData ?? new List<string>();

        #endregion

        #region Static Methods

        public static async Task<CustomerTestData> LoadFromFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            using var stream = File.OpenRead(filePath);
            var customerTestData = await JsonSerializer.DeserializeAsync<CustomerTestData>(stream);

            return customerTestData ?? new CustomerTestData();
        }

        #endregion
    }
}
