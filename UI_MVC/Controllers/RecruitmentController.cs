using CitizenPanel.BL;
using CitizenPanel.BL.Domain.Draw;
using Microsoft.AspNetCore.Mvc;
using CitizenPanel.UI.MVC.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Newtonsoft.Json;

namespace CitizenPanel.UI.MVC.Controllers;

public class RecruitmentController : Controller
{
    private readonly IDrawManager _drawManager;

    public RecruitmentController(IDrawManager drawManager)
    {
        _drawManager = drawManager;
    }

    [HttpGet]
    public IActionResult Index(int panelId = 1)
    {
        if (TempData["CriteriaFormData"] is string json)
        {
            var tModel = JsonConvert.DeserializeObject<RecruitmentCriteriaViewModel>(json);
            return View(tModel); 
        }
        
        var criteriaList = _drawManager.GetCriteriaByPanel(panelId);

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

        return View("Result", result);
    }
    
    [Authorize(Roles = "Organization")]
    [HttpPost]
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