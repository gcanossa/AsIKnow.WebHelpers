using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsIKnow.WebHelpers.Filters
{
    public class ApiExplorerIncludeOnlyControllerConvention : IActionModelConvention
    {
        protected IEnumerable<Type> AcceptedTypes { get; set; }
        public ApiExplorerIncludeOnlyControllerConvention(IEnumerable<Type> acceptedTypes)
        {
            acceptedTypes = acceptedTypes ?? throw new ArgumentNullException(nameof(acceptedTypes));
            if (!acceptedTypes.Any())
                throw new ArgumentException($"Must contain at least one element.", nameof(acceptedTypes));
            if (acceptedTypes.Any(p => !typeof(Controller).IsAssignableFrom(p)))
                throw new ArgumentException("Only Controller derived types are allowed.", nameof(acceptedTypes));
        }
        public void Apply(ActionModel action)
        {
            action.ApiExplorer.IsVisible = AcceptedTypes.Any(p=> p == action.Controller.ControllerType);
        }
    }
}
