using CitizenPanel.BL;
using CitizenPanel.BL.Domain.Draw;
using Microsoft.AspNetCore.Mvc;
using CitizenPanel.UI.MVC.Models;

namespace CitizenPanel.UI.MVC.Controllers;

public class RecruitmentController : Controller
{
    private readonly IDrawManager _drawManager;
    private readonly IPanelManager _panelManager;
    // private readonly IMemberManager _memberManager;

    public RecruitmentController(IDrawManager drawManager, IPanelManager panelManager)
    {
        _drawManager = drawManager;
        _panelManager = panelManager;
        // _memberManager = memberManager;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Index(int panelId)
    {
        var criteriaList = _drawManager.GetInitialCriteria();

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
        var result = _drawManager.CalculateRecruitment(model.TotalAvailablePotentialPanelmembers, criteria);

        var resultModel = new ResultViewModel()
        {
            TotalAvailablePotentialPanelmembers = model.TotalAvailablePotentialPanelmembers,
            ReservePotPanelmembers = result.ReservePotPanelmembers,
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
    [Authorize(Roles = "Organization")]
    public IActionResult Save(RecruitmentCriteriaViewModel model)
    {
        
        ModelState.Clear(); //voor validatiefouten te omzeilen, je mag opslaan!

        var domainCriteria = new List<Criteria>();

        for (int i = 0; i < model.Criteria.Count; i++)
        {
            var cvm = model.Criteria[i];
            var criteria = new Criteria
            {
                Id = cvm.Id,
                Name = cvm.Name,
                SubCriteria = new List<SubCriteria>()
            };

            for (int j = 0; j < cvm.SubCriteria.Count; j++)
            {
                var svm = cvm.SubCriteria[j];
                var subCriteria = new SubCriteria
                {
                    Id = svm.Id,
                    Name = svm.Name,
                    Percentage = svm.Percentage
                };

                criteria.SubCriteria.Add(subCriteria);
            }

            domainCriteria.Add(criteria);
        }

        _drawManager.EditCriteria(model.PanelId, domainCriteria);

        return RedirectToAction(nameof(Index), new { panelId = model.PanelId });
    }
}