using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using AE.Services.Services;
using AE.Services.Configuration;
using AE.Services.Dto;

namespace AE.Tests
{
    [TestFixture]
    public class ImagesServiceTests
    {
        private IConfiguration configuration;
        private IServiceProvider provider;

        [OneTimeSetUpAttribute]
        public void Setup() 
        {
            configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();

            var services = new ServiceCollection();
            services.AddImagesService(configuration);
            provider = services.BuildServiceProvider();
        }

        [Test]
        public async Task GetImages_Success()
        {
            var imagesService = provider.GetService<IImagesService>();
            var images = await imagesService.GetImages();

            Assert.That(images, Is.Not.Null);
            Assert.That(images.Pictures, Is.Not.Null);
            Assert.That(images.Pictures.Length, Is.GreaterThan(0));
        }
        
        [TestCase(2)]
        [TestCase(27)]
        public async Task GetImagesAtPage_Success(int page)
        {
            var imagesService = provider.GetService<IImagesService>();
            var images = await imagesService.GetImages(page);

            Assert.That(images, Is.Not.Null);
            Assert.That(images.Pictures, Is.Not.Null);

            if (page <= images.PageCount)
            {
                Assert.That(images.Pictures.Length, Is.GreaterThan(0));
            }
            else
            {
                Assert.That(images.Pictures.Length, Is.EqualTo(0));
            }
        }

        [Test]
        public async Task GetImageDetail_ValidId_ImageDetail()
        {
            var imagesService = provider.GetService<IImagesService>();

            var imageId = "be89995bc7886d5a7312";
            var detail = await imagesService.GetImage(imageId);

            Assert.That(detail, Is.Not.Null);
            Assert.That(detail.Id, Is.EqualTo(imageId));
        }
        
        [Test]
        public async Task GetImageDetail_InvalidValidId_Null()
        {
            var imagesService = provider.GetService<IImagesService>();

            var imageId = "asdasd";
            var detail = await imagesService.GetImage(imageId);

            Assert.That(detail, Is.Null);
        }

        [Test]
        public async Task GetImages_NotAuthenticated_AuthenticatedUsingRetryPolicy()
        {
            var imagesService = provider.GetService<IImagesService>();

            var images = await imagesService.GetImages();

            Assert.That(images, Is.Not.Null);
            Assert.That(images.Pictures, Is.Not.Null);
            Assert.That(images.Pictures.Length, Is.GreaterThan(0));
        }

        [Test]
        public void Search_SearchTerm_Picture()
        {
            var imagesCache = provider.GetService<IImagesServiceCache>();
            imagesCache.Set(new List<PictureDetail>
            {
                new PictureDetail
                {
                    Author = "VItaly",
                    Camera = "Nikkon"
                }
            });
            var imagesService = provider.GetService<IImagesService>();
            var images = imagesService.Search("ita");

            Assert.That(images, Is.Not.Empty);
            Assert.That(images.Length, Is.EqualTo(1));
        }
    }
}
