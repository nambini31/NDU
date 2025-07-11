using Core.Entity.Exorabilis;
using Core.Entity.MyCore;
using Core.RequestPostGet;
using Core.ServiceEncryptor;
using Core.UserModels;
using Core.UserServices.Role;
using Core.ViewModel;
using Core.ViewModel.Exorabilis;
using DataProviders.BatchProvider;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using PdfSharp.Pdf.Filters;
using Services.BatchService;
using Services.DocumentService;
using Services.Filters;
using System.IO.Compression;
using static Org.BouncyCastle.Utilities.Test.FixedSecureRandom;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EDMS2025.Controllers
{
    [TypeFilter(typeof(AuthenticationFilter))]
    public class EDMSController : Controller
    {

        private readonly InterfaceBatchAppService interfaceBatchAppService;
        private readonly InterfaceDocumentAppService interfaceDocumentAppService;
        private readonly IConfiguration _configuration;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly IWebHostEnvironment _webhostEnvironment;
        private readonly IRoleService _roleService;
        private UserModel userConnected;
        private string request;

        public EDMSController(InterfaceBatchAppService interfaceBatchApp, InterfaceDocumentAppService interfaceDocumentApp, IConfiguration configuration, ICompositeViewEngine viewEngine, IWebHostEnvironment webHostEnvironment, IRoleService roleService)
        {
            this.interfaceBatchAppService = interfaceBatchApp;
            this.interfaceDocumentAppService = interfaceDocumentApp;
            _configuration = configuration;
            _viewEngine = viewEngine;
            this._webhostEnvironment = webHostEnvironment;
            _roleService = roleService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            var userSession = HttpContext.Session.GetString("user");
            if (!string.IsNullOrEmpty(userSession))
            {
                userConnected = JsonConvert.DeserializeObject<UserModel>(userSession);
            }
            request = $"{context.HttpContext.Request.Path.Value}{context.HttpContext.Request.QueryString}";
        }
        public IActionResult Index()
        {
            ViewBag.step = 15;
            return View();
        }

        [Route("Dashboard/{step}")]
        public IActionResult Dashboard(string step)
        {
            ViewBag.step = 15;
            return View("Index");
        }



        /// <summary>
        /// affichage tous ce qui est par batch avec ( step id crypter dans l'url)
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        [Route("BatchLoad/{step}")]
        public async Task<IActionResult> BatchLoadAsync(string step)
        {
            // on utilise UnescapeDataString pour echapper le % dans l'url

            if (Encryption.Decrypt(Uri.UnescapeDataString(step)) == "3") // batch indexation
            {
                ViewBag.scripts = "LoadBatch";
                ViewBag.step = 3;
                ViewData["Title"] = "Indexation";
            }
            else if (Encryption.Decrypt(Uri.UnescapeDataString(step)) == "5") // batch sanity check
            {
                ViewBag.scripts = "LoadBatch";
                ViewBag.step = 5;
                ViewData["Title"] = "Sanity Check";
            }
            else if (Encryption.Decrypt(Uri.UnescapeDataString(step)) == "2") // quality control
            {
                ViewBag.scripts = "LoadBatch";
                ViewBag.step = 2;
                ViewData["Title"] = "Quality Control";
            }

            else if (Encryption.Decrypt(Uri.UnescapeDataString(step)) == "11") // batch unlock
            {
                ViewBag.scripts = "LoadBatch";
                ViewBag.step = 11;
                ViewData["Title"] = "Unlock Batch";
            }
            else if (Encryption.Decrypt(Uri.UnescapeDataString(step)) == "12") // Btach rescan
            {
                ViewBag.scripts = "LoadBatch";
                ViewBag.step = 12;
                ViewData["Title"] = "Rescan Batch";
            }
            else if (Encryption.Decrypt(Uri.UnescapeDataString(step)) == "14")  // batch search
            {
                ViewBag.scripts = "LoadBatch";
                ViewBag.step = 14;
                ViewData["Title"] = "Batch Search";
            }
            List<ElementModel> dsd = _roleService.GetElementsByRoleId(userConnected.UserRoleId);
            bool containsStep1 = dsd.Any(element => element.Step == ViewBag.step ||
                (element.ChildElements != null && element.ChildElements.Any(child => child.Step == ViewBag.step))
            );
            if (!containsStep1) {
                return Redirect($"/user/login?accesDenied=True&next={request}");
            }


            /*
             --  toutes les types de reject dans la table rejectioncode
             --  extension de l'image;
             */
            var data = new BatchAndCodeRejectViewModel
            {
                RejectionCodeAll = await this.interfaceBatchAppService.getAllRejectionCode(),
                imageextension = interfaceBatchAppService.GetValueParam("imageExtension").Value
            };

            ViewBag.imageextension = data.imageextension.ToString();

            /*
            --  obtenir le bouton attacher au step
            */
            var elementsBoutton = new List<ElementBoutton>();
            if (step != null)
            {
                int stepInt = (int)(ViewBag.step);
                elementsBoutton = _roleService.GetElementsBouttonByStep(stepInt);
            }
            data.ElemBoutton = elementsBoutton;

            return View(data);

        }


        /// <summary>
        /// detail du batch : pour voir toutes les documents
        /// </summary>
        /// <param name="batchwithStep"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DetailBatch(BatchwithStepRequest batchwithStep)
        {
            // get all documents in the batch when clikck the icon 'view batch'
            var rsult = await interfaceDocumentAppService.getAllDocumentsByBatchWithStep(batchwithStep);

            // verouiller le batch pour que personne d'autre ne le touche

            if (batchwithStep.step == 2 || batchwithStep.step == 3 || batchwithStep.step == 5)
            {
            
                await interfaceBatchAppService.LockBatch(batchwithStep.batchId, userConnected.UserName);

            }

            //premier document afficher quand on entre dans le detail du batch
            ViewBag.firstdoc = rsult.firstIdDocEncrypt?.IdEncrypt.ToString();
            ViewBag.imageextension = interfaceBatchAppService.GetValueParam("imageExtension").Value;
            ViewBag.batchid = batchwithStep.batchId;


            if (batchwithStep.step == 3) // detail de l'indexation
            {
                ViewBag.scripts = "detailbatch";
                ViewBag.step = 3;
                ViewData["Title"] = $"Indexing - Batch Number {rsult?.Batch?.BatchNumber} -- File Number : {rsult?.Batch?.FileNumber}";
            }
            else if (batchwithStep.step == 5) // detail de batch dans la sanity
            {
                ViewBag.scripts = "detailbatch";
                ViewBag.step = 5;
                ViewData["Title"] = $"Sanity Check - Batch Number: {rsult?.Batch?.BatchNumber}  -- File Number :  {rsult?.Batch?.FileNumber}";
            }
            else if (batchwithStep.step == 2) // detail du batch dans quality control
            {
                ViewBag.scripts = "detailbatch";
                ViewBag.step = 2;
                ViewData["Title"] = $"Quality Control - Batch Number: {rsult?.Batch?.BatchNumber} -- File Number : {rsult?.Batch?.FileNumber}";
            }
            else if (batchwithStep.step == 12)
            {
                ViewBag.scripts = "detailbatch";
                ViewBag.step = 12;
                ViewData["Title"] = $"Rescan - Batch Number: {rsult?.Batch?.BatchNumber} -- File Number : {rsult?.Batch?.FileNumber}";
            }
            else if (batchwithStep.step == 11)
            {
                ViewBag.scripts = "detailbatch";
                ViewBag.step = 11;
                ViewData["Title"] = $"Unlock - Batch Number: {rsult?.Batch?.BatchNumber} -- File Number : {rsult?.Batch?.FileNumber}";
            }
            else if (batchwithStep.step == 14)
            {
                ViewBag.scripts = "detailbatch";
                ViewBag.step = 14;
                ViewData["Title"] = $"Search - Batch Number: {rsult?.Batch?.BatchNumber} -- File Number :  {rsult?.Batch?.FileNumber}";
            }

            return View(rsult);

        }

        /// <summary>
        /// orderind documents detail du batch : pour voir toutes les documents pour l'ordering
        /// </summary>
        /// <param name="batchwithStep"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> OrderDetailBatch(BatchwithStepRequest batchwithStep)
        {
            // get all documents in the batch when clikck the icon 'view batch'
            var rsult = await interfaceDocumentAppService.getAllDocumentsByBatchWithStep(batchwithStep);

            // verouiller le batch pour que personne d'autre ne le touche
            await interfaceBatchAppService.LockBatch(batchwithStep.batchId, userConnected.UserName);

            //premier document afficher quand on entre dans le detail du batch
            ViewBag.firstdoc = rsult.firstIdDocEncrypt?.IdEncrypt.ToString();
            ViewBag.imageextension = interfaceBatchAppService.GetValueParam("imageExtension").Value;
            ViewBag.batchid = batchwithStep.batchId;

            ViewData["Title"] = $"Ordering - Batch Number: {rsult?.Batch?.BatchNumber} , File Number : {rsult?.Batch?.FileNumber}";


            ViewBag.scripts = "detailbatch";
            ViewBag.step = batchwithStep.step;


            return View(rsult);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchwithStep"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> MakeCompletedBatch(BatchwithStepRequest batchwithStep)
        {
            batchwithStep.UserId = userConnected.Id;
            batchwithStep.UserName = userConnected.UserName;
            batchwithStep.webhostEnvironment = _webhostEnvironment;
            await interfaceBatchAppService.MakeCompletedBatch(batchwithStep);
            return Json(new { status = "success" });

        }

        [HttpPost]
        public async Task<IActionResult> RejectBatch(string batchid, long reason, string otherreason)
        {
            await interfaceBatchAppService.RejectBatch(batchid, reason, userConnected.Id, userConnected.UserName, otherreason);
            return Json(new { status = "success" });

        }

        [HttpPost]
        public async Task<IActionResult> DeletePages(bool isRecto, long idVerso, long idRecto , long step)
        {
            int retour = await interfaceDocumentAppService.DeletePages(isRecto, idVerso, idRecto);

            return Json(new { status = "success" , retour = retour , link = "/BatchLoad/" + Encryption.Encrypt(step.ToString()) });

        }

        [Route("DocumentReviewModificationSearch/{step}")]
        public async Task<IActionResult> DocumentReviewModificationSearch(string step)
        {
            var data = new BatchAndDocumentViewModel
            {
                DocumentIndexAll = await interfaceDocumentAppService.getAllIndexList(),
                RejectionCodeAll = await interfaceBatchAppService.getAllRejectionCode(),
                imageextension = interfaceBatchAppService.GetValueParam("imageExtension").Value
            };


            ViewBag.step = Convert.ToInt64(Encryption.Decrypt(Uri.UnescapeDataString(step)));
            ViewBag.scripts = "detailbatch";
            ViewBag.imageextension = data.imageextension.ToString();

            if (ViewBag.step == 8)
            {
                ViewData["Title"] = "Document Review";
            }
            else if (ViewBag.step == 9)
            {
                ViewData["Title"] = "Document Search";
            }
            else if (ViewBag.step == 10)
            {
                ViewData["Title"] = "Document Modification";
            }
            else if (ViewBag.step == 13)
            {
                ViewData["Title"] = "Document to Rescan";
            }

            List<ElementModel> dsd = _roleService.GetElementsByRoleId(userConnected.UserRoleId);
            bool containsStep1 = dsd.Any(element => element.Step == ViewBag.step ||
                (element.ChildElements != null && element.ChildElements.Any(child => child.Step == ViewBag.step))
            );
            if (!containsStep1) {
                return Redirect($"/user/login?accesDenied=True&next={request}");
            }

            var elementsBoutton = new List<ElementBoutton>();
            if (step != null)
            {
                int stepInt = (int)(ViewBag.step);
                elementsBoutton = _roleService.GetElementsBouttonByStep(stepInt);
            }
            data.ElemBoutton = elementsBoutton;

            return View(data);

        }

        [HttpPost]
        public async Task<IActionResult> getAllDocumentsReviewModificationSearch(DocumentSearchRequest searchRequest)
        {
            var rsult = await interfaceDocumentAppService.getAllDocumentsReview(searchRequest);

            var partialViewHtml = "";
            var elementsBoutton = new List<ElementBoutton>();
            if (searchRequest.step != null)
            {
                int step = (int)(searchRequest.step);
                elementsBoutton = _roleService.GetElementsBouttonByStep(step);
            }
            rsult.ElemBoutton = elementsBoutton;

            partialViewHtml = RenderPartialViewToString("TbodyDocumentReviewModificationSearch", rsult);


            return Json(new { vue = partialViewHtml });

        }

        [HttpPost]
        public async Task<IActionResult> Load_document_index_crop_sanity_control(BatchwithStepRequest batchwithStep)
        {
            var rsult = await interfaceDocumentAppService.documentinbatchAsync(batchwithStep);

            var firstdoc = rsult.firstIdDocEncrypt?.IdEncrypt.ToString();

            var elementsBoutton = new List<ElementBoutton>();
            if (batchwithStep.step != null)
            {
                int step = (int)(batchwithStep.step);
                elementsBoutton = _roleService.GetElementsBouttonByStep(step);
            }
            rsult.ElemBoutton = elementsBoutton;

            var partialViewHtml = RenderPartialViewToString("TbodyDetailBatch", rsult);

            return Json(new { vue = partialViewHtml, firstdoc = firstdoc });
        }

        [HttpPost]
        public async Task<IActionResult> Load_document_index_crop_sanity_control_order(BatchwithStepRequest batchwithStep)
        {
            var rsult = await interfaceDocumentAppService.documentinbatchOrder(batchwithStep);

            var firstdoc = rsult.firstIdDocEncrypt?.IdEncrypt.ToString();

            var elementsBoutton = new List<ElementBoutton>();
            if (batchwithStep.step != null)
            {
                int step = (int)(batchwithStep.step);
                elementsBoutton = _roleService.GetElementsBouttonByStep(step);
            }
            rsult.ElemBoutton = elementsBoutton;

            var partialViewHtml = RenderPartialViewToString("TbodyDetailBatch", rsult);

            return Json(new { vue = partialViewHtml, firstdoc = firstdoc });
        }

        [HttpPost]
        public async Task<IActionResult> SaveAllDocumentsReview(DocumentSearchRequest searchRequest)
        {
            searchRequest.userId = userConnected.Id;
            searchRequest.UserName = userConnected.UserName;

            var rsult = await interfaceDocumentAppService.SaveAllDocumentsReview(searchRequest);

            return Json(new { success = true });

        }

        [HttpPost]
        public async Task<IActionResult> RescanAllDocumentsReview(DocumentSearchRequest searchRequest)
        {
            searchRequest.userId = userConnected.Id;
            searchRequest.UserName = userConnected.UserName;

            var rsult = await interfaceDocumentAppService.RescanAllDocumentsReview(searchRequest);

            return Json(new { success = true });

        }

        [HttpPost]
        public async Task<IActionResult> ViewDocumentIndexing(string docId, long batchstep, int reorder)
        {
            var isCrop = interfaceBatchAppService.GetValueParam("isCrop").Value;
            var rsult = await interfaceDocumentAppService.getAllImageWithDocumentByDocId(docId);
            rsult.isCrop = isCrop;
            
            var elementsBoutton = new List<ElementBoutton>();

            if (batchstep != 0)
            {
                int step = (int)(batchstep);
                elementsBoutton = _roleService.GetElementsBouttonByStep(step);
            }
            rsult.ElemBoutton = elementsBoutton;

            var partialViewHtml = "";

            ViewBag.step = batchstep;

            //if ((((batchstep == 9 || batchstep == 12 || batchstep == 13 || batchstep == 11 || batchstep == 14) && reorder == 0) || reorder == 1) )
            //{
            //    partialViewHtml = RenderPartialViewToString("Showdocumentreadonly", rsult);
            //}
            //else
            //{
            //    if (rsult.doccumentImage.DocumentStep == 3)
            //    {

            //        partialViewHtml = RenderPartialViewToString("Showindexingdocument", rsult);



            //    }
            //    else if (rsult.doccumentImage.DocumentStep == 2)
            //    {

            //            partialViewHtml = RenderPartialViewToString("Showqualitycontroldocument", rsult);


            //    }
            //    else if (rsult.doccumentImage.DocumentStep == 5 || rsult.doccumentImage.DocumentStep == 8)
            //    {
            //        if (batchstep == 10)
            //        {
            //            rsult.doccumentImage.DocumentStep = 10;
            //        }

            //        if ((rsult.doccumentImage.DocumentStatus == 2 || rsult.doccumentImage.DocumentStatus == 5) && batchstep != 8)
            //        {
            //            partialViewHtml = RenderPartialViewToString("Showdocumentreadonly", rsult);
            //        }
            //        else
            //        { 
            //partialViewHtml = RenderPartialViewToString("Showqualitycontroldocument", rsult);
            //        }

            //    }
            //}

            partialViewHtml = RenderPartialViewToString("Showqualitycontroldocument", rsult);
            rsult.doccumentImage.DocumentStep = null;

            return Json(new { vue = partialViewHtml, data = rsult });

        }

        [HttpPost]
        public async Task<IActionResult> Load_dashboard(DocumentSearchRequest search)
        {
            var rsult = await interfaceDocumentAppService.Dashboard(search);

            var partialViewHtml = RenderPartialViewToString("Dashboard", rsult);

            return Json(new { vue = partialViewHtml, data = rsult });

        }

        [HttpPost]
        public async Task<IActionResult> ShowOtherNextDocument(string docId, string referenceNumber, long fileTypeId, string nextorprev , long step)
        {
            var rsult = await interfaceDocumentAppService.getImagesOfNextDocumentsIFMultiByDocIdNext(docId, referenceNumber, fileTypeId, nextorprev , step);
            rsult.doccumentImage.DocumentStep = null;
            return Json(new { data = rsult });

        }

        [HttpPost]
        public async Task<IActionResult> SaveIndexation(string docId, string referenceNumber, long fileTypeId, long type, long Project_folio_id, string nextorprev, string step, long reason, string otherreason)
        {
            if (step == "10")
            {
                var rsult = await interfaceDocumentAppService.SaveModification(docId, referenceNumber, fileTypeId, userConnected.Id, userConnected.UserName, reason, type, Project_folio_id, nextorprev);

                return Json(new { indexdocrestant = rsult, status = "OK" });
            }
            else
            {

                var rsult = await interfaceDocumentAppService.SaveIndexation(docId, referenceNumber, fileTypeId, userConnected.Id, userConnected.UserName, reason, type, Project_folio_id, nextorprev, otherreason , step);

                if (step != "8" && step != "9" && step != "10")
                {
                    return Json(new { indexdocrestant = rsult, status = "OK", link = "/BatchLoad/" + Encryption.Encrypt(step) });
                }
                else
                {
                    return Json(new { indexdocrestant = rsult, status = "OK" });

                }

            }



        }
        [HttpPost]
        public async Task<IActionResult> Saveordereddocuments(long batchId, List<ValueOrderRequest> orderData)
        {
           
                var rsult = await interfaceDocumentAppService.Saveordereddocuments(batchId , orderData);

                return Json(new { indexdocrestant = rsult, status = "OK" });
           

        }


        [HttpPost]
        public async Task<IActionResult> cropimage([FromForm] string docId,[FromForm] string referenceNumber,[FromForm] long fileTypeId,[FromForm] IFormFile image,[FromForm] bool isRecto,[FromForm] string BatchId)
        {
            try
            {
                if (image == null || image.Length == 0)
                {
                    return BadRequest("L'image est invalide.");
                }

                // Lire l'image en Base64 pour l'envoyer à ton service
                using (var ms = new MemoryStream())
                {
                    await image.CopyToAsync(ms);
                    var imageBase64 = Convert.ToBase64String(ms.ToArray());

                    // Envoi au service en conservant la logique existante
                    var result = await this.interfaceDocumentAppService.SaveImageCropped(
                        _configuration, _webhostEnvironment, docId, BatchId, referenceNumber, imageBase64, isRecto, userConnected.Id, userConnected.UserName);

                    return Json(new { data = result });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur serveur: {ex.Message}");
            }
        }


        [HttpPost]
        public async Task<JsonResult> Load_index_crop_sanity_control(BatchSearchRequest filter)
        {

            try
            {

                var data = await this.interfaceBatchAppService.getAllBatchByStep(filter);
                data.username = userConnected.UserName;
                ViewBag.step = filter.step;
                var elementsBoutton = new List<ElementBoutton>();

                if (filter.step != null)
                {
                    int step = (int)(filter.step);
                    elementsBoutton = _roleService.GetElementsBouttonByStep(step);
                }
                data.ElemBoutton = elementsBoutton;

                var partialViewHtml = RenderPartialViewToString("TbodyOfBatch", data);

                return Json(new { vue = partialViewHtml });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + ex.StackTrace);
            }

        }
        
        [HttpPost]
        public async Task<JsonResult> excel_for_batch(BatchSearchRequest filter)
        {

            try
            {

                var data = await this.interfaceBatchAppService.getAllBatchByStep(filter);
                data.username = userConnected.UserName;
                ViewBag.step = filter.step;
                var elementsBoutton = new List<ElementBoutton>();

                if (filter.step != null)
                {
                    int step = (int)(filter.step);
                    elementsBoutton = _roleService.GetElementsBouttonByStep(step);
                }
                data.ElemBoutton = elementsBoutton;

                var partialViewHtml = RenderPartialViewToString("TbodyOfBatch", data);

                return Json(new { vue = partialViewHtml });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + ex.StackTrace);
            }

        }

        [HttpPost]
        public async Task<IActionResult> IsBatchLocked(BatchwithStepRequest batchstep)
        {
            try
            {
                var (isLocked, lockedby) = await this.interfaceBatchAppService.IsBatchLocked(batchstep, userConnected.UserName);
                string urlredirect = "";
                if (!isLocked)
                {
                    urlredirect = "/EDMS/DetailBatch";

                }
                return Json(new { success = true, isLocked, urlredirect, userlock = lockedby });
            }
            catch (Exception)
            {
                throw;
            }

        }
        [HttpPost]
        public async Task<IActionResult> IsBatchLockedOrder(BatchwithStepRequest batchstep)
        {
            try
            {
                var (isLocked, lockedby) = await this.interfaceBatchAppService.IsBatchLocked(batchstep, userConnected.UserName);
                string urlredirect = "";
                if (!isLocked)
                {
                    urlredirect = "/EDMS/OrderDetailBatch";

                }
                return Json(new { success = true, isLocked, urlredirect, userlock = lockedby });
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpPost]
        public IActionResult ProcessFolder(string folderPath)
        {
            var isYear = interfaceBatchAppService.GetValueParam("YearCompletOnChangeName").Value.ToString();

            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
            {
                return BadRequest(new { status = "error", message = "Dossier introuvable." });
            }

            var pdfFiles = Directory.GetFiles(folderPath, "*.pdf", SearchOption.AllDirectories);

            if (pdfFiles.Length == 0)
            {
                return BadRequest(new { status = "error", message = "Aucun fichier PDF trouvé dans ce dossier." });
            }

            foreach (var filePath in pdfFiles)
            {
                string fileName = Path.GetFileName(filePath);

                // Retire "MU-M-" si déjà présent
                string cleanName = fileName.Replace("MU-M-", "");

                string[] parts = cleanName.Split('-', 2);
                if (parts.Length == 2)
                {
                    string rawYear = parts[0];
                    string rest = parts[1];

                    if (int.TryParse(rawYear, out int yearNum))
                    {
                        string newYear;
                        if (isYear == "1")
                        {
                            newYear = yearNum < 100 ? $"20{yearNum:D2}" : yearNum.ToString();
                        }
                        else
                        {
                            newYear = (yearNum % 100).ToString("D2");
                        }

                        string newFileName = $"MU-M-{newYear}-{rest}";
                        string currentDirectory = Path.GetDirectoryName(filePath);
                        string newPath = Path.Combine(currentDirectory, newFileName);

                        if (!fileName.Equals(newFileName, StringComparison.OrdinalIgnoreCase))
                        {
                            System.IO.File.Move(filePath, newPath);
                        }
                    }
                }
            }


            return Ok(new { status = "ok", message = "Fichiers PDF renommés et traités avec succès." });
        }

        private string RenderPartialViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var writer = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);
                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                viewResult.View.RenderAsync(viewContext).Wait();
                return writer.GetStringBuilder().ToString();
            }
        }

        [HttpPost]
        public IActionResult GetAllTrademarksWithPages(string folderPath)
        {

            var data =  this.interfaceDocumentAppService.GetAllTrademarksWithPages(folderPath);

            var partialViewHtml = RenderPartialViewToString("TbodyOfReport", data);

            return Json(new { vue = partialViewHtml });


        }

        [HttpPost]
        public IActionResult upload_excel(IFormFile excelFile)
        {

            var data = this.interfaceDocumentAppService.upload_excel(excelFile);

            //var partialViewHtml = RenderPartialViewToString("TbodyOfReport", data);

            return Json(new { vue = "jj" });


        }

    }
}
