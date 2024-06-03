using Infrastructure.Services.Impls;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace VMControlPanelTest.Tests
{
    public class CacheServiceTests
    {
        private readonly IDistributedCache _cache;
        private readonly CacheService _underTest;

        public CacheServiceTests()
        {
            var options = Options.Create(new MemoryDistributedCacheOptions());
            
            _cache = new MemoryDistributedCache(options);
            _underTest = new CacheService(_cache);
        }

        [Fact]
        public async Task GetValueAsync_NormalFlow()
        {
            // Given
            var key = "some_key";
            var data = "some_data";

            await _underTest.SetValueAsync(key, data, 1);

            // When
            var result = await _underTest.GetValueAsync<string>(key);

            // Then
            Assert.Equal(data, result);
        }

        [Fact]
        public async Task GetValueAsync_BadKey()
        {
            // Given
            var key = "some_key";
            var badKey = "bad_key";
            var data = "some_data";

            await _underTest.SetValueAsync(key, data, 1);

            // When
            var result = await _underTest.GetValueAsync<string>(badKey);

            // Then
            Assert.NotEqual(data, result);
        }

        [Fact]
        public async Task SetValueAsync_NormalFlow()
        {
            // Given
            var key = "some_key";
            var data = "some_data";

            // When
            await _underTest.SetValueAsync(key, data, 1);
            var result = await _underTest.GetValueAsync<string>(key);

            // Then
            Assert.Equal(data, result);
        }

        [Fact]
        public async Task SetValueAsync_BadKey()
        {
            // Given
            var key = "some_key";
            var badKey = "bad_key";
            var data = "some_data";


            // When
            await _underTest.SetValueAsync(key, data, 1);

            var result = await _underTest.GetValueAsync<string>(badKey);

            // Then
            Assert.NotEqual(data, result);
        }

        [Fact]
        public async Task RemoveDataAsync_NormalFlow()
        {
            // Given
            var key = "some_key";
            var data = "some_data";

            await _underTest.SetValueAsync(key, data, 1);
            
            // When
            await _underTest.RemoveDataAsync(key);

            var result = await _underTest.GetValueAsync<string>(key);

            // Then
            Assert.Null(result);
        }

        [Fact]
        public async Task RemoveDataAsync_BadKey()
        {
            // Given
            var key = "some_key";
            var badKey = "bad_key";
            var data = "some_data";

            await _underTest.SetValueAsync(key, data, 1);

            // When
            await _underTest.RemoveDataAsync(badKey);

            var result = await _underTest.GetValueAsync<string>(key);

            // Then
            Assert.Equal(data, result);
        }
    }
}
