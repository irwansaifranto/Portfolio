using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using WebUI.Infrastructure.Abstract;
using WebUI.Infrastructure.Concrete;

namespace System.Web.Mvc.Html
{
    public static class MyHtmlHelpers
    {
        private static readonly IObjectExtender Extender
            = new ObjectExtender();

        public static MvcHtmlString AngularEditorFor<TModel, TProperty>(
            this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TProperty>> expression,
            string ngModel = null,
            object htmlAttributes = null)
        {
            //helper.ViewData["htmlAttributes"]
            var htmlAttributesData = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            //htmlAttributes

            System.Web.Mvc.ModelMetadata oModelMetadata =
                System.Web.Mvc.ModelMetadata.FromLambdaExpression(expression, helper.ViewData);


            if (ngModel != null)
            {
                if (htmlAttributesData.ContainsKey("ng-model") == false)
                {
                    string ng_model = ngModel;
                    htmlAttributes = Extender.Extend(new { ng_model }, htmlAttributes);
                    //htmlAttributesData.Add("ng_model", ngModel);
                }
            }

            //htmlAttributes = MyHtmlHelpers.ToAnonymousObject(htmlAttributesData);

            return (helper.EditorFor(expression, new { htmlAttributes = htmlAttributes }));
        }

        public static MvcHtmlString AngularCheckboxFor<TModel>(
            this HtmlHelper<TModel> helper,
            Expression<Func<TModel, bool>> expression,
            string ngModel = null,
            object htmlAttributes = null,
            string ngTrueValue = null,
            string ngFalseValue = null)
        {
            //helper.ViewData["htmlAttributes"]
            var htmlAttributesData = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            //htmlAttributes

            System.Web.Mvc.ModelMetadata oModelMetadata =
                System.Web.Mvc.ModelMetadata.FromLambdaExpression(expression, helper.ViewData);


            if (ngModel != null)
            {
                if (htmlAttributesData.ContainsKey("ng-model") == false)
                {
                    htmlAttributesData.Add("ng-model", ngModel);
                }
            }

            if (ngTrueValue != null)
            {
                if (htmlAttributesData.ContainsKey("ng-true-value") == false)
                {
                    htmlAttributesData.Add("ng-true-value", ngTrueValue);
                }
            }

            if (ngFalseValue != null)
            {
                if (htmlAttributesData.ContainsKey("ng-false-value") == false)
                {
                    htmlAttributesData.Add("ng-false-value", ngFalseValue);
                }
            }

            //htmlAttributes = MyHtmlHelpers.ToAnonymousObject(htmlAttributesData);

            return (helper.CheckBoxFor(expression, htmlAttributesData));
        }

        public static MvcHtmlString AngularRadioButtonFor<TModel,TProperty>(
            this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TProperty>> expression,
            string ngModel = null,
            string value = null,
            object htmlAttributes = null)
        {
            //helper.ViewData["htmlAttributes"]
            var htmlAttributesData = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            //htmlAttributes

            System.Web.Mvc.ModelMetadata oModelMetadata =
                System.Web.Mvc.ModelMetadata.FromLambdaExpression(expression, helper.ViewData);


            if (ngModel != null)
            {
                if (htmlAttributesData.ContainsKey("ng-model") == false)
                {
                    htmlAttributesData.Add("ng-model", ngModel);
                }
            }

            //htmlAttributes = MyHtmlHelpers.ToAnonymousObject(htmlAttributesData);

            return (helper.RadioButtonFor(expression, value, htmlAttributesData));
        }
    }
}