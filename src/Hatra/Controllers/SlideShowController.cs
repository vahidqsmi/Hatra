﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using DNTBreadCrumb.Core;
using DNTCommon.Web.Core;
using Hatra.Common.GuardToolkit;
using Hatra.Services.Contracts;
using Hatra.Services.Identity;
using Hatra.ViewModels;
using Hatra.ViewModels.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hatra.Controllers
{
    [Authorize(Policy = ConstantPolicies.DynamicPermission)]
    [BreadCrumb(UseDefaultRouteUrl = true, Order = 0)]
    [DisplayName("مدیریت اسلاید شو ها")]
    public class SlideShowController : Controller
    {
        private readonly ISlideShowService _slideShowService;

        private const string RequestNotFound = "اسلاید شو درخواستی یافت نشد.";

        public SlideShowController(ISlideShowService slideShowService)
        {
            _slideShowService = slideShowService;
            _slideShowService.CheckArgumentIsNull(nameof(_slideShowService));
        }

        [DisplayName("ایندکس")]
        [BreadCrumb(Title = "ایندکس", Order = 1)]
        public async Task<IActionResult> Index()
        {
            var viewModels = await _slideShowService.GetAllAsync();

            return View(viewModels);
        }

        [HttpGet]
        [DisplayName("نمایش فرم اسلاید شو جدید")]
        [BreadCrumb(Order = 1)]
        public async Task<IActionResult> RenderCreate()
        {
            var viewModel = new SlideShowViewModel();

            return View("Create", viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [DisplayName("ایجاد یک اسلاید شو جدید")]
        public async Task<IActionResult> Create(SlideShowViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (await _slideShowService.CheckExistTitleAsync(viewModel.Id, viewModel.Title))
                {
                    ModelState.AddModelError(nameof(viewModel.Title), "عنوان وارد شده تکراری است");
                    return View(viewModel);
                }

                var result = await _slideShowService.InsertAsync(viewModel);
                if (result)
                {
                    return RedirectToAction("Index", "SlideShow");
                }

                return View(viewModel);
            }

            return View(viewModel);
        }

        [HttpGet]
        [DisplayName("نمایش فرم ویرایش اسلاید شو")]
        [BreadCrumb(Order = 1)]
        public async Task<IActionResult> RenderEdit(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var viewModel = await _slideShowService.GetByIdAsync(id.GetValueOrDefault());

            if (viewModel == null)
            {
                return NotFound();
            }

            return View("Edit", viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [DisplayName("ویرایش اسلاید شو")]
        public async Task<IActionResult> Edit(SlideShowViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (await _slideShowService.CheckExistTitleAsync(viewModel.Id, viewModel.Title))
                {
                    ModelState.AddModelError(nameof(viewModel.Title), "عنوان وارد شده تکراری است");
                    return View(viewModel);
                }

                var result = await _slideShowService.UpdateAsync(viewModel);
                if (result)
                {
                    return RedirectToAction("Index", "SlideShow");
                }

                return View(viewModel);
            }

            return View(viewModel);
        }

        [AjaxOnly]
        [DisplayName("نمایش فرم حذف اسلاید شو")]
        public async Task<IActionResult> RenderDelete([FromBody]ModelIdViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model?.Id))
            {
                return PartialView("_Delete");
            }

            var viewModel = await _slideShowService.GetByIdAsync(Convert.ToInt32(model.Id));
            if (viewModel == null)
            {
                ModelState.AddModelError("", RequestNotFound);
                return PartialView("_Delete");
            }

            return PartialView("_Delete", model: viewModel);
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [DisplayName("حذف اسلاید شو")]
        public async Task<IActionResult> Delete(SlideShowViewModel viewModel)
        {
            var slideShowViewModel = await _slideShowService.GetByIdAsync(viewModel.Id);
            if (slideShowViewModel == null)
            {
                ModelState.AddModelError("", RequestNotFound);
            }
            else
            {
                var result = await _slideShowService.DeleteAsync(slideShowViewModel.Id);
                if (result)
                {
                    return Json(new { success = true });
                }

                ModelState.AddModelError("", RequestNotFound);
            }

            return PartialView("_Delete", model: viewModel);
        }

        /// <summary>
        /// For [Remote] validation
        /// </summary>
        [AjaxOnly, HttpPost, ValidateAntiForgeryToken]
        [DisplayName("اعتبار سنجی عنوان")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> ValidateTitle(string title, int id)
        {
            var result = await _slideShowService.CheckExistTitleAsync(id, title);
            return Json(result ? "عنوان وارد شده تکراری است" : "true");
        }
    }
}