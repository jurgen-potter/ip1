using CitizenPanel.BL.Domain.Draws;
using CitizenPanel.BL.Utilities;
using CitizenPanel.UI.MVC.Models.Draws;
using CitizenPanel.UI.MVC.Models.Panels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CitizenPanel.UI.MVC.Controllers.Draws;

public class RecruitmentController(IUtilityManager utilityManager) : Controller
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Index(int panelId)
    {
        var criteriaList = utilityManager.GetInitialCriteria();

        var model = new RecruitmentCriteriaViewModel
        {
            PanelId = panelId,
            TotalAvailablePotentialPanelmembers = 0,
            Criteria = new List<CriteriaViewModel>()
        };

        foreach (var criteria in criteriaList)
        {
            var criteriaVm = new CriteriaViewModel
            {
                Id = criteria.Id,
                Name = criteria.Name,
                SubCriteria = new List<SubCriteriaViewModel>()
            };

            foreach (var subcriteria in criteria.SubCriteria)
            {
                var subCriteriaVm = new SubCriteriaViewModel
                {
                    Id = subcriteria.Id,
                    Name = subcriteria.Name,
                    Percentage = subcriteria.Percentage
                };

                criteriaVm.SubCriteria.Add(subCriteriaVm);
            }

            model.Criteria.Add(criteriaVm);
        }
        
        return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    public IActionResult Calculate(RecruitmentCriteriaViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        var criteria = new List<Criteria>();

        for (int i = 0; i < model.Criteria.Count; i++)
        {
            var cvm = model.Criteria[i];
            var cr = new Criteria
            {
                Id = cvm.Id,
                Name = cvm.Name,
                SubCriteria = new List<SubCriteria>()
            };

            for (int j = 0; j < cvm.SubCriteria.Count; j++)
            {
                var svm = cvm.SubCriteria[j];
                var subCr = new SubCriteria
                {
                    Id = svm.Id,
                    Name = svm.Name,
                    Percentage = svm.Percentage
                };
                cr.SubCriteria.Add(subCr);
            }

            criteria.Add(cr);
        }
        var result = utilityManager.CalculateRecruitment(model.TotalAvailablePotentialPanelmembers, criteria);

        var resultModel = new ResultViewModel()
        {
            TotalAvailablePotentialPanelmembers = model.TotalAvailablePotentialPanelmembers,
            TotalNeededInvitations = result.TotalNeededInvitations,
            TotalNeededPanelmembers = result.TotalNeededPanelmembers,
            Criteria = model.Criteria
        };

        foreach (var bucket in result.Buckets)
        {
            resultModel.Buckets.Add(new BucketViewModel()
            {
                Count = bucket.Count,
                CriteriaNames = bucket.CriteriaNames,
                SubCriteriaNames = bucket.SubCriteriaNames
            });
        }
        return View("Result", resultModel);
    }
    
    [HttpPost]
    [AllowAnonymous]
    public IActionResult ReadResult (ResultViewModel resultViewModel)
    {
        if (!ModelState.IsValid)
        {
            return View("Result", resultViewModel);
        }
        
        TempData["ResultViewModel"] = JsonConvert.SerializeObject(resultViewModel);

        return RedirectToAction("CreatePanelFromResult", "Recruitment");
    }
    
    [HttpGet]
    [Authorize(Roles = "Organization")]
    public IActionResult CreatePanelFromResult()
    {
        if (!TempData.ContainsKey("ResultViewModel"))
        {
            return RedirectToAction("Index", "Recruitment");
        }

        var resultModelJson = TempData["ResultViewModel"] as string;
        var resultViewModel = JsonConvert.DeserializeObject<ResultViewModel>(resultModelJson);

        var model = new CreatePanelViewModel()
        {
            Result = resultViewModel
        };

        return View("~/Views/Panel/CreatePanel.cshtml", model);
    }
}