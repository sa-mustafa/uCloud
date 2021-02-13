/*
 * uCloud API collection
 *
 * uCloud is a collection of image APIs.
 *
 * OpenAPI spec version: 1.0.2
 * Contact: help@bps-eng.co.ir
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */

namespace Bps.uCloud.API.Controllers
{
    using Bps.uCloud.API.Attributes;
    using Bps.uCloud.API.Models;
    using Bps.uCloud.API.Settings;
    using Bps.uCloud.Contracts.Entities;
    using MassTransit;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using MongoDB.Driver;
    using Newtonsoft.Json;
    using Swashbuckle.AspNetCore.Annotations;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using static Bps.uCloud.API.Extensions;

    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public class ImageApiController : ControllerBase
    {
        #region Fields

        IAppSettings app;
        IBusControl bus;
        ISendEndpoint busSend;
        IMongoDatabase db;
        IMongoCollection<Item> images;
        ILogger<ImageApiController> logger;
        string userId = "test";
        static JsonSerializerSettings jsonOption = new JsonSerializerSettings { NullValueHandling =  NullValueHandling.Ignore };

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageApiController"/> class.
        /// </summary>
        /// <param name="app">The application settings.</param>
        /// <param name="bus">The masstransit bus.</param>
        /// <param name="db">The mongo database object.</param>
        /// <param name="logger">The logger object.</param>
        public ImageApiController(IAppSettings app, IBusControl bus, IMongoDatabase db, ILogger<ImageApiController> logger)
        {
            this.app = app;
            this.bus = bus;
            this.db = db;
            this.logger = logger;

            using var endpoint = bus.GetSendEndpoint(SuggestQueueName("Image", "1.0", app));
            endpoint.Wait();
            busSend = endpoint.Result;

            images = db.GetCollection<Item>(nameof(Item));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Detect barcodes
        /// </summary>
        /// <remarks>Detect and read barcodes found in the image.</remarks>
        /// <param name="image">Image data for further processing</param>
        /// <param name="codeType">Specify the barcode type.</param>
        /// <param name="darkOnLight">Specify the barcode polarity.</param>
        /// <param name="barcodeHeightMin">Specify minimum barcode height.</param>
        /// <param name="barcodeWidthMin">Specify minimum barcode width.</param>
        /// <param name="elementSizeMin">Specify minimum element size.</param>
        /// <param name="elementSizeMax">Specify maximum element size.</param>
        /// <param name="minCodeLength">Specify expected minimum code length.</param>
        /// <param name="minContrast">Specify minimum barcode contrast.</param>
        /// <param name="minIdenticalScanlines">Specify minimum number of identical scanlines.</param>
        /// <param name="orientation">Specify expected barcode orientation.</param>
        /// <param name="orientationTolerance">Specify barcode orientation tolerance.</param>
        /// <response code="200">Successful operation</response>
        [HttpPost]
        [Route("/ucloud/v1/image/barcode")]
        [ValidateModelState]
        [SwaggerOperation("Barcode")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<Barcode>), description: "Successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(ApiResponse), description: "Invalid parameter(s) supplied")]
        [SwaggerResponse(statusCode: 422, type: typeof(ApiResponse), description: "Invalid input")]
        public virtual async Task<IActionResult> Barcode([FromBody]Image image, [FromQuery][Required()]string codeType, [FromQuery]bool? darkOnLight, [FromQuery]int? barcodeHeightMin, [FromQuery]int? barcodeWidthMin, [FromQuery][Range(0.6, 64)]float? elementSizeMin, [FromQuery][Range(1, 256)]float? elementSizeMax, [FromQuery]decimal? minCodeLength, [FromQuery]decimal? minContrast, [FromQuery]int? minIdenticalScanlines, [FromQuery][Range(-180, 180)]float? orientation, [FromQuery][Range(0, 90)]float? orientationTolerance)
        {
            var id = Guid.NewGuid().ToString();
            logger.LogTrace("Detecting barcode for image id {0}:{1}.", id, image.Name);
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse { Code = 400, Id = image.Name, Message = "Invalid parameter(s) supplied" });

            if (!TryValidateModel(image, nameof(Image)))
                return UnprocessableEntity(new ApiResponse { Code = 422, Id = image.Name, Message = "Invalid input: image is invalid." });

            var item = new Item(image) { UserId = userId };
            // serialize and anonymous object based on query objects.
            item.Tag = JsonConvert.SerializeObject(new
            {
                codeType,
                darkOnLight,
                barcodeHeightMin,
                barcodeWidthMin,
                elementSizeMin,
                elementSizeMax,
                minCodeLength,
                minContrast,
                minIdenticalScanlines,
                orientation,
                orientationTolerance
            }, Formatting.None, jsonOption);
            //await images.InsertOneAsync(item);
            var result = await bus.Request<IItem, Barcode>(item);

            return Ok();
        }

        /// <summary>
        /// Detect datacodes
        /// </summary>
        /// <remarks>Detect and read datacodes (2D barcodes) found in the image.</remarks>
        /// <param name="image">Image data for further processing</param>
        /// <param name="codeType">Specify the barcode type.</param>
        /// <param name="recognition">Specify the recognition parameters.</param>
        /// <param name="modelTypeAztec">Specify Aztec model type.</param>
        /// <param name="symbolSizeMinAztec">Specify minimum symbol size for Aztec model.</param>
        /// <param name="symbolSizeMaxAztec">Specify maximum symbol size for Aztec model.</param>
        /// <param name="symbolColsMinEcc">Specify minimum symbol columns for ECC 200.</param>
        /// <param name="symbolColsMaxEcc">Specify maximum symbol columns for ECC 200.</param>
        /// <param name="symbolRowsMinEcc">Specify minimum symbol rows for ECC 200.</param>
        /// <param name="symbolRowsMaxEcc">Specify maximum symbol rows for ECC 200.</param>
        /// <param name="modelTypeQr">Specify QR model type. 0 means both types 1 &amp; 2.</param>
        /// <param name="symbolSizeMinQr">Specify minimum symbol size for QR model.</param>
        /// <param name="symbolSizeMaxQr">Specify maximum symbol size for QR model.</param>
        /// <param name="symbolSizeMinUqr">Specify minimum symbol size for micro QR models.</param>
        /// <param name="symbolSizeMaxUqr">Specify maximum symbol size for micro QR models.</param>
        /// <param name="symbolColsMinPdf">Specify minimum symbol columns for PDF-417 models.</param>
        /// <param name="symbolColsMaxPdf">Specify maximum symbol columns PDF-417 models.</param>
        /// <param name="symbolRowsMinPdf">Specify minimum symbol rows for PDF-417 models.</param>
        /// <param name="symbolRowsMaxPdf">Specify maximum symbol rows PDF-417 models.</param>
        /// <param name="contrastMin">Specify minimum datacode contrast.</param>
        /// <param name="contrastTolerance">Specify datacode contrast tolerance.</param>
        /// <param name="darkOnLight">Specify the datacode polarity.</param>
        /// <param name="moduleGapMin">Specify how big minimum gaps in the symbol are.</param>
        /// <param name="moduleGapMax">Specify how big maximum gaps in the symbol are.</param>
        /// <param name="slantMax">Specify maximum datacode slant from ideal right angle in degrees.</param>
        /// <response code="200">Successful operation</response>
        [HttpPost]
        [Route("/ucloud/v1/image/datacode")]
        [ValidateModelState]
        [SwaggerOperation("Datacode")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<Barcode>), description: "Successful operation")]
        public virtual async Task<IActionResult> Datacode([FromBody]Image image, [FromQuery][Required()]string codeType, [FromQuery]string recognition, [FromQuery]List<string> modelTypeAztec, [FromQuery][Range(11, 151)]decimal? symbolSizeMinAztec, [FromQuery][Range(11, 151)]decimal? symbolSizeMaxAztec, [FromQuery][Range(10, 144)]int? symbolColsMinEcc, [FromQuery][Range(10, 144)]int? symbolColsMaxEcc, [FromQuery][Range(8, 144)]int? symbolRowsMinEcc, [FromQuery][Range(8, 144)]int? symbolRowsMaxEcc, [FromQuery]int? modelTypeQr, [FromQuery][Range(21, 177)]decimal? symbolSizeMinQr, [FromQuery][Range(21, 177)]decimal? symbolSizeMaxQr, [FromQuery][Range(11, 17)]decimal? symbolSizeMinUqr, [FromQuery][Range(11, 17)]decimal? symbolSizeMaxUqr, [FromQuery][Range(1, 30)]decimal? symbolColsMinPdf, [FromQuery][Range(1, 30)]decimal? symbolColsMaxPdf, [FromQuery][Range(3, 90)]decimal? symbolRowsMinPdf, [FromQuery][Range(3, 90)]decimal? symbolRowsMaxPdf, [FromQuery]decimal? contrastMin, [FromQuery]string contrastTolerance, [FromQuery]bool? darkOnLight, [FromQuery]string moduleGapMin, [FromQuery]string moduleGapMax, [FromQuery][Range(0, 30)]float? slantMax)
        { 
            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(List<Barcode>));
            string exampleJson = null;
            exampleJson = "[ {\n  \"data\" : \"data\",\n  \"location\" : \"location\",\n  \"type\" : \"type\"\n}, {\n  \"data\" : \"data\",\n  \"location\" : \"location\",\n  \"type\" : \"type\"\n} ]";
            
                        var example = exampleJson != null
                        ? JsonConvert.DeserializeObject<List<Barcode>>(exampleJson)
                        : default(List<Barcode>);            //TODO: Change the data returned
            return new ObjectResult(example);
        }

        #endregion

    }
}
