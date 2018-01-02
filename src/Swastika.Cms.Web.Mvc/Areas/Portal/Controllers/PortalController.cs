﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Swastika.Cms.Mvc.Controllers;
using Microsoft.AspNetCore.Hosting;
using Swastika.Cms.Lib;
using Swastika.Cms.Lib.ViewModels;
using Swastika.Cms.Lib.ViewModels.Info;

namespace Swastika.Cms.Mvc.Areas.Portal.Controllers
{
    [Authorize]
    [Area("Portal")]
    [Route("{culture}/Portal")]
    public class PortalController : BaseController<PortalController>
    {
        public PortalController(IHostingEnvironment env)
            : base(env)
        {
        }

        [Route("")]
        public IActionResult Index()
        {
            return RedirectToAction("", "Pages");
            //return View();
        }


        /// <summary>
        /// Searches the specified keyword.
        /// </summary>
        /// <param name="keyword">The keyword.</param>
        /// <param name="searchType">Type of the search Ex: All / Article / Module / Page .</param>
        /// <returns></returns>
        [Route("Search")]
        public async Task<IActionResult> Search(string keyword, SWCmsConstants.SearchType searchType)
        {
            switch (searchType)
            {
                case SWCmsConstants.SearchType.All:
                    ViewData["Articles"] = await InfoArticleViewModel.Repository.GetModelListByAsync(
                        c => c.Specificulture == _lang && (c.Title.Contains(keyword) || c.Excerpt.Contains(keyword) || c.Content.Contains(keyword)));
                    ViewData["Pages"] = InfoCategoryViewModel.Repository.GetModelListBy(
                        c => c.Specificulture == _lang 
                        && (c.Title.Contains(keyword) || c.Excerpt.Contains(keyword)));
                    ViewData["Modules"] = InfoModuleViewModel.Repository.GetModelListBy(
                        c => c.Specificulture == _lang && (c.Title.Contains(keyword) || 
                        c.Description.Contains(keyword)));
                    break;
                case SWCmsConstants.SearchType.Article:
                    ViewData["Articles"] = await InfoArticleViewModel.Repository.GetModelListByAsync(
                        c => c.Specificulture == _lang && (c.Title.Contains(keyword) || c.Excerpt.Contains(keyword) || c.Content.Contains(keyword)));
                    break;
                case SWCmsConstants.SearchType.Module:
                    ViewData["Modules"] = InfoModuleViewModel.Repository.GetModelListBy(
                        c => c.Specificulture == _lang && (c.Title.Contains(keyword) || c.Description.Contains(keyword)));
                    break;
                case SWCmsConstants.SearchType.Page:
                    ViewData["Pages"] = InfoCategoryViewModel.Repository.GetModelListBy(
                        c => c.Specificulture == _lang 
                        && (c.Title.Contains(keyword) || c.Excerpt.Contains(keyword)));
                    break;
                default:
                    break;
            }
            ViewBag.searchType = searchType;
            return View();
        }
    }
}