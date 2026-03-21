using InsureYouAI.Context;
using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.ViewComponents.DefaultViewComponents
{
    public class _DefaultPricingPlanComponentPartial:ViewComponent
    { 
        private readonly InsureContext _context;

        public _DefaultPricingPlanComponentPartial(InsureContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var values = _context.PricingPlans.Where(x=>x.IsFeature==true).ToList();
            return View(values);
        }
    }
}
