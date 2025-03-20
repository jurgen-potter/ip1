using CitizenPanel.BL;
using CitizenPanel.BL.Domain.Draw;
using Microsoft.AspNetCore.Mvc;

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
        return View(new RecruitmentCriteria());
    }

    [HttpPost]
    public IActionResult Calculate(RecruitmentCriteria model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }
        var result = _drawManager.CalculateRecruitment(model);

        return View("Result", result);
    }

    [HttpPost]
    public IActionResult AddCustomCriteria(RecruitmentCriteria model)
    {
        model.ExtraCriteria.Add(new ExtraCriteria());
        return View("Index", model);
    }

    [HttpPost]
    public IActionResult AddSubCriteria(RecruitmentCriteria model, int criteriaIndex)
    {
        model.ExtraCriteria[criteriaIndex].SubCriteria.Add(new SubCriteria());
        return View("Index", model);
    }

    [HttpPost]
    public IActionResult RemoveCustomCriteria(RecruitmentCriteria model, int criteriaIndex)
    {
        model.ExtraCriteria.RemoveAt(criteriaIndex);
        return View("Index", model);
    }

    [HttpPost]
    public IActionResult RemoveSubCriteria(RecruitmentCriteria model, int criteriaIndex, int subIndex)
    {
        model.ExtraCriteria.RemoveAll(c => string.IsNullOrWhiteSpace(c.Name));
        // model.ExtraCriteria[criteriaIndex].SubCriteria.RemoveAt(subIndex);
        return View("Index", model);
    }
}