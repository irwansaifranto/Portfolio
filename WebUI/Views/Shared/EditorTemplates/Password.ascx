<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%= Html.Password("", ViewData.TemplateInfo.FormattedModelValue, ViewData["htmlAttributes"]) %>