namespace GIR_Capstone.Server.Services
{
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Defines the <see cref="GlobeStatusDecoderService" />
    /// </summary>
    public class GlobeStatusDecoderService
    {
        /// <summary>
        /// Defines the _scopeFactory
        /// </summary>
        private readonly IServiceScopeFactory _scopeFactory;

        /// <summary>
        /// Defines the _globeStatusCache
        /// </summary>
        private readonly Dictionary<string, string> _globeStatusCache = new();

        /// <summary>
        /// Defines the _isLoaded
        /// </summary>
        private bool _isLoaded = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobeStatusDecoderService"/> class.
        /// </summary>
        /// <param name="scopeFactory">The scopeFactory<see cref="IServiceScopeFactory"/></param>
        public GlobeStatusDecoderService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        /// <summary>
        /// The LoadStatusMappingsAsync
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        public async Task LoadStatusMappingsAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            try
            {
                var mappings = await context.CodeDecodeGlobeStatus
                    .ToDictionaryAsync(s => s.Code, s => s.Abbreviation);

                if (mappings.Count == 0)
                {
                    Console.WriteLine("Warning: No mappings found in CodeDecodeGlobeStatus table.");
                }

                foreach (var kvp in mappings)
                {
                    _globeStatusCache[kvp.Key] = kvp.Value;
                }

                _isLoaded = true;
                Console.WriteLine($"Successfully loaded {mappings.Count} status mappings.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading status mappings: {ex.Message}");
                _isLoaded = false;
            }
        }

        /// <summary>
        /// The Decode
        /// </summary>
        /// <param name="statusCode">The statusCode<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        public string Decode(string statusCode)
        {
            return _globeStatusCache.TryGetValue(statusCode, out var abbreviation)
                ? abbreviation
                : "UNKNOWN";
        }
    }
}
