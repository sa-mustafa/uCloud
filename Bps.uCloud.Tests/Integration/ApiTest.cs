namespace Bps.uCloud.Tests.Integration
{
    using Bps.uCloud.Contracts.API;
    using Moq;
    using Refit;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// This class should test individual functions of API module in real world conditions.
    /// </summary>
    public class ApiTest
    {
        const string userId = "test";

        public ApiTest()
        {
            var api = RestService.For<IGalleryApi>("http://localhost:490/api");
            var gallery = new Contracts.Entities.Gallery
            {
                Name = "vegetables",
                UserId = userId
            };
            api.NewAsync(gallery).Wait();
        }

        [Fact]
        public async Task Hello_ShouldReturnHelloAsync()
        {
            // Arrange
            var api = RestService.For<ICommonApi>("http://localhost:490/api");
            // Act
            var response = await api.HelloAsync();
            //Assert
            Assert.Equal("hello", response);
        }

        [Fact]
        public async Task GalleryList_ShouldReturnListAsync()
        {
            // Arrange
            var api = RestService.For<IGalleryApi>("http://localhost:490/api");
            var gallery = new Contracts.Entities.Gallery
            {
                Name = "fruits",
                UserId = userId
            };
            // Act
            var response = await api.ListAsync();
            //Assert
            Assert.NotEmpty(response);
        }

        [Fact]
        public async Task GalleryNew_ShouldReturnIdAsync()
        {
            // Arrange
            var api = RestService.For<IGalleryApi>("http://localhost:490/api");
            var gallery = new Contracts.Entities.Gallery
            {
                Name = "fruits",
                UserId = userId
            };
            // Act
            var response = await api.NewAsync(gallery);
            //Assert
            Assert.Equal(gallery.Id, response);
        }

        [Fact]
        public async Task GalleryView_ShouldReturnGalleryAsync()
        {
            // Arrange
            var api = RestService.For<IGalleryApi>("http://localhost:490/api");
            // Act
            var response = await api.ViewAsync("fruits");
            //Assert
            Assert.NotEmpty(response.Id);
            Assert.Equal("fruits", response.Name);
            Assert.Null(response.Tag);
            Assert.Equal(userId, response.UserId);
        }

        [Fact]
        public async Task GalleryRemove_ShouldReturnIdAsync()
        {
            // Arrange
            var api = RestService.For<IGalleryApi>("http://localhost:490/api");
            // Act
            var response = await api.RemoveAsync("fruits");
            //Assert
            Assert.NotEmpty(response);
        }

        [Theory]
        [InlineData(@"C:\Users\Public\Documents\MVTec\HALCON-18.11-Progress\examples\images\food\vegetables\apple_1.png")]
        public async Task ItemsNew_ShouldReturnIdAsync(string filePath)
        {
            // Arrange
            var api = RestService.For<IItemsApi>("http://localhost:490/api");
            var fileInfo = new FileInfo(filePath);
            var format = string.Format($"image{Path.GetExtension(filePath)}").Replace('.', '/');
            var data = new Contracts.Data(format, Contracts.DataTypes.Image) { Size = (int)fileInfo.Length };
            var fileInfoPart = new FileInfoPart(fileInfo, Path.GetFileName(filePath), format);

            // Act
            //var response = await api.NewAsync("fruits", "apple", fileInfoPart, data);
            var response = await api.NewAsync("fruits", "apple", fileInfo, format, data.Size, data.Type);
            //Assert
            Assert.NotEmpty(response);
        }

        [Fact]
        public async Task ApiDetect_ShouldReturnIdAsync()
        {
            // Arrange
            var api = RestService.For<ICommonApi>("http://localhost:490/api");
            // Act
            var response = await api.DetectAsync("fruits", "apple");
            //Assert
            Assert.NotEmpty(response);
        }


    }
}
