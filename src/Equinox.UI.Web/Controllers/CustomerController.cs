using Equinox.Application.Interfaces;
using Equinox.Application.ViewModels;
using Equinox.Infra.CrossCutting.Identity.Authorization;
using Equinox.UI.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Equinox.UI.Web.Controllers
{
    [Authorize]
    public class CustomerController : BaseController
    {
        private readonly ICustomerAppService _customerAppService;

        public CustomerController(ICustomerAppService customerAppService)
        {
            _customerAppService = customerAppService;
        }

        [AllowAnonymous]
        [HttpGet("customer-management/list-paginated")]
        public async Task<IActionResult> Index(int pageNumber =1, int pageSize = 10)
        {
            var customerList = await _customerAppService.GetPaginatedList(pageNumber, pageSize);
            var totalCustomerCount = await _customerAppService.GetCustomerCount();

            return View(await PaginatedList<CustomerViewModel>.CreateAsync( customerList, totalCustomerCount, pageNumber, pageSize));
        }

        [AllowAnonymous]
        [HttpGet("customer-management/customer-details/{id:guid}")]
        public async Task<IActionResult> Details(Guid? id, int pageNumber)
        {
            if (id == null) return NotFound();

            TempData["pageIndex"] = pageNumber;
            var customerViewModel = await _customerAppService.GetById(id.Value);

            if (customerViewModel == null) return NotFound();

            return View(customerViewModel);
        }

        [CustomAuthorize("Customers", "Write")]
        [HttpGet("customer-management/register-new")]
        public IActionResult Create()
        {
            return View();
        }

        [CustomAuthorize("Customers", "Write")]
        [HttpPost("customer-management/register-new")]
        public async Task<IActionResult> Create(CustomerViewModel customerViewModel, int pageNumber)
        {
            if (!ModelState.IsValid) return View(customerViewModel);
            
            if (ResponseHasErrors(await _customerAppService.Register(customerViewModel)))
                return View(customerViewModel);

            ViewBag.Sucesso = "Customer Registered!";
            TempData["pageIndex"] = pageNumber;
            return View(customerViewModel);
        }

        [CustomAuthorize("Customers", "Write")]
        [HttpGet("customer-management/edit-customer/{id:guid}")]
        public async Task<IActionResult> Edit(Guid? id, int pageNumber)
        {
            if (id == null) return NotFound();

            var customerViewModel = await _customerAppService.GetById(id.Value);

            if (customerViewModel == null) return NotFound();

            TempData["pageIndex"] = pageNumber;

            return View(customerViewModel);
        }

        [CustomAuthorize("Customers", "Write")]
        [HttpPost("customer-management/edit-customer/{id:guid}")]
        public async Task<IActionResult> Edit(CustomerViewModel customerViewModel, int pageNumber)
        {
            if (!ModelState.IsValid) return View(customerViewModel);
            
            if (ResponseHasErrors(await _customerAppService.Update(customerViewModel)))
                return View(customerViewModel);

            ViewBag.Sucesso = "Customer Updated!";
            TempData["pageIndex"] = pageNumber;

            return View(customerViewModel);
        }

        [CustomAuthorize("Customers", "Remove")]
        [HttpGet("customer-management/remove-customer/{id:guid}")]
        public async Task<IActionResult> Delete(Guid? id, int pageNumber)
        {
            if (id == null) return NotFound();

            var customerViewModel = await _customerAppService.GetById(id.Value);

            if (customerViewModel == null) return NotFound();

            TempData["pageIndex"] = pageNumber;

            return View(customerViewModel);
        }

        [CustomAuthorize("Customers", "Remove")]
        [HttpPost("customer-management/remove-customer/{id:guid}"), ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id, int pageNumber)
        {
            if (ResponseHasErrors(await _customerAppService.Remove(id)))
                return View(await _customerAppService.GetById(id));

            ViewBag.Sucesso = "Customer Removed!";

            TempData["pageIndex"] = pageNumber;

            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        [HttpGet("customer-management/customer-history/{id:guid}")]
        public async Task<JsonResult> History(Guid id)
        {
            var customerHistoryData = await _customerAppService.GetAllHistory(id);

            return Json(customerHistoryData);
        }
    }
}
