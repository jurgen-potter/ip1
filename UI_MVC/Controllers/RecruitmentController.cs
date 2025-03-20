using CitizenPanel.BL;
using CitizenPanel.BL.Domain.Draw;
using Microsoft.AspNetCore.Mvc;
using CitizenPanel.UI.MVC.Models;

namespace CitizenPanel.UI.MVC.Controllers;

public class RecruitmentController : Controller
{
    private readonly IDrawManager _drawManager;

    public RecruitmentController(IDrawManager drawManager)
    {
        _drawManager = drawManager;
    }
    
    [HttpGet]
    public IActionResult Index()
    {
        return View(new RecruitmentCriteriaViewModel());
    }

    [HttpPost]
    public IActionResult Calculate(RecruitmentCriteriaViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }
        
        var result = _drawManager.CalculateRecruitment(model.TotalAvailablePotentialPanelmembers, model.MalePercentage, model.FemalePercentage, model.Age18_25Percentage, model.Age26_40Percentage, model.Age41_60Percentage, model.Age60PlusPercentage, model.ExtraCriteria);

        return View("Result", result);
    }

    [HttpPost]
    public IActionResult AddCustomCriteria(RecruitmentCriteriaViewModel model)
    {
        model.ExtraCriteria.Add(new ExtraCriteria());
        return View("Index", model);
    }

    [HttpPost]
    public IActionResult AddSubCriteria(RecruitmentCriteriaViewModel model, int criteriaIndex)
    {
        model.ExtraCriteria[criteriaIndex].SubCriteria.Add(new SubCriteria());
        return View("Index", model);
    }

    [HttpPost]
    public IActionResult RemoveCustomCriteria(RecruitmentCriteriaViewModel model, int criteriaIndex)
    {
        model.ExtraCriteria.RemoveAt(criteriaIndex);
        return View("Index", model);
    }

    [HttpPost]
    public IActionResult RemoveSubCriteria(RecruitmentCriteriaViewModel model, int criteriaIndex, int subIndex)
    {
        model.ExtraCriteria.RemoveAll(c => string.IsNullOrWhiteSpace(c.Name));
        // model.ExtraCriteria[criteriaIndex].SubCriteria.RemoveAt(subIndex);
        return View("Index", model);
    }
}