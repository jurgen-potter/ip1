using CitizenPanel.BL;
using CitizenPanel.BL.Domain.Draw;
using Microsoft.AspNetCore.Mvc;
using CitizenPanel.UI.MVC.Models;
using System.Collections.Generic;
using System.Linq;

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

        var criteria = _drawManager.GetCriteriaByPanel(model.PanelId);
        
        var result = _drawManager.CalculateRecruitment(model.TotalAvailablePotentialPanelmembers,criteria);
        
        
        return View("Result", result);
    }

    [HttpPost]
    public IActionResult AddCriteria(RecruitmentCriteriaViewModel model)
    {
        _drawManager.addCriteria(model.PanelId, model.Criteria);
        
        return RedirectToAction("Index", new { panelId });
    }
    
    [HttpPost]
    public IActionResult RemoveCriteria(int panelId, int criteriaId)
    {
        
        _drawManager.RemoveCriteria(panelId, criteriaId);
        
        return RedirectToAction("Index", new { panelId });
    }

    [HttpPost]
    public IActionResult RemoveSubCriteria(int panelId, int criteriaId, int subCriteriaId)
    {
        _drawManager.RemoveSubCriteria(panelId, criteriaId ,subCriteriaId);
        
        return RedirectToAction("Index", new { panelId });
    }
}