using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using PollyTest;
using PollyTest.Services;

namespace Tests
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
            services.ConfigureService(configuration);
            provider = services.BuildServiceProvider();
        }


        [Test]
        public async Task GetImages_Success()
        {
            var imagesService = provider.GetService<IImagesService>();
            await imagesService.CreateAccessToken();

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
            await imagesService.CreateAccessToken();

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
            await imagesService.CreateAccessToken();

            var imageId = "be89995bc7886d5a7312";
            var detail = await imagesService.GetImage(imageId);

            Assert.That(detail, Is.Not.Null);
            Assert.That(detail.Id, Is.EqualTo(imageId));
        }
        
        [Test]
        public async Task GetImageDetail_InvalidValidId_Null()
        {
            var imagesService = provider.GetService<IImagesService>();
            await imagesService.CreateAccessToken();

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
        public async Task CreateAccessToken_TokenCreated()
        {
            var imagesService = provider.GetService<IImagesService>();
            var token = await imagesService.CreateAccessToken();

            Assert.That(token, Is.Not.Null);
        }
    }
}
